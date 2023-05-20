namespace Kore.AST {

    /// <summary>
    /// Node representing a comment in the assembly code.
    /// </summary>
    public class CommentNode : AstNode {

        /// <summary>
        /// The comment text.
        /// </summary>
        public string Text { get; set; }

        public CommentNode(string text) {
            Text = text;
        }

        public override AstNode CallProcessor(ASTProcessor processor) {
            return processor.ProcessASTNode(this);
        }

        public override bool Equals(object obj) {
            if (obj == null || GetType() != obj.GetType())
                return false;

            CommentNode other = (CommentNode)obj;
            return Text == other.Text;
        }

        public override int GetHashCode() {
            return Text.GetHashCode();
        }
    }
}
