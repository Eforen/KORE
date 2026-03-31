using System.Text;
using Kuick.Elf.Models;

namespace Kuick.Elf.Formatting.ReadelfFormatters;

public sealed class SymbolFormatter : IElfFormatter
{
    public string Format(ElfObject elfObject, FormatterOptions? options = null)
    {
        if (elfObject.Symbols.Count == 0 && options?.IncludeEmptyTables != true)
        {
            return string.Empty;
        }

        var builder = new StringBuilder();
        builder.AppendLine("Symbol table:");
        for (var index = 0; index < elfObject.Symbols.Count; index++)
        {
            var symbol = elfObject.Symbols[index];
            builder.AppendLine(
                $"  {index,4}: value=0x{symbol.Value:X16} size={symbol.Size,5} info={symbol.Info,3} shndx={symbol.SectionIndex,3} name={symbol.Name}");
        }

        return builder.ToString();
    }
}
