using Kore.RiscMeta;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kore.AST {
    /// <summary>
    /// Represents an I-Type RISC-V instruction.
    /// </summary>
    public class InstructionNodeTypeI : InstructionNode<Kore.RiscMeta.Instructions.TypeI> {
        /// <summary>
        /// The destination register for the instruction.
        /// </summary>
        public Register Rd { get; set; }

        /// <summary>
        /// The source register for the instruction.
        /// </summary>
        public Register Rs1 { get; set; }

        /// <summary>
        /// The immediate value for the instruction.
        /// </summary>
        public int Immediate { get; set; }

        public InstructionNodeTypeI(Kore.RiscMeta.Instructions.TypeI op, Register rd, Register rs1, int immediate)
            : base(op) {
            Rd = rd;
            Rs1 = rs1;
            Immediate = immediate;
        }

        public override void CallProcessor(ASTProcessor processor) {
            processor.ProcessASTNode(this);
        }
    }
}
