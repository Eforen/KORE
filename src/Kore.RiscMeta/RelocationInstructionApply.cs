// src/Kore.RiscMeta/RelocationInstructionApply.cs
using System;

namespace Kore.RiscMeta;

/// <content>Static link–time application of ELF relocations to 32-bit RISC-V instruction words (RV32 addressing).</content>
public partial class Relocation
{
    /// <summary>
    /// Applies this relocation to a 32-bit instruction word, using resolved symbol and layout values.
    /// </summary>
    /// <param name="instruction">
    /// The placeholder instruction (opcode, registers, funct3/7 must already match the relocation site).
    /// </param>
    /// <param name="symbolValue">
    /// <c>S</c> — final VMA of the symbol named by this relocation (or the resolved address for GOT/TLS when the toolchain has already folded that into <paramref name="symbolValue"/>).
    /// </param>
    /// <param name="relocationAddress">
    /// <c>P</c> — VMA of the instruction being patched (must match where this relocation is recorded).
    /// </param>
    /// <param name="pairedPcrelHiAddress">
    /// For <see cref="RiscvRelocationType.R_RISCV_PCREL_LO12_I"/>, <see cref="RiscvRelocationType.R_RISCV_PCREL_LO12_S"/>, and the
    /// <c>jalr</c> half of <see cref="RiscvRelocationType.R_RISCV_CALL"/> / <see cref="RiscvRelocationType.R_RISCV_CALL_PLT"/>:
    /// <c>P</c> of the paired <c>auipc</c> (<c>%pcrel_hi</c> site). Ignored for other types.
    /// </param>
    /// <returns>The instruction word with immediates patched.</returns>
    /// <exception cref="InvalidOperationException">Immediate out of range, wrong instruction shape, or missing <paramref name="pairedPcrelHiAddress"/> when required.</exception>
    /// <exception cref="NotSupportedException">Relocation kind not implemented for instruction patching (TLS, linker-only, compressed, …).</exception>
    public uint ApplyToInstruction32(
        uint instruction,
        long symbolValue,
        long relocationAddress,
        long? pairedPcrelHiAddress = null)
    {
        long s = symbolValue;
        long a = Addend;
        long p = relocationAddress;

        return Type switch
        {
            RiscvRelocationType.R_RISCV_NONE => instruction,

            RiscvRelocationType.R_RISCV_32 => ApplyWord32(s + a),
            RiscvRelocationType.R_RISCV_32_PCREL => ApplyWord32(s + a - p),
            RiscvRelocationType.R_RISCV_RELATIVE => ApplyWord32(s + a),

            RiscvRelocationType.R_RISCV_BRANCH => ApplyBranch(instruction, s + a - p),
            RiscvRelocationType.R_RISCV_JAL => ApplyJal(instruction, s + a - p),

            RiscvRelocationType.R_RISCV_PCREL_HI20
            or RiscvRelocationType.R_RISCV_GOT_HI20
            or RiscvRelocationType.R_RISCV_TLS_GOT_HI20
            or RiscvRelocationType.R_RISCV_TLS_GD_HI20
                => ApplyUTypeHi20(instruction, RiscvRelocationArithmetic.PcrelHi20Imm(s, a, p), OpCodes.AUIPC),

            RiscvRelocationType.R_RISCV_HI20 => ApplyUTypeHi20(
                instruction,
                RiscvRelocationArithmetic.AbsHi20FromValue(checked((int)(s + a))),
                OpCodes.LUI),

            RiscvRelocationType.R_RISCV_PCREL_LO12_I => ApplyILo12(instruction, PcrelLo12(s, a, pairedPcrelHiAddress)),
            RiscvRelocationType.R_RISCV_PCREL_LO12_S => ApplySLo12(instruction, PcrelLo12(s, a, pairedPcrelHiAddress)),

            RiscvRelocationType.R_RISCV_LO12_I => ApplyILo12(
                instruction,
                RiscvRelocationArithmetic.AbsLo12(checked((int)(s + a)))),

            RiscvRelocationType.R_RISCV_LO12_S => ApplySLo12(
                instruction,
                RiscvRelocationArithmetic.AbsLo12(checked((int)(s + a)))),

            RiscvRelocationType.R_RISCV_CALL
            or RiscvRelocationType.R_RISCV_CALL_PLT
                => ApplyCallOrCallPlt(instruction, s, a, p, pairedPcrelHiAddress),

            RiscvRelocationType.R_RISCV_RELAX
            or RiscvRelocationType.R_RISCV_ALIGN
                => instruction,

            _ => throw new NotSupportedException(
                $"Relocation type {Type} is not supported by {nameof(ApplyToInstruction32)} (TLS layout, compressed instructions, or linker-only metadata may require a different path)."),
        };
    }

    private static uint ApplyWord32(long value) => unchecked((uint)value);

