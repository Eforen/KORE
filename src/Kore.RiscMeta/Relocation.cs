// src/Kore.RiscMeta/Relocation.cs
namespace Kore.RiscMeta;

/// <summary>ELF relocation type for RISC-V (subset used by the assembler/linker).</summary>
public enum RiscvRelocationType {
    Undefined = 0,
    R_RISCV_PCREL_HI20 = 1,
    R_RISCV_PCREL_LO12_I = 2,
    R_RISCV_PCREL_LO12_S = 3,
    R_RISCV_JAL = 4,
}

/// <summary>
/// Represents a single relocation entry that needs to be resolved by the linker or loader.
/// </summary>
public class Relocation
{
    /// <summary>
    /// Byte offset within the target section where the relocation should be applied
    /// </summary>
    public ulong Offset { get; }

    /// <summary>
    /// Name of the symbol this relocation refers to
    /// </summary>
    public string SymbolName { get; }

    /// <summary>
    /// Extra constant offset (e.g. my_label + 12)
    /// </summary>
    public long Addend { get; }

    /// <summary>
    /// Type of relocation (R_RISCV_PCREL_HI20, R_RISCV_LO12_I, etc.)
    /// </summary>
    public RiscvRelocationType Type { get; }

    /// <summary>
    /// For PCREL_LO12, this points to the label of the paired HI20 instruction
    /// </summary>
    public string? RelatedLabel { get; }

    public Relocation(ulong offset, string symbolName, long addend, RiscvRelocationType type, string? relatedLabel = null)
    {
        Offset = offset;
        SymbolName = symbolName;
        Addend = addend;
        Type = type;
        RelatedLabel = relatedLabel;
    }
}