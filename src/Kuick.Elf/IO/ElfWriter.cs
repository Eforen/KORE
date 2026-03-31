using Kuick.Elf.Models;

namespace Kuick.Elf.IO;

public sealed class ElfWriter
{
    private const byte ElfClass32 = 1;
    private const byte ElfClass64 = 2;

    public void Write(ElfObject elfObject, string filePath)
    {
        ArgumentNullException.ThrowIfNull(elfObject);
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        using var stream = File.Create(filePath);
        using var writer = new BinaryWriter(stream);

        var ident = elfObject.Header.Ident.Length >= 16
            ? elfObject.Header.Ident
            : PadIdent(elfObject.Header.Ident);

        writer.Write(ident);
        writer.Write(elfObject.Header.Type);
        writer.Write(elfObject.Header.Machine);
        writer.Write(elfObject.Header.FileVersion);

        var cls = ident[4];
        if (cls == ElfClass64)
        {
            writer.Write(elfObject.Header.Entry);
            writer.Write(elfObject.Header.ProgramHeaderOffset);
            writer.Write(elfObject.Header.SectionHeaderOffset);
        }
        else
        {
            writer.Write((uint)elfObject.Header.Entry);
            writer.Write((uint)elfObject.Header.ProgramHeaderOffset);
            writer.Write((uint)elfObject.Header.SectionHeaderOffset);
        }

        writer.Write(elfObject.Header.Flags);
        writer.Write(elfObject.Header.HeaderSize);
        writer.Write(elfObject.Header.ProgramHeaderEntrySize);
        writer.Write(elfObject.Header.ProgramHeaderCount);
        writer.Write(elfObject.Header.SectionHeaderEntrySize);
        writer.Write(elfObject.Header.SectionHeaderCount);
        writer.Write(elfObject.Header.SectionHeaderStringIndex);
    }

    private static byte[] PadIdent(byte[] ident)
    {
        var buf = new byte[16];
        Array.Copy(ident, buf, Math.Min(ident.Length, 16));
        if (buf[0] == 0)
        {
            buf[0] = 0x7F;
            buf[1] = 0x45;
            buf[2] = 0x4C;
            buf[3] = 0x46;
        }

        return buf;
    }
}
