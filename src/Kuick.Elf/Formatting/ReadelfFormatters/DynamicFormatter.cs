using System.Text;
using Kuick.Elf.Models;

namespace Kuick.Elf.Formatting.ReadelfFormatters;

/// <summary>
/// Dynamic section listing (similar intent to GNU <c>readelf -d</c>).
/// </summary>
public sealed class DynamicFormatter : IElfFormatter
{
    private const string ColSep = "  ";

    // Standard gABI <c>d_tag</c> values (numeric tags; OS-specific use high bits).
    private const long DtNull = 0;
    private const long DtNeeded = 1;
    private const long DtPltrelsz = 2;
    private const long DtPltgot = 3;
    private const long DtHash = 4;
    private const long DtStrtab = 5;
    private const long DtSymtab = 6;
    private const long DtRela = 7;
    private const long DtRelasz = 8;
    private const long DtRelaent = 9;
    private const long DtStrsz = 10;
    private const long DtSyment = 11;
    private const long DtInit = 12;
    private const long DtFini = 13;
    private const long DtSoname = 14;
    private const long DtRpath = 15;
    private const long DtSymbolic = 16;
    private const long DtRel = 17;
    private const long DtRelsz = 18;
    private const long DtRelent = 19;
    private const long DtPltrel = 20;
    private const long DtDebug = 21;
    private const long DtTextrel = 22;
    private const long DtJmprel = 23;
    private const long DtBindNow = 24;
    private const long DtInitArray = 25;
    private const long DtFiniArray = 26;
    private const long DtInitArraysz = 27;
    private const long DtFiniArraysz = 28;
    private const long DtRunpath = 29;
    private const long DtFlags = 30;
    private const long DtPreinitArray = 32;
    private const long DtPreinitArraysz = 33;
    private const long DtSymtabShndx = 34;
    private const long DtFlags1 = 0x6ffffffb;
    private const long DtGnuHash = unchecked((long)0x6ffffef5);
    private const long DtVersym = unchecked((long)0x6ffffff0);
    private const long DtVerneed = unchecked((long)0x6ffffffe);
    private const long DtVerneednum = unchecked((long)0x6fffffff);

    public string Format(ElfObject elfObject, FormatterOptions? options = null)
    {
        if (elfObject.DynamicEntries.Count == 0 && options?.IncludeEmptyTables != true)
        {
            return string.Empty;
        }

        var b = new StringBuilder();
        b.AppendLine("Dynamic section");
        b.AppendLine(new string('─', 96));

        if (elfObject.DynamicEntries.Count == 0)
        {
            b.AppendLine("  (none — no SHT_DYNAMIC section or empty)");
            return b.ToString();
        }

        var is64 = elfObject.Header.ElfClass == 2;
        var str = elfObject.DynamicStrtab;

        b.Append("  ");
        b.Append(PadRight("Tag", is64 ? 18 : 10));
        b.Append(ColSep);
        b.Append(PadRight("Type", 28));
        b.Append(ColSep);
        b.AppendLine("Name / value");

        foreach (var e in elfObject.DynamicEntries)
        {
            b.Append("  ");
            b.Append(PadRight(HexTag(e.Tag, is64), is64 ? 18 : 10));
            b.Append(ColSep);
            b.Append(PadRight($"({FormatTagName(e.Tag)})", 28));
            b.Append(ColSep);
            b.AppendLine(FormatValue(e.Tag, e.Value, str, is64));
        }

        return b.ToString().TrimEnd() + Environment.NewLine;
    }

