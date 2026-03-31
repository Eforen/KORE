using Kuick.Elf.Models;

namespace Kuick.Elf.IO;

public sealed class ElfLoader
{
    public ElfObject Load(string filePath)
    {
        using var stream = File.OpenRead(filePath);
        using var reader = new BinaryReader(stream);

        var ident = reader.ReadBytes(16);
        if (ident.Length < 16 || ident[0] != 0x7F || ident[1] != 0x45 || ident[2] != 0x4C || ident[3] != 0x46)
        {
            throw new InvalidDataException("Input file is not a valid ELF file.");
        }

        var header = new ElfHeader
        {
            Magic = ident.Take(4).ToArray(),
            ElfClass = ident[4],
            DataEncoding = ident[5],
            ElfVersion = ident[6],
            OsAbi = ident[7],
            AbiVersion = ident[8],
            Type = reader.ReadUInt16(),
            Machine = reader.ReadUInt16(),
            Version = reader.ReadUInt32(),
            EntryPoint = ident[4] == 2 ? reader.ReadUInt64() : reader.ReadUInt32()
        };

        return new ElfObject
        {
            Header = header
        };
    }
}
