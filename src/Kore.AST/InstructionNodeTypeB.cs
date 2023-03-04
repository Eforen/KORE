using Kore.RiscMeta;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kore.AST {
    /// <summary>
    /// Represents a B-Type RISC-V instruction.
    /// </summary>
    public class InstructionNodeTypeB : InstructionNode<Kore.RiscMeta.Instructions.TypeB> {
        /// <summary>
        /// The first source register for the instruction.
        /// </summary>
        public Register Rs1 { get; set; }

        /// <summary>
        /// The second source register for the instruction.
        /// </summary>
        public Register Rs2 { get; set; }

        /// <summary>
        /// The immediate value for the instruction.
        /// </summary>
        public int Immediate { get; set; }

        public InstructionNodeTypeB(Kore.RiscMeta.Instructions.TypeB op, Register rs1, Register rs2, int immediate)
            : base(op) {
            Rs1 = rs1;
            Rs2 = rs2;
            Immediate = immediate;
        }

        public override void CallProcessor(ASTProcessor processor) {
            processor.ProcessASTNode(this);
        }
    }
}