    private static string FormatTagName(long tag) =>
        tag switch
        {
            DtNull => "NULL",
            DtNeeded => "NEEDED",
            DtPltrelsz => "PLTRELSZ",
            DtPltgot => "PLTGOT",
            DtHash => "HASH",
            DtStrtab => "STRTAB",
            DtSymtab => "SYMTAB",
            DtRela => "RELA",
            DtRelasz => "RELASZ",
            DtRelaent => "RELAENT",
            DtStrsz => "STRSZ",
            DtSyment => "SYMENT",
            DtInit => "INIT",
            DtFini => "FINI",
            DtSoname => "SONAME",
            DtRpath => "RPATH",
            DtSymbolic => "SYMBOLIC",
            DtRel => "REL",
            DtRelsz => "RELSZ",
            DtRelent => "RELENT",
            DtPltrel => "PLTREL",
            DtDebug => "DEBUG",
            DtTextrel => "TEXTREL",
            DtJmprel => "JMPREL",
            DtBindNow => "BIND_NOW",
            DtInitArray => "INIT_ARRAY",
            DtFiniArray => "FINI_ARRAY",
            DtInitArraysz => "INIT_ARRAYSZ",
            DtFiniArraysz => "FINI_ARRAYSZ",
            DtRunpath => "RUNPATH",
            DtFlags => "FLAGS",
            DtPreinitArray => "PREINIT_ARRAY",
            DtPreinitArraysz => "PREINIT_ARRAYSZ",
            DtSymtabShndx => "SYMTAB_SHNDX",
            DtFlags1 => "FLAGS_1",
            DtGnuHash => "GNU_HASH",
            DtVersym => "VERSYM",
            DtVerneed => "VERNEED",
            DtVerneednum => "VERNEEDNUM",
            _ => "unknown"
        };

    private static string FormatValue(long tag, ulong value, byte[] dynstr, bool is64)
    {
        if (tag == DtNull)
        {
            return "(terminator)";
        }

        if (IsStringTag(tag))
        {
            var s = ReadDynstr(dynstr, value);
            if (tag == DtNeeded)
            {
                return string.IsNullOrEmpty(s)
                    ? "Shared library: []"
                    : $"Shared library: [{s}]";
            }

            if (string.IsNullOrEmpty(s))
            {
                return HexVal(value, is64);
            }

            return tag switch
            {
                DtSoname => $"Library soname: [{s}]",
                DtRpath => $"Library rpath: [{s}]",
                DtRunpath => $"Library runpath: [{s}]",
                _ => $"[{s}]"
            };
        }

        if (tag == DtFlags)
        {
            return $"{HexVal(value, is64)}  {FormatDtFlags(value)}".TrimEnd();
        }

        if (tag == DtPltrel)
        {
            var relName = value == 7 ? "RELA" : value == 17 ? "REL" : HexVal(value, is64);
            return $"{HexVal(value, is64)}  ({relName})";
        }

        return HexVal(value, is64);
    }

    private static bool IsStringTag(long tag) =>
        tag is DtNeeded or DtSoname or DtRpath or DtRunpath;

    private static string ReadDynstr(byte[] tab, ulong offset)
    {
        if (tab.Length == 0 || offset >= (ulong)tab.Length)
        {
            return string.Empty;
        }

        var i = (int)offset;
        var end = i;
        while (end < tab.Length && tab[end] != 0)
        {
            end++;
        }

        return Encoding.UTF8.GetString(tab, i, end - i);
    }

    private static string FormatDtFlags(ulong value)
    {
        if (value == 0)
        {
            return string.Empty;
        }

        var parts = new List<string>();
        if ((value & 1) != 0)
        {
            parts.Add("ORIGIN");
        }

        if ((value & 2) != 0)
        {
            parts.Add("SYMBOLIC");
        }

        if ((value & 4) != 0)
        {
            parts.Add("TEXTREL");
        }

        if ((value & 8) != 0)
        {
            parts.Add("BIND_NOW");
        }

        if ((value & 0x10) != 0)
        {
            parts.Add("STATIC_TLS");
        }

        return parts.Count == 0 ? string.Empty : $"({string.Join(", ", parts)})";
    }

    private static string HexTag(long tag, bool is64) =>
        is64
            ? $"0x{unchecked((ulong)tag):X16}"
            : $"0x{(uint)(int)tag:X8}";

    private static string HexVal(ulong val, bool is64) =>
        is64 ? $"0x{val:X16}" : $"0x{(uint)val:X8}";

    private static string PadRight(string s, int width)
    {
        if (s.Length >= width)
        {
            return s;
        }

        return s + new string(' ', width - s.Length);
    }
}
