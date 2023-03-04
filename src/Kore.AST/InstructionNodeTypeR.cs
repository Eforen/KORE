using Kore.RiscMeta;
using Kore.RiscMeta.Instructions;

namespace Kore.AST {
    /// <summary>
    /// Represents an R-Type RISC-V instruction.
    /// </summary>
    public class InstructionNodeTypeR : InstructionNode<Kore.RiscMeta.Instructions.TypeR> {
        /// <summary>
        /// The destination register for the instruction.
        /// </summary>
        public Register rd { get; set; }

        /// <summary>
        /// The first source register for the instruction.
        /// </summary>
        public Register rs1 { get; set; }

        /// <summary>
        /// The second source register for the instruction.
        /// </summary>
        public Register rs2 { get; set; }

        public InstructionNodeTypeR(Kore.RiscMeta.Instructions.TypeR op, Register rd, Register rs1, Register rs2) : base(op) {
            this.rd = rd;
            this.rs1 = rs1;
            this.rs2 = rs2;
        }

        public override void CallProcessor(ASTProcessor processor) {
            processor.ProcessASTNode(this);
        }
    }
}