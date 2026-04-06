// src/Kore.RiscMeta/InstructionKind.cs
namespace Kore.RiscMeta;

/// <summary>High-level decoded opcode (used by <see cref="Decoding"/> / <see cref="RawInst32"/>).</summary>
public enum Instruction {
    Invalid,
    ADD,
    SUB,
    AND,
    OR,
    XOR,
    LUI,
    AUIPC,
    JAL,
    ADDI,
    SLTI,
    XORI,
    ORI,
    ANDI,
    SB,
    SH,
    SW,
    SD,
    BEQ,
    BNE,
    BLT,
    BGE,
    BLTU,
    BGEU,
}
