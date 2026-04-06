// src/Kore.RiscMeta/Decoding.cs
namespace Kore.RiscMeta;

/// <summary>
/// Provides static methods to decode 32-bit and 64-bit RISC-V instructions
/// into their respective RawInst structs.
/// </summary>
public static class Decoding
{
    /// <summary>
    /// Decodes a 32-bit RISC-V instruction word into a RawInst32 struct.
    /// </summary>
    /// <param name="instr">The raw 32-bit instruction</param>
    /// <returns>A decoded RawInst32. Returns Invalid instruction if opcode is unknown.</returns>
    public static RawInst32 Decode(uint instr)
    {
        uint opcode = instr & 0b1111111u;

        if (opcode == (uint)OpCodes.OP) return DecodeRType(instr);
        if (opcode == (uint)OpCodes.OP_IMM) return DecodeIType(instr);
        if (opcode == (uint)OpCodes.LOAD) return DecodeIType(instr);
        if (opcode == (uint)OpCodes.STORE) return DecodeSType(instr);
        if (opcode == (uint)OpCodes.BRANCH) return DecodeBType(instr);
        if (opcode == (uint)OpCodes.LUI || opcode == (uint)OpCodes.AUIPC) return DecodeUType(instr);
        if (opcode == (uint)OpCodes.JAL) return DecodeUJType(instr);
        if (opcode == (uint)OpCodes.JALR || opcode == (uint)OpCodes.SYSTEM) return DecodeIType(instr);

        return CreateInvalid(instr);
    }

    private static RawInst32 DecodeRType(uint instr)
    {
        var rd     = (Register)((instr >> 7)  & 0x1F);
        var funct3 = (byte)((instr >> 12) & 0x7);
        var rs1    = (Register)((instr >> 15) & 0x1F);
        var rs2    = (Register)((instr >> 20) & 0x1F);
        var funct7 = (byte)((instr >> 25) & 0x7F);

        var instruction = GetRTypeInstruction(funct3, funct7);

        return new RawInst32(instr, rd, rs1, rs2, 0, funct3, funct7, InstructionType.R, instruction);
    }

    private static RawInst32 DecodeIType(uint instr)
    {
        var rd     = (Register)((instr >> 7)  & 0x1F);
        var funct3 = (byte)((instr >> 12) & 0x7);
        var rs1    = (Register)((instr >> 15) & 0x1F);
        var imm12  = (int)(instr >> 20);                    // sign-extended 12-bit immediate

        var instruction = GetITypeInstruction(funct3, rs1, imm12);

        return new RawInst32(instr, rd, rs1, Register.zero, imm12, funct3, 0, InstructionType.I, instruction);
    }

    private static RawInst32 DecodeSType(uint instr)
    {
        var funct3 = (byte)((instr >> 12) & 0x7);
        var rs1    = (Register)((instr >> 15) & 0x1F);
        var rs2    = (Register)((instr >> 20) & 0x1F);

        // Reconstruct 12-bit immediate from split fields
        int imm12 = (int)(((instr >> 25) & 0x7F) << 5) | (int)((instr >> 7) & 0x1F);

        var instruction = GetSTypeInstruction(funct3);

        return new RawInst32(instr, Register.zero, rs1, rs2, imm12, funct3, 0, InstructionType.S, instruction);
    }

    private static RawInst32 DecodeBType(uint instr)
    {
        var funct3 = (byte)((instr >> 12) & 0x7);
        var rs1    = (Register)((instr >> 15) & 0x1F);
        var rs2    = (Register)((instr >> 20) & 0x1F);

        // Reconstruct 13-bit immediate (sign-extended, 2-byte aligned)
        int imm13 = (int)(((instr >> 31) & 0x1) << 12) |
                    (int)(((instr >> 7)  & 0x1) << 11) |
                    (int)(((instr >> 25) & 0x3F) << 5) |
                    (int)(((instr >> 8)  & 0x0F) << 1);

        var instruction = GetBTypeInstruction(funct3);

        return new RawInst32(instr, Register.zero, rs1, rs2, imm13, funct3, 0, InstructionType.B, instruction);
    }

    private static RawInst32 DecodeUType(uint instr)
    {
        var rd     = (Register)((instr >> 7) & 0x1F);
        uint imm20 = (instr >> 12);                     // imm[31:12]

        var instruction = (instr & 0b1111111) == OpCodes.LUI ? Instruction.LUI : Instruction.AUIPC;

        // U-type immediate is shifted left by 12 bits
        return new RawInst32(instr, rd, Register.zero, Register.zero, (int)(imm20 << 12), 0, 0, InstructionType.U, instruction);
    }

    private static RawInst32 DecodeUJType(uint instr)
    {
        var rd = (Register)((instr >> 7) & 0x1F);

        // Reconstruct 21-bit immediate for JAL
        int imm21 = (int)(((instr >> 31) & 0x1) << 20) |
                    (int)(((instr >> 21) & 0x3FF) << 1) |
                    (int)(((instr >> 20) & 0x1) << 11) |
                    (int)(((instr >> 12) & 0xFF) << 12);

        return new RawInst32(instr, rd, Register.zero, Register.zero, imm21, 0, 0, InstructionType.UJ, Instruction.JAL);
    }

    private static RawInst32 CreateInvalid(uint raw)
    {
        return new RawInst32(raw, 0, 0, 0, 0, 0, 0, InstructionType.Unknown, Instruction.Invalid);
    }

    // ==================================================================
    // Helper methods to map raw fields to high-level Instruction enum
    // ==================================================================

    private static Instruction GetRTypeInstruction(byte funct3, byte funct7)
    {
        return (funct3, funct7) switch
        {
            (0b000, 0b0000000) => Instruction.ADD,
            (0b000, 0b0100000) => Instruction.SUB,
            (0b111, 0b0000000) => Instruction.AND,
            (0b110, 0b0000000) => Instruction.OR,
            (0b100, 0b0000000) => Instruction.XOR,
            // Add more as you implement them
            _ => Instruction.Invalid
        };
    }

    private static Instruction GetITypeInstruction(byte funct3, Register rs1, int imm)
    {
        return funct3 switch
        {
            0b000 => Instruction.ADDI,
            0b010 when rs1 == Register.zero && imm == 0 => Instruction.Invalid, // placeholder before SLTI
            0b010 => Instruction.SLTI,
            0b100 => Instruction.XORI,
            0b110 => Instruction.ORI,
            0b111 => Instruction.ANDI,
            _ => Instruction.Invalid
        };
    }

    private static Instruction GetSTypeInstruction(byte funct3)
    {
        return funct3 switch
        {
            0b000 => Instruction.SB,
            0b001 => Instruction.SH,
            0b010 => Instruction.SW,
            0b011 => Instruction.SD,
            _ => Instruction.Invalid
        };
    }

    private static Instruction GetBTypeInstruction(byte funct3)
    {
        return funct3 switch
        {
            0b000 => Instruction.BEQ,
            0b001 => Instruction.BNE,
            0b100 => Instruction.BLT,
            0b101 => Instruction.BGE,
            0b110 => Instruction.BLTU,
            0b111 => Instruction.BGEU,
            _ => Instruction.Invalid
        };
    }
}