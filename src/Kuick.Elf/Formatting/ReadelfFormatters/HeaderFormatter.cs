using System.Globalization;
using System.Text;
using Kuick.Elf.Models;

namespace Kuick.Elf.Formatting.ReadelfFormatters;

/// <summary>
/// KORE-style ELF header listing (not GNU readelf-compatible wording).
/// </summary>
public sealed class HeaderFormatter : IElfFormatter
{
    private const byte ElfClass32 = 1;
    private const byte ElfClass64 = 2;
    private const byte ElfData2Lsb = 1;
    private const byte ElfData2Msb = 2;
    private const ushort EmRiscv = 0xF3;

    // RISC-V ELF e_flags (Linux/glibc convention)
    private const uint RiscvRvc = 0x0001;
    private const uint RiscvFloatAbiMask = 0x0006;

    public string Format(ElfObject elfObject, FormatterOptions? options = null)
    {
        var h = elfObject.Header;
        var b = new StringBuilder();
        b.AppendLine("ELF file header");
        b.AppendLine(new string('─', 48));

        b.Append("Magic (e_ident):  ");
        b.AppendLine(FormatIdentHex(h.Ident));

        var cls = h.ElfClass;
        b.AppendLine($"Class:              {FormatElfClass(cls)}");

        b.AppendLine($"Data encoding:      {FormatDataEncoding(h.DataEncoding)}");
        b.AppendLine($"ELF version:        {FormatIdentVersion(h.IdentVersion)}");
        b.AppendLine($"OS/ABI:             {FormatOsAbi(h.OsAbi)}");
        b.AppendLine($"ABI version:        {h.AbiVersion}");

        b.AppendLine($"Type:               {FormatType(h.Type)}");
        b.AppendLine($"Machine:            {FormatMachine(h.Machine)}");
        b.AppendLine($"Object version:     0x{h.FileVersion:X8} ({h.FileVersion})");

        b.AppendLine($"Entry:              0x{h.Entry:X}");
        b.AppendLine(
            $"Program headers:    file offset 0x{h.ProgramHeaderOffset:X}, entry size {h.ProgramHeaderEntrySize} bytes, count {h.ProgramHeaderCount}");
        b.AppendLine(
            $"Section headers:    file offset 0x{h.SectionHeaderOffset:X}, entry size {h.SectionHeaderEntrySize} bytes, count {h.SectionHeaderCount}");

        b.AppendLine($"Flags:              0x{h.Flags:X} ({h.Flags}){FormatFlagsExtra(h.Machine, h.Flags)}");

        b.AppendLine($"Header size:        {h.HeaderSize} bytes");
        b.AppendLine($"Section name index: {h.SectionHeaderStringIndex}");

        return b.ToString();
    }

    private static string FormatIdentHex(byte[] ident)
    {
        if (ident.Length == 0)
        {
            return "(empty)";
        }

        var span = ident.Length >= 16 ? ident.AsSpan(0, 16) : ident.AsSpan();
        var parts = new string[span.Length];
        for (var i = 0; i < span.Length; i++)
        {
            parts[i] = span[i].ToString("x2", CultureInfo.InvariantCulture);
        }

        return string.Join(' ', parts);
    }

    private static string FormatElfClass(byte cls) =>
        cls switch
        {
            ElfClass32 => "ELF32",
            ElfClass64 => "ELF64",
            _ => $"unknown ({cls})"
        };

    private static string FormatDataEncoding(byte data) =>
        data switch
        {
            ElfData2Lsb => "2's complement, little-endian (ELFDATA2LSB)",
            ElfData2Msb => "2's complement, big-endian (ELFDATA2MSB)",
            _ => $"unknown ({data})"
        };

    private static string FormatIdentVersion(byte v) =>
        v switch
        {
            0 => "0 (invalid)",
            1 => "1 (current)",
            _ => v.ToString(CultureInfo.InvariantCulture)
        };

    private static string FormatOsAbi(byte abi) =>
        abi switch
        {
            0 => "UNIX — System V",
            3 => "GNU/Linux",
            _ => $"0x{abi:X2}"
        };

    private static string FormatType(ushort type) =>
        type switch
        {
            0 => "ET_NONE",
            1 => "ET_REL (relocatable object)",
            2 => "ET_EXEC (executable)",
            3 => "ET_DYN (shared object)",
            4 => "ET_CORE (core file)",
            _ => $"0x{type:X4}"
        };

    private static string FormatMachine(ushort machine) =>
        machine switch
        {
            EmRiscv => "RISC-V (EM_RISCV, 0x00F3)",
            0x3E => "x86-64 (EM_X86_64)",
            _ => $"0x{machine:X4}"
        };

    private static string FormatFlagsExtra(ushort machine, uint flags)
    {
        if (machine != EmRiscv)
        {
            return string.Empty;
        }

        var parts = new List<string>();
        if ((flags & RiscvRvc) != 0)
        {
            parts.Add("RVC");
        }

        var fa = flags & RiscvFloatAbiMask;
        parts.Add(fa switch
        {
            0 => "float ABI: soft",
            0x2 => "float ABI: single",
            0x4 => "float ABI: double",
            0x6 => "float ABI: quad",
            _ => $"float ABI: 0x{fa:X}"
        });

        return " — " + string.Join(", ", parts);
    }
}
