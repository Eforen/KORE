using Kuick.Elf.Models;

namespace Kuick.Elf.IO;

public sealed class ElfWriter
{
    public void Write(ElfObject elfObject, string filePath)
    {
        ArgumentNullException.ThrowIfNull(elfObject);
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        using var stream = File.Create(filePath);
        using var writer = new BinaryWriter(stream);

        var magic = elfObject.Header.Magic;
        writer.Write(magic.Length >= 4 ? magic : [0x7F, 0x45, 0x4C, 0x46]);
        writer.Write(elfObject.Header.ElfClass);
        writer.Write(elfObject.Header.DataEncoding);
        writer.Write(elfObject.Header.ElfVersion);
        writer.Write(elfObject.Header.OsAbi);
        writer.Write(elfObject.Header.AbiVersion);
        writer.Write(new byte[7]);
        writer.Write(elfObject.Header.Type);
        writer.Write(elfObject.Header.Machine);
        writer.Write(elfObject.Header.Version);
        if (elfObject.Header.ElfClass == 2)
        {
            writer.Write(elfObject.Header.EntryPoint);
        }
        else
        {
            writer.Write((uint)elfObject.Header.EntryPoint);
        }
    }
}
