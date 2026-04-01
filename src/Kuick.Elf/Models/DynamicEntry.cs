namespace Kuick.Elf.Models;

/// <summary>One <c>Elf{32,64}_Dyn</c> record (<c>d_tag</c>, <c>d_un</c>).</summary>
public sealed class DynamicEntry
{
    public long Tag { get; init; }
    public ulong Value { get; init; }
}
