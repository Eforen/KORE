// src/Kore.RiscMeta/RawInst64.cs
namespace Kore.RiscMeta;

/// <summary>
/// Compact struct representing a decoded 64-bit RISC-V instruction.
/// Reserved for future RV64 support.
/// </summary>
public readonly struct RawInst64
{
    public readonly ulong Raw;

    public readonly Register Rd;
    public readonly Register Rs1;
    public readonly Register Rs2;

    public readonly long Imm;

    public readonly byte Funct3;
    public readonly byte Funct7;

    public readonly InstructionType Format;
    public readonly Instruction Instruction;

    public RawInst64(
        ulong raw,
        Register rd,
        Register rs1,
        Register rs2,
        long imm,
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

    public bool IsValid => Instruction != Instruction.Invalid;
}