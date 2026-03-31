namespace Kuick.Elf.Models;

public sealed class ElfHeader
{
    public byte[] Magic { get; init; } = [0x7F, 0x45, 0x4C, 0x46];
    public byte ElfClass { get; init; }
    public byte DataEncoding { get; init; }
    public byte ElfVersion { get; init; }
    public byte OsAbi { get; init; }
    public byte AbiVersion { get; init; }
    public ushort Type { get; init; }
    public ushort Machine { get; init; }
    public uint Version { get; init; }
    public ulong EntryPoint { get; init; }
}
