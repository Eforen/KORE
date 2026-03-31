namespace Kuick.Elf.Models;

public sealed class Symbol
{
    public required string Name { get; init; }
    public ulong Value { get; init; }
    public ulong Size { get; init; }
    public byte Info { get; init; }
    public byte Other { get; init; }
    public ushort SectionIndex { get; init; }
}
