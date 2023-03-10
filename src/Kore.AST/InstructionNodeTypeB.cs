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
    public class InstructionNodeTypeBImmidiate : InstructionNode<Kore.RiscMeta.Instructions.TypeB> {
        /// <summary>
        /// The first source register for the instruction.
        /// </summary>
        public Register rs1 { get; set; }

        /// <summary>
        /// The second source register for the instruction.
        /// </summary>
        public Register rs2 { get; set; }

        /// <summary>
        /// The immediate value for the instruction.
        /// </summary>
        public int imm { get; set; }

        public InstructionNodeTypeBImmidiate(Kore.RiscMeta.Instructions.TypeB op, Register rs1, Register rs2, int immediate)
            : base(op) {
            this.rs1 = rs1;
            this.rs2 = rs2;
            this.imm = immediate;
        }

        public override void CallProcessor(ASTProcessor processor) {
            processor.ProcessASTNode(this);
        }
    }
    /// <summary>
    /// Represents a B-Type RISC-V instruction.
    /// </summary>
    public class InstructionNodeTypeBLabel : InstructionNode<Kore.RiscMeta.Instructions.TypeB> {
        /// <summary>
        /// The first source register for the instruction.
        /// </summary>
        public Register rs1 { get; set; }

        /// <summary>
        /// The second source register for the instruction.
        /// </summary>
        public Register rs2 { get; set; }

        /// <summary>
        /// The label value for the instruction.
        /// </summary>
        public string label { get; set; }

        public InstructionNodeTypeBLabel(Kore.RiscMeta.Instructions.TypeB op, Register rs1, Register rs2, string label)
            : base(op) {
            this.rs1 = rs1;
            this.rs2 = rs2;
            this.label = label;
        }

        public override void CallProcessor(ASTProcessor processor) {
            processor.ProcessASTNode(this);
        }
    }
}
