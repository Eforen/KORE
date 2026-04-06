// src/Kore.RiscMeta/InstructionParts.cs

namespace Kore.RiscMeta;

// These are zero-cost at runtime — purely for compile-time type safety
// Implicit conversions from int/uint replace the old `using funct3 = Funct3`-style aliases at call sites.
public readonly struct Funct3  {
    public readonly uint Value;
    public Funct3(uint v) => Value = v;
    public static implicit operator uint(Funct3 f) => f.Value;
    public static implicit operator Funct3(uint v) => new Funct3(v);
    public static implicit operator Funct3(int v) => new Funct3((uint)v);
}
public readonly struct Funct7  {
    public readonly uint Value;
    public Funct7(uint v) => Value = v;
    public static implicit operator uint(Funct7 f) => f.Value;
    public static implicit operator Funct7(uint v) => new Funct7(v);
    public static implicit operator Funct7(int v) => new Funct7((uint)v);
}
public readonly struct Opcode  {
    public readonly uint Value;
    public Opcode(uint v) => Value = v;
    public static implicit operator uint(Opcode o) => o.Value;
    public static implicit operator Opcode(uint v) => new Opcode(v);
    public static implicit operator Opcode(int v) => new Opcode((uint)v);
}