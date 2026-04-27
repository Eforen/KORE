using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

        /// <summary>
        /// Index of this section in <see cref="ProgramNode.Sections"/> (set by the parser). Used for <see cref="getDebugText"/> alignment with symbol table lines.
        /// </summary>
        public int SectionIndex { get; set; } = -1;

        public SectionNode(string name) {
            Name = name;
            Contents = new List<AstNode>();
        }
        public override AstNode CallProcessor(ASTProcessor processor) {
            return processor.ProcessASTNode(this);
        }

        public override bool Equals(object obj) {
            if (obj == null || GetType() != obj.GetType())
                return false;

            SectionNode other = (SectionNode)obj;

            if (Name != other.Name)
                return false;

            if (Contents.Count != other.Contents.Count)
                return false;

            for (int i = 0; i < Contents.Count; i++) {
                if (!Contents[i].Equals(other.Contents[i]))
                    return false;
            }

            return true;
        }

        public override int GetHashCode() {
            int hash = Name.GetHashCode();

            foreach (AstNode content in Contents) {
                hash ^= content.GetHashCode();
            }

            return hash;
        }

        public override StringBuilder getDebugText(int indentLevel, StringBuilder builder) {
            string indexDisplay = SectionIndex >= 0 ? SectionIndex.ToString() : "UNKNOWN";
            addDebugTextHeader(false, -1, indentLevel, builder).AppendLine($"SECTION[{indexDisplay}] {Name} [{Contents.Count}]{{");
            foreach(var node in Contents) {
                node.getDebugText(indentLevel + 1, builder);
            }
            return addDebugTextHeader(false, -1, indentLevel, builder).AppendLine("}");
        }
    }
}

