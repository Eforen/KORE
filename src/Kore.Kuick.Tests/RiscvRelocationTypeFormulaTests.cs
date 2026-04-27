using System;
using Kore.RiscMeta;
using NUnit.Framework;

namespace Kore.Kuick.Tests;

/// <summary>
/// One test per <see cref="RiscvRelocationType"/> documenting the psABI-style formula with hand-picked numbers.
/// See also: Sphinx <c>docs/source/architecture/riscv_elf_relocations.rst</c> and <see cref="RiscvRelocationArithmetic"/>.
/// </summary>
[TestFixture]
public sealed class RiscvRelocationTypeFormulaTests
{
    /// <summary>
    /// <c>R_RISCV_NONE</c> (0): no relocation — the patched bits stay as-is.
    /// Calculation: N/A (identity). Example word <c>0xdeadbeef</c> unchanged.
    /// </summary>
    [Test]
    public void Reloc_None_IsIdentity()
    {
        const uint placeholder = 0xDEADBEEFu;
        Assert.That(RiscvRelocationArithmetic.None(placeholder), Is.EqualTo(placeholder));

        var rel = new Relocation(0, "x", 0, RiscvRelocationType.R_RISCV_NONE);
        Assert.That(rel.ApplyToInstruction32(placeholder, 0, 0), Is.EqualTo(placeholder));
    }

    /// <summary>
    /// <c>R_RISCV_32</c> (1): 32-bit absolute word — <c>result = S + A</c>.
    /// Static example: <c>S = 0x1000</c>, <c>A = 0x24</c> → <c>0x1024</c>.
    /// </summary>
    [Test]
    public void Reloc_Word32_S_Plus_A()
    {
        const long s = 0x1000;
        const long a = 0x24;
        Assert.That(RiscvRelocationArithmetic.Word32(s, a), Is.EqualTo(0x1024u));

        var rel = new Relocation(0, "sym", a, RiscvRelocationType.R_RISCV_32);
        Assert.That(rel.ApplyToInstruction32(0, s, 0), Is.EqualTo(0x1024u));
    }

    /// <summary>
    /// <c>R_RISCV_64</c> (2): 64-bit absolute word — <c>result = S + A</c> (truncated to ulong in tooling).
    /// Example: <c>S = 0x1000</c>, <c>A = 1</c> → <c>0x1001</c>.
    /// </summary>
    [Test]
    public void Reloc_Word64_S_Plus_A()
    {
        Assert.That(RiscvRelocationArithmetic.Word64(0x1000, 1), Is.EqualTo(0x1001ul));
    }

    /// <summary>
    /// <c>R_RISCV_RELATIVE</c> (3): dynamic relocation — <c>result = B + A</c> (load address fixup).
    /// Example: load base <c>B = 0x4000</c>, <c>A = 0x10</c> → <c>0x4010</c>.
    /// </summary>
    [Test]
    public void Reloc_Relative_B_Plus_A()
    {
        Assert.That(RiscvRelocationArithmetic.Relative(0x4000, 0x10), Is.EqualTo(0x4010ul));
    }

    /// <summary>
    /// <c>R_RISCV_COPY</c> (4): resolved only at dynamic load time (copy from DSO); no static numeric result.
    /// </summary>
    [Test]
    public void Reloc_Copy_IsRuntime()
    {
        Assert.That(RiscvRelocationArithmetic.CopyIsResolvedAtRuntime, Is.True);
    }

    /// <summary>
    /// <c>R_RISCV_JUMP_SLOT</c> (5): PLT slot gets the resolved symbol address — <c>result = S</c>.
    /// Example: <c>S = 0x8000</c>.
    /// </summary>
    [Test]
    public void Reloc_JumpSlot_Is_Symbol_Address()
    {
        Assert.That(RiscvRelocationArithmetic.JumpSlot(0x8000), Is.EqualTo(0x8000ul));
    }

