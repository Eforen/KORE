namespace Kuick.Tools.Versioning;

public sealed class ToolVersion
{
    public required string Major { get; init; }
    public required string Minor { get; init; }
    public required string Patch { get; init; }
    public required string Compile { get; init; }

    public static ToolVersion Load()
    {
        var versionDir = Path.Combine(AppContext.BaseDirectory, "Version");
        return new ToolVersion
        {
            Major = ReadPart(versionDir, "major.txt", "0"),
            Minor = ReadPart(versionDir, "minor.txt", "1"),
            Patch = ReadPart(versionDir, "patch.txt", "0"),
            Compile = ReadPart(versionDir, "compile.txt", "0")
        };
    }

    public string ToSemanticString()
    {
        return $"{Major}.{Minor}.{Patch}.{Compile}";
    }

    private static string ReadPart(string directory, string fileName, string fallback)
    {
        var path = Path.Combine(directory, fileName);
        if (!File.Exists(path))
        {
            return fallback;
        }

        var value = File.ReadAllText(path).Trim();
        return string.IsNullOrWhiteSpace(value) ? fallback : value;
    }
}
