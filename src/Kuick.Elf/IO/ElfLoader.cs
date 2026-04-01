using System.Text;
using Kuick.Elf.Models;

namespace Kuick.Elf.IO;

public sealed class ElfLoader
{
    private const byte ElfClassNone = 0;
    private const byte ElfClass32 = 1;
    private const byte ElfClass64 = 2;

    public ElfObject Load(string filePath)
    {
        using var stream = File.OpenRead(filePath);
        using var reader = new BinaryReader(stream);

        var ident = reader.ReadBytes(16);
        if (ident.Length < 16 || ident[0] != 0x7F || ident[1] != 0x45 || ident[2] != 0x4C || ident[3] != 0x46)
        {
            throw new InvalidDataException("Input file is not a valid ELF file.");
        }

        var elfClass = ident[4];
        if (elfClass is ElfClassNone or not (ElfClass32 or ElfClass64))
        {
            throw new InvalidDataException($"Unsupported ELF class: {elfClass}.");
        }

        var type = reader.ReadUInt16();
        var machine = reader.ReadUInt16();
        var fileVersion = reader.ReadUInt32();

        ulong entry;
        ulong phoff;
        ulong shoff;
        if (elfClass == ElfClass32)
        {
            entry = reader.ReadUInt32();
            phoff = reader.ReadUInt32();
            shoff = reader.ReadUInt32();
        }
        else
        {
            entry = reader.ReadUInt64();
            phoff = reader.ReadUInt64();
            shoff = reader.ReadUInt64();
        }

        var flags = reader.ReadUInt32();
        var ehsize = reader.ReadUInt16();
        var phentsize = reader.ReadUInt16();
        var phnum = reader.ReadUInt16();
        var shentsize = reader.ReadUInt16();
        var shnum = reader.ReadUInt16();
        var shstrndx = reader.ReadUInt16();

        var header = new ElfHeader
        {
            Ident = ident,
            Type = type,
            Machine = machine,
            FileVersion = fileVersion,
            Entry = entry,
            ProgramHeaderOffset = phoff,
            SectionHeaderOffset = shoff,
            Flags = flags,
            HeaderSize = ehsize,
            ProgramHeaderEntrySize = phentsize,
            ProgramHeaderCount = phnum,
            SectionHeaderEntrySize = shentsize,
            SectionHeaderCount = shnum,
            SectionHeaderStringIndex = shstrndx
        };

        var elfObject = new ElfObject
        {
            Header = header
        };

        LoadProgramHeaders(stream, reader, elfObject, elfClass);
        LoadSectionHeaders(stream, reader, elfObject, elfClass);
        LoadSymbols(stream, reader, elfObject, elfClass);
        LoadRelocations(stream, reader, elfObject, elfClass);
        return elfObject;
    }

    private readonly struct RawSectionHeader
    {
        public required uint NameIndex { get; init; }
        public required uint Type { get; init; }
        public required ulong Flags { get; init; }
        public required ulong Address { get; init; }
        public required ulong Offset { get; init; }
        public required ulong Size { get; init; }
        public required uint Link { get; init; }
        public required uint Info { get; init; }
        public required ulong Addralign { get; init; }
        public required ulong Entsize { get; init; }
    }

