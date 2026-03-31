namespace Kuick.Tools.Formatters;

/// <summary>
/// Which views to emit (single-request vs full dump).
/// </summary>
public enum ReadelfDisplayMode
{
    /// <summary>Header, program headers (if any), sections, symbols, etc.</summary>
    Default,

    /// <summary><c>-h</c> only.</summary>
    FileHeaderOnly,

    /// <summary><c>-l</c> only.</summary>
    ProgramHeadersOnly,

    /// <summary><c>-h</c> and <c>-l</c> together, nothing else.</summary>
    FileHeaderAndProgramHeaders
}
