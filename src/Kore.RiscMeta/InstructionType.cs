using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kore.RiscMeta {
    /// <summary>Decoded instruction format (decoder / RawInst32).</summary>
    public enum InstructionType {
        Unknown,
        R,
        I,
        S,
        B,
        U,
        UJ,
    }
}
