using System.Collections.Generic;

namespace Kore.AST {
    /// <summary>
    /// Represents a section of the program, such as .text or .data.
    /// </summary>
    public class SectionNode : AstNode {
        /// <summary>
        /// The name of the section.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The list of instructions or directives that make up the section.
        /// </summary>
        public List<AstNode> Contents { get; }

        public SectionNode(string name) {
            Name = name;
            Contents = new List<AstNode>();
        }
        public override void CallProcessor(ASTProcessor processor) {
            processor.ProcessASTNode(this);
        }
    }
}

