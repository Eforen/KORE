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
    public IList<string> StringTableEntries { get; } = new List<string>();
}
