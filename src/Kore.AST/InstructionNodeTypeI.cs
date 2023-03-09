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
        public Register rd { get; set; }

        /// <summary>
        /// The source register for the instruction.
        /// </summary>
        public Register rs { get; set; }

        /// <summary>
        /// The immediate value for the instruction.
        /// </summary>
        public int immediate { get; set; }

        public InstructionNodeTypeI(Kore.RiscMeta.Instructions.TypeI op, Register rd, Register rs, int immediate)
            : base(op) {
            this.rd = rd;
            this.rs = rs;
            this.immediate = immediate;
        }

        public override void CallProcessor(ASTProcessor processor) {
            processor.ProcessASTNode(this);
        }
    }
}
