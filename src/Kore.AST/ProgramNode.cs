using System.Collections.Generic;
using System.Text;

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
        public override AstNode CallProcessor(ASTProcessor processor) {
            return processor.ProcessASTNode(this);
        }

        // override object.Equals
        public override bool Equals(object obj) {
            if(obj == null || GetType() != obj.GetType()) {
                return false;
            }
            ProgramNode node = obj as ProgramNode;

            if(Sections.Count != node.Sections.Count) return false;

            foreach(SectionNode section in Sections) {
                string name = section.Name;
                SectionNode otherSection = node.Sections.Find(s => s.Name == name);
                if(otherSection == null) return false;
                if(!section.Equals(otherSection)) return false;
            }

            return base.Equals(obj);
        }

        // override object.GetHashCode
        public override int GetHashCode() {
            Sections.GetHashCode();
            return base.GetHashCode();
        }

        public override StringBuilder getDebugText(int indentLevel, StringBuilder builder) {
            addDebugTextHeader(indentLevel, builder).AppendLine($"PROGRAM [{Sections.Count}]{{");
            foreach(var section in Sections) {
                section.getDebugText(indentLevel+1, builder);
            }
            return addDebugTextHeader(indentLevel, builder).AppendLine("}");
        }
    }
}

