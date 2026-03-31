using Kuick.Elf.Formatting;
using Kuick.Elf.Formatting.ReadelfFormatters;
using Kuick.Elf.Models;

namespace Kuick.Tools.Formatters;

public sealed class ReadelfOutputWriter
{
    private readonly HeaderFormatter _headerFormatter = new();
    private readonly SectionFormatter _sectionFormatter = new();
    private readonly SymbolFormatter _symbolFormatter = new();
    private readonly RelocationFormatter _relocationFormatter = new();
    private readonly StringTableFormatter _stringTableFormatter = new();

    public string Format(ElfObject elfObject, bool fileHeaderOnly, FormatterOptions options)
    {
        var sections = new List<string>
        {
            _headerFormatter.Format(elfObject, options)
        };

        if (!fileHeaderOnly)
        {
            sections.Add(_sectionFormatter.Format(elfObject, options));
            sections.Add(_symbolFormatter.Format(elfObject, options));
            sections.Add(_relocationFormatter.Format(elfObject, options));
            sections.Add(_stringTableFormatter.Format(elfObject, options));
        }

        return string.Join(Environment.NewLine, sections.Where(static s => !string.IsNullOrWhiteSpace(s)));
    }
}
