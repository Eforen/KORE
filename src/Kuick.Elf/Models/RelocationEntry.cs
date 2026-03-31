namespace Kuick.Elf.Models;

public sealed class RelocationEntry
{
    public ulong Offset { get; init; }
    public ulong Info { get; init; }
    public long Addend { get; init; }
}
