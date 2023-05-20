namespace Kore.AST {
    public abstract class AstNode {
        public int lineNumber = -1;
        public abstract AstNode CallProcessor(ASTProcessor processor);

        public override bool Equals(object obj) {
            if (obj == null || GetType() != obj.GetType())
                return false;

            AstNode other = (AstNode)obj;
            return lineNumber == other.lineNumber;
        }

        public override int GetHashCode() {
            return lineNumber.GetHashCode();
        }
    }
}
