using System.Text;
using Kuick.Elf.Models;

namespace Kuick.Elf.Formatting.ReadelfFormatters;

public sealed class HeaderFormatter : IElfFormatter
{
    public string Format(ElfObject elfObject, FormatterOptions? options = null)
    {
        var h = elfObject.Header;
        var builder = new StringBuilder();
        builder.AppendLine("ELF Header:");
        builder.AppendLine($"  Magic:                             {BitConverter.ToString(h.Magic)}");
        builder.AppendLine($"  Class:                             {h.ElfClass}");
        builder.AppendLine($"  Data:                              {h.DataEncoding}");
        builder.AppendLine($"  Version:                           {h.Version}");
        builder.AppendLine($"  OS/ABI:                            {h.OsAbi}");
        builder.AppendLine($"  ABI Version:                       {h.AbiVersion}");
        builder.AppendLine($"  Type:                              0x{h.Type:X4}");
        builder.AppendLine($"  Machine:                           0x{h.Machine:X4}");
        builder.AppendLine($"  Entry point address:               0x{h.EntryPoint:X}");
        return builder.ToString();
    }
}
