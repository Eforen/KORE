using Kore.RiscMeta;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kore.AST {
    /// <summary>
    /// Represents a J-type instruction in the RISC-V assembly language.
    /// </summary>
    public class InstructionNodeTypeJ : InstructionNode<Kore.RiscMeta.Instructions.TypeJ> {
        /// <summary>
        /// The destination register for the result of the instruction.
        /// </summary>
        public Register Rd { get; set; }

        /// <summary>
        /// The immediate value used in the instruction.
        /// </summary>
        public int Immediate { get; set; }

        public InstructionNodeTypeJ(Kore.RiscMeta.Instructions.TypeJ op, Register rd, int immediate) : base(op) {
            Rd = rd;
            Immediate = immediate;
        }

        public override void CallProcessor(ASTProcessor processor) {
            processor.ProcessASTNode(this);
        }
    }
}

