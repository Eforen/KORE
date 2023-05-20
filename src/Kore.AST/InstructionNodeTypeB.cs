using Kore.RiscMeta;

namespace Kore.AST {
    /// <summary>
    /// Represents a B-Type RISC-V instruction.
    /// </summary>
    public class InstructionNodeTypeBImmediate : InstructionNode<Kore.RiscMeta.Instructions.TypeB> {
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

        public InstructionNodeTypeBImmediate(Kore.RiscMeta.Instructions.TypeB op, Register rs1, Register rs2, int immediate)
            : base(op) {
            this.rs1 = rs1;
            this.rs2 = rs2;
            this.imm = immediate;
        }

        public override AstNode CallProcessor(ASTProcessor processor) {
            return processor.ProcessASTNode(this);
        }

        public override bool Equals(object obj) {
            if (obj == null || GetType() != obj.GetType())
                return false;

            InstructionNodeTypeBImmediate other = (InstructionNodeTypeBImmediate)obj;
            return base.Equals(other) && rs1 == other.rs1 && rs2 == other.rs2 && imm == other.imm;
        }

        public override int GetHashCode() {
            unchecked {
                int hash = base.GetHashCode();
                hash = (hash * 397) ^ rs1.GetHashCode();
                hash = (hash * 397) ^ rs2.GetHashCode();
                hash = (hash * 397) ^ imm.GetHashCode();
                return hash;
            }
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

        public override AstNode CallProcessor(ASTProcessor processor) {
            return processor.ProcessASTNode(this);
        }

        public override bool Equals(object obj) {
            if (obj == null || GetType() != obj.GetType())
                return false;

            InstructionNodeTypeBLabel other = (InstructionNodeTypeBLabel)obj;
            return base.Equals(other) && rs1 == other.rs1 && rs2 == other.rs2 && label == other.label;
        }

        public override int GetHashCode() {
            unchecked {
                int hash = base.GetHashCode();
                hash = (hash * 397) ^ rs1.GetHashCode();
                hash = (hash * 397) ^ rs2.GetHashCode();
                hash = (hash * 397) ^ label.GetHashCode();
                return hash;
            }
        }
    }
}
