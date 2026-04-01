namespace Kuick.Elf.Models;

public sealed class RelocationEntry
{
    /// <summary>ELF section that contained this record (e.g. <c>.rela.text</c>).</summary>
    public string SectionName { get; init; } = string.Empty;

    /// <summary>Section index of the symbol table to use for <c>r_sym</c> (<c>sh_link</c> of this REL/RELA section).</summary>
    public uint SymTabLink { get; init; }

    /// <summary>Section index of the section to which relocations apply (<c>sh_info</c> of this REL/RELA section); <c>0</c> if unspecified (use VMA matching for dynamic objects).</summary>
    public uint TargetSectionIndex { get; init; }

    public ulong Offset { get; init; }
    public ulong Info { get; init; }
    public long Addend { get; init; }
}
