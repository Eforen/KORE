namespace Kuick.Tools.Commands.Options;

public sealed class ReadelfOptions
{
    public required string InputPath { get; init; }
    /// <summary>Only print ELF file header (-h, --file-header, --header).</summary>
    public bool FileHeaderOnly { get; init; }
    /// <summary>Only print program headers / segment table (-l, --program-headers).</summary>
    public bool ProgramHeadersOnly { get; init; }
    /// <summary>Only print section header table (-S, --section-headers, --sections).</summary>
    public bool SectionHeadersOnly { get; init; }
    /// <summary>Only print symbol tables (-s, --symbols, --syms).</summary>
    public bool SymbolsOnly { get; init; }
    /// <summary>Only print relocation sections (-r, --relocations, --relocs).</summary>
    public bool RelocationsOnly { get; init; }
    /// <summary>Only print the dynamic section (-d, --dynamic-section, --dynamic).</summary>
    public bool DynamicSectionOnly { get; init; }
    public bool IncludeEmpty { get; init; }
    public bool Verbose { get; init; }
}
