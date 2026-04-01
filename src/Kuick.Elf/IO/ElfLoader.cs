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
        LoadDynamic(stream, reader, elfObject, elfClass);
        LoadGnuVersion(stream, reader, elfObject);
        LoadRiscvAttributes(stream, reader, elfObject);
        LoadGnuHash(stream, reader, elfObject);
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
    private const uint ShtDynamic = 6;
    private const uint ShtGnuHash = 0x6ffffff6;
    private const uint ShtGnuVerdef = 0x6ffffffd;
    private const uint ShtGnuVerneed = 0x6ffffffe;
    private const uint ShtGnuVersym = 0x6fffffff;
    /// <summary><c>SHT_RISCV_ATTRIBUTES</c> (<c>SHT_LOPROC + 3</c>).</summary>
    private const uint ShtRiscvAttributes = 0x70000003;
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

    private static void LoadDynamic(Stream stream, BinaryReader reader, ElfObject obj, byte elfClass)
    {
        foreach (var sec in obj.Sections)
        {
            if (sec.Type != ShtDynamic)
            {
                continue;
            }

            var expectedEs = elfClass == ElfClass64 ? 16u : 8u;
            if (sec.EntrySize < expectedEs || sec.Size == 0 || sec.Size > int.MaxValue)
            {
                continue;
            }

            if ((long)sec.Offset + (long)sec.Size > stream.Length)
            {
                continue;
            }

            if (sec.Link < (uint)obj.Sections.Count)
            {
                var strSec = obj.Sections[(int)sec.Link];
                if (strSec.Type == ShtStrtab && strSec.Size <= int.MaxValue
                    && (long)strSec.Offset + (long)strSec.Size <= stream.Length)
                {
                    stream.Seek((long)strSec.Offset, SeekOrigin.Begin);
                    obj.DynamicStrtab = reader.ReadBytes((int)strSec.Size);
                }
            }

            stream.Seek((long)sec.Offset, SeekOrigin.Begin);
            var extra = (int)(sec.EntrySize - expectedEs);
            var count = sec.Size / sec.EntrySize;
            for (ulong n = 0; n < count; n++)
            {
                long tag;
                ulong val;
                if (elfClass == ElfClass64)
                {
                    tag = reader.ReadInt64();
                    val = reader.ReadUInt64();
                }
                else
                {
                    tag = reader.ReadInt32();
                    val = reader.ReadUInt32();
                }

                if (extra > 0)
                {
                    reader.ReadBytes(extra);
                }

                obj.DynamicEntries.Add(new DynamicEntry { Tag = tag, Value = val });
                if (tag == 0)
                {
                    break;
                }
            }

            break;
        }
    }

    private static void LoadGnuVersion(Stream stream, BinaryReader reader, ElfObject obj)
    {
        var le = obj.Header.DataEncoding == 1;
        foreach (var sec in obj.Sections)
        {
            if (sec.Type == ShtGnuVersym)
            {
                TryLoadVersym(stream, reader, obj, sec, le);
            }
        }

        foreach (var sec in obj.Sections)
        {
            if (sec.Type == ShtGnuVerdef)
            {
                TryLoadVerdef(stream, reader, obj, sec);
            }
        }

        foreach (var sec in obj.Sections)
        {
            if (sec.Type == ShtGnuVerneed)
            {
                TryLoadVerneed(stream, reader, obj, sec);
            }
        }
    }

    private static void TryLoadVersym(Stream stream, BinaryReader reader, ElfObject obj, Section sec, bool littleEndian)
    {
        if (sec.Size < 2 || sec.Size % 2 != 0 || sec.Size > int.MaxValue)
        {
            return;
        }

        if ((long)sec.Offset + (long)sec.Size > stream.Length || sec.Link >= (uint)obj.Sections.Count)
        {
            return;
        }

        stream.Seek((long)sec.Offset, SeekOrigin.Begin);
        var bytes = reader.ReadBytes((int)sec.Size);
        var n = bytes.Length / 2;
        var entries = new ushort[n];
        for (var i = 0; i < n; i++)
        {
            entries[i] = ReadUInt16Elf(bytes, i * 2, littleEndian);
        }

        obj.GnuVersion.Versym = new VersymSection
        {
            Name = sec.Name,
            Address = sec.Address,
            Offset = sec.Offset,
            DynsymLinkName = obj.Sections[(int)sec.Link].Name,
            Entries = entries
        };
    }

    private static ushort ReadUInt16Elf(byte[] b, int i, bool littleEndian) =>
        littleEndian
            ? (ushort)(b[i] | (b[i + 1] << 8))
            : (ushort)((b[i] << 8) | b[i + 1]);

    private static void TryLoadVerneed(Stream stream, BinaryReader reader, ElfObject obj, Section sec)
    {
        if (sec.Size == 0 || sec.Size > int.MaxValue || (long)sec.Offset + (long)sec.Size > stream.Length)
        {
            return;
        }

        if (sec.Link >= (uint)obj.Sections.Count)
        {
            return;
        }

        var dynstrSec = obj.Sections[(int)sec.Link];
        if (dynstrSec.Type != ShtStrtab || dynstrSec.Size > int.MaxValue
            || (long)dynstrSec.Offset + (long)dynstrSec.Size > stream.Length)
        {
            return;
        }

        stream.Seek((long)dynstrSec.Offset, SeekOrigin.Begin);
        var dynstr = reader.ReadBytes((int)dynstrSec.Size);

        var parsed = new VerneedSection
        {
            Name = sec.Name,
            Address = sec.Address,
            Offset = sec.Offset,
            DynstrLinkName = dynstrSec.Name
        };

        ulong pos = 0;
        while (pos < sec.Size)
        {
            if (pos + 16 > sec.Size)
            {
                break;
            }

            var headerStart = pos;
            stream.Seek((long)(sec.Offset + pos), SeekOrigin.Begin);
            var vnVersion = reader.ReadUInt16();
            var vnCnt = reader.ReadUInt16();
            var vnFile = reader.ReadUInt32();
            var vnAux = reader.ReadUInt32();
            var vnNext = reader.ReadUInt32();

            var chain = new VerneedChain
            {
                HeaderOffsetInSection = headerStart,
                VnVersion = vnVersion,
                VnCount = vnCnt,
                FileName = ReadSectionName(dynstr, vnFile)
            };

            var auxPos = headerStart + vnAux;
            while (true)
            {
                if (auxPos + 16 > sec.Size)
                {
                    break;
                }

                stream.Seek((long)(sec.Offset + auxPos), SeekOrigin.Begin);
                var vnaHash = reader.ReadUInt32();
                var vnaFlags = reader.ReadUInt16();
                var vnaOther = reader.ReadUInt16();
                var vnaName = reader.ReadUInt32();
                var vnaNext = reader.ReadUInt32();

                chain.Aux.Add(new VernauxEntry
                {
                    OffsetInSection = auxPos,
                    Hash = vnaHash,
                    Flags = vnaFlags,
                    VersionIndex = vnaOther,
                    Name = ReadSectionName(dynstr, vnaName)
                });

                if (vnaNext == 0)
                {
                    break;
                }

                auxPos += vnaNext;
            }

            parsed.Chains.Add(chain);
            if (vnNext == 0)
            {
                break;
            }

            pos += vnNext;
        }

        if (parsed.Chains.Count > 0)
        {
            obj.GnuVersion.Verneed.Add(parsed);
        }
    }

    private static void TryLoadVerdef(Stream stream, BinaryReader reader, ElfObject obj, Section sec)
    {
        if (sec.Size == 0 || sec.Size > int.MaxValue || (long)sec.Offset + (long)sec.Size > stream.Length)
        {
            return;
        }

        if (sec.Link >= (uint)obj.Sections.Count)
        {
            return;
        }

        var dynstrSec = obj.Sections[(int)sec.Link];
        if (dynstrSec.Type != ShtStrtab || dynstrSec.Size > int.MaxValue
            || (long)dynstrSec.Offset + (long)dynstrSec.Size > stream.Length)
        {
            return;
        }

        stream.Seek((long)dynstrSec.Offset, SeekOrigin.Begin);
        var dynstr = reader.ReadBytes((int)dynstrSec.Size);

        var parsed = new VerdefSection
        {
            Name = sec.Name,
            Address = sec.Address,
            Offset = sec.Offset,
            DynstrLinkName = dynstrSec.Name
        };

        ulong pos = 0;
        while (pos < sec.Size)
        {
            if (pos + 20 > sec.Size)
            {
                break;
            }

            var entryStart = pos;
            stream.Seek((long)(sec.Offset + pos), SeekOrigin.Begin);
            var vdVersion = reader.ReadUInt16();
            var vdFlags = reader.ReadUInt16();
            var vdNdx = reader.ReadUInt16();
            var vdCnt = reader.ReadUInt16();
            _ = reader.ReadUInt32();
            var vdAux = reader.ReadUInt32();
            var vdNext = reader.ReadUInt32();

            var auxPos = entryStart + vdAux;
            var names = new List<(ulong Off, string Name)>();
            while (true)
            {
                if (auxPos + 8 > sec.Size)
                {
                    break;
                }

                stream.Seek((long)(sec.Offset + auxPos), SeekOrigin.Begin);
                var vdaName = reader.ReadUInt32();
                var vdaNext = reader.ReadUInt32();
                names.Add((auxPos, ReadSectionName(dynstr, vdaName)));
                if (vdaNext == 0)
                {
                    break;
                }

                auxPos += vdaNext;
            }

            var entry = new VerdefEntry
            {
                VerdefOffsetInSection = entryStart,
                VdVersion = vdVersion,
                VdFlags = vdFlags,
                VdNdx = vdNdx,
                VdCount = vdCnt,
                Name = names.Count > 0 ? names[0].Name : string.Empty
            };

            for (var p = 1; p < names.Count; p++)
            {
                entry.Parents.Add(new VerdefParentLine
                {
                    OffsetInSection = names[p].Off,
                    ParentIndex = p,
                    Name = names[p].Name
                });
            }

            parsed.Entries.Add(entry);
            if (vdNext == 0)
            {
                break;
            }

            pos += vdNext;
        }

        if (parsed.Entries.Count > 0)
        {
            obj.GnuVersion.Verdef.Add(parsed);
        }
    }

    private static void LoadRiscvAttributes(Stream stream, BinaryReader reader, ElfObject obj)
    {
        foreach (var sec in obj.Sections)
        {
            if (sec.Type != ShtRiscvAttributes)
            {
                continue;
            }

            if (sec.Size == 0 || sec.Size > int.MaxValue)
            {
                continue;
            }

            if ((long)sec.Offset + (long)sec.Size > stream.Length)
            {
                continue;
            }

            stream.Seek((long)sec.Offset, SeekOrigin.Begin);
            obj.RiscvAttributes = reader.ReadBytes((int)sec.Size);
            return;
        }
    }

    private static void LoadGnuHash(Stream stream, BinaryReader reader, ElfObject obj)
    {
        foreach (var sec in obj.Sections)
        {
            if (sec.Type != ShtGnuHash)
            {
                continue;
            }

            if (sec.Size == 0 || sec.Size > int.MaxValue)
            {
                continue;
            }

            if ((long)sec.Offset + (long)sec.Size > stream.Length)
            {
                continue;
            }

            stream.Seek((long)sec.Offset, SeekOrigin.Begin);
            obj.GnuHash = reader.ReadBytes((int)sec.Size);
            return;
        }
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
