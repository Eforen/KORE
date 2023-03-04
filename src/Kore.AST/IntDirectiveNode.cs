using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kore.AST {
    /// <summary>
    /// Represents an integer-valued assembler directive in a RISC-V assembly program, such as ".align" or ".word".
    /// </summary>
    ///
    /// <example>
    /// The following code creates a new `IntDirectiveNode` instance with the name ".align" and the value 4:
    /// <code>
    /// var alignDirective = new IntDirectiveNode
    /// {
    ///     Name = ".align",
    ///     Value = 4
    /// };
    /// </code>
    /// </example>
    public class IntDirectiveNode : DirectiveNode {
        /// <summary>
        /// The integer value of the directive, if any. The interpretation of the value depends on the directive.
        /// </summary>
        public int Value { get; set; }
        public override void CallProcessor(ASTProcessor processor) {
            processor.ProcessASTNode(this);
        }
    }
}
