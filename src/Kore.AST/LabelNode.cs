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

        public override void CallProcessor(ASTProcessor processor) {
            processor.ProcessASTNode(this);
        }
    }
}
