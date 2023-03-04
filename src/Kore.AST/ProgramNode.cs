using System.Collections.Generic;
namespace Kore.AST {
    /// <summary>
    /// The root node of the AST, representing a RISC-V program.
    /// </summary>
    public class ProgramNode : AstNode {
        /// <summary>
        /// A list of sections that make up the program.
        /// </summary>
        public List<SectionNode> Sections { get; }

        public ProgramNode() {
            Sections = new List<SectionNode>();
        }
        public override void CallProcessor(ASTProcessor processor) {
            processor.ProcessASTNode(this);
        }
    }
}

