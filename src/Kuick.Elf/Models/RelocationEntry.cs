namespace Kuick.Elf.Models;

public sealed class RelocationEntry
{
    /// <summary>ELF section that contained this record (e.g. <c>.rela.text</c>).</summary>
    public string SectionName { get; init; } = string.Empty;

    /// <summary>Section index of the symbol table to use for <c>r_sym</c> (<c>sh_link</c> of this REL/RELA section).</summary>
    public uint SymTabLink { get; init; }

    public ulong Offset { get; init; }
    public ulong Info { get; init; }
    public long Addend { get; init; }
}
