using Kuick.Elf.Formatting;
using Kuick.Elf.Formatting.ReadelfFormatters;
using Kuick.Elf.Models;

namespace Kuick.Tools.Formatters;

public sealed class ReadelfOutputWriter
{
    private readonly HeaderFormatter _headerFormatter = new();
    private readonly ProgramHeaderFormatter _programHeaderFormatter = new();
    private readonly SectionFormatter _sectionFormatter = new();
    private readonly SymbolFormatter _symbolFormatter = new();
    private readonly RelocationFormatter _relocationFormatter = new();
    private readonly StringTableFormatter _stringTableFormatter = new();

    public string Format(ElfObject elfObject, ReadelfDisplayMode mode, FormatterOptions options)
    {
        return mode switch
        {
            ReadelfDisplayMode.FileHeaderOnly => _headerFormatter.Format(elfObject, options),
            ReadelfDisplayMode.ProgramHeadersOnly => _programHeaderFormatter.Format(elfObject, options),
            ReadelfDisplayMode.FileHeaderAndProgramHeaders => JoinNonEmpty(
                _headerFormatter.Format(elfObject, options),
                _programHeaderFormatter.Format(elfObject, options)),
            _ => FormatDefault(elfObject, options)
        };
    }

    private static string JoinNonEmpty(params string[] parts) =>
        string.Join(Environment.NewLine, parts.Where(static s => !string.IsNullOrWhiteSpace(s)));

    private string FormatDefault(ElfObject elfObject, FormatterOptions options)
    {
        var sections = new List<string>
        {
            _headerFormatter.Format(elfObject, options)
        };

        var programBlock = _programHeaderFormatter.Format(elfObject, options);
        if (!string.IsNullOrWhiteSpace(programBlock))
        {
            sections.Add(programBlock);
        }

        sections.Add(_sectionFormatter.Format(elfObject, options));
        sections.Add(_symbolFormatter.Format(elfObject, options));
        sections.Add(_relocationFormatter.Format(elfObject, options));
        sections.Add(_stringTableFormatter.Format(elfObject, options));

        return string.Join(Environment.NewLine, sections.Where(static s => !string.IsNullOrWhiteSpace(s)));
    }
}
