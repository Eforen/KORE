// src/Kore.RiscMeta/Relocation.cs
namespace Kore.RiscMeta;

/// <summary>
/// ELF <c>r_type</c> values for RISC-V relocations. Member names and numeric values follow the
/// RISC-V ELF psABI relocation table (same as GNU <c>include/elf/riscv.h</c> / LLVM
/// <c>ELFRelocs/RISCV.def</c>). Use these when emitting or interpreting relocation records.
/// </summary>
public enum RiscvRelocationType {
    /// <summary>Placeholder / unused relocation slot.</summary>
    R_RISCV_NONE = 0,

    /// <summary>32-bit absolute word; e.g. <c>.word</c> of a symbol in data — absolute pointer.</summary>
    R_RISCV_32 = 1,

    /// <summary>64-bit absolute word (64-bit code model / data).</summary>
    R_RISCV_64 = 2,

    /// <summary>Dynamic relocation: adjust a link address to its load address.</summary>
    R_RISCV_RELATIVE = 3,

    /// <summary>Dynamic copy relocation (executable only).</summary>
    R_RISCV_COPY = 4,

    /// <summary>PLT slot: address of the resolved symbol.</summary>
    R_RISCV_JUMP_SLOT = 5,

    /// <summary>TLS: module ID for dynamic TLS (32-bit).</summary>
    R_RISCV_TLS_DTPMOD32 = 6,

    /// <summary>TLS: module ID for dynamic TLS (64-bit).</summary>
    R_RISCV_TLS_DTPMOD64 = 7,

    /// <summary>TLS: offset in TLS block (32-bit DTP-relative).</summary>
    R_RISCV_TLS_DTPREL32 = 8,

    /// <summary>TLS: offset in TLS block (64-bit DTP-relative).</summary>
    R_RISCV_TLS_DTPREL64 = 9,

    /// <summary>TLS: TP-relative offset (32-bit).</summary>
    R_RISCV_TLS_TPREL32 = 10,

    /// <summary>TLS: TP-relative offset (64-bit).</summary>
    R_RISCV_TLS_TPREL64 = 11,

    /// <summary>TLS: general TLSDESC dynamic relocation.</summary>
    R_RISCV_TLSDESC = 12,

    /// <summary>12-bit PC-relative branch (<c>beq</c>, <c>bne</c>, <c>blt</c>, …).</summary>
    R_RISCV_BRANCH = 16,

    /// <summary>20-bit PC-relative unconditional jump (<c>jal</c>).</summary>
    R_RISCV_JAL = 17,

    /// <summary>Deprecated: 32-bit PC-relative <c>call</c>/<c>tail</c> sequence — prefer <see cref="R_RISCV_CALL_PLT"/>.</summary>
    R_RISCV_CALL = 18,

    /// <summary>32-bit PC-relative <c>call</c>/<c>tail</c> (PIC); <c>auipc</c>+<c>jalr</c> with PLT semantics.</summary>
    R_RISCV_CALL_PLT = 19,

    /// <summary>GOT: high 20 bits of PC-relative GOT address (<c>%got_pcrel_hi</c>); important for <c>-fPIC</c>.</summary>
    R_RISCV_GOT_HI20 = 20,

    /// <summary>TLS IE: high 20 bits of PC-relative TLS IE GOT entry (<c>la.tls.ie</c>).</summary>
    R_RISCV_TLS_GOT_HI20 = 21,

    /// <summary>TLS GD: high 20 bits of PC-relative TLS GD GOT pair (<c>la.tls.gd</c>).</summary>
    R_RISCV_TLS_GD_HI20 = 22,

    /// <summary>High 20 bits of PC-relative address (<c>auipc</c> / <c>%pcrel_hi</c>) — <c>la</c>, <c>call</c> in PIC; pairs with LO12.</summary>
    R_RISCV_PCREL_HI20 = 23,

    /// <summary>Low 12 bits, I-type, PC-relative (<c>%pcrel_lo</c> of matching <c>%pcrel_hi</c>) — <c>addi</c>/<c>lw</c>/<c>jalr</c> after <c>auipc</c>.</summary>
    R_RISCV_PCREL_LO12_I = 24,

    /// <summary>Low 12 bits, S-type store, PC-relative — rare stores after <c>auipc</c>.</summary>
    R_RISCV_PCREL_LO12_S = 25,

    /// <summary>High 20 bits of absolute address (<c>lui</c> / <c>%hi</c>) — medlow absolute model.</summary>
    R_RISCV_HI20 = 26,

    /// <summary>Low 12 bits, I-type, absolute (<c>%lo</c>) — <c>addi</c> and loads after <c>lui</c>.</summary>
    R_RISCV_LO12_I = 27,

    /// <summary>Low 12 bits, S-type store, absolute (<c>%lo</c>) — stores after <c>lui</c>.</summary>
    R_RISCV_LO12_S = 28,

