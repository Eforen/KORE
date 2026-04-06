// src/Kore.RiscMeta/Encoding.cs
namespace Kore.RiscMeta;

/// <summary>
/// Provides type-safe encoding methods for all RISC-V base instruction formats.
/// These methods construct 32-bit instruction words according to the official RISC-V specification.
/// </summary>
public static class Encoding
{
    /// <summary>
    /// Encodes an R-type instruction (register-register operations).
    /// Format: [funct7][rs2][rs1][funct3][rd][opcode]
    /// Used for: ADD, SUB, AND, OR, XOR, SLL, SRL, SRA, etc.
    /// </summary>
    public static uint EncodeRType(Register rd, Register rs1, Register rs2, Funct3 f3, Funct7 f7, Opcode opc)
    {
        return (f7 << 25) | (rs2.AsUint() << 20) | (rs1.AsUint() << 15)
             | (f3 << 12) | (rd.AsUint() << 7) | opc;
    }

    /// <summary>
    /// Encodes an I-type instruction (immediate arithmetic and loads).
    /// Format: [imm[11:0]][rs1][funct3][rd][opcode]
    /// Used for: ADDI, SLTI, XORI, ORI, ANDI, LB, LH, LW, LD, JALR, etc.
    /// </summary>
    public static uint EncodeIType(Register rd, Register rs1, uint imm12, Funct3 f3, Opcode opc)
    {
        return ((imm12 & 0xFFF) << 20) | (rs1.AsUint() << 15)
             | (f3 << 12) | (rd.AsUint() << 7) | opc;
    }

    /// <summary>
    /// Encodes an S-type instruction (stores).
    /// Format: [imm[11:5]][rs2][rs1][funct3][imm[4:0]][opcode]
    /// Used for: SB, SH, SW, SD.
    /// </summary>
    public static uint EncodeSType(Register rs1, Register rs2, uint imm12, Funct3 f3, Opcode opc)
    {
        uint imm11_5 = (imm12 >> 5) & 0x7F;
        uint imm4_0  = imm12 & 0x1F;

        return (imm11_5 << 25) | (rs2.AsUint() << 20) | (rs1.AsUint() << 15)
             | (f3 << 12) | (imm4_0 << 7) | opc;
    }

    /// <summary>
    /// Encodes a B-type instruction (conditional branches).
    /// Format: [imm[12|10:5]][rs2][rs1][funct3][imm[4:1|11]][opcode]
    /// Used for: BEQ, BNE, BLT, BGE, BLTU, BGEU.
    /// Note: imm13 must be sign-extended and 2-byte aligned (lowest bit is implicit).
    /// </summary>
    public static uint EncodeBType(Register rs1, Register rs2, uint imm13, Funct3 f3, Opcode opc)
    {
        uint imm12   = (imm13 >> 12) & 0x01;
        uint imm10_5 = (imm13 >> 5)  & 0x3F;
        uint imm4_1  = (imm13 >> 1)  & 0x0F;
        uint imm11   = (imm13 >> 11) & 0x01;

        uint immPart = (imm12 << 31) | (imm10_5 << 25) | (imm4_1 << 8) | (imm11 << 7);
        return immPart | (rs2.AsUint() << 20) | (rs1.AsUint() << 15) | (f3 << 12) | opc;
    }

    /// <summary>
    /// Encodes a U-type instruction (upper immediate).
    /// Format: [imm[31:12]][rd][opcode]
    /// Used for: LUI, AUIPC.
    /// </summary>
    public static uint EncodeUType(Register rd, uint imm20, Opcode opc)
    {
        return (imm20 << 12) | (rd.AsUint() << 7) | opc;
    }

    /// <summary>
    /// Encodes a UJ-type instruction (unconditional jump - JAL).
    /// Format: [imm[20|10:1|11|19:12]][rd][opcode]
    /// Used for: JAL.
    /// Note: imm21 must be sign-extended and 2-byte aligned.
    /// </summary>
    public static uint EncodeUJType(Register rd, uint imm21, Opcode opc)
    {
        uint imm20    = (imm21 >> 20) & 0x01;
        uint imm10_1  = (imm21 >> 1)  & 0x3FF;
        uint imm11    = (imm21 >> 11) & 0x01;
        uint imm19_12 = (imm21 >> 12) & 0xFF;

        uint immPart = (imm20 << 31) | (imm10_1 << 21) | (imm11 << 20) | (imm19_12 << 12);
        return immPart | (rd.AsUint() << 7) | opc;
    }
}
