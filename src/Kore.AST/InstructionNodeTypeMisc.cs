namespace Kore.AST {
    /// <summary>
    /// Represents a miscellaneous instruction in the RISC-V assembly language.
    /// </summary>
    public class InstructionNodeTypeMisc : InstructionNode<string> {
        public InstructionNodeTypeMisc(string name) : base(name) { }

        public override AstNode CallProcessor(ASTProcessor processor) {
            return processor.ProcessASTNode(this);
        }

        public override bool Equals(object obj) {
            if (obj == null || GetType() != obj.GetType())
                return false;

            InstructionNodeTypeMisc other = (InstructionNodeTypeMisc)obj;
            return op == other.op;
        }

        public override int GetHashCode() {
            return op.GetHashCode();
        }
    }
}
