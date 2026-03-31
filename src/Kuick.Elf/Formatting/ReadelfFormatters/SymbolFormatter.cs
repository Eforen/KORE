using System.Globalization;
using System.Text;
using Kuick.Elf.Models;

namespace Kuick.Elf.Formatting.ReadelfFormatters;

/// <summary>
/// KORE-style symbol table listing (similar intent to GNU <c>readelf -s</c>).
/// </summary>
public sealed class SymbolFormatter : IElfFormatter
{
    private const string ColSep = "  ";

    public string Format(ElfObject elfObject, FormatterOptions? options = null)
    {
        if (elfObject.Symbols.Count == 0 && options?.IncludeEmptyTables != true)
        {
            return string.Empty;
        }

        var b = new StringBuilder();
        b.AppendLine("Symbol tables");
        b.AppendLine(new string('─', 96));

        if (elfObject.Symbols.Count == 0)
        {
            b.AppendLine("  (none — no SYMTAB/DYNSYM or empty)");
            return b.ToString();
        }

        var is64 = elfObject.Header.ElfClass == 2;
        var groups = elfObject.Symbols
            .Select((s, fileOrder) => (s, fileOrder))
            .GroupBy(x => x.s.TableName, StringComparer.Ordinal)
            .OrderBy(g => g.Min(x => x.fileOrder));

        foreach (var group in groups)
        {
            var tableLabel = string.IsNullOrEmpty(group.Key) ? "(default)" : group.Key;
            var rows = group.OrderBy(x => x.fileOrder).ToList();
            b.AppendLine($"  Table '{tableLabel}'  ({rows.Count} entr{(rows.Count == 1 ? "y" : "ies")})");
            b.AppendLine(
                string.Concat(
                    "    ",
                    PadRight("Num", 5),
                    ColSep,
                    PadLeft(is64 ? "Value" : "Value", 18),
                    ColSep,
                    PadLeft("Size", 10),
                    ColSep,
                    PadLeft("Type", 8),
                    ColSep,
                    PadLeft("Bind", 8),
                    ColSep,
                    PadLeft("Ndx", 6),
                    ColSep,
                    "Name"));

            var num = 0;
            foreach (var (symbol, _) in rows)
            {
                b.Append("    ");
                b.Append(PadRight(num.ToString(CultureInfo.InvariantCulture), 5));
                b.Append(ColSep);
                b.Append(PadLeft(is64 ? HexU64(symbol.Value) : HexU32(symbol.Value), 18));
                b.Append(ColSep);
                b.Append(PadLeft(HexU32(symbol.Size), 10));
                b.Append(ColSep);
                b.Append(PadLeft(FormatStType(symbol.Info), 8));
                b.Append(ColSep);
                b.Append(PadLeft(FormatStBind(symbol.Info), 8));
                b.Append(ColSep);
                b.Append(PadLeft(FormatShNdx(symbol.SectionIndex), 6));
                b.Append(ColSep);
                b.Append(GetSymbolDisplayName(symbol, elfObject));
                b.AppendLine();
                num++;
            }

            b.AppendLine();
        }

        return b.ToString().TrimEnd() + Environment.NewLine;
    }

    private static string HexU64(ulong v) => $"0x{v:X16}";

    private static string HexU32(ulong v) => $"0x{v:X8}";

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

    /// <summary>
    /// For STT_SECTION (3), the name in the symbol string table is often empty; GNU readelf
    /// shows the name of the section referenced by <c>st_shndx</c> instead.
    /// </summary>
    private static string GetSymbolDisplayName(Symbol symbol, ElfObject elf)
    {
        if ((symbol.Info & 0xf) != 3)
        {
            return symbol.Name;
        }

        var idx = symbol.SectionIndex;
        if (idx < elf.Sections.Count && !IsReservedShndx(idx))
        {
            return elf.Sections[idx].Name;
        }

        return symbol.Name;
    }

    /// <summary>ELF reserved section indices (SHN_LORESERVE..SHN_HIRESERVE).</summary>
    private static bool IsReservedShndx(ushort shndx) => shndx >= 0xff00;

    private static string FormatStType(byte info)
    {
        var t = info & 0xf;
        return t switch
        {
            0 => "NOTYPE",
            1 => "OBJECT",
            2 => "FUNC",
            3 => "SECTION",
            4 => "FILE",
            5 => "COMMON",
            6 => "TLS",
            10 => "LOOS",
            12 => "HIOS",
            13 => "LOPROC",
            15 => "HIPROC",
            _ => $"0x{t:X}"
        };
    }

    private static string FormatStBind(byte info)
    {
        var bind = info >> 4;
        return bind switch
        {
            0 => "LOCAL",
            1 => "GLOBAL",
            2 => "WEAK",
            10 => "LOOS",
            12 => "HIOS",
            13 => "LOPROC",
            15 => "HIPROC",
            _ => $"0x{bind:X}"
        };
    }

    private static string FormatShNdx(ushort shndx) =>
        shndx switch
        {
            0 => "UND",
            0xfff1 => "ABS",
            0xfff2 => "COM",
            0xff00 => "LOPROC",
            0xff1f => "HIPROC",
            0xff20 => "LOOS",
            0xff3f => "HIOS",
            0xffff => "XINDEX",
            _ => shndx.ToString(CultureInfo.InvariantCulture)
        };
}
