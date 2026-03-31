namespace Kuick.Tools.Commands.Options;

public sealed class ReadelfOptions
{
    public required string InputPath { get; init; }
    public bool HeaderOnly { get; init; }
    public bool IncludeEmpty { get; init; }
    public bool Verbose { get; init; }
}
