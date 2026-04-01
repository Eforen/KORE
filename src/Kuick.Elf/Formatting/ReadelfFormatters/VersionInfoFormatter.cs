using System.Globalization;
using System.Text;
using Kuick.Elf.Models;

namespace Kuick.Elf.Formatting.ReadelfFormatters;

/// <summary>
/// GNU version sections (similar intent to GNU <c>readelf -V</c>).
/// </summary>
public sealed class VersionInfoFormatter : IElfFormatter
{
    private const ushort VerFlgBase = 1;
    private const ushort VerFlgWeak = 2;

    public string Format(ElfObject elfObject, FormatterOptions? options = null)
    {
        var gv = elfObject.GnuVersion;
        var hasAny = gv.Versym is not null || gv.Verneed.Count > 0 || gv.Verdef.Count > 0;
        if (!hasAny && options?.IncludeEmptyTables != true)
        {
            return string.Empty;
        }

        if (!hasAny)
        {
            return "GNU version information" + Environment.NewLine
                + new string('─', 72) + Environment.NewLine
                + "  (none — no .gnu.version / .gnu.version_d / .gnu.version_r)" + Environment.NewLine;
        }

        var map = BuildVersionIndexMap(gv);
        var is64 = elfObject.Header.ElfClass == 2;
        var b = new StringBuilder();

        if (gv.Versym is not null)
        {
            AppendVersym(b, gv.Versym, map, is64);
        }

        foreach (var vd in gv.Verdef)
        {
            AppendVerdef(b, vd, is64);
        }

        foreach (var vn in gv.Verneed)
        {
            AppendVerneed(b, vn, is64);
        }

        return b.ToString().TrimEnd() + Environment.NewLine;
    }

    private static Dictionary<ushort, string> BuildVersionIndexMap(GnuVersionInfo gv)
    {
        var map = new Dictionary<ushort, string>();
        foreach (var sec in gv.Verdef)
        {
            foreach (var e in sec.Entries)
            {
                if (e.VdNdx != 0 && !string.IsNullOrEmpty(e.Name))
                {
                    map[e.VdNdx] = e.Name;
                }
            }
        }

        foreach (var sec in gv.Verneed)
        {
            foreach (var chain in sec.Chains)
            {
                foreach (var a in chain.Aux)
                {
                    if (a.VersionIndex != 0)
                    {
                        map[a.VersionIndex] = a.Name;
                    }
                }
            }
        }

        return map;
    }

    private static void AppendVersym(StringBuilder b, VersymSection vs, Dictionary<ushort, string> map, bool is64)
    {
        var entries = vs.Entries;
        b.Append("Version symbols section '");
        b.Append(vs.Name);
        b.Append("' contains ");
        b.Append(entries.Length);
        b.Append(entries.Length == 1 ? " entry:" : " entries:");
        b.AppendLine();
        b.Append(" Addr: ");
        b.Append(HexAddr(vs.Address, is64));
        b.Append("  Offset: ");
        b.Append(HexAddr(vs.Offset, is64));
        b.Append("  Link: ");
        b.Append(vs.DynsymLinkName);
        b.AppendLine();

        const int cols = 4;
        const int cellW = 22;
        for (var rowStart = 0; rowStart < entries.Length; rowStart += cols)
        {
            b.Append("  ");
            b.Append(rowStart.ToString("x3", CultureInfo.InvariantCulture));
            b.Append(": ");
            for (var c = 0; c < cols && rowStart + c < entries.Length; c++)
            {
                var cell = FormatVersymCell(entries[rowStart + c], map);
                b.Append(PadRight(cell, cellW));
            }

            b.AppendLine();
        }

        b.AppendLine();
    }

    private static string FormatVersymCell(ushort v, Dictionary<ushort, string> map)
    {
        var hidden = (v & 0x8000) != 0;
        var idx = (ushort)(v & 0x7FFFu);
        string name;
        if (idx == 0)
        {
            name = "*local*";
        }
        else if (idx == 1)
        {
            name = "*global*";
        }
        else if (!map.TryGetValue(idx, out name!))
        {
            name = "?" + idx.ToString(CultureInfo.InvariantCulture);
        }

        var num = idx.ToString("x", CultureInfo.InvariantCulture);
        if (hidden)
        {
            num += "h";
        }

        return num + " (" + name + ")";
    }