    private static uint ApplyBranch(uint instruction, long delta)
    {
        var d = Decoding.Decode(instruction);
        if (d.Format != InstructionType.B)
        {
            throw new InvalidOperationException("R_RISCV_BRANCH requires a B-type branch instruction.");
        }

        if ((delta & 1) != 0)
        {
            throw new InvalidOperationException("Branch relocation offset must be 2-byte aligned.");
        }

        if (delta is < -(1 << 12) or > (1 << 12) - 2)
        {
            throw new InvalidOperationException($"Branch offset {delta} is out of 13-bit signed range.");
        }

        uint imm13 = Imm13FromSigned((int)delta);
        return Encoding.EncodeBType(d.Rs1, d.Rs2, imm13, new Funct3((uint)d.Funct3), Low7Opcode(instruction));
    }

    private static uint ApplyJal(uint instruction, long delta)
    {
        var d = Decoding.Decode(instruction);
        if (d.Instruction != Instruction.JAL)
        {
            throw new InvalidOperationException("R_RISCV_JAL requires a J-type jal instruction.");
        }

        if ((delta & 1) != 0)
        {
            throw new InvalidOperationException("JAL relocation offset must be 2-byte aligned.");
        }

        if (delta is < -(1 << 20) or > (1 << 20) - 2)
        {
            throw new InvalidOperationException($"JAL offset {delta} is out of 21-bit signed range.");
        }

        uint imm21 = Imm21FromSigned((int)delta);
        return Encoding.EncodeUJType(d.Rd, imm21, Low7Opcode(instruction));
    }

    private static uint ApplyUTypeHi20(uint instruction, int hi20, Opcode expectedOpcode)
    {
        var d = Decoding.Decode(instruction);
        if (d.Format != InstructionType.U)
        {
            throw new InvalidOperationException("U-type relocation requires LUI or AUIPC.");
        }

        var opc = Low7Opcode(instruction);
        if ((uint)opc != (uint)expectedOpcode)
        {
            throw new InvalidOperationException($"Expected opcode {expectedOpcode}, got {opc}.");
        }

        if (hi20 < -(1 << 19) || hi20 >= 1 << 19)
        {
            throw new InvalidOperationException($"U-type immediate {hi20} is out of 20-bit signed range.");
        }

        uint imm20 = (uint)(hi20 & 0xFFFFF);
        return Encoding.EncodeUType(d.Rd, imm20, opc);
    }

    private static uint ApplyILo12(uint instruction, int lo12)
    {
        var d = Decoding.Decode(instruction);
        if (d.Format != InstructionType.I)
        {
            throw new InvalidOperationException("R_RISCV_*LO12_I requires an I-type instruction.");
        }

        uint imm12 = ToImm12Bits(lo12);
        return Encoding.EncodeIType(d.Rd, d.Rs1, imm12, new Funct3((uint)d.Funct3), Low7Opcode(instruction));
    }

    private static uint ApplySLo12(uint instruction, int lo12)
    {
        var d = Decoding.Decode(instruction);
        if (d.Format != InstructionType.S)
        {
            throw new InvalidOperationException("R_RISCV_*LO12_S requires an S-type store instruction.");
        }

        uint imm12 = ToImm12Bits(lo12);
        return Encoding.EncodeSType(d.Rs1, d.Rs2, imm12, new Funct3((uint)d.Funct3), Low7Opcode(instruction));
    }

    private static uint ApplyCallOrCallPlt(uint instruction, long s, long a, long p, long? pairedHi)
    {
        var opc = instruction & 0x7Fu;
        if (opc == (uint)OpCodes.AUIPC)
        {
            return ApplyUTypeHi20(instruction, RiscvRelocationArithmetic.PcrelHi20Imm(s, a, p), OpCodes.AUIPC);
        }

        if (opc == (uint)OpCodes.JALR)
        {
            return ApplyILo12(instruction, PcrelLo12(s, a, pairedHi));
        }

        throw new InvalidOperationException(
            "R_RISCV_CALL / R_RISCV_CALL_PLT apply to auipc (first) or jalr (second); opcode does not match.");
    }

    private static int PcrelLo12(long s, long a, long? pHi)
    {
        if (pHi is null)
        {
            throw new InvalidOperationException(
                "PC-relative LO12 relocations require pairedPcrelHiAddress (VMA of the matching auipc instruction).");
        }

        return RiscvRelocationArithmetic.PcrelLo12Imm(s, a, pHi.Value);
    }

    /// <summary>Low 12 bits of <paramref name="lo12"/> as stored in an I/S immediate (unsigned bitmask).</summary>
    private static uint ToImm12Bits(int lo12) => unchecked((uint)lo12) & 0xFFFu;

    private static uint Imm13FromSigned(int delta) => unchecked((uint)(delta & 0x1FFF));

    private static uint Imm21FromSigned(int delta) => unchecked((uint)(delta & 0x1FFFFF));

    private static Opcode Low7Opcode(uint instruction) => instruction & 0x7Fu;
}
