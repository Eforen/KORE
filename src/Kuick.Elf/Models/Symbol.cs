namespace Kuick.Elf.Models;

public sealed class Symbol
{
    public required string Name { get; init; }
    /// <summary>ELF section that contained this symbol table (e.g. <c>.symtab</c>, <c>.dynsym</c>).</summary>
    public string TableName { get; init; } = string.Empty;
    public ulong Value { get; init; }
    public ulong Size { get; init; }
    public byte Info { get; init; }
    public byte Other { get; init; }
    public ushort SectionIndex { get; init; }
}
