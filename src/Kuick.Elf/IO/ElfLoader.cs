using System.Buffers.Binary;
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

        return new ElfObject
        {
            Header = header
        };
    }
}
