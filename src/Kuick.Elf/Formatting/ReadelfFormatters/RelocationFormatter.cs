using System.Globalization;
using System.Text;
using Kuick.Elf.Models;

namespace Kuick.Elf.Formatting.ReadelfFormatters;

/// <summary>
/// KORE-style relocation listing (similar intent to GNU <c>readelf -r</c>).
/// </summary>
public sealed class RelocationFormatter : IElfFormatter
{
    private const ushort EmRiscv = 0xF3;
    private const string ColSep = "  ";

    public string Format(ElfObject elfObject, FormatterOptions? options = null)
    {
        if (elfObject.Relocations.Count == 0 && options?.IncludeEmptyTables != true)
        {
            return string.Empty;
        }

        var b = new StringBuilder();
        b.AppendLine("Relocation sections");
        b.AppendLine(new string('─', 96));

        if (elfObject.Relocations.Count == 0)
        {
            b.AppendLine("  (none — no REL/RELA sections or empty)");
            return b.ToString();
        }

        var is64 = elfObject.Header.ElfClass == 2;
        var machine = elfObject.Header.Machine;
        var ordered = elfObject.Relocations
            .Select((r, fileOrder) => (r, fileOrder))
            .GroupBy(x => x.r.SectionName, StringComparer.Ordinal)
            .OrderBy(g => g.Min(x => x.fileOrder));

        foreach (var group in ordered)
        {
            var label = string.IsNullOrEmpty(group.Key) ? "(default)" : group.Key;
            var rows = group.OrderBy(x => x.fileOrder).ToList();
            b.AppendLine($"  Section '{label}'  ({rows.Count} entr{(rows.Count == 1 ? "y" : "ies")})");
            b.AppendLine(
                string.Concat(
                    "    ",
                    PadRight("Num", 5),
                    ColSep,
                    PadLeft("Offset", 18),
                    ColSep,
                    PadLeft("Info", 18),
                    ColSep,
                    PadLeft("Type", 22),
                    ColSep,
                    "Symbol's Name + Addend"));

            var num = 0;
            foreach (var (r, _) in rows)
            {
                var (symIdx, relType) = RelMacros(r.Info, is64);
                var typeStr = FormatRelType(machine, relType);
                var namePlus = FormatNamePlusAddend(
                    ResolveSymbolName(elfObject, r.SymTabLink, symIdx),
                    r.Addend);
                b.Append("    ");
                b.Append(PadRight(num.ToString(CultureInfo.InvariantCulture), 5));
                b.Append(ColSep);
                b.Append(PadLeft(is64 ? HexU64(r.Offset) : HexU32(r.Offset), 18));
                b.Append(ColSep);
                b.Append(PadLeft(is64 ? HexU64(r.Info) : HexU32(r.Info), 18));
                b.Append(ColSep);
                b.Append(PadLeft(typeStr, 22));
                b.Append(ColSep);
                b.Append(namePlus);
                b.AppendLine();
                num++;
            }

            b.AppendLine();
        }

        return b.ToString().TrimEnd() + Environment.NewLine;
    }

    private static string ResolveSymbolName(ElfObject elf, uint symTabSectionIndex, uint symIdx)
    {
        if (symTabSectionIndex >= elf.Sections.Count)
        {
            return string.Empty;
        }

        var tableName = elf.Sections[(int)symTabSectionIndex].Name;
        var i = 0u;
        foreach (var s in elf.Symbols)
        {
            if (s.TableName != tableName)
            {
                continue;
            }

            if (i == symIdx)
            {
                return s.Name;
            }

            i++;
        }

        return $"?{symIdx}";
    }

    private static string FormatNamePlusAddend(string symbolName, long addend)
    {
        if (string.IsNullOrEmpty(symbolName))
        {
            return $"+ {addend}";
        }

        return $"{symbolName} + {addend}";
    }

    private static (uint Sym, uint Type) RelMacros(ulong info, bool is64)
    {
        if (is64)
        {
            return ((uint)(info >> 32), (uint)(info & 0xffffffff));
        }

        var low = (uint)info;
        return (low >> 8, low & 0xff);
    }

    private static string FormatRelType(ushort machine, uint type)
    {
        if (machine == EmRiscv)
        {
            return FormatRiscvRelType(type);
        }

        return $"0x{type:X}";
    }

    private static string FormatRiscvRelType(uint t) =>
        t switch
        {
            0 => "R_RISCV_NONE",
            1 => "R_RISCV_32",
            2 => "R_RISCV_64",
            3 => "R_RISCV_RELATIVE",
            4 => "R_RISCV_COPY",
            5 => "R_RISCV_JUMP_SLOT",
            6 => "R_RISCV_TLS_DTPMOD32",
            7 => "R_RISCV_TLS_DTPMOD64",
            8 => "R_RISCV_TLS_DTPREL32",
            9 => "R_RISCV_TLS_DTPREL64",
            10 => "R_RISCV_TLS_TPREL32",
            11 => "R_RISCV_TLS_TPREL64",
            16 => "R_RISCV_BRANCH",
            17 => "R_RISCV_JAL",
            18 => "R_RISCV_CALL",
            19 => "R_RISCV_CALL_PLT",
            20 => "R_RISCV_GOT_HI20",
            21 => "R_RISCV_TLS_GOT_HI20",
            22 => "R_RISCV_TLS_GD_HI20",
            23 => "R_RISCV_PCREL_HI20",
            24 => "R_RISCV_PCREL_LO12_I",
            25 => "R_RISCV_PCREL_LO12_S",
            26 => "R_RISCV_HI20",
            27 => "R_RISCV_LO12_I",
            28 => "R_RISCV_LO12_S",
            29 => "R_RISCV_TPREL_HI20",
            30 => "R_RISCV_TPREL_ADD_LO12_I",
            31 => "R_RISCV_TPREL_ADD_LO12_S",
            32 => "R_RISCV_TPREL_LO12_I",
            33 => "R_RISCV_TPREL_LO12_S",
            34 => "R_RISCV_TLS_IE_HI20",
            35 => "R_RISCV_TLS_IE_LO12_I",
            36 => "R_RISCV_TLS_IE_LO12_S",
            37 => "R_RISCV_TLS_IE_ADD",
            40 => "R_RISCV_ALIGN",
            41 => "R_RISCV_RVC_BRANCH",
            42 => "R_RISCV_RVC_JUMP",
            43 => "R_RISCV_SUB6",
            44 => "R_RISCV_SET6",
            45 => "R_RISCV_SET8",
            46 => "R_RISCV_SET16",
            47 => "R_RISCV_SET32",
            48 => "R_RISCV_32_PCREL",
            51 => "R_RISCV_SET_ULEB128",
            52 => "R_RISCV_SUB_ULEB128",
            53 => "R_RISCV_TLSDESC_HI20",
            54 => "R_RISCV_TLSDESC_LOAD_LO12",
            55 => "R_RISCV_TLSDESC_ADD_LO12",
            56 => "R_RISCV_TLSDESC_CALL",
            57 => "R_RISCV_SUBW",
            58 => "R_RISCV_ADDW",
            59 => "R_RISCV_GNU_VTINHERIT",
            60 => "R_RISCV_GNU_VTENTRY",
            _ => $"R_RISCV_{t}"
        };

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
}
