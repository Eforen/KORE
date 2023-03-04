using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kore.AST {
    /// <summary>
    /// Represents an assembler directive in a RISC-V assembly program, such as ".text" or ".data".
    /// </summary>
    ///
    /// <example>
    /// The following code creates a new `DirectiveNode` instance with the name ".text":
    /// <code>
    /// var textDirective = new DirectiveNode
    /// {
    ///     Name = ".text"
    /// };
    /// </code>
    /// </example>
    public class DirectiveNode : AstNode {
        /// <summary>
        /// The name of the directive, including the leading dot, such as ".text".
        /// </summary>
        public string Name { get; set; }

        public override void CallProcessor(ASTProcessor processor) {
            processor.ProcessASTNode(this);
        }
    }
}
