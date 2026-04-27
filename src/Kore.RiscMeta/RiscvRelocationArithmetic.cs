// src/Kore.RiscMeta/RiscvRelocationArithmetic.cs
using System;

namespace Kore.RiscMeta;

/// <summary>
/// <para>
/// Relocation **resolution** formulas for RISC-V ELF (static link / conceptual dynamic link).
/// Naming follows the RISC-V ELF psABI “Relocation types” table (variables <c>S</c>, <c>A</c>, <c>P</c>, <c>B</c>, <c>G</c>, <c>GOT</c>, <c>TP</c>, etc.).
/// </para>
/// <para>
/// Instruction bit-patching is implemented on <see cref="Relocation.ApplyToInstruction32"/>; these helpers return the **numeric results**
/// or field values that the psABI assigns to each relocation kind.
/// </para>
/// </summary>
public static class RiscvRelocationArithmetic
{
    /// <summary>R_RISCV_NONE (0): no relocation; value unchanged.</summary>
    public static uint None(uint placeholder) => placeholder;

    /// <summary>R_RISCV_32 (1): 32-bit word — <c>result = S + A</c> (word32).</summary>
    public static uint Word32(long s, long a) => unchecked((uint)(s + a));

    /// <summary>R_RISCV_64 (2): 64-bit word — <c>result = S + A</c> (word64).</summary>
    public static ulong Word64(long s, long a) => unchecked((ulong)(s + a));

    /// <summary>R_RISCV_RELATIVE (3): dynamic — <c>result = B + A</c> (wordclass; adjust link address to load address).</summary>
    public static ulong Relative(ulong loadBaseB, long a) => unchecked((ulong)((long)loadBaseB + a));

    /// <summary>R_RISCV_COPY (4): resolved by dynamic loader (copy from shared object); not a compile-time constant.</summary>
    public static bool CopyIsResolvedAtRuntime => true;

    /// <summary>R_RISCV_JUMP_SLOT (5): dynamic PLT — <c>result = S</c> (address of resolved definition).</summary>
    public static ulong JumpSlot(ulong symbolS) => symbolS;

    /// <summary>R_RISCV_TLS_DTPMOD32/64 (6–7): module id for general dynamic TLS — value is TLS module id from runtime.</summary>
    public static uint TlsDtpmod32(uint tlsModuleId) => tlsModuleId;

    /// <summary>R_RISCV_TLS_DTPMOD64 (7): 64-bit module id.</summary>
    public static ulong TlsDtpmod64(ulong tlsModuleId) => tlsModuleId;

    /// <summary>R_RISCV_TLS_DTPREL32 (8): <c>S + A - TLS_DTV_OFFSET</c> (32-bit DTP-relative).</summary>
    public static uint TlsDtprel32(long s, long a, long tlsDtvOffset) =>
        unchecked((uint)(s + a - tlsDtvOffset));

    /// <summary>R_RISCV_TLS_DTPREL64 (9): 64-bit DTP-relative.</summary>
    public static ulong TlsDtprel64(long s, long a, long tlsDtvOffset) =>
        unchecked((ulong)(s + a - tlsDtvOffset));

    /// <summary>R_RISCV_TLS_TPREL32 (10): <c>S + A + TLSOFFSET</c> (TP-relative, 32-bit).</summary>
    public static uint TlsTprel32(long s, long a, long tlsOffset) =>
        unchecked((uint)(s + a + tlsOffset));

    /// <summary>R_RISCV_TLS_TPREL64 (11): TP-relative, 64-bit.</summary>
    public static ulong TlsTprel64(long s, long a, long tlsOffset) =>
        unchecked((ulong)(s + a + tlsOffset));

    /// <summary>R_RISCV_TLSDESC (12): dynamic TLSDESC resolver — runtime fills descriptor; not a simple static word.</summary>
    public static bool TlsDescIsDynamic => true;

    /// <summary>R_RISCV_BRANCH (16): B-type offset — <c>S + A - P</c> (bytes, multiple of 2).</summary>
    public static long BranchOffset(long s, long a, long p) => s + a - p;

    /// <summary>R_RISCV_JAL (17): J-type offset — <c>S + A - P</c>.</summary>
    public static long JalOffset(long s, long a, long p) => s + a - p;

