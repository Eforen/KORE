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
        public override AstNode CallProcessor(ASTProcessor processor) {
            return processor.ProcessASTNode(this);
        }

        public override bool Equals(object obj) {
            if (obj == null || GetType() != obj.GetType())
                return false;

            IntDirectiveNode other = (IntDirectiveNode)obj;
            return Name == other.Name && Value == other.Value;
        }

        public override int GetHashCode() {
            unchecked {
                int hash = base.GetHashCode();
                hash = (hash * 397) ^ Name.GetHashCode();
                hash = (hash * 397) ^ Value.GetHashCode();
                return hash;
            }
        }
    }
}
