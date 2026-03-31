using System.Globalization;
using System.Text;
using Kuick.Elf.Models;

namespace Kuick.Elf.Formatting.ReadelfFormatters;

/// <summary>
/// KORE-style program header listing (segment table).
/// </summary>
public sealed class ProgramHeaderFormatter : IElfFormatter
{
    public string Format(ElfObject elfObject, FormatterOptions? options = null)
    {
        var list = elfObject.ProgramHeaders;
        if (list.Count == 0 && options?.IncludeEmptyTables != true)
        {
            return string.Empty;
        }

        var b = new StringBuilder();
        b.AppendLine("Program headers");
        b.AppendLine(new string('─', 48));

        if (list.Count == 0)
        {
            b.AppendLine("  (none — common for relocatable .o objects without loadable segments)");
            return b.ToString();
        }

        b.AppendLine(
            "  Type           Offset     VirtAddr           PhysAddr           FileSize MemSize  Align  Flags");
        for (var i = 0; i < list.Count; i++)
        {
            var p = list[i];
            b.Append("  ");
            b.AppendFormat(CultureInfo.InvariantCulture, "[{0}] {1,-14} ", i, FormatPtType(p.Type));
            b.AppendFormat(CultureInfo.InvariantCulture, "off=0x{0:X} ", p.Offset);
            b.AppendFormat(CultureInfo.InvariantCulture, "vaddr=0x{0:X} ", p.VirtualAddress);
            b.AppendFormat(CultureInfo.InvariantCulture, "paddr=0x{0:X} ", p.PhysicalAddress);
            b.AppendFormat(CultureInfo.InvariantCulture, "filesz=0x{0:X} ", p.FileSize);
            b.AppendFormat(CultureInfo.InvariantCulture, "memsz=0x{0:X} ", p.MemorySize);
            b.AppendFormat(CultureInfo.InvariantCulture, "align=0x{0:X} ", p.Align);
            b.AppendFormat(CultureInfo.InvariantCulture, "flags=0x{0:X} ({1})", p.Flags, FormatPfFlags(p.Flags));
            b.AppendLine();
        }

        return b.ToString();
    }

    private static string FormatPtType(uint t) =>
        t switch
        {
            0 => "NULL",
            1 => "LOAD",
            2 => "DYNAMIC",
            3 => "INTERP",
            4 => "NOTE",
            5 => "SHLIB",
            6 => "PHDR",
            7 => "TLS",
            0x60000000 => "LOOS",
            0x6FFFFFFF => "HIOS",
            0x70000000 => "LOPROC",
            0x7FFFFFFF => "HIPROC",
            _ => $"0x{t:X8}"
        };

    private static string FormatPfFlags(uint f)
    {
        var s = new StringBuilder(3);
        s.Append((f & 4) != 0 ? 'R' : ' ');
        s.Append((f & 2) != 0 ? 'W' : ' ');
        s.Append((f & 1) != 0 ? 'X' : ' ');
        return s.ToString();
    }
}
