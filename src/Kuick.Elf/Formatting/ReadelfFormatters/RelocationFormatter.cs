using System.Text;
using Kuick.Elf.Models;

namespace Kuick.Elf.Formatting.ReadelfFormatters;

public sealed class RelocationFormatter : IElfFormatter
{
    public string Format(ElfObject elfObject, FormatterOptions? options = null)
    {
        if (elfObject.Relocations.Count == 0 && options?.IncludeEmptyTables != true)
        {
            return string.Empty;
        }

        var builder = new StringBuilder();
        builder.AppendLine("Relocation section:");
        for (var index = 0; index < elfObject.Relocations.Count; index++)
        {
            var relocation = elfObject.Relocations[index];
            builder.AppendLine(
                $"  {index,4}: offset=0x{relocation.Offset:X16} info=0x{relocation.Info:X16} addend={relocation.Addend}");
        }

        return builder.ToString();
    }
}
