using System.Text;

namespace Kore.AST {
    /// <summary>
    /// Assembly alignment directive (e.g. GNU <c>.align n</c>); does not emit an instruction word by itself.
    /// </summary>
    public class AlignmentNode : AstNode {
        /// <summary>Alignment argument from the source (assembler-specific meaning).</summary>
        public int Bytes { get; set; }

        public override AstNode CallProcessor(ASTProcessor processor) {
            return processor.ProcessASTNode(this);
        }

        public override bool Equals(object obj) {
            if (obj == null || GetType() != obj.GetType()) {
                return false;
            }
            var other = (AlignmentNode)obj;
            return Bytes == other.Bytes && base.Equals(obj);
        }

        public override int GetHashCode() {
            unchecked {
                return (base.GetHashCode() * 397) ^ Bytes;
            }
        }

        public override int GetOwnByteSize() => 0;

        public override int GetTotalByteSize() => 0;

        public override StringBuilder getDebugText(int indentLevel, StringBuilder builder) {
            return addDebugTextHeader(false, -1, indentLevel, builder).AppendLine($"ALIGN {Bytes} BYTES");
        }
    }
}
