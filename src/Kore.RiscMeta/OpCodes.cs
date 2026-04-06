// src/Kore.RiscMeta/OpCodes.cs
namespace Kore.RiscMeta;

/// <summary>
/// Contains all base opcodes and common funct3/funct7 values for RISC-V instructions.
/// These values are used by the encoding helpers to generate correct 32-bit instruction words.
/// </summary>
public static class OpCodes
{
    /// <summary>Base opcode for LUI (Load Upper Immediate) - U-type</summary>
    public static readonly Opcode LUI = 0b0110111;

    /// <summary>Base opcode for AUIPC (Add Upper Immediate to PC) - U-type</summary>
    public static readonly Opcode AUIPC = 0b0010111;

    /// <summary>Base opcode for JAL (Jump and Link) - UJ-type</summary>
    public static readonly Opcode JAL = 0b1101111;

    /// <summary>Base opcode for JALR (Jump and Link Register) - I-type</summary>
    public static readonly Opcode JALR = 0b1100111;

    /// <summary>Base opcode for Branch instructions - B-type</summary>
    public static readonly Opcode BRANCH = 0b1100011;

    /// <summary>Base opcode for Load instructions - I-type</summary>
    public static readonly Opcode LOAD = 0b0000011;

    /// <summary>Base opcode for Store instructions - S-type</summary>
    public static readonly Opcode STORE = 0b0100011;

    /// <summary>Base opcode for OP-IMM (Immediate arithmetic) - I-type</summary>
    public static readonly Opcode OP_IMM = 0b0010011;

    /// <summary>Base opcode for OP (Register arithmetic) - R-type</summary>
    public static readonly Opcode OP = 0b0110011;

    /// <summary>Base opcode for SYSTEM instructions (including CSR access)</summary>
    public static readonly Opcode SYSTEM = 0b1110011;

    /// <summary>
    /// Common funct3 values used across multiple instruction types.
    /// Named <c>Funct3Codes</c> (not <c>Funct3</c>) so this static class does not shadow the <see cref="Funct3"/> value type.
    /// </summary>
    public static class Funct3Codes
    {
        public static readonly Funct3 BEQ  = 0b000;
        public static readonly Funct3 BNE  = 0b001;
        public static readonly Funct3 BLT  = 0b100;
        public static readonly Funct3 BGE  = 0b101;
        public static readonly Funct3 BLTU = 0b110;
        public static readonly Funct3 BGEU = 0b111;

        public static readonly Funct3 LB   = 0b000;
        public static readonly Funct3 LH   = 0b001;
        public static readonly Funct3 LW   = 0b010;
        public static readonly Funct3 LD   = 0b011;
        public static readonly Funct3 LBU  = 0b100;
        public static readonly Funct3 LHU  = 0b101;

        public static readonly Funct3 SB   = 0b000;
        public static readonly Funct3 SH   = 0b001;
        public static readonly Funct3 SW   = 0b010;
        public static readonly Funct3 SD   = 0b011;

        public static readonly Funct3 ADDI = 0b000;
        public static readonly Funct3 SLTI = 0b010;
        public static readonly Funct3 XORI = 0b100;
        public static readonly Funct3 ORI  = 0b110;
        public static readonly Funct3 ANDI = 0b111;
    }

    /// <summary>
    /// Common funct7 values used in R-type instructions.
    /// Named <c>Funct7Codes</c> for the same reason as <see cref="Funct3Codes"/>.
    /// </summary>
    public static class Funct7Codes
    {
        public static readonly Funct7 ADD = 0b0000000;
        public static readonly Funct7 SUB = 0b0100000;
        public static readonly Funct7 SLL = 0b0000000;
        public static readonly Funct7 SRL = 0b0000000;
        public static readonly Funct7 SRA = 0b0100000;
    }
}