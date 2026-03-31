using System.Globalization;
using System.Text;
using Kuick.Elf.Models;

namespace Kuick.Elf.Formatting.ReadelfFormatters;

/// <summary>
/// KORE-style section header listing (similar intent to GNU <c>readelf -S</c>).
/// </summary>
public sealed class SectionFormatter : IElfFormatter
{
    private const int ColName = 18;
    private const int ColType = 18;
    private const int ColAddr = 18;
    private const int ColOff = 10;
    private const int ColSize = 10;
    private const int ColEs = 6;
    private const int ColFlg = 4;
    private const int ColLk = 3;
    private const int ColInf = 4;
    private const int ColAlign = 10;

    /// <summary>Spaces between fixed-width columns so hex values do not visually merge.</summary>
    private const string ColSep = "  ";

    public string Format(ElfObject elfObject, FormatterOptions? options = null)
    {
        var h = elfObject.Header;
        var list = elfObject.Sections;
        if (list.Count == 0 && options?.IncludeEmptyTables != true)
        {
            return string.Empty;
        }

        var b = new StringBuilder();
        b.AppendLine("Section headers");
        b.AppendLine(new string('─', 120));

        if (list.Count == 0)
        {
            b.AppendLine("  (none — e_shnum == 0 or no section table)");
            return b.ToString();
        }

        var is64 = h.ElfClass == 2;

        b.AppendLine(
            $"  Table offset: 0x{h.SectionHeaderOffset:X}, entries: {h.SectionHeaderCount}, entry size: {h.SectionHeaderEntrySize} bytes");
        b.AppendLine();
        b.AppendLine(
            string.Concat(
                "  [Nr] ",
                PadRight("Name", ColName),
                ColSep,
                PadRight("Type", ColType),
                ColSep,
                PadLeft("Address", ColAddr),
                ColSep,
                PadLeft("Offset", ColOff),
                ColSep,
                PadLeft("Size", ColSize),
                ColSep,
                PadLeft("ES", ColEs),
                ColSep,
                PadLeft("Flg", ColFlg),
                ColSep,
                PadLeft("Lk", ColLk),
                ColSep,
                PadLeft("Inf", ColInf),
                ColSep,
                PadLeft("Align", ColAlign)));

        for (var i = 0; i < list.Count; i++)
        {
            var s = list[i];
            b.Append("  [");
            b.AppendFormat(CultureInfo.InvariantCulture, "{0,2}] ", i);
            b.Append(PadRight(Trim(s.Name, ColName), ColName));
            b.Append(ColSep);
            b.Append(PadRight(Trim(FormatShType(s.Type), ColType), ColType));
            b.Append(ColSep);
            b.Append(PadLeft(HexAddr(s.Address, is64), ColAddr));
            b.Append(ColSep);
            b.Append(PadLeft(HexU32(s.Offset), ColOff));
            b.Append(ColSep);
            b.Append(PadLeft(HexU32(s.Size), ColSize));
            b.Append(ColSep);
            b.Append(PadLeft(HexU32(s.EntrySize), ColEs));
            b.Append(ColSep);
            b.Append(PadLeft(FormatShFlags(s.Flags), ColFlg));
            b.Append(ColSep);
            b.Append(PadLeft(s.Link.ToString(CultureInfo.InvariantCulture), ColLk));
            b.Append(ColSep);
            b.Append(PadLeft(s.Info.ToString(CultureInfo.InvariantCulture), ColInf));
            b.Append(ColSep);
            b.Append(PadLeft(HexU32(s.AddressAlign), ColAlign));
            b.AppendLine();
        }

        return b.ToString();
    }

    private static string Trim(string s, int maxLen) =>
        s.Length <= maxLen ? s : s[..maxLen];

    private static string PadRight(string s, int width)
    {
        if (s.Length >= width)
        {
            return s;
        }

        return s + new string(' ', width - s.Length);
    }

    private static string PadLeft(string s, int width)
    {
        if (s.Length >= width)
        {
            return s;
        }

        return new string(' ', width - s.Length) + s;
    }

    private static string HexAddr(ulong v, bool elf64) =>
        elf64 ? $"0x{v:X16}" : $"0x{v:X8}";

    private static string HexU32(ulong v) => $"0x{v:X8}";

    private static string FormatShType(uint t) =>
        t switch
        {
            0 => "NULL",
            1 => "PROGBITS",
            2 => "SYMTAB",
            3 => "STRTAB",
            4 => "RELA",
            5 => "HASH",
            6 => "DYNAMIC",
            7 => "NOTE",
            8 => "NOBITS",
            9 => "REL",
            10 => "SHLIB",
            11 => "DYNSYM",
            14 => "INIT_ARRAY",
            15 => "FINI_ARRAY",
            16 => "PREINIT_ARRAY",
            17 => "GROUP",
            18 => "SYMTAB_SHNDX",
            0x60000000 => "LOOS",
            0x6FFFFFFF => "HIOS",
            0x70000000 => "LOPROC",
            0x7FFFFFFF => "HIPROC",
            _ => $"0x{t:X8}"
        };

    private static string FormatShFlags(ulong f)
    {
        if (f == 0)
        {
            return "-";
        }

        var s = new StringBuilder();
        if ((f & 0x001) != 0)
        {
            s.Append('W');
        }

        if ((f & 0x002) != 0)
        {
            s.Append('A');
        }

        if ((f & 0x004) != 0)
        {
            s.Append('X');
        }

        if ((f & 0x010) != 0)
        {
            s.Append('M');
        }

        if ((f & 0x020) != 0)
        {
            s.Append('S');
        }

        if ((f & 0x040) != 0)
        {
            s.Append('I');
        }

        if ((f & 0x080) != 0)
        {
            s.Append('L');
        }

        if ((f & 0x200) != 0)
        {
            s.Append('G');
        }

        if ((f & 0x400) != 0)
        {
            s.Append('T');
        }

        return s.Length == 0 ? $"0x{f:X}" : s.ToString();
    }
}
