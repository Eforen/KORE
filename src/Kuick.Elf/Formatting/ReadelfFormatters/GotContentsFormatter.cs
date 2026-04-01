using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Kuick.Elf.Models;

namespace Kuick.Elf.Formatting.ReadelfFormatters;

/// <summary>
/// GOT slot listing (similar intent to GNU <c>readelf --got-contents</c>).
/// </summary>
public sealed class GotContentsFormatter : IElfFormatter
{
    private const ushort EtRel = 1;
    private const ushort EmRiscv = 0xF3;
    private const ushort Em386 = 3;
    private const ushort EmX8664 = 62;
    private const string ColSep = "  ";

    public string Format(ElfObject elfObject, FormatterOptions? options = null)
    {
        var gotSec = SelectGotSection(elfObject.Sections);
        if (gotSec is null)
        {
            if (options?.IncludeEmptyTables != true)
            {
                return string.Empty;
            }

            return "Global offset table (.got)" + Environment.NewLine
                + new string('─', 48) + Environment.NewLine
                + "  (no .got or .got.plt section)" + Environment.NewLine;
        }

        var is64 = elfObject.Header.ElfClass == 2;
        var ptrSize = is64 ? 8 : 4;
        if (gotSec.Size == 0)
        {
            if (options?.IncludeEmptyTables != true)
            {
                return string.Empty;
            }

            return "Global offset table (.got)" + Environment.NewLine
                + new string('─', 48) + Environment.NewLine
                + $"  Section '{gotSec.Name}' is empty" + Environment.NewLine;
        }

        var nSlots = (int)(gotSec.Size / (ulong)ptrSize);
        var bytes = elfObject.GotSectionBytes ?? Array.Empty<byte>();

        var gotIdx = elfObject.Sections.IndexOf(gotSec);
        var relBySlot = BuildRelocationMap(elfObject, gotSec, gotIdx, ptrSize);

        var secName = gotSec.Name;
        var b = new StringBuilder();
        b.AppendLine("Global offset table (.got)");
        b.AppendLine(new string('─', 72));
        b.AppendLine(
            $"  Section '{secName}' contains {nSlots} entr{(nSlots == 1 ? "y" : "ies")} ({ptrSize}-byte slots)");
        b.AppendLine(
            string.Concat(
                "    ",
                PadRight("Index", 6),
                ColSep,
                PadLeft("Address", is64 ? 18 : 10),
                ColSep,
                PadLeft("Reloc", 22),
                ColSep,
                "Symbol + addend / value"));

        var littleEndian = elfObject.Header.DataEncoding == 1;
        for (var i = 0; i < nSlots; i++)
        {
            var addr = gotSec.Address + (ulong)(i * ptrSize);
            var off = i * ptrSize;
            var raw = ReadGotScalar(bytes, off, ptrSize, littleEndian);

            b.Append("    ");
            b.Append(PadRight(i.ToString(CultureInfo.InvariantCulture), 6));
            b.Append(ColSep);
            b.Append(PadLeft(is64 ? $"0x{addr:X16}" : $"0x{addr:X8}", is64 ? 18 : 10));
            b.Append(ColSep);

            if (relBySlot.TryGetValue(i, out var r))
            {
                var (symIdx, relType) = RelMacros(r.Info, is64);
                var typeStr = FormatRelType(elfObject.Header.Machine, relType);
                var namePlus = FormatNamePlusAddend(
                    ResolveSymbolName(elfObject, r.SymTabLink, symIdx),
                    r.Addend);
                b.Append(PadLeft(typeStr, 22));
                b.Append(ColSep);
                b.Append(namePlus);
            }
            else
            {
                b.Append(PadLeft(string.Empty, 22));
                b.Append(ColSep);
                b.Append(FormatRawValue(raw, is64));
            }

            b.AppendLine();
        }

        return b.ToString().TrimEnd() + Environment.NewLine;
    }

    /// <summary>
    /// Matches <see cref="Kuick.Elf.IO.ElfLoader"/> <c>LoadGotSectionBytes</c>: prefer the first of
    /// <c>.got</c>, <c>.got.plt</c> with non-zero size (and loadable size). If only empty sections exist,
    /// returns the first present section so empty-table output stays consistent.
    /// </summary>
    private static Section? SelectGotSection(IList<Section> sections)
    {
        foreach (var name in new[] { ".got", ".got.plt" })
        {
            foreach (var s in sections)
            {
                if (s.Name != name || s.Size == 0 || s.Size > int.MaxValue)
                {
                    continue;
                }

                return s;
            }
        }

        foreach (var name in new[] { ".got", ".got.plt" })
        {
            foreach (var s in sections)
            {
                if (s.Name == name)
                {
                    return s;
                }
            }
        }

        return null;
    }

    private static Dictionary<int, RelocationEntry> BuildRelocationMap(
        ElfObject elf,
        Section got,
        int gotIdx,
        int ptrSize)
    {
        var map = new Dictionary<int, RelocationEntry>();
        var isRelObj = elf.Header.Type == EtRel;

        foreach (var r in elf.Relocations)
        {
            if (!TryGetSlotIndex(r, got, gotIdx, isRelObj, ptrSize, out var slot))
            {
                continue;
            }

            map[slot] = r;
        }

        return map;
    }

