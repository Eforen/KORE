namespace Kuick.Elf.Models;

public sealed class ElfObject
{
    public required ElfHeader Header { get; init; }
    public IList<Section> Sections { get; } = new List<Section>();
    public IList<Symbol> Symbols { get; } = new List<Symbol>();
    public IList<RelocationEntry> Relocations { get; } = new List<RelocationEntry>();
    public IList<string> StringTableEntries { get; } = new List<string>();
}
