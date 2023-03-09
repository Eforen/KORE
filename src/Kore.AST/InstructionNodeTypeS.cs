using Kore.RiscMeta;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kore.AST {
    /// <summary>
    /// Represents an S-type instruction in the RISC-V assembly language.
    /// </summary>
    public class InstructionNodeTypeS : InstructionNode<Kore.RiscMeta.Instructions.TypeS> {
        /// <summary>
        /// The destination register for the result of the instruction.
        /// </summary>
        public Register rs1 { get; set; }

        /// <summary>
        /// The source register for the instruction.
        /// </summary>
        public Register rs2 { get; set; }

        /// <summary>
        /// The offset used in the instruction.
        /// </summary>
        public int imm { get; set; }

        public InstructionNodeTypeS(Kore.RiscMeta.Instructions.TypeS op, Register rs1, Register rs2, int immediate) : base(op) {
            this.rs1 = rs1;
            this.rs2 = rs2;
            this.imm = immediate;
        }

        public override void CallProcessor(ASTProcessor processor) {
            processor.ProcessASTNode(this);
        }
    }
}