    /// <summary>TLS LE: high 20 bits of TP-relative offset (<c>%tprel_hi</c>).</summary>
    R_RISCV_TPREL_HI20 = 29,

    /// <summary>TLS LE: low 12 bits, I-type (<c>%tprel_lo</c>).</summary>
    R_RISCV_TPREL_LO12_I = 30,

    /// <summary>TLS LE: low 12 bits, S-type (<c>%tprel_lo</c>).</summary>
    R_RISCV_TPREL_LO12_S = 31,

    /// <summary>TLS LE: thread-pointer adjustment (<c>%tprel_add</c>).</summary>
    R_RISCV_TPREL_ADD = 32,

    /// <summary>8-bit label addition fragment (complex expressions).</summary>
    R_RISCV_ADD8 = 33,

    /// <summary>16-bit label addition fragment (complex expressions).</summary>
    R_RISCV_ADD16 = 34,

    /// <summary>32-bit label addition fragment (complex expressions).</summary>
    R_RISCV_ADD32 = 35,

    /// <summary>64-bit label addition fragment (complex expressions).</summary>
    R_RISCV_ADD64 = 36,

    /// <summary>8-bit label subtraction fragment (complex expressions).</summary>
    R_RISCV_SUB8 = 37,

    /// <summary>16-bit label subtraction fragment (complex expressions).</summary>
    R_RISCV_SUB16 = 38,

    /// <summary>32-bit label subtraction fragment (complex expressions).</summary>
    R_RISCV_SUB32 = 39,

    /// <summary>64-bit label subtraction fragment (complex expressions).</summary>
    R_RISCV_SUB64 = 40,

    /// <summary>32-bit PC-relative offset to GOT slot for a symbol.</summary>
    R_RISCV_GOT32_PCREL = 41,

    /// <summary>Linker: alignment marker for relaxation (padding / <c>.balign</c> metadata).</summary>
    R_RISCV_ALIGN = 43,

    /// <summary>Compressed branch (RVC) PC-relative offset.</summary>
    R_RISCV_RVC_BRANCH = 44,

    /// <summary>Compressed jump (RVC) PC-relative offset.</summary>
    R_RISCV_RVC_JUMP = 45,

    /// <summary>Linker relaxation hint — paired at the same offset as another relocation; enables instruction shrinking.</summary>
    R_RISCV_RELAX = 51,

    /// <summary>6-bit local label subtraction.</summary>
    R_RISCV_SUB6 = 52,

    /// <summary>6-bit local label assignment.</summary>
    R_RISCV_SET6 = 53,

    /// <summary>8-bit local label assignment.</summary>
    R_RISCV_SET8 = 54,

    /// <summary>16-bit local label assignment.</summary>
    R_RISCV_SET16 = 55,

    /// <summary>32-bit local label assignment.</summary>
    R_RISCV_SET32 = 56,

    /// <summary>32-bit PC-relative word (full-range PC offset).</summary>
    R_RISCV_32_PCREL = 57,

    /// <summary>Dynamic ifunc resolution (<c>IRELATIVE</c>).</summary>
    R_RISCV_IRELATIVE = 58,

    /// <summary>32-bit offset to function or PLT entry.</summary>
    R_RISCV_PLT32 = 59,

    /// <summary>ULEB128 pair: set — must precede <see cref="R_RISCV_SUB_ULEB128"/> at same offset.</summary>
    R_RISCV_SET_ULEB128 = 60,

    /// <summary>ULEB128 pair: subtract — follows <see cref="R_RISCV_SET_ULEB128"/>.</summary>
    R_RISCV_SUB_ULEB128 = 61,

    /// <summary>TLS descriptor: high 20 bits (<c>%tlsdesc_hi</c>).</summary>
    R_RISCV_TLSDESC_HI20 = 62,

    /// <summary>TLS descriptor: load low 12 (<c>%tlsdesc_load_lo</c>).</summary>
    R_RISCV_TLSDESC_LOAD_LO12 = 63,

    /// <summary>TLS descriptor: add low 12 (<c>%tlsdesc_add_lo</c>).</summary>
    R_RISCV_TLSDESC_ADD_LO12 = 64,

    /// <summary>TLS descriptor: annotates call to resolver (<c>%tlsdesc_call</c>).</summary>
    R_RISCV_TLSDESC_CALL = 65,

    /// <summary>Must precede a vendor-specific relocation; identifies owning vendor.</summary>
    R_RISCV_VENDOR = 191,
}

/// <summary>
/// Represents a single relocation entry that needs to be resolved by the linker or loader.
/// </summary>
public partial class Relocation
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
    /// ELF relocation type (<see cref="RiscvRelocationType"/>); numeric value matches <c>r_type</c> in the psABI.
    /// </summary>
    public RiscvRelocationType Type { get; }

    /// <summary>
    /// For PC-relative LO12 relocations, the label of the paired AUIPC / preceding HI20 site (<c>%pcrel_lo</c> pairing).
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