    private static void AppendVerdef(StringBuilder b, VerdefSection sec, bool is64)
    {
        var count = sec.Entries.Count;
        b.Append("Version definition section '");
        b.Append(sec.Name);
        b.Append("' contains ");
        b.Append(count);
        b.Append(count == 1 ? " entry:" : " entries:");
        b.AppendLine();
        b.Append(" Addr: ");
        b.Append(HexAddr(sec.Address, is64));
        b.Append("  Offset: ");
        b.Append(HexAddr(sec.Offset, is64));
        b.Append("  Link: ");
        b.Append(sec.DynstrLinkName);
        b.AppendLine();

        foreach (var e in sec.Entries)
        {
            b.Append("  ");
            b.Append(e.VerdefOffsetInSection.ToString("x6", CultureInfo.InvariantCulture));
            b.Append(": Rev: ");
            b.Append(e.VdVersion);
            b.Append("  Flags: ");
            b.Append(FormatVdFlags(e.VdFlags));
            b.Append("  Index: ");
            b.Append(e.VdNdx);
            b.Append("  Cnt: ");
            b.Append(e.VdCount);
            b.Append("  Name: ");
            b.Append(e.Name);
            b.AppendLine();

            foreach (var p in e.Parents)
            {
                b.Append("  0x");
                b.Append(p.OffsetInSection.ToString("x4", CultureInfo.InvariantCulture));
                b.Append(": Parent ");
                b.Append(p.ParentIndex);
                b.Append(": ");
                b.Append(p.Name);
                b.AppendLine();
            }
        }

        b.AppendLine();
    }

    private static void AppendVerneed(StringBuilder b, VerneedSection sec, bool is64)
    {
        var chainCount = sec.Chains.Count;
        b.Append("Version needs section '");
        b.Append(sec.Name);
        b.Append("' contains ");
        b.Append(chainCount);
        b.Append(chainCount == 1 ? " entry:" : " entries:");
        b.AppendLine();
        b.Append(" Addr: ");
        b.Append(HexAddr(sec.Address, is64));
        b.Append("  Offset: ");
        b.Append(HexAddr(sec.Offset, is64));
        b.Append("  Link: ");
        b.Append(sec.DynstrLinkName);
        b.AppendLine();

        foreach (var chain in sec.Chains)
        {
            b.Append("  ");
            b.Append(chain.HeaderOffsetInSection.ToString("x6", CultureInfo.InvariantCulture));
            b.Append(": Version: ");
            b.Append(chain.VnVersion);
            b.Append("  File: ");
            b.Append(chain.FileName);
            b.Append("  Cnt: ");
            b.Append(chain.VnCount);
            b.AppendLine();

            foreach (var a in chain.Aux)
            {
                b.Append("  0x");
                b.Append(a.OffsetInSection.ToString("x4", CultureInfo.InvariantCulture));
                b.Append(":   Name: ");
                b.Append(a.Name);
                b.Append("  Flags: ");
                b.Append(FormatVnaFlags(a.Flags));
                b.Append("  Version: ");
                b.Append(a.VersionIndex);
                b.AppendLine();
            }
        }

        b.AppendLine();
    }

    private static string FormatVdFlags(ushort f)
    {
        if (f == 0)
        {
            return "none";
        }

        var parts = new List<string>();
        if ((f & VerFlgBase) != 0)
        {
            parts.Add("BASE");
        }

        if ((f & VerFlgWeak) != 0)
        {
            parts.Add("weak");
        }

        return parts.Count == 0 ? $"0x{f:X}" : string.Join("|", parts);
    }

    private static string FormatVnaFlags(ushort f) =>
        f == 0 ? "none" : ((f & VerFlgWeak) != 0 ? "weak" : $"0x{f:X}");

    private static string HexAddr(ulong v, bool is64) =>
        is64 ? $"0x{v:X16}" : $"0x{(uint)v:X8}";

    private static string PadRight(string s, int width)
    {
        if (s.Length >= width)
        {
            return s;
        }

        return s + new string(' ', width - s.Length);
    }
}
