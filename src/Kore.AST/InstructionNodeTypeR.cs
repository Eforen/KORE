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

        public override AstNode CallProcessor(ASTProcessor processor) {
            return processor.ProcessASTNode(this);
        }

        public override bool Equals(object obj) {
            if (obj == null || GetType() != obj.GetType())
                return false;

            InstructionNodeTypeR other = (InstructionNodeTypeR)obj;
            return base.Equals(other) && rd == other.rd && rs1 == other.rs1 && rs2 == other.rs2;
        }

        public override int GetHashCode() {
            unchecked {
                int hash = base.GetHashCode();
                hash = (hash * 397) ^ rd.GetHashCode();
                hash = (hash * 397) ^ rs1.GetHashCode();
                hash = (hash * 397) ^ rs2.GetHashCode();
                return hash;
            }
        }
    }
}