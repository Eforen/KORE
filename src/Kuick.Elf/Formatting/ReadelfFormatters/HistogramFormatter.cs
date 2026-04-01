using System.Globalization;
using System.Text;
using Kuick.Elf.Models;

namespace Kuick.Elf.Formatting.ReadelfFormatters;

/// <summary>
/// GNU hash bucket histogram (similar intent to GNU <c>readelf -I</c> / <c>--histogram</c>).
/// </summary>
public sealed class HistogramFormatter : IElfFormatter
{
    public string Format(ElfObject elfObject, FormatterOptions? options = null)
    {
        var raw = elfObject.GnuHash;
        if (raw is null or { Length: 0 })
        {
            if (options?.IncludeEmptyTables == true)
            {
                return "Histogram for `.gnu.hash' bucket list length"
                    + Environment.NewLine
                    + new string('─', 72)
                    + Environment.NewLine
                    + "  (none — no SHT_GNU_HASH / .gnu.hash section or empty)"
                    + Environment.NewLine;
            }

            return string.Empty;
        }

        var is64 = elfObject.Header.ElfClass == 2;
        var littleEndian = elfObject.Header.DataEncoding == 1;
        if (!TryParseGnuHash(raw, is64, littleEndian, out var nbuckets, out var bucketLengths, out var err))
        {
            var b = new StringBuilder();
            b.AppendLine("Histogram for `.gnu.hash' bucket list length");
            b.AppendLine(new string('─', 72));
            b.Append("  ");
            b.AppendLine(err ?? "Could not parse .gnu.hash section.");
            return b.ToString().TrimEnd() + Environment.NewLine;
        }

        if (bucketLengths is null)
        {
            return string.Empty;
        }

        var totalSyms = 0L;
        foreach (var len in bucketLengths)
        {
            totalSyms += len;
        }

        var hist = new Dictionary<int, int>();
        foreach (var len in bucketLengths)
        {
            hist.TryGetValue(len, out var c);
            hist[len] = c + 1;
        }

        var sortedLens = hist.Keys.OrderBy(x => x).ToList();

        var o = new StringBuilder();
        o.Append("Histogram for `.gnu.hash' bucket list length (total of ");
        o.Append(nbuckets);
        o.AppendLine(" buckets):");
        o.AppendLine(" Length  Number     % of total  Coverage");
        long cumSyms = 0;
        foreach (var len in sortedLens)
        {
            var num = hist[len];
            var pctBuckets = nbuckets == 0 ? 0 : 100.0 * num / nbuckets;
            cumSyms += (long)len * num;

            o.Append(string.Format(CultureInfo.InvariantCulture, "{0,7}", len));
            o.Append("  ");
            o.Append(string.Format(CultureInfo.InvariantCulture, "{0,-9}", num));
            o.Append(string.Format(CultureInfo.InvariantCulture, " ({0,6:F1}%)", pctBuckets));

            if (cumSyms == 0)
            {
                o.AppendLine();
                continue;
            }

            var cov = totalSyms == 0 ? 0 : 100.0 * cumSyms / totalSyms;
            o.Append(string.Format(CultureInfo.InvariantCulture, "    {0:F1}%", cov));
            o.AppendLine();
        }

        return o.ToString().TrimEnd() + Environment.NewLine;
    }

    private static bool TryParseGnuHash(byte[] raw, bool is64, bool littleEndian, out int nbuckets, out int[]? bucketLengths, out string? error)
    {
        nbuckets = 0;
        bucketLengths = null;
        error = null;

        if (raw.Length < 16)
        {
            error = "Section too small for GNU hash header.";
            return false;
        }

        var nB = ReadU32(raw, 0, littleEndian);
        var symndx = ReadU32(raw, 4, littleEndian);
        var bloomSize = ReadU32(raw, 8, littleEndian);
        _ = ReadU32(raw, 12, littleEndian);

        if (nB > int.MaxValue / 4)
        {
            error = "Unreasonable nbuckets.";
            return false;
        }

        nbuckets = (int)nB;
        var off = 16;
        var bloomWords = (int)bloomSize;
        var bloomBytes = is64 ? bloomWords * 8 : bloomWords * 4;
        if (off > raw.Length - bloomBytes)
        {
            error = "Bloom filter extends past section.";
            return false;
        }

        off += bloomBytes;

        if (off > raw.Length - nbuckets * 4L)
        {
            error = "Bucket array extends past section.";
            return false;
        }

        var buckets = new uint[nbuckets];
        for (var i = 0; i < nbuckets; i++)
        {
            buckets[i] = ReadU32(raw, off + i * 4, littleEndian);
        }

        off += nbuckets * 4;
        var chainWords = (raw.Length - off) / 4;
        if (chainWords < 0)
        {
            error = "No chain table.";
            return false;
        }

        bucketLengths = new int[nbuckets];
        for (var b = 0; b < nbuckets; b++)
        {
            var w = WalkBucketChain(buckets[b], symndx, raw, off, chainWords, littleEndian, out var e);
            if (e is not null)
            {
                error = e;
                return false;
            }

            bucketLengths[b] = w;
        }

        return true;
    }

    private static int WalkBucketChain(uint first, uint symndx, byte[] raw, int chainOff, int chainWords, bool littleEndian, out string? error)
    {
        error = null;
        if (first == 0)
        {
            return 0;
        }

        var idx = first;
        var count = 0;
        while (true)
        {
            if (idx < symndx)
            {
                error = "Bucket chain start index precedes symndx.";
                return 0;
            }

            var ci = (long)idx - symndx;
            if (ci < 0 || ci >= chainWords)
            {
                error = "Chain index out of range.";
                return 0;
            }

            count++;
            var c = ReadU32(raw, chainOff + (int)ci * 4, littleEndian);
            if ((c & 1) != 0)
            {
                return count;
            }

            idx++;
            if (count > chainWords + 256)
            {
                error = "Chain walk did not terminate.";
                return 0;
            }
        }
    }

    private static uint ReadU32(byte[] raw, int offset, bool littleEndian)
    {
        if (littleEndian)
        {
            return (uint)(raw[offset] | (raw[offset + 1] << 8) | (raw[offset + 2] << 16) | (raw[offset + 3] << 24));
        }

        return (uint)((raw[offset] << 24) | (raw[offset + 1] << 16) | (raw[offset + 2] << 8) | raw[offset + 3]);
    }
}
