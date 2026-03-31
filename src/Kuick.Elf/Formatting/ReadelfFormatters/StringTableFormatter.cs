using System.Text;
using Kuick.Elf.Models;

namespace Kuick.Elf.Formatting.ReadelfFormatters;

public sealed class StringTableFormatter : IElfFormatter
{
    public string Format(ElfObject elfObject, FormatterOptions? options = null)
    {
        if (elfObject.StringTableEntries.Count == 0 && options?.IncludeEmptyTables != true)
        {
            return string.Empty;
        }

        var builder = new StringBuilder();
        builder.AppendLine("String table:");
        for (var index = 0; index < elfObject.StringTableEntries.Count; index++)
        {
            builder.AppendLine($"  [{index,4}] {elfObject.StringTableEntries[index]}");
        }

        return builder.ToString();
    }
}
