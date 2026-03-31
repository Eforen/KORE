using Kuick.Tools.Commands.Options;

namespace Kuick.Tools.Formatters;

internal static class ReadelfDisplayModeHelper
{
    public static ReadelfDisplayMode FromOptions(ReadelfOptions o)
    {
        if (!o.FileHeaderOnly && !o.ProgramHeadersOnly && !o.SectionHeadersOnly)
        {
            return ReadelfDisplayMode.Default;
        }

        var mask = (o.FileHeaderOnly ? 1 : 0) | (o.ProgramHeadersOnly ? 2 : 0) | (o.SectionHeadersOnly ? 4 : 0);
        return mask switch
        {
            1 => ReadelfDisplayMode.FileHeaderOnly,
            2 => ReadelfDisplayMode.ProgramHeadersOnly,
            4 => ReadelfDisplayMode.SectionHeadersOnly,
            3 => ReadelfDisplayMode.FileHeaderAndProgramHeaders,
            5 => ReadelfDisplayMode.FileHeaderAndSectionHeaders,
            6 => ReadelfDisplayMode.ProgramHeaderAndSectionHeaders,
            7 => ReadelfDisplayMode.FileHeaderProgramHeadersAndSectionHeaders,
            _ => ReadelfDisplayMode.Default
        };
    }
}
