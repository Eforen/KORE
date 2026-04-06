// src/Kore.RiscMeta/RawInst32.cs
namespace Kore.RiscMeta;

/// <summary>
/// Compact struct representing a decoded 32-bit RISC-V instruction.
/// Designed to be cache-friendly and easy to work with in emulators and assemblers.
/// </summary>
public readonly struct RawInst32
{
    /// <summary>
    /// The original raw 32-bit instruction word.
    /// </summary>
    public readonly uint Raw;

    /// <summary>
    /// Destination register (rd)
    /// </summary>
    public readonly Register Rd;

    /// <summary>
    /// First source register (rs1)
    /// </summary>
    public readonly Register Rs1;

    /// <summary>
    /// Second source register (rs2) - only valid for R, S, B types
    /// </summary>
    public readonly Register Rs2;

    /// <summary>
    /// Immediate value (sign-extended where applicable)
    /// </summary>
    public readonly int Imm;

    /// <summary>
    /// funct3 field
    /// </summary>
    public readonly byte Funct3;

    /// <summary>
    /// funct7 field (only valid for R-type instructions)
    /// </summary>
    public readonly byte Funct7;

    /// <summary>
    /// The instruction format (R, I, S, B, U, UJ)
    /// </summary>
    public readonly InstructionType Format;

    /// <summary>
    /// High-level instruction (ADD, LW, JAL, etc.)
    /// </summary>
    public readonly Instruction Instruction;

    public RawInst32(
        uint raw,
        Register rd,
        Register rs1,
        Register rs2,
        int imm,
        byte funct3,
        byte funct7,
        InstructionType format,
        Instruction instruction)
    {
        Raw = raw;
        Rd = rd;
        Rs1 = rs1;
        Rs2 = rs2;
        Imm = imm;
        Funct3 = funct3;
        Funct7 = funct7;
        Format = format;
        Instruction = instruction;
    }

    /// <summary>
    /// Returns true if this is a valid decoded instruction
    /// </summary>
    public bool IsValid => Instruction != Instruction.Invalid;

    public override string ToString() => 
        $"RawInst32: {Instruction} (0x{Raw:X8}) Format={Format}";
}