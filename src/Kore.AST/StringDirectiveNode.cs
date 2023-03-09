﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kore.AST {
    /// <summary>
    /// Represents a string-valued assembler directive in a RISC-V assembly program, such as ".asciz" or ".string".
    /// </summary>
    ///
    /// <example>
    /// The following code creates a new `StringDirectiveNode` instance with the name ".string" and the value "Hello, world!":
    /// <code>
    /// var helloString = new StringDirectiveNode
    /// {
    ///     Name = ".string",
    ///     Value = "Hello, world!"
    /// };
    /// </code>
    /// </example>
    public class StringDirectiveNode : DirectiveNode {
        /// <summary>
        /// The string value of the directive, if any. The interpretation of the value depends on the directive.
        /// </summary>
        public string Value { get; set; }
        public override void CallProcessor(ASTProcessor processor) {
            processor.ProcessASTNode(this);
        }
    }
}