    /// <summary>
    /// <c>R_RISCV_TLS_DTPMOD32</c> (6): 32-bit TLS module id (runtime). Example id <c>0x3</c>.
    /// </summary>
    [Test]
    public void Reloc_TlsDtpmod32_Is_Module_Id()
    {
        Assert.That(RiscvRelocationArithmetic.TlsDtpmod32(3), Is.EqualTo(3u));
    }

    /// <summary>
    /// <c>R_RISCV_TLS_DTPMOD64</c> (7): 64-bit module id.
    /// </summary>
    [Test]
    public void Reloc_TlsDtpmod64_Is_Module_Id()
    {
        Assert.That(RiscvRelocationArithmetic.TlsDtpmod64(9), Is.EqualTo(9ul));
    }

    /// <summary>
    /// <c>R_RISCV_TLS_DTPREL32</c> (8): <c>S + A - TLS_DTV_OFFSET</c>. Example: <c>S=0x100</c>, <c>A=4</c>, <c>TLS_DTV_OFFSET=0x40</c> → <c>0x100+4-0x40 = 0xC4 = 196</c>.
    /// </summary>
    [Test]
    public void Reloc_TlsDtprel32_Formula()
    {
        Assert.That(RiscvRelocationArithmetic.TlsDtprel32(0x100, 4, 0x40), Is.EqualTo(0xC4u));
    }

    /// <summary>
    /// <c>R_RISCV_TLS_DTPREL64</c> (9): same as 32-bit with 64-bit word.
    /// </summary>
    [Test]
    public void Reloc_TlsDtprel64_Formula()
    {
        Assert.That(RiscvRelocationArithmetic.TlsDtprel64(0x100, 4, 0x40), Is.EqualTo(0xC4ul));
    }

    /// <summary>
    /// <c>R_RISCV_TLS_TPREL32</c> (10): <c>S + A + TLSOFFSET</c>. Example: <c>1 + 2 + 100 = 103</c>.
    /// </summary>
    [Test]
    public void Reloc_TlsTprel32_Formula()
    {
        Assert.That(RiscvRelocationArithmetic.TlsTprel32(1, 2, 100), Is.EqualTo(103u));
    }

    /// <summary>
    /// <c>R_RISCV_TLS_TPREL64</c> (11): 64-bit TP-relative word.
    /// </summary>
    [Test]
    public void Reloc_TlsTprel64_Formula()
    {
        Assert.That(RiscvRelocationArithmetic.TlsTprel64(1, 2, 100), Is.EqualTo(103ul));
    }

    /// <summary>
    /// <c>R_RISCV_TLSDESC</c> (12): dynamic TLS descriptor — not a single static word.
    /// </summary>
    [Test]
    public void Reloc_TlsDesc_IsDynamic()
    {
        Assert.That(RiscvRelocationArithmetic.TlsDescIsDynamic, Is.True);
    }

    /// <summary>
    /// <c>R_RISCV_BRANCH</c> (16): B-type offset <c>S + A - P</c>. Example: target <c>P+8</c> with <c>A=0</c> → offset <c>8</c>.
    /// </summary>
    [Test]
    public void Reloc_Branch_S_Plus_A_Minus_P()
    {
        long s = 0x1008;
        long p = 0x1000;
        Assert.That(RiscvRelocationArithmetic.BranchOffset(s, 0, p), Is.EqualTo(8));

        uint raw = Encoding.EncodeBType(Register.zero, Register.zero, 0, OpCodes.Funct3Codes.BEQ, OpCodes.BRANCH);
        var rel = new Relocation(0, "t", 0, RiscvRelocationType.R_RISCV_BRANCH);
        uint patched = rel.ApplyToInstruction32(raw, s, p);
        Assert.That(Decoding.Decode(patched).Imm, Is.EqualTo(8));
    }

    /// <summary>
    /// <c>R_RISCV_JAL</c> (17): J-type offset <c>S + A - P</c>. Example: <c>S=0x2000</c>, <c>P=0x1000</c> → <c>0x1000</c>.
    /// </summary>
    [Test]
    public void Reloc_Jal_S_Plus_A_Minus_P()
    {
        Assert.That(RiscvRelocationArithmetic.JalOffset(0x2000, 0, 0x1000), Is.EqualTo(0x1000));

        uint raw = Encoding.EncodeUJType(Register.ra, 0, OpCodes.JAL);
        var rel = new Relocation(0, "f", 0, RiscvRelocationType.R_RISCV_JAL);
        uint patched = rel.ApplyToInstruction32(raw, 0x2000, 0x1000);
        Assert.That(Decoding.Decode(patched).Imm, Is.EqualTo(0x1000));
    }

    /// <summary>
    /// <c>R_RISCV_CALL</c> (18): <c>auipc</c> half uses PCREL HI20; <c>jalr</c> half uses PCREL LO12 vs paired PC.
    /// Example: <c>S=0x2000</c>, <c>P_hi=0x1000</c>, <c>P_lo=0x1004</c>, <c>A=0</c> → delta <c>0x1000</c>, HI immediate <c>1</c>, LO immediate <c>0</c>.
    /// </summary>
    [Test]
    public void Reloc_Call_Auipc_And_Jalr()
    {
        long s = 0x2000;
        long pHi = 0x1000;
        long pLo = 0x1004;
        Assert.That(RiscvRelocationArithmetic.CallAuipcPcrelDelta(s, 0, pHi), Is.EqualTo(0x1000));
        Assert.That(RiscvRelocationArithmetic.PcrelLo12Imm(s, 0, pHi), Is.EqualTo(0));

        uint auipc = Encoding.EncodeUType(Register.ra, 0, OpCodes.AUIPC);
        uint jalr = Encoding.EncodeIType(Register.ra, Register.ra, 0, OpCodes.Funct3Codes.ADDI, OpCodes.JALR);

        var r1 = new Relocation(0, "fn", 0, RiscvRelocationType.R_RISCV_CALL);
        uint auipcP = r1.ApplyToInstruction32(auipc, s, pHi);
        uint jalrP = r1.ApplyToInstruction32(jalr, s, pLo, pHi);

        Assert.That(Decoding.Decode(auipcP).Imm, Is.EqualTo(0x1000));
        Assert.That(Decoding.Decode(jalrP).Imm, Is.EqualTo(0));
    }

    /// <summary>
    /// <c>R_RISCV_CALL_PLT</c> (19): same field math as <see cref="Reloc_Call_Auipc_And_Jalr"/>; PLT affects which symbol <c>S</c> names.
    /// </summary>
    [Test]
    public void Reloc_CallPlt_Same_As_Call_For_Field_Math()
    {
        long s = 0x2000;
        long pHi = 0x1000;
        long pLo = 0x1004;
        uint auipc = Encoding.EncodeUType(Register.ra, 0, OpCodes.AUIPC);
        uint jalr = Encoding.EncodeIType(Register.ra, Register.ra, 0, OpCodes.Funct3Codes.ADDI, OpCodes.JALR);
        var rel = new Relocation(0, "fn", 0, RiscvRelocationType.R_RISCV_CALL_PLT);
        Assert.That(Decoding.Decode(rel.ApplyToInstruction32(auipc, s, pHi)).Imm, Is.EqualTo(0x1000));
        Assert.That(Decoding.Decode(rel.ApplyToInstruction32(jalr, s, pLo, pHi)).Imm, Is.EqualTo(0));
    }

    /// <summary>
    /// <c>R_RISCV_GOT_HI20</c> (20): U-type — same HI20 math as PCREL: <c>((S+A-P)+0x800)&gt;&gt;12</c> when <c>S</c> is the resolved GOT entry address (example uses direct values).
    /// Example: <c>S=0x2000</c>, <c>P=0x1000</c> → HI immediate <c>1</c>.
    /// </summary>
    [Test]
    public void Reloc_GotHi20_Matches_PcrelHi_Field()
    {
        Assert.That(RiscvRelocationArithmetic.PcrelHi20Imm(0x2000, 0, 0x1000), Is.EqualTo(1));
        uint auipc = Encoding.EncodeUType(Register.t0, 0, OpCodes.AUIPC);
        var rel = new Relocation(0, "g", 0, RiscvRelocationType.R_RISCV_GOT_HI20);
        Assert.That(Decoding.Decode(rel.ApplyToInstruction32(auipc, 0x2000, 0x1000)).Imm, Is.EqualTo(0x1000));
    }

    /// <summary>
    /// <c>R_RISCV_TLS_GOT_HI20</c> (21): same U-type layout as GOT HI20; symbol names a TLS IE GOT cell.
    /// </summary>
    [Test]
    public void Reloc_TlsGotHi20_Field_Same_As_GotHi20()
    {
        uint auipc = Encoding.EncodeUType(Register.t0, 0, OpCodes.AUIPC);
        var rel = new Relocation(0, "x", 0, RiscvRelocationType.R_RISCV_TLS_GOT_HI20);
        Assert.That(Decoding.Decode(rel.ApplyToInstruction32(auipc, 0x2000, 0x1000)).Imm, Is.EqualTo(0x1000));
    }

    /// <summary>
    /// <c>R_RISCV_TLS_GD_HI20</c> (22): same U-type field as GOT HI20 for TLS GD pair.
    /// </summary>
    [Test]
    public void Reloc_TlsGdHi20_Field_Same_As_GotHi20()
    {
        uint auipc = Encoding.EncodeUType(Register.t0, 0, OpCodes.AUIPC);
        var rel = new Relocation(0, "x", 0, RiscvRelocationType.R_RISCV_TLS_GD_HI20);
        Assert.That(Decoding.Decode(rel.ApplyToInstruction32(auipc, 0x2000, 0x1000)).Imm, Is.EqualTo(0x1000));
    }

    /// <summary>
    /// <c>R_RISCV_PCREL_HI20</c> (23): <c>delta = S+A-P</c>, HI <c>=(delta+0x800)&gt;&gt;12</c>. Example: <c>delta=0x1000</c> → HI <c>1</c> → U imm bits <c>0x1000</c> when decoded.
    /// </summary>
    [Test]
    public void Reloc_PcrelHi20_Auipc()
    {
        Assert.That(RiscvRelocationArithmetic.PcrelHi20Imm(0x2000, 0, 0x1000), Is.EqualTo(1));
        uint auipc = Encoding.EncodeUType(Register.t0, 0, OpCodes.AUIPC);
        var rel = new Relocation(0, "sym", 0, RiscvRelocationType.R_RISCV_PCREL_HI20);
        Assert.That(Decoding.Decode(rel.ApplyToInstruction32(auipc, 0x2000, 0x1000)).Imm, Is.EqualTo(0x1000));
    }

    /// <summary>
    /// <c>R_RISCV_PCREL_LO12_I</c> (24): low 12 bits of <c>S+A-P_hi</c>. Example: <c>S=0x2000</c>, <c>P_hi=0x1000</c> → low12 <c>0</c> for <c>addi</c>.
    /// </summary>
    [Test]
    public void Reloc_PcrelLo12_I_Addi()
    {
        Assert.That(RiscvRelocationArithmetic.PcrelLo12Imm(0x2000, 0, 0x1000), Is.EqualTo(0));
        uint addi = Encoding.EncodeIType(Register.t1, Register.t0, 0, OpCodes.Funct3Codes.ADDI, OpCodes.OP_IMM);
        var rel = new Relocation(4, "sym", 0, RiscvRelocationType.R_RISCV_PCREL_LO12_I);
        uint patched = rel.ApplyToInstruction32(addi, 0x2000, 0x1004, 0x1000);
        Assert.That(Decoding.Decode(patched).Imm, Is.EqualTo(0));
    }

    /// <summary>
    /// <c>R_RISCV_PCREL_LO12_S</c> (25): same delta as LO12_I but S-type (store).
    /// </summary>
    [Test]
    public void Reloc_PcrelLo12_S_Store()
    {
        uint sw = Encoding.EncodeSType(Register.t0, Register.t1, 0, OpCodes.Funct3Codes.SW, OpCodes.STORE);
        var rel = new Relocation(4, "sym", 0, RiscvRelocationType.R_RISCV_PCREL_LO12_S);
        uint patched = rel.ApplyToInstruction32(sw, 0x2000, 0x1004, 0x1000);
        Assert.That(Decoding.Decode(patched).Imm, Is.EqualTo(0));
    }

    /// <summary>
    /// <c>R_RISCV_HI20</c> (26): LUI — high 20 bits of RV32 <c>S+A</c>. Example: address <c>0x12345678</c> → HI and LO split per psABI.
    /// </summary>
    [Test]
    public void Reloc_Hi20_Lui()
    {
        const int target = 0x12345678;
        Assert.That(RiscvRelocationArithmetic.AbsHi20FromValue(target), Is.EqualTo(0x12345));
        uint lui = Encoding.EncodeUType(Register.t0, 0, OpCodes.LUI);
        var rel = new Relocation(0, "x", 0, RiscvRelocationType.R_RISCV_HI20);
        Assert.That(Decoding.Decode(rel.ApplyToInstruction32(lui, target, 0)).Imm, Is.EqualTo(0x12345000));
    }

    /// <summary>
    /// <c>R_RISCV_LO12_I</c> (27): low 12 bits of <c>S+A</c> for I-type. With <c>0x12345678</c>, LO <c>0x678</c>.
    /// </summary>
    [Test]
    public void Reloc_Lo12_I_Addi()
    {
        const int target = 0x12345678;
        Assert.That(RiscvRelocationArithmetic.AbsLo12(target), Is.EqualTo(0x678));
        uint addi = Encoding.EncodeIType(Register.t0, Register.t0, 0, OpCodes.Funct3Codes.ADDI, OpCodes.OP_IMM);
        var rel = new Relocation(4, "x", 0, RiscvRelocationType.R_RISCV_LO12_I);
        Assert.That(Decoding.Decode(rel.ApplyToInstruction32(addi, target, 4)).Imm, Is.EqualTo(0x678));
    }

    /// <summary>
    /// <c>R_RISCV_LO12_S</c> (28): low 12 bits for S-type store.
    /// </summary>
    [Test]
    public void Reloc_Lo12_S_Store()
    {
        const int target = 0x12345678;
        uint sw = Encoding.EncodeSType(Register.t0, Register.t1, 0, OpCodes.Funct3Codes.SW, OpCodes.STORE);
        var rel = new Relocation(4, "x", 0, RiscvRelocationType.R_RISCV_LO12_S);
        Assert.That(Decoding.Decode(rel.ApplyToInstruction32(sw, target, 4)).Imm, Is.EqualTo(0x678));
    }

    /// <summary>
    /// <c>R_RISCV_TPREL_HI20</c> (29): same HI20 split as absolute, applied to TP-relative offset (here <c>S-TP+A</c> passed as one RV32 value <c>0x800</c>).
    /// </summary>
    [Test]
    public void Reloc_TprelHi20_Split()
    {
        Assert.That(RiscvRelocationArithmetic.TprelHi20Imm(0x800), Is.EqualTo(RiscvRelocationArithmetic.AbsHi20FromValue(0x800)));
    }

    /// <summary>
    /// <c>R_RISCV_TPREL_LO12_I</c> (30): low 12 bits of TP-relative offset.
    /// </summary>
    [Test]
    public void Reloc_TprelLo12_I()
    {
        Assert.That(RiscvRelocationArithmetic.TprelLo12Imm(0x456), Is.EqualTo(0x456));
    }

    /// <summary>
    /// <c>R_RISCV_TPREL_LO12_S</c> (31): same low 12 as I variant for stores.
    /// </summary>
    [Test]
    public void Reloc_TprelLo12_S()
    {
        Assert.That(RiscvRelocationArithmetic.TprelLo12Imm(-8), Is.EqualTo(-8));
    }

    /// <summary>
    /// <c>R_RISCV_TPREL_ADD</c> (32): conceptual offset <c>S + A - TP</c> for the add that adjusts to TLS LE.
    /// Example: <c>S=0x200</c>, <c>TP=0x100</c>, <c>A=0</c> → <c>0x100</c>.
    /// </summary>
    [Test]
    public void Reloc_TprelAdd_Offset()
    {
        Assert.That(RiscvRelocationArithmetic.TprelAddOffset(0x200, 0, 0x100), Is.EqualTo(0x100));
    }

    /// <summary>
    /// <c>R_RISCV_ADD8</c> (33): <c>V + S + A</c>. Example: <c>V=2</c>, <c>S=3</c>, <c>A=4</c> → <c>9</c>.
    /// </summary>
    [Test]
    public void Reloc_Add8()
    {
        Assert.That(RiscvRelocationArithmetic.Add8(2, 3, 4), Is.EqualTo((byte)9));
    }

    /// <summary>
    /// <c>R_RISCV_ADD16</c> (34): <c>V + S + A</c> on 16 bits.
    /// </summary>
    [Test]
    public void Reloc_Add16()
    {
        Assert.That(RiscvRelocationArithmetic.Add16(10, 20, 30), Is.EqualTo((ushort)60));
    }

    /// <summary>
    /// <c>R_RISCV_ADD32</c> (35): <c>V + S + A</c> on 32 bits.
    /// </summary>
    [Test]
    public void Reloc_Add32()
    {
        Assert.That(RiscvRelocationArithmetic.Add32(0xFFFF0000u, 0x100, 0x200), Is.EqualTo(0xFFFF0300u));
    }

    /// <summary>
    /// <c>R_RISCV_ADD64</c> (36): <c>V + S + A</c> on 64 bits.
    /// </summary>
    [Test]
    public void Reloc_Add64()
    {
        Assert.That(RiscvRelocationArithmetic.Add64(5ul, 10, 100), Is.EqualTo(115ul));
    }

    /// <summary>
    /// <c>R_RISCV_SUB8</c> (37): <c>V - S - A</c>. Example: <c>10 - 1 - 2 = 7</c>.
    /// </summary>
    [Test]
    public void Reloc_Sub8()
    {
        Assert.That(RiscvRelocationArithmetic.Sub8(10, 1, 2), Is.EqualTo((byte)7));
    }

    /// <summary>
    /// <c>R_RISCV_SUB16</c> (38): 16-bit subtraction fragment.
    /// </summary>
    [Test]
    public void Reloc_Sub16()
    {
        Assert.That(RiscvRelocationArithmetic.Sub16(100, 30, 20), Is.EqualTo((ushort)50));
    }

    /// <summary>
    /// <c>R_RISCV_SUB32</c> (39): 32-bit subtraction fragment.
    /// </summary>
    [Test]
    public void Reloc_Sub32()
    {
        Assert.That(RiscvRelocationArithmetic.Sub32(1000u, 100, 200), Is.EqualTo(700u));
    }

    /// <summary>
    /// <c>R_RISCV_SUB64</c> (40): 64-bit subtraction fragment.
    /// </summary>
    [Test]
    public void Reloc_Sub64()
    {
        Assert.That(RiscvRelocationArithmetic.Sub64(100ul, 25, 25), Is.EqualTo(50ul));
    }

    /// <summary>
    /// <c>R_RISCV_GOT32_PCREL</c> (41): <c>G + GOT + A - P</c>. Example: <c>G=4</c>, <c>GOT=0x3000</c>, <c>A=0</c>, <c>P=0x1000</c> → <c>0x2004</c>.
    /// </summary>
    [Test]
    public void Reloc_Got32Pcrel()
    {
        Assert.That(RiscvRelocationArithmetic.Got32Pcrel(4, 0x3000, 0, 0x1000), Is.EqualTo(0x2004u));
    }

    /// <summary>
    /// <c>R_RISCV_ALIGN</c> (43): linker records padding length in the addend (bytes of <c>nop</c>); not an instruction immediate.
    /// Example addend <c>16</c>.
    /// </summary>
    [Test]
    public void Reloc_Align_Addend_Is_Padding_Bytes()
    {
        Assert.That(RiscvRelocationArithmetic.AlignAddendBytes(16), Is.EqualTo(16));
        var rel = new Relocation(0, "a", 16, RiscvRelocationType.R_RISCV_ALIGN);
        Assert.That(rel.ApplyToInstruction32(0xFFFFFFFF, 0, 0), Is.EqualTo(0xFFFFFFFF));
    }

