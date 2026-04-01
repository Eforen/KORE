namespace Kuick.Elf.Models;

public sealed class ElfObject
{
    public required ElfHeader Header { get; init; }
    public IList<ProgramHeader> ProgramHeaders { get; } = new List<ProgramHeader>();
    public IList<Section> Sections { get; } = new List<Section>();
    public IList<Symbol> Symbols { get; } = new List<Symbol>();
    public IList<RelocationEntry> Relocations { get; } = new List<RelocationEntry>();
    public IList<DynamicEntry> DynamicEntries { get; } = new List<DynamicEntry>();
    /// <summary>Bytes of <c>.dynstr</c> when a <c>SHT_DYNAMIC</c> section was loaded (for resolving string-valued tags).</summary>
    public byte[] DynamicStrtab { get; set; } = Array.Empty<byte>();
    public GnuVersionInfo GnuVersion { get; } = new();
    /// <summary>Raw bytes of <c>.riscv.attributes</c> (<c>SHT_RISCV_ATTRIBUTES</c>) when present.</summary>
    public byte[]? RiscvAttributes { get; set; }
    /// <summary>Raw bytes of <c>.gnu.hash</c> (<c>SHT_GNU_HASH</c>) when present.</summary>
    public byte[]? GnuHash { get; set; }
    /// <summary>Raw bytes of the first <c>.got</c> or <c>.got.plt</c> section when present (for <c>--got-contents</c>).</summary>
    public byte[]? GotSectionBytes { get; set; }
    public IList<string> StringTableEntries { get; } = new List<string>();
}
