using Kuick.Elf.Formatting;
using Kuick.Elf.Formatting.ReadelfFormatters;
using Kuick.Elf.Models;
using Kuick.Tools.Commands.Options;

namespace Kuick.Tools.Formatters;

public sealed class ReadelfOutputWriter
{
    private const int MaskFileHeader = 1;
    private const int MaskProgramHeaders = 2;
    private const int MaskSectionHeaders = 4;
    private const int MaskSymbols = 8;
    private const int MaskRelocations = 16;
    private const int MaskDynamic = 32;

    private readonly HeaderFormatter _headerFormatter = new();
    private readonly ProgramHeaderFormatter _programHeaderFormatter = new();
    private readonly SectionFormatter _sectionFormatter = new();
    private readonly SymbolFormatter _symbolFormatter = new();
    private readonly RelocationFormatter _relocationFormatter = new();
    private readonly DynamicFormatter _dynamicFormatter = new();
    private readonly StringTableFormatter _stringTableFormatter = new();

    /// <summary>Bit order: file header, program headers, section headers, symbols, relocations, dynamic (same order as output).</summary>
    public static int GetViewMask(ReadelfOptions o) =>
        (o.FileHeaderOnly ? MaskFileHeader : 0)
        | (o.ProgramHeadersOnly ? MaskProgramHeaders : 0)
        | (o.SectionHeadersOnly ? MaskSectionHeaders : 0)
        | (o.SymbolsOnly ? MaskSymbols : 0)
        | (o.RelocationsOnly ? MaskRelocations : 0)
        | (o.DynamicSectionOnly ? MaskDynamic : 0);

    public string Format(ElfObject elfObject, ReadelfOptions opts, FormatterOptions options)
    {
        var mask = GetViewMask(opts);
        if (mask == 0)
        {
            return FormatDefault(elfObject, options);
        }

        var parts = new List<string>();
        if ((mask & MaskFileHeader) != 0)
        {
            parts.Add(_headerFormatter.Format(elfObject, options));
        }

        if ((mask & MaskProgramHeaders) != 0)
        {
            parts.Add(_programHeaderFormatter.Format(elfObject, options));
        }

        if ((mask & MaskSectionHeaders) != 0)
        {
            parts.Add(_sectionFormatter.Format(elfObject, options));
        }

        if ((mask & MaskSymbols) != 0)
        {
            parts.Add(_symbolFormatter.Format(elfObject, options));
        }

        if ((mask & MaskRelocations) != 0)
        {
            parts.Add(_relocationFormatter.Format(elfObject, options));
        }

        if ((mask & MaskDynamic) != 0)
        {
            parts.Add(_dynamicFormatter.Format(elfObject, options));
        }

        return JoinNonEmpty(parts.ToArray());
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
        sections.Add(_dynamicFormatter.Format(elfObject, options));
        sections.Add(_stringTableFormatter.Format(elfObject, options));

        return string.Join(Environment.NewLine, sections.Where(static s => !string.IsNullOrWhiteSpace(s)));
    }
}
