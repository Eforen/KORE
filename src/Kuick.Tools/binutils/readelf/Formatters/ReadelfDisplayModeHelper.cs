using Kuick.Tools.Commands.Options;

namespace Kuick.Tools.Formatters;

internal static class ReadelfDisplayModeHelper
{
    public static ReadelfDisplayMode FromOptions(ReadelfOptions o)
    {
        if (o.FileHeaderOnly && o.ProgramHeadersOnly)
        {
            return ReadelfDisplayMode.FileHeaderAndProgramHeaders;
        }

        if (o.FileHeaderOnly)
        {
            return ReadelfDisplayMode.FileHeaderOnly;
        }

        if (o.ProgramHeadersOnly)
        {
            return ReadelfDisplayMode.ProgramHeadersOnly;
        }

        return ReadelfDisplayMode.Default;
    }
}
