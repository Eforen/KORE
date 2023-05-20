using Kore.RiscMeta;

namespace Kore.AST {
    /// <summary>
    /// Represents a J-type instruction in the RISC-V assembly language.
    /// </summary>
    public class InstructionNodeTypeJImmediate : InstructionNode<Kore.RiscMeta.Instructions.TypeJ> {
        /// <summary>
        /// The destination register for the result of the instruction.
        /// </summary>
        public Register rd { get; set; }

        /// <summary>
        /// The immediate value used in the instruction.
        /// </summary>
        public int imm { get; set; }

        public InstructionNodeTypeJImmediate(Kore.RiscMeta.Instructions.TypeJ op, Register rd, int immediate) : base(op) {
            this.rd = rd;
            this.imm = immediate;
        }

        public override AstNode CallProcessor(ASTProcessor processor) {
            return processor.ProcessASTNode(this);
        }

        public override bool Equals(object obj) {
            if (obj == null || GetType() != obj.GetType())
                return false;

            InstructionNodeTypeJImmediate other = (InstructionNodeTypeJImmediate)obj;
            return base.Equals(other) && rd == other.rd && imm == other.imm;
        }

        public override int GetHashCode() {
            unchecked {
                int hash = base.GetHashCode();
                hash = (hash * 397) ^ rd.GetHashCode();
                hash = (hash * 397) ^ imm.GetHashCode();
                return hash;
            }
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

        public override AstNode CallProcessor(ASTProcessor processor) {
            return processor.ProcessASTNode(this);
        }

        public override bool Equals(object obj) {
            if (obj == null || GetType() != obj.GetType())
                return false;

            InstructionNodeTypeJLabel other = (InstructionNodeTypeJLabel)obj;
            return base.Equals(other) && rd == other.rd && label == other.label;
        }

        public override int GetHashCode() {
            unchecked {
                int hash = base.GetHashCode();
                hash = (hash * 397) ^ rd.GetHashCode();
                hash = (hash * 397) ^ label.GetHashCode();
                return hash;
            }
        }
    }
}