    private static bool TryGetSlotIndex(
        RelocationEntry r,
        Section got,
        int gotIdx,
        bool isRelObj,
        int ptrSize,
        out int slot)
    {
        slot = -1;
        var nSlots = (int)(got.Size / (ulong)ptrSize);

        if (r.TargetSectionIndex != 0 && r.TargetSectionIndex == (uint)gotIdx)
        {
            if (isRelObj)
            {
                slot = (int)(r.Offset / (ulong)ptrSize);
            }
            else
            {
                slot = (int)(((long)r.Offset - (long)got.Address) / ptrSize);
            }

            return slot >= 0 && slot < nSlots;
        }

        if (!isRelObj && r.Offset >= got.Address && r.Offset < got.Address + got.Size)
        {
            slot = (int)(((long)r.Offset - (long)got.Address) / ptrSize);
            return slot >= 0 && slot < nSlots;
        }

        return false;
    }

    private static ulong ReadGotScalar(byte[] bytes, int offset, int ptrSize, bool littleEndian)
    {
        if (offset + ptrSize > bytes.Length)
        {
            return 0;
        }

        return ptrSize == 8
            ? ReadU64(bytes, offset, littleEndian)
            : ReadU32(bytes, offset, littleEndian);
    }

    private static uint ReadU32(byte[] raw, int offset, bool littleEndian)
    {
        if (littleEndian)
        {
            return (uint)(raw[offset]
                | (raw[offset + 1] << 8)
                | (raw[offset + 2] << 16)
                | (raw[offset + 3] << 24));
        }

        return (uint)((raw[offset] << 24)
            | (raw[offset + 1] << 16)
            | (raw[offset + 2] << 8)
            | raw[offset + 3]);
    }

    private static ulong ReadU64(byte[] raw, int offset, bool littleEndian)
    {
        if (littleEndian)
        {
            var lo = ReadU32(raw, offset, true);
            var hi = ReadU32(raw, offset + 4, true);
            return ((ulong)hi << 32) | lo;
        }

        var hi2 = ReadU32(raw, offset, false);
        var lo2 = ReadU32(raw, offset + 4, false);
        return ((ulong)hi2 << 32) | lo2;
    }

    private static string FormatRawValue(ulong raw, bool is64) =>
        is64 ? $"0x{raw:X16}" : $"0x{raw:X8}";

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
        return machine switch
        {
            EmRiscv => FormatRiscvRelType(type),
            EmX8664 => FormatX8664RelType(type),
            Em386 => FormatI386RelType(type),
            _ => $"0x{type:X}"
        };
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

    private static string FormatX8664RelType(uint t) =>
        t switch
        {
            0 => "R_X86_64_NONE",
            1 => "R_X86_64_64",
            2 => "R_X86_64_PC32",
            3 => "R_X86_64_GOT32",
            4 => "R_X86_64_PLT32",
            5 => "R_X86_64_COPY",
            6 => "R_X86_64_GLOB_DAT",
            7 => "R_X86_64_JUMP_SLOT",
            8 => "R_X86_64_RELATIVE",
            9 => "R_X86_64_GOTPCREL",
            10 => "R_X86_64_32",
            11 => "R_X86_64_32S",
            12 => "R_X86_64_16",
            13 => "R_X86_64_PC16",
            14 => "R_X86_64_8",
            15 => "R_X86_64_PC8",
            16 => "R_X86_64_DTPMOD64",
            17 => "R_X86_64_DTPOFF64",
            18 => "R_X86_64_TPOFF64",
            19 => "R_X86_64_TLSGD",
            20 => "R_X86_64_TLSLD",
            21 => "R_X86_64_DTPOFF32",
            22 => "R_X86_64_GOTTPOFF",
            23 => "R_X86_64_TPOFF32",
            24 => "R_X86_64_PC64",
            25 => "R_X86_64_GOTOFF64",
            26 => "R_X86_64_GOTPC32",
            27 => "R_X86_64_GOT64",
            28 => "R_X86_64_GOTPCREL64",
            29 => "R_X86_64_GOTPCRELX",
            30 => "R_X86_64_REX_GOTPCRELX",
            32 => "R_X86_64_SIZE32",
            33 => "R_X86_64_SIZE64",
            34 => "R_X86_64_GOTPC32_TLSDESC",
            35 => "R_X86_64_TLSDESC_CALL",
            36 => "R_X86_64_TLSDESC",
            37 => "R_X86_64_IRELATIVE",
            38 => "R_X86_64_RELATIVE64",
            _ => $"R_X86_64_{t}"
        };

    private static string FormatI386RelType(uint t) =>
        t switch
        {
            0 => "R_386_NONE",
            1 => "R_386_32",
            2 => "R_386_PC32",
            3 => "R_386_GOT32",
            4 => "R_386_PLT32",
            5 => "R_386_COPY",
            6 => "R_386_GLOB_DAT",
            7 => "R_386_JMP_SLOT",
            8 => "R_386_RELATIVE",
            9 => "R_386_GOTOFF",
            10 => "R_386_GOTPC",
            _ => $"R_386_{t}"
        };

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