    private static void LoadSectionHeaders(Stream stream, BinaryReader reader, ElfObject obj, byte elfClass)
    {
        var h = obj.Header;
        if (h.SectionHeaderCount == 0 || h.SectionHeaderOffset == 0)
        {
            return;
        }

        var expected = elfClass == ElfClass64 ? 64u : 40u;
        if (h.SectionHeaderEntrySize < expected)
        {
            throw new InvalidDataException(
                $"Invalid section header entry size {h.SectionHeaderEntrySize}; expected at least {expected} for ELF{elfClass * 32}.");
        }

        stream.Seek((long)h.SectionHeaderOffset, SeekOrigin.Begin);
        var extra = h.SectionHeaderEntrySize - expected;
        var raw = new RawSectionHeader[h.SectionHeaderCount];
        for (var i = 0; i < h.SectionHeaderCount; i++)
        {
            raw[i] = elfClass == ElfClass64 ? ReadSectionHeader64(reader) : ReadSectionHeader32(reader);
            if (extra > 0)
            {
                reader.ReadBytes((int)extra);
            }
        }

        if (h.SectionHeaderStringIndex >= h.SectionHeaderCount)
        {
            throw new InvalidDataException($"Invalid e_shstrndx {h.SectionHeaderStringIndex} (section count {h.SectionHeaderCount}).");
        }

        var shstr = raw[h.SectionHeaderStringIndex];
        if (shstr.Size > int.MaxValue)
        {
            throw new InvalidDataException("Section header string table is too large.");
        }

        if (shstr.Size > 0 && (long)shstr.Offset + (long)shstr.Size > stream.Length)
        {
            throw new InvalidDataException("Section header string table extends past end of file.");
        }

        stream.Seek((long)shstr.Offset, SeekOrigin.Begin);
        var shstrtab = shstr.Size == 0 ? Array.Empty<byte>() : reader.ReadBytes((int)shstr.Size);

        for (var i = 0; i < h.SectionHeaderCount; i++)
        {
            var r = raw[i];
            var name = ReadSectionName(shstrtab, r.NameIndex);
            obj.Sections.Add(new Section
            {
                Name = name,
                Type = r.Type,
                Flags = r.Flags,
                Address = r.Address,
                Offset = r.Offset,
                Size = r.Size,
                Link = r.Link,
                Info = r.Info,
                AddressAlign = r.Addralign,
                EntrySize = r.Entsize
            });
        }
    }

    private static string ReadSectionName(byte[] shstrtab, uint nameIndex)
    {
        if (shstrtab.Length == 0 || nameIndex >= shstrtab.Length)
        {
            return string.Empty;
        }

        var end = (int)nameIndex;
        while (end < shstrtab.Length && shstrtab[end] != 0)
        {
            end++;
        }

        return Encoding.UTF8.GetString(shstrtab, (int)nameIndex, end - (int)nameIndex);
    }

    private static RawSectionHeader ReadSectionHeader64(BinaryReader reader)
    {
        var name = reader.ReadUInt32();
        var type = reader.ReadUInt32();
        var flags = reader.ReadUInt64();
        var addr = reader.ReadUInt64();
        var offset = reader.ReadUInt64();
        var size = reader.ReadUInt64();
        var link = reader.ReadUInt32();
        var info = reader.ReadUInt32();
        var addralign = reader.ReadUInt64();
        var entsize = reader.ReadUInt64();
        return new RawSectionHeader
        {
            NameIndex = name,
            Type = type,
            Flags = flags,
            Address = addr,
            Offset = offset,
            Size = size,
            Link = link,
            Info = info,
            Addralign = addralign,
            Entsize = entsize
        };
    }

    private static RawSectionHeader ReadSectionHeader32(BinaryReader reader)
    {
        var name = reader.ReadUInt32();
        var type = reader.ReadUInt32();
        var flags = reader.ReadUInt32();
        var addr = reader.ReadUInt32();
        var offset = reader.ReadUInt32();
        var size = reader.ReadUInt32();
        var link = reader.ReadUInt32();
        var info = reader.ReadUInt32();
        var addralign = reader.ReadUInt32();
        var entsize = reader.ReadUInt32();
        return new RawSectionHeader
        {
            NameIndex = name,
            Type = type,
            Flags = flags,
            Address = addr,
            Offset = offset,
            Size = size,
            Link = link,
            Info = info,
            Addralign = addralign,
            Entsize = entsize
        };
    }

    private const uint ShtSymtab = 2;
    private const uint ShtStrtab = 3;
    private const uint ShtRela = 4;
    private const uint ShtRel = 9;
    private const uint ShtDynsym = 11;

