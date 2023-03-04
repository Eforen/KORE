using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kore.RiscMeta {
    [Flags]
    public enum ISAVersion {
        RV32I, // RISC-V x32 Integer Extension
        RV32M, // RISC-V x32 Multiply-Divide Extension
        RV32F, // RISC-V x32 Float Single-Point Extension
        RV32D, // RISC-V x32 Float Double-Point Extension
        RV32A, // RISC-V x32 Atomic Extension
        RV32C, // RISC-V x32 Compressed Extension
        RV32V, // RISC-V x32 Vector Extension
        RV64I, // RISC-V x64 Integer Extension
        RV64M, // RISC-V x64 Multiply-Divide Extension
        RV64A, // RISC-V x64 Atomic Extension
        RV64F, // RISC-V x64 Float Single-Point Extension
        RV64D, // RISC-V x64 Float Double-Point Extension
        RV64V, // RISC-V x64 Vector Extension
    }
}
