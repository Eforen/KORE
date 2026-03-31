namespace Kuick.Elf.Models;

public sealed class Section
{
    public required string Name { get; init; }
    public uint Type { get; init; }
    public ulong Flags { get; init; }
    public ulong Address { get; init; }
    public ulong Offset { get; init; }
    public ulong Size { get; init; }
    public uint Link { get; init; }
    public uint Info { get; init; }
    public ulong AddressAlign { get; init; }
    public ulong EntrySize { get; init; }
}
