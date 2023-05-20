namespace Kore.AST {

    /// <summary>
    /// Node representing a label in the assembly code.
    /// </summary>
    public class LabelNode : AstNode {

        /// <summary>
        /// The name of the label.
        /// </summary>
        public string Name { get; set; }

        public LabelNode(string name) {
            Name = name;
        }

        public override AstNode CallProcessor(ASTProcessor processor) {
            return processor.ProcessASTNode(this);
        }

        public override bool Equals(object obj) {
            if (obj == null || GetType() != obj.GetType())
                return false;

            LabelNode other = (LabelNode)obj;
            return Name == other.Name;
        }

        public override int GetHashCode() {
            return Name.GetHashCode();
        }
    }
}
