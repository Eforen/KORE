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
    public class InstructionNodeTypeJImmidiate : InstructionNode<Kore.RiscMeta.Instructions.TypeJ> {
        /// <summary>
        /// The destination register for the result of the instruction.
        /// </summary>
        public Register rd { get; set; }

        /// <summary>
        /// The immediate value used in the instruction.
        /// </summary>
        public int imm { get; set; }

        public InstructionNodeTypeJImmidiate(Kore.RiscMeta.Instructions.TypeJ op, Register rd, int immediate) : base(op) {
            this.rd = rd;
            this.imm = immediate;
        }

        public override void CallProcessor(ASTProcessor processor) {
            processor.ProcessASTNode(this);
        }
    }
    /// <summary>
    /// Represents a J-type instruction in the RISC-V assembly language.
    /// </summary>
    public class InstructionNodeTypeJLabel : InstructionNode<Kore.RiscMeta.Instructions.TypeJ> {
        /// <summary>
        /// The destination register for the result of the instruction.
        /// </summary>
        public Register rd { get; set; }

        /// <summary>
        /// The label used in the instruction.
        /// </summary>
        public string label { get; set; }

        public InstructionNodeTypeJLabel(Kore.RiscMeta.Instructions.TypeJ op, Register rd, string label) : base(op) {
            this.rd = rd;
            this.label = label;
        }

        public override void CallProcessor(ASTProcessor processor) {
            processor.ProcessASTNode(this);
        }
    }
}

