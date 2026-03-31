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
    public bool IncludeEmpty { get; init; }
    public bool Verbose { get; init; }
}
