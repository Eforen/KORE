using System.Text;
using Kuick.Elf.Models;

namespace Kuick.Elf.Formatting.ReadelfFormatters;

public sealed class SectionFormatter : IElfFormatter
{
    public string Format(ElfObject elfObject, FormatterOptions? options = null)
    {
        if (elfObject.Sections.Count == 0 && options?.IncludeEmptyTables != true)
        {
            return string.Empty;
        }

        var builder = new StringBuilder();
        builder.AppendLine("Section Headers:");
        for (var index = 0; index < elfObject.Sections.Count; index++)
        {
            var section = elfObject.Sections[index];
            builder.AppendLine(
                $"  [{index,2}] {section.Name,-16} type=0x{section.Type:X8} addr=0x{section.Address:X} off=0x{section.Offset:X} size=0x{section.Size:X}");
        }

        return builder.ToString();
    }
}