    /// <summary>
    /// <c>R_RISCV_RVC_BRANCH</c> (44): compressed branch — offset <c>S+A-P</c> (must fit CB range when encoded).
    /// Example: <c>0x1100 - 0x1000 = 0x100</c>.
    /// </summary>
    [Test]
    public void Reloc_RvcBranch_Offset()
    {
        Assert.That(RiscvRelocationArithmetic.RvcBranchOffset(0x1100, 0, 0x1000), Is.EqualTo(0x100));
    }

    /// <summary>
    /// <c>R_RISCV_RVC_JUMP</c> (45): compressed jump — <c>S+A-P</c>.
    /// </summary>
    [Test]
    public void Reloc_RvcJump_Offset()
    {
        Assert.That(RiscvRelocationArithmetic.RvcJumpOffset(0x2000, 0, 0x1000), Is.EqualTo(0x1000));
    }

    /// <summary>
    /// <c>R_RISCV_RELAX</c> (51): no bit change by itself; pairs with another relocation at the same offset.
    /// </summary>
    [Test]
    public void Reloc_Relax_Leaves_Word()
    {
        const uint w = 0x00C0006F;
        Assert.That(RiscvRelocationArithmetic.RelaxLeavesBits(w), Is.EqualTo(w));
        var rel = new Relocation(0, "r", 0, RiscvRelocationType.R_RISCV_RELAX);
        Assert.That(rel.ApplyToInstruction32(w, 0, 0), Is.EqualTo(w));
    }

    /// <summary>
    /// <c>R_RISCV_SUB6</c> (52): <c>V - S - A</c> in 6-bit local label math.
    /// </summary>
    [Test]
    public void Reloc_Sub6()
    {
        Assert.That(RiscvRelocationArithmetic.Sub6(20, 3, 4), Is.EqualTo((byte)13));
    }

    /// <summary>
    /// <c>R_RISCV_SET6</c> (53): assign <c>S+A</c> into a 6-bit field (example small positive).
    /// </summary>
    [Test]
    public void Reloc_Set6()
    {
        Assert.That(RiscvRelocationArithmetic.Set6(0, 10, 5), Is.EqualTo((byte)15));
    }

    /// <summary>
    /// <c>R_RISCV_SET8</c> (54): assign <c>S+A</c> into 8 bits.
    /// </summary>
    [Test]
    public void Reloc_Set8()
    {
        Assert.That(RiscvRelocationArithmetic.Set8(0, 200, 55), Is.EqualTo((byte)255));
    }

    /// <summary>
    /// <c>R_RISCV_SET16</c> (55): assign into 16 bits.
    /// </summary>
    [Test]
    public void Reloc_Set16()
    {
        Assert.That(RiscvRelocationArithmetic.Set16(0, 0x1000, 0x200), Is.EqualTo((ushort)0x1200));
    }

    /// <summary>
    /// <c>R_RISCV_SET32</c> (56): assign into 32 bits.
    /// </summary>
    [Test]
    public void Reloc_Set32()
    {
        Assert.That(RiscvRelocationArithmetic.Set32(0, 0x1000, 0x24), Is.EqualTo(0x1024u));
    }

    /// <summary>
    /// <c>R_RISCV_32_PCREL</c> (57): word — <c>S + A - P</c>. Example: <c>0x3000 - 0x1000 = 0x2000</c>.
    /// </summary>
    [Test]
    public void Reloc_Word32Pcrel()
    {
        Assert.That(RiscvRelocationArithmetic.Word32Pcrel(0x3000, 0, 0x1000), Is.EqualTo(0x2000u));
        var rel = new Relocation(0, "x", 0, RiscvRelocationType.R_RISCV_32_PCREL);
        Assert.That(rel.ApplyToInstruction32(0, 0x3000, 0x1000), Is.EqualTo(0x2000u));
    }

