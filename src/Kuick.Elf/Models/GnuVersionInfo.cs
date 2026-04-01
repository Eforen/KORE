namespace Kuick.Elf.Models;

/// <summary>Parsed GNU symbol-versioning sections (<c>.gnu.version</c>, <c>.gnu.version_d</c>, <c>.gnu.version_r</c>).</summary>
public sealed class GnuVersionInfo
{
    public VersymSection? Versym { get; set; }
    public IList<VerneedSection> Verneed { get; } = new List<VerneedSection>();
    public IList<VerdefSection> Verdef { get; } = new List<VerdefSection>();
}

public sealed class VersymSection
{
    public required string Name { get; init; }
    public ulong Address { get; init; }
    public ulong Offset { get; init; }
    public required string DynsymLinkName { get; init; }
    public required ushort[] Entries { get; init; }
}

public sealed class VerneedSection
{
    public required string Name { get; init; }
    public ulong Address { get; init; }
    public ulong Offset { get; init; }
    public required string DynstrLinkName { get; init; }
    public IList<VerneedChain> Chains { get; } = new List<VerneedChain>();
}

public sealed class VerneedChain
{
    public ulong HeaderOffsetInSection { get; init; }
    public ushort VnVersion { get; init; }
    public ushort VnCount { get; init; }
    public required string FileName { get; init; }
    public IList<VernauxEntry> Aux { get; } = new List<VernauxEntry>();
}

public sealed class VernauxEntry
{
    public ulong OffsetInSection { get; init; }
    public uint Hash { get; init; }
    public ushort Flags { get; init; }
    public ushort VersionIndex { get; init; }
    public required string Name { get; init; }
}

public sealed class VerdefSection
{
    public required string Name { get; init; }
    public ulong Address { get; init; }
    public ulong Offset { get; init; }
    public required string DynstrLinkName { get; init; }
    public IList<VerdefEntry> Entries { get; } = new List<VerdefEntry>();
}

public sealed class VerdefEntry
{
    public ulong VerdefOffsetInSection { get; init; }
    public ushort VdVersion { get; init; }
    public ushort VdFlags { get; init; }
    public ushort VdNdx { get; init; }
    public ushort VdCount { get; init; }
    public required string Name { get; init; }
    public IList<VerdefParentLine> Parents { get; } = new List<VerdefParentLine>();
}

public sealed class VerdefParentLine
{
    public ulong OffsetInSection { get; init; }
    public int ParentIndex { get; init; }
    public required string Name { get; init; }
}