    private static void LoadRelocations(Stream stream, BinaryReader reader, ElfObject obj, byte elfClass)
    {
        foreach (var sec in obj.Sections)
        {
            if (sec.Type is not (ShtRela or ShtRel))
            {
                continue;
            }

            var isRela = sec.Type == ShtRela;
            var expectedEs = elfClass == ElfClass64
                ? (isRela ? 24u : 16u)
                : (isRela ? 12u : 8u);

            if (sec.Size == 0 || sec.EntrySize < expectedEs || sec.Size > int.MaxValue)
            {
                continue;
            }

            if ((long)sec.Offset + (long)sec.Size > stream.Length)
            {
                continue;
            }

            if (sec.Link >= (uint)obj.Sections.Count)
            {
                continue;
            }

            var count = sec.Size / sec.EntrySize;
            if (count == 0)
            {
                continue;
            }

            stream.Seek((long)sec.Offset, SeekOrigin.Begin);
            var extra = (int)(sec.EntrySize - expectedEs);
            for (ulong n = 0; n < count; n++)
            {
                RelocationEntry re;
                if (elfClass == ElfClass64)
                {
                    re = isRela ? ReadRela64(reader, sec.Name, sec.Link) : ReadRel64(reader, sec.Name, sec.Link);
                }
                else
                {
                    re = isRela ? ReadRela32(reader, sec.Name, sec.Link) : ReadRel32(reader, sec.Name, sec.Link);
                }

                if (extra > 0)
                {
                    reader.ReadBytes(extra);
                }

                obj.Relocations.Add(re);
            }
        }
    }

    private static RelocationEntry ReadRela64(BinaryReader reader, string sectionName, uint symTabLink)
    {
        var offset = reader.ReadUInt64();
        var info = reader.ReadUInt64();
        var addend = reader.ReadInt64();
        return new RelocationEntry
        {
            SectionName = sectionName,
            SymTabLink = symTabLink,
            Offset = offset,
            Info = info,
            Addend = addend
        };
    }

    private static RelocationEntry ReadRel64(BinaryReader reader, string sectionName, uint symTabLink)
    {
        var offset = reader.ReadUInt64();
        var info = reader.ReadUInt64();
        return new RelocationEntry
        {
            SectionName = sectionName,
            SymTabLink = symTabLink,
            Offset = offset,
            Info = info,
            Addend = 0
        };
    }

    private static RelocationEntry ReadRela32(BinaryReader reader, string sectionName, uint symTabLink)
    {
        var offset = reader.ReadUInt32();
        var info = reader.ReadUInt32();
        var addend = reader.ReadInt32();
        return new RelocationEntry
        {
            SectionName = sectionName,
            SymTabLink = symTabLink,
            Offset = offset,
            Info = info,
            Addend = addend
        };
    }

    private static RelocationEntry ReadRel32(BinaryReader reader, string sectionName, uint symTabLink)
    {
        var offset = reader.ReadUInt32();
        var info = reader.ReadUInt32();
        return new RelocationEntry
        {
            SectionName = sectionName,
            SymTabLink = symTabLink,
            Offset = offset,
            Info = info,
            Addend = 0
        };
    }

    private static void LoadSymbols(Stream stream, BinaryReader reader, ElfObject obj, byte elfClass)
    {
        var expectedEs = elfClass == ElfClass64 ? 24u : 16u;
        foreach (var sec in obj.Sections)
        {
            if (sec.Type is not (ShtSymtab or ShtDynsym))
            {
                continue;
            }

            if (sec.Size == 0 || sec.EntrySize < expectedEs || sec.Link >= (uint)obj.Sections.Count)
            {
                continue;
            }

            var strtabSec = obj.Sections[(int)sec.Link];
            if (strtabSec.Type != ShtStrtab)
            {
                continue;
            }

            if (strtabSec.Size > int.MaxValue || sec.Size > int.MaxValue)
            {
                continue;
            }

            if ((long)strtabSec.Offset + (long)strtabSec.Size > stream.Length
                || (long)sec.Offset + (long)sec.Size > stream.Length)
            {
                continue;
            }

            var count = sec.Size / sec.EntrySize;
            if (count == 0)
            {
                continue;
            }

            stream.Seek((long)strtabSec.Offset, SeekOrigin.Begin);
            var strtab = reader.ReadBytes((int)strtabSec.Size);

            stream.Seek((long)sec.Offset, SeekOrigin.Begin);
            var extra = (int)(sec.EntrySize - expectedEs);
            for (ulong n = 0; n < count; n++)
            {
                var sym = elfClass == ElfClass64
                    ? ReadSymbol64(reader, strtab, sec.Name)
                    : ReadSymbol32(reader, strtab, sec.Name);
                if (extra > 0)
                {
                    reader.ReadBytes(extra);
                }

                obj.Symbols.Add(sym);
            }
        }
    }