    /// <summary>R_RISCV_CALL / R_RISCV_CALL_PLT (18–19): auipc uses PCREL HI20; jalr uses PCREL LO12 — same deltas as rows 23–24.</summary>
    public static long CallAuipcPcrelDelta(long s, long a, long p) => s + a - p;

    /// <summary>GOT-based HI20 (20–22): U-type — <c>G + GOT + A - P</c> (GOT / TLS GOT variants use same field layout).</summary>
    public static long GotHi20Delta(long g, long gotBase, long a, long p) => g + gotBase + a - p;

    /// <summary>R_RISCV_PCREL_HI20 (23): U-type (auipc) high 20 bits of <c>S + A - P</c>: <c>((delta + 0x800) &gt;&gt; 12)</c> in range ±2^19.</summary>
    public static int PcrelHi20Imm(long s, long a, long p) => PcrelHi20ImmFromDelta(s + a - p);

    /// <summary>R_RISCV_PCREL_LO12_I / PCREL_LO12_S (24–25): low 12 bits of <c>S + A - P_hi</c> (signed), where <c>P_hi</c> is the auipc PC.</summary>
    public static int PcrelLo12Imm(long s, long a, long pHi) => SignExtend12(s + a - pHi);

    /// <summary>R_RISCV_LO12_I / LO12_S (27–28): low 12 bits of <c>S + A</c> (signed).</summary>
    public static int AbsLo12Imm(long sPlusA_Rv32) => AbsLo12(checked((int)sPlusA_Rv32));

    /// <summary>R_RISCV_TPREL_HI20 (29): U-type — high 20 bits of TP-relative offset (conceptually <c>(S - TP + A)</c> split like HI20).</summary>
    public static int TprelHi20Imm(long sMinusTpPlusA_Rv32) => AbsHi20FromValue(checked((int)sMinusTpPlusA_Rv32));

    /// <summary>R_RISCV_TPREL_LO12_I/S (30–31): low 12 bits of TP-relative offset.</summary>
    public static int TprelLo12Imm(long sMinusTpPlusA_Rv32) => AbsLo12(checked((int)sMinusTpPlusA_Rv32));

    /// <summary>R_RISCV_TPREL_ADD (32): R-type/add — documents TLS LE add; value depends on emitted instruction (psABI pairing).</summary>
    public static long TprelAddOffset(long s, long a, long tp) => s + a - tp;

    /// <summary>R_RISCV_ADD8..ADD64 (33–36): <c>V + S + A</c> on N-bit field.</summary>
    public static byte Add8(byte v, long s, long a) => unchecked((byte)(v + s + a));

    public static ushort Add16(ushort v, long s, long a) => unchecked((ushort)(v + s + a));

    public static uint Add32(uint v, long s, long a) => unchecked((uint)((long)v + s + a));

    public static ulong Add64(ulong v, long s, long a) => unchecked((ulong)((long)v + s + a));

    /// <summary>R_RISCV_SUB8..SUB64 (37–40): <c>V - S - A</c>.</summary>
    public static byte Sub8(byte v, long s, long a) => unchecked((byte)(v - s - a));

    public static ushort Sub16(ushort v, long s, long a) => unchecked((ushort)(v - s - a));

    public static uint Sub32(uint v, long s, long a) => unchecked((uint)((long)v - s - a));

    public static ulong Sub64(ulong v, long s, long a) => unchecked((ulong)((long)v - s - a));

    /// <summary>R_RISCV_GOT32_PCREL (41): <c>G + GOT + A - P</c> (32-bit word).</summary>
    public static uint Got32Pcrel(long g, long gotBase, long a, long p) =>
        unchecked((uint)(g + gotBase + a - p));

    /// <summary>R_RISCV_ALIGN (43): linker-only; addend carries padding size in bytes (not an instruction immediate).</summary>
    public static int AlignAddendBytes(int addend) => addend;

    /// <summary>R_RISCV_RVC_BRANCH (44): CB-type — <c>S + A - P</c> in ±256 bytes (8-bit scaled offset in insn).</summary>
    public static long RvcBranchOffset(long s, long a, long p) => s + a - p;

    /// <summary>R_RISCV_RVC_JUMP (45): CJ-type — <c>S + A - P</c> in ±2 KiB.</summary>
    public static long RvcJumpOffset(long s, long a, long p) => s + a - p;

    /// <summary>R_RISCV_RELAX (51): no immediate change; pairs with another relocation at the same offset.</summary>
    public static uint RelaxLeavesBits(uint instruction) => instruction;

