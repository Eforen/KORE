using Kuick.Elf.Formatting;
using Kuick.Elf.IO;
using Kuick.Tools.Commands.Options;
using Kuick.Tools.Formatters;

namespace Kuick.Tools.Commands;

public sealed class ReadelfCommand
{
    private readonly ElfLoader _loader;
    private readonly ReadelfOutputWriter _writer;

    public ReadelfCommand()
    {
        _loader = new ElfLoader();
        _writer = new ReadelfOutputWriter();
    }

    public int Execute(ReadelfOptions options)
    {
        try
        {
            var elfObject = _loader.Load(options.InputPath);
            var output = _writer.Format(
                elfObject,
                options,
                new FormatterOptions
                {
                    IncludeEmptyTables = options.IncludeEmpty || options.ProgramHeadersOnly || options.SectionHeadersOnly
                        || options.SymbolsOnly || options.RelocationsOnly || options.DynamicSectionOnly
                        || options.VersionInfoOnly || options.ArchSpecificOnly || options.HistogramOnly
                        || options.GotContentsOnly,
                    Verbose = options.Verbose
                });

            Console.WriteLine(output);
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"readelf error: {ex.Message}");
            return 1;
        }
    }
}
