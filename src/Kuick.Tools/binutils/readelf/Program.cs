using Kuick.Tools.Commands;
using Kuick.Tools.Commands.Options;
using Kuick.Tools.Versioning;

if (args.Length == 0 || args[0] is "--help")
{
    PrintHelp();
    return 0;
}

if (args[0] is "-v" or "--version")
{
    var version = ToolVersion.Load();
    Console.WriteLine($"KORE readelf (KORE Binutils) {version.ToSemanticString()}");
    Console.WriteLine("Copyright (C) 2026 Eforen (Ariel Lothlorien)");
    Console.WriteLine("This program is free software; you may redistribute it under the terms of");
    Console.WriteLine("the GNU General Public License version 3.");
    Console.WriteLine("This program has absolutely no warranty.");
    return 0;
}

var readelfArgs = string.Equals(args[0], "readelf", StringComparison.OrdinalIgnoreCase)
    ? args.Skip(1).ToArray()
    : args;

var parseResult = ParseReadelfOptions(readelfArgs);
if (!parseResult.Success || parseResult.Options is null)
{
    Console.Error.WriteLine(parseResult.ErrorMessage);
    Console.WriteLine();
    PrintHelp();
    return 1;
}

return new ReadelfCommand().Execute(parseResult.Options);

static void PrintHelp()
{
    Console.WriteLine("Kuick.Tools");
    Console.WriteLine("Usage:");
    Console.WriteLine("  kuick-readelf <input-path> [-h|--file-header] [-l|--program-headers] [-S|--section-headers|--sections] [-s|--symbols|--syms] [-r|--relocations|--relocs] [-d|--dynamic-section|--dynamic] [-V|--version-info] [-A|--arch-specific] [--header] [--include-empty] [--verbose]");
    Console.WriteLine("  kuick-readelf readelf <input-path> [options]");
    Console.WriteLine("  kuick-readelf --version");
    Console.WriteLine("  kuick-readelf --help");
}

static (bool Success, ReadelfOptions? Options, string ErrorMessage) ParseReadelfOptions(string[] args)
{
    if (args.Length == 0)
    {
        return (false, null, "Missing input path.");
    }

    string? inputPath = null;
    var fileHeaderOnly = false;
    var programHeadersOnly = false;
    var sectionHeadersOnly = false;
    var symbolsOnly = false;
    var relocationsOnly = false;
    var dynamicSectionOnly = false;
    var versionInfoOnly = false;
    var archSpecificOnly = false;
    var includeEmpty = false;
    var verbose = false;

    foreach (var arg in args)
    {
        switch (arg)
        {
            case "-h":
            case "--file-header":
            case "--header":
                fileHeaderOnly = true;
                break;
            case "-l":
            case "--program-headers":
                programHeadersOnly = true;
                break;
            case "-S":
            case "--section-headers":
            case "--sections":
                sectionHeadersOnly = true;
                break;
            case "-s":
            case "--symbols":
            case "--syms":
                symbolsOnly = true;
                break;
            case "-r":
            case "--relocations":
            case "--relocs":
                relocationsOnly = true;
                break;
            case "-d":
            case "--dynamic-section":
            case "--dynamic":
                dynamicSectionOnly = true;
                break;
            case "-V":
            case "--version-info":
                versionInfoOnly = true;
                break;
            case "-A":
            case "--arch-specific":
                archSpecificOnly = true;
                break;
            case "--include-empty":
                includeEmpty = true;
                break;
            case "--verbose":
                verbose = true;
                break;
            default:
                if (arg.StartsWith("-", StringComparison.Ordinal))
                {
                    return (false, null, $"Unknown option '{arg}'.");
                }

                if (inputPath is not null)
                {
                    return (false, null, "Only one input path is supported.");
                }

                inputPath = arg;
                break;
        }
    }

    if (string.IsNullOrWhiteSpace(inputPath))
    {
        return (false, null, "Missing input path.");
    }

    return (true, new ReadelfOptions
    {
        InputPath = inputPath,
        FileHeaderOnly = fileHeaderOnly,
        ProgramHeadersOnly = programHeadersOnly,
        SectionHeadersOnly = sectionHeadersOnly,
        SymbolsOnly = symbolsOnly,
        RelocationsOnly = relocationsOnly,
        DynamicSectionOnly = dynamicSectionOnly,
        VersionInfoOnly = versionInfoOnly,
        ArchSpecificOnly = archSpecificOnly,
        IncludeEmpty = includeEmpty,
        Verbose = verbose
    }, string.Empty);
}