    /// <summary>R_RISCV_SUB6 (52): <c>V - S - A</c> in 6-bit local label math.</summary>
    public static byte Sub6(byte v, long s, long a) => unchecked((byte)(v - s - a));

    /// <summary>R_RISCV_SET6..SET32 (53–56): <c>S + A</c> in N-bit field.</summary>
    public static byte Set6(byte field, long s, long a) => unchecked((byte)(s + a));

    public static byte Set8(byte field, long s, long a) => unchecked((byte)(s + a));

    public static ushort Set16(ushort field, long s, long a) => unchecked((ushort)(s + a));

    public static uint Set32(uint field, long s, long a) => unchecked((uint)(s + a));

    /// <summary>R_RISCV_32_PCREL (57): word32 — <c>S + A - P</c>.</summary>
    public static uint Word32Pcrel(long s, long a, long p) => unchecked((uint)(s + a - p));

    /// <summary>R_RISCV_IRELATIVE (58): dynamic — resolved to <c>ifunc_resolver(B + A)</c>.</summary>
    public static bool IrelativeIsDynamic => true;

    /// <summary>R_RISCV_PLT32 (59): word32 — <c>S + A - P</c> (offset to function or PLT stub).</summary>
    public static uint Plt32(long s, long a, long p) => unchecked((uint)(s + a - p));

    /// <summary>R_RISCV_SET_ULEB128 / SUB_ULEB128 (60–61): ULEB128 pair — set <c>S + A</c>, subtract <c>V - S - A</c> (linker merges).</summary>
    public static ulong Uleb128Set(ulong streamValue, long s, long a) => unchecked((ulong)((long)streamValue + s + a));

    public static ulong Uleb128Sub(ulong streamValue, long s, long a) => unchecked((ulong)((long)streamValue - s - a));

    /// <summary>R_RISCV_TLSDESC_HI20 (62): U-type — same PC-relative HI20 layout as auipc for descriptor address.</summary>
    public static int TlsdescHi20Imm(long s, long a, long p) => PcrelHi20Imm(s, a, p);

    /// <summary>R_RISCV_TLSDESC_LOAD_LO12 / ADD_LO12 (63–64): same as PCREL LO12 with paired HI.</summary>
    public static int TlsdescLo12Imm(long s, long a, long pHi) => PcrelLo12Imm(s, a, pHi);

    /// <summary>R_RISCV_TLSDESC_CALL (65): annotation for resolver call (relaxation); no standalone immediate formula.</summary>
    public static bool TlsdescCallIsAnnotation => true;

    /// <summary>R_RISCV_VENDOR (191): prefix relocation; vendor-specific r_type follows.</summary>
    public static bool VendorIsPrefix => true;

    // ---- shared pieces (also used by Relocation.ApplyToInstruction32) ----

    /// <summary>High 20 bits of PC-relative delta for auipc: <c>((delta + 0x800) &gt;&gt; 12)</c>.</summary>
    public static int PcrelHi20ImmFromDelta(long delta)
    {
        long hi = (delta + 0x800) >> 12;
        if (hi is < -(1L << 19) or >= 1L << 19)
        {
            throw new InvalidOperationException($"PC-relative HI20 delta {delta} yields out-of-range high immediate {hi}.");
        }

        return (int)hi;
    }

    /// <summary>RV32 <c>lui</c> + <c>addi</c> split: <c>hi = (v - lo) &gt;&gt; 12</c>, <c>lo = sign_extend_12(v)</c>.</summary>
    public static int AbsHi20FromValue(int v)
    {
        int lo = (v << 20) >> 20;
        int hi = (v - lo) >> 12;
        if (hi < -(1 << 19) || hi >= 1 << 19)
        {
            throw new InvalidOperationException($"Absolute HI20 out of range for value 0x{v:X8}.");
        }

        return hi;
    }

    /// <summary>Low 12 bits of RV32 absolute address (signed).</summary>
    public static int AbsLo12(int v) => (v << 20) >> 20;

    /// <summary>Sign-extend low 12 bits of a 32-bit offset to int.</summary>
    public static int SignExtend12(long delta)
    {
        if (delta < int.MinValue || delta > int.MaxValue)
        {
            throw new InvalidOperationException($"Value {delta} does not fit in 32-bit.");
        }

        int d32 = (int)delta;
        return (d32 << 20) >> 20;
    }
}
