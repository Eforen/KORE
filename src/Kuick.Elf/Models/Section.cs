namespace Kuick.Elf.Models;

public sealed class Section
{
    public required string Name { get; init; }
    public ulong Address { get; init; }
    public ulong Offset { get; init; }
    public ulong Size { get; init; }
    public uint Type { get; init; }
    public ulong Flags { get; init; }
}