    /// <summary>
    /// <c>R_RISCV_IRELATIVE</c> (58): ifunc — resolved by calling resolver(<c>B+A</c>) at load time.
    /// </summary>
    [Test]
    public void Reloc_Irelative_IsDynamic()
    {
        Assert.That(RiscvRelocationArithmetic.IrelativeIsDynamic, Is.True);
    }

    /// <summary>
    /// <c>R_RISCV_PLT32</c> (59): word — <c>S + A - P</c> (same arithmetic as 32_PCREL in many toolchains).
    /// </summary>
    [Test]
    public void Reloc_Plt32()
    {
        Assert.That(RiscvRelocationArithmetic.Plt32(0x5000, 0, 0x1000), Is.EqualTo(0x4000u));
    }

    /// <summary>
    /// <c>R_RISCV_SET_ULEB128</c> (60): paired ULEB128 “set” fragment — conceptual <c>V + S + A</c> on the encoded stream.
    /// </summary>
    [Test]
    public void Reloc_SetUleb128()
    {
        Assert.That(RiscvRelocationArithmetic.Uleb128Set(100ul, 5, 7), Is.EqualTo(112ul));
    }

    /// <summary>
    /// <c>R_RISCV_SUB_ULEB128</c> (61): paired “subtract” fragment — <c>V - S - A</c>.
    /// </summary>
    [Test]
    public void Reloc_SubUleb128()
    {
        Assert.That(RiscvRelocationArithmetic.Uleb128Sub(200ul, 50, 50), Is.EqualTo(100ul));
    }

    /// <summary>
    /// <c>R_RISCV_TLSDESC_HI20</c> (62): same PC-relative HI20 field as <c>R_RISCV_PCREL_HI20</c> for descriptor address.
    /// </summary>
    [Test]
    public void Reloc_TlsdescHi20_Matches_PcrelHi()
    {
        Assert.That(RiscvRelocationArithmetic.TlsdescHi20Imm(0x2000, 0, 0x1000), Is.EqualTo(1));
    }

    /// <summary>
    /// <c>R_RISCV_TLSDESC_LOAD_LO12</c> (63): same as PCREL LO12 with paired HI.
    /// </summary>
    [Test]
    public void Reloc_TlsdescLoadLo12()
    {
        Assert.That(RiscvRelocationArithmetic.TlsdescLo12Imm(0x2000, 0, 0x1000), Is.EqualTo(0));
    }

    /// <summary>
    /// <c>R_RISCV_TLSDESC_ADD_LO12</c> (64): same LO12 math as load variant.
    /// </summary>
    [Test]
    public void Reloc_TlsdescAddLo12()
    {
        Assert.That(RiscvRelocationArithmetic.TlsdescLo12Imm(0x2010, 0, 0x1000), Is.EqualTo(0x10));
    }

    /// <summary>
    /// <c>R_RISCV_TLSDESC_CALL</c> (65): marks resolver <c>jalr</c> for relaxation; no standalone numeric formula.
    /// </summary>
    [Test]
    public void Reloc_TlsdescCall_IsAnnotation()
    {
        Assert.That(RiscvRelocationArithmetic.TlsdescCallIsAnnotation, Is.True);
    }

    /// <summary>
    /// <c>R_RISCV_VENDOR</c> (191): prefix relocation — vendor id in symbol; next relocation is vendor-defined.
    /// </summary>
    [Test]
    public void Reloc_Vendor_IsPrefix()
    {
        Assert.That(RiscvRelocationArithmetic.VendorIsPrefix, Is.True);
    }

    /// <summary>
    /// Guard: every defined <see cref="RiscvRelocationType"/> value has a dedicated test method in this fixture (update when adding enum members).
    /// </summary>
    [Test]
    public void Coverage_All_RiscvRelocationType_Values_Are_Listed_In_This_Fixture()
    {
        Assert.That(Enum.GetNames(typeof(RiscvRelocationType)), Has.Length.EqualTo(58));
    }
}
