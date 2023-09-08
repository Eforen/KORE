using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kore.AST {
    public class LabeledInlineDirectiveNode : InlineDirectiveNode {
        /// <summary>
        /// The string value of the directive, if any. The interpretation of the value depends on the directive.
        /// </summary>
        public string Label { get; set; }
        public override AstNode CallProcessor(ASTProcessor processor) {
            return processor.ProcessASTNode(this);
        }

        public override bool Equals(object obj) {
            if (obj == null || GetType() != obj.GetType())
                return false;

            LabeledInlineDirectiveNode other = (LabeledInlineDirectiveNode)obj;
            return Label == other.Label;
        }

        public override int GetHashCode() {
            unchecked {
                int hash = base.GetHashCode();
                hash = (hash * 397) ^ Label.GetHashCode();
                return hash;
            }
        }

        public override StringBuilder getDebugText(int indentLevel, StringBuilder builder) {
            return addDebugTextHeader(indentLevel, builder).Append($"INLINE DIRECTIVE {Name} LABEL:{Label}");
        }
    }
    public class LabeledInlineDirectiveNode<T> : LabeledInlineDirectiveNode where T : InstructionNode {
        /// <summary>
        /// The instruction that should replace this directive node when positions have been calculated.
        /// </summary>
        public InstructionNode WrappedInstruction { get; set; }
        public override AstNode CallProcessor(ASTProcessor processor) {
            return processor.ProcessASTNode(this);
        }

        public override bool Equals(object obj) {
            if (obj == null || GetType() != obj.GetType())
                return false;

            LabeledInlineDirectiveNode<T> other = (LabeledInlineDirectiveNode<T>)obj;
            return Label == other.Label && WrappedInstruction == other.WrappedInstruction;
        }

        public override int GetHashCode() {
            unchecked {
                int hash = base.GetHashCode();
                hash = (hash * 397) ^ WrappedInstruction.GetHashCode();
                return hash;
            }
        }

        public override StringBuilder getDebugText(int indentLevel, StringBuilder builder) {
            addDebugTextHeader(indentLevel, builder).AppendLine($"INLINE DIRECTIVE {Name} LABEL:{Label} {{");
            WrappedInstruction.getDebugText(indentLevel + 1, builder);
            return addDebugTextHeader(indentLevel, builder).AppendLine("}");
        }
    }
}
