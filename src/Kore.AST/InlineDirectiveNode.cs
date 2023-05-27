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
    public class InlineDirectiveNode : AstNode {
        public enum InlineDirectiveType {
            /// <summary>The inline directive that represents the address of a symbol.</summary>
            ADDR,
            /// <summary>The inline directive that represents upper 20 bits of a PC-relative address.</summary>
            PCREL_HI,
            /// <summary>The inline directive that represents the lower 12 bits of a PC-relative address.</summary>
            PCREL_LO,
            /// <summary>The inline directive that represents the upper 20 bits of an absolute address.</summary>
            HI,
            /// <summary>The inline directive that represents the lower 12 bits of an absolute address.</summary>
            LO,
            // The stuff after this is not relevant to the position dependent assembler we are making at the moment.
            // /// <summary>The inline directive that represents the address of a symbol in the Global Offset Table (GOT).</summary>
            // GOT,
            // /// <summary>The inline directive that represents the PC-relative address of a symbol in the Global Offset Table (GOT).</summary>
            // GOT_PCREL,
            // /// <summary>The inline directive that represents the upper 20 bits of a PC-relative address of a symbol in the Global Offset Table (GOT).</summary>
            // GOT_PCREL_HI,
            // /// <summary>The inline directive that represents the lower 12 bits of a PC-relative address of a symbol in the Global Offset Table (GOT).</summary>
            // GOT_PCREL_LO,
            // /// <summary>The inline directive that represents the address of a symbol in the Procedure Linkage Table (PLT).</summary>
            // PLT,
            // /// <summary>The inline directive that represents the PC-relative address of a symbol in the Procedure Linkage Table (PLT).</summary>
            // PLT_PCREL,
            // /// <summary>The inline directive that represents the upper 20 bits of a PC-relative address of a symbol in the Procedure Linkage Table (PLT).</summary>
            // PLT_PCREL_HI,
            // /// <summary>The inline directive that represents the lower 12 bits of a PC-relative address of a symbol in the Procedure Linkage Table (PLT).</summary>
            // PLT_PCREL_LO,
            // /// <summary>The inline directive that represents the address of a symbol in the Small Data Area (SDA).</summary>
            // SDA,
            // /// <summary>The inline directive that represents the PC-relative address of a symbol in the Small Data Area (SDA).</summary>
            // SDA_PCREL,
            // /// <summary>The inline directive that represents the upper 20 bits of a PC-relative address of a symbol in the Small Data Area (SDA).</summary>
            // SDA_PCREL_HI,
            // /// <summary>The inline directive that represents the lower 12 bits of a PC-relative address of a symbol in the Small Data Area (SDA).</summary>
            // SDA_PCREL_LO,
            // /// <summary>The inline directive that represents the address of a symbol in the Small Data Area (SDA2).</summary>
            // SDA2,
            // /// <summary>The inline directive that represents the PC-relative address of a symbol in the Small Data Area (SDA2).</summary>
            // SDA2_PCREL,
            // /// <summary>The inline directive that represents the upper 20 bits of a PC-relative address of a symbol in the Small Data Area (SDA2).</summary>
            // SDA2_PCREL_HI,
            // /// <summary>The inline directive that represents the lower 12 bits of a PC-relative address of a symbol in the Small Data Area (SDA2).</summary>
            // SDA2_PCREL_LO,
        }

        /// <summary>
        /// The name of the directive, including the leading dot, such as ".text".
        /// </summary>
        public InlineDirectiveType Name { get; set; }

        public override AstNode CallProcessor(ASTProcessor processor) {
            return processor.ProcessASTNode(this);
        }

        public override bool Equals(object obj) {
            if (obj == null || GetType() != obj.GetType())
                return false;

            InlineDirectiveNode other = (InlineDirectiveNode)obj;
            return Name == other.Name;
        }

        public override int GetHashCode() {
            return Name.GetHashCode();
        }
    }
}
