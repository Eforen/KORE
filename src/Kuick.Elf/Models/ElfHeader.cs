namespace Kuick.Elf.Models;

/// <summary>
/// ELF executable header (32- or 64-bit); numeric fields match file layout (width depends on class).
/// </summary>
public sealed class ElfHeader
{
    /// <summary>Full 16-byte e_ident.</summary>
    public byte[] Ident { get; init; } = new byte[16];

    public byte ElfClass => Ident.Length >= 5 ? Ident[4] : (byte)0;
    public byte DataEncoding => Ident.Length >= 6 ? Ident[5] : (byte)0;
    public byte IdentVersion => Ident.Length >= 7 ? Ident[6] : (byte)0;
    public byte OsAbi => Ident.Length >= 8 ? Ident[7] : (byte)0;
    public byte AbiVersion => Ident.Length >= 9 ? Ident[8] : (byte)0;

    public ushort Type { get; init; }
    public ushort Machine { get; init; }
    /// <summary>e_version in the ELF header (object file version).</summary>
    public uint FileVersion { get; init; }

    public ulong Entry { get; init; }
    public ulong ProgramHeaderOffset { get; init; }
    public ulong SectionHeaderOffset { get; init; }
    public uint Flags { get; init; }

    public ushort HeaderSize { get; init; }
    public ushort ProgramHeaderEntrySize { get; init; }
    public ushort ProgramHeaderCount { get; init; }
    public ushort SectionHeaderEntrySize { get; init; }
    public ushort SectionHeaderCount { get; init; }
    public ushort SectionHeaderStringIndex { get; init; }
}
