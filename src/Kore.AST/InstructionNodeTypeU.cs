using Kore.RiscMeta;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kore.AST {
    /// <summary>
    /// Represents a U-Type RISC-V instruction.
    /// </summary>
    public class InstructionNodeTypeU : InstructionNode<Kore.RiscMeta.Instructions.TypeU> {
        /// <summary>
        /// The destination register for the instruction.
        /// </summary>
        public Register rd { get; set; }

        /// <summary>
        /// The immediate value for the instruction.
        /// </summary>
        public int imm { get; set; }

        public InstructionNodeTypeU(Kore.RiscMeta.Instructions.TypeU op, Register rd, int immediate): base(op) {
            this.rd = rd;
            this.imm = immediate;
        }

        public override void CallProcessor(ASTProcessor processor) {
            processor.ProcessASTNode(this);
        }
    }
}
