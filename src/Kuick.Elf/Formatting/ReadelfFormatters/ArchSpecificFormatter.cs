using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Kuick.Elf.Models;

namespace Kuick.Elf.Formatting.ReadelfFormatters;

/// <summary>
/// Architecture-specific ELF information (similar intent to GNU <c>readelf -A</c>).
/// </summary>
public sealed class ArchSpecificFormatter : IElfFormatter
{
    private const ushort EmRiscv = 0xF3;
    private const uint EfRiscvRvc = 0x0001;
    private const uint EfRiscvFloatAbiMask = 0x0006;
    private const uint EfRiscvRve = 0x0008;
    private const uint EfRiscvTso = 0x0010;

    private static readonly Regex RiscvArchString = new(
        @"rv(?:32|64)[a-z0-9_\.\d]+",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);

    public string Format(ElfObject elfObject, FormatterOptions? options = null)
    {
        var machine = elfObject.Header.Machine;
        if (machine != EmRiscv)
        {
            if (options?.IncludeEmptyTables != true)
            {
                return string.Empty;
            }

            var b = new StringBuilder();
            b.AppendLine("Architecture-specific information");
            b.AppendLine(new string('─', 56));
            b.Append("Machine: ");
            b.AppendFormat(CultureInfo.InvariantCulture, "0x{0:X4}", machine);
            b.AppendLine();
            b.AppendLine("  (KORE only decodes RISC-V architecture-specific details; use GNU readelf -A for this machine.)");
            return b.ToString().TrimEnd() + Environment.NewLine;
        }

        return FormatRiscv(elfObject, options).TrimEnd() + Environment.NewLine;
    }

    private static string FormatRiscv(ElfObject elfObject, FormatterOptions? options)
    {
        var b = new StringBuilder();
        b.AppendLine("Architecture-specific information");
        b.AppendLine(new string('─', 56));
        b.AppendLine("RISC-V (EM_RISCV)");
        b.AppendLine();
        b.AppendLine("ELF header flags (e_flags)");
        var flags = elfObject.Header.Flags;
        b.Append("  Value: ");
        b.AppendFormat(CultureInfo.InvariantCulture, "0x{0:X}", flags);
        b.AppendLine();
        b.Append("  EF_RISCV_RVC: ");
        b.AppendLine((flags & EfRiscvRvc) != 0 ? "yes" : "no");
        b.Append("  EF_RISCV_FLOAT_ABI: ");
        b.AppendLine(FormatFloatAbi(flags & EfRiscvFloatAbiMask));
        b.Append("  EF_RISCV_RVE: ");
        b.AppendLine((flags & EfRiscvRve) != 0 ? "yes" : "no");
        b.Append("  EF_RISCV_TSO: ");
        b.AppendLine((flags & EfRiscvTso) != 0 ? "yes" : "no");
        b.AppendLine();

        var raw = elfObject.RiscvAttributes;
        if (raw is { Length: > 0 })
        {
            b.AppendLine("Attribute Section: riscv");
            b.AppendLine("File Attributes");
            var arch = TryExtractRiscvArchString(raw);
            if (arch is not null)
            {
                b.Append("  Tag_RISCV_arch: \"");
                b.Append(arch);
                b.AppendLine("\"");
            }
            else
            {
                b.AppendLine("  (could not decode Tag_RISCV_arch from .riscv.attributes)");
                if (options?.Verbose == true && raw.Length <= 256)
                {
                    b.Append("  Raw (hex): ");
                    b.AppendLine(FormatHexPrefix(raw, 128));
                }
            }
        }
        else if (options?.IncludeEmptyTables == true)
        {
            b.AppendLine(".riscv.attributes");
            b.AppendLine("  (none — no SHT_RISCV_ATTRIBUTES section or empty)");
        }

        return b.ToString();
    }

    private static string FormatFloatAbi(uint fa) =>
        fa switch
        {
            0 => "EF_RISCV_FLOAT_ABI_SOFT (0)",
            0x2 => "EF_RISCV_FLOAT_ABI_SINGLE (2)",
            0x4 => "EF_RISCV_FLOAT_ABI_DOUBLE (4)",
            0x6 => "EF_RISCV_FLOAT_ABI_QUAD (6)",
            _ => string.Format(CultureInfo.InvariantCulture, "0x{0:X} (reserved/unknown)", fa)
        };

    private static string? TryExtractRiscvArchString(byte[] raw)
    {
        var s = Encoding.UTF8.GetString(raw);
        var m = RiscvArchString.Match(s);
        return m.Success ? m.Value : null;
    }

    private static string FormatHexPrefix(byte[] raw, int maxBytes)
    {
        var n = Math.Min(raw.Length, maxBytes);
        var parts = new string[n];
        for (var i = 0; i < n; i++)
        {
            parts[i] = raw[i].ToString("x2", CultureInfo.InvariantCulture);
        }

        return string.Join(' ', parts) + (raw.Length > maxBytes ? " …" : string.Empty);
    }
}