    private static Symbol ReadSymbol64(BinaryReader reader, byte[] strtab, string tableName)
    {
        var nameIdx = reader.ReadUInt32();
        var info = reader.ReadByte();
        var other = reader.ReadByte();
        var shndx = reader.ReadUInt16();
        var value = reader.ReadUInt64();
        var size = reader.ReadUInt64();
        return new Symbol
        {
            Name = ReadSectionName(strtab, nameIdx),
            TableName = tableName,
            Value = value,
            Size = size,
            Info = info,
            Other = other,
            SectionIndex = shndx
        };
    }

    private static Symbol ReadSymbol32(BinaryReader reader, byte[] strtab, string tableName)
    {
        var nameIdx = reader.ReadUInt32();
        var value = reader.ReadUInt32();
        var size = reader.ReadUInt32();
        var info = reader.ReadByte();
        var other = reader.ReadByte();
        var shndx = reader.ReadUInt16();
        return new Symbol
        {
            Name = ReadSectionName(strtab, nameIdx),
            TableName = tableName,
            Value = value,
            Size = size,
            Info = info,
            Other = other,
            SectionIndex = shndx
        };
    }

    private static void LoadProgramHeaders(Stream stream, BinaryReader reader, ElfObject obj, byte elfClass)
    {
        var h = obj.Header;
        if (h.ProgramHeaderCount == 0 || h.ProgramHeaderOffset == 0)
        {
            return;
        }

        var expected = elfClass == ElfClass64 ? 56u : 32u;
        if (h.ProgramHeaderEntrySize < expected)
        {
            throw new InvalidDataException(
                $"Invalid program header entry size {h.ProgramHeaderEntrySize}; expected at least {expected} for ELF{elfClass * 32}.");
        }

        stream.Seek((long)h.ProgramHeaderOffset, SeekOrigin.Begin);
        var extra = h.ProgramHeaderEntrySize - expected;
        for (var i = 0; i < h.ProgramHeaderCount; i++)
        {
            ProgramHeader ph;
            if (elfClass == ElfClass64)
            {
                ph = ReadProgramHeader64(reader);
            }
            else
            {
                ph = ReadProgramHeader32(reader);
            }

            obj.ProgramHeaders.Add(ph);
            if (extra > 0)
            {
                reader.ReadBytes((int)extra);
            }
        }
    }

    private static ProgramHeader ReadProgramHeader64(BinaryReader reader)
    {
        var type = reader.ReadUInt32();
        var flags = reader.ReadUInt32();
        var offset = reader.ReadUInt64();
        var vaddr = reader.ReadUInt64();
        var paddr = reader.ReadUInt64();
        var filesz = reader.ReadUInt64();
        var memsz = reader.ReadUInt64();
        var align = reader.ReadUInt64();
        return new ProgramHeader
        {
            Type = type,
            Flags = flags,
            Offset = offset,
            VirtualAddress = vaddr,
            PhysicalAddress = paddr,
            FileSize = filesz,
            MemorySize = memsz,
            Align = align
        };
    }

    private static ProgramHeader ReadProgramHeader32(BinaryReader reader)
    {
        var type = reader.ReadUInt32();
        var offset = reader.ReadUInt32();
        var vaddr = reader.ReadUInt32();
        var paddr = reader.ReadUInt32();
        var filesz = reader.ReadUInt32();
        var memsz = reader.ReadUInt32();
        var flags = reader.ReadUInt32();
        var align = reader.ReadUInt32();
        return new ProgramHeader
        {
            Type = type,
            Flags = flags,
            Offset = offset,
            VirtualAddress = vaddr,
            PhysicalAddress = paddr,
            FileSize = filesz,
            MemorySize = memsz,
            Align = align
        };
    }
}
