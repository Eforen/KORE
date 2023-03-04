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

        public override void CallProcessor(ASTProcessor processor) {
            processor.ProcessASTNode(this);
        }
    }
}
