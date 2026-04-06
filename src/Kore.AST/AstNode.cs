using System.Text;

namespace Kore.AST {
    public abstract class AstNode {
        public const int DEBUG_INDENT_COUNT = 4;
        public const int DEBUG_LINE_NUMBER_LEN = 5;
        public int lineNumber = -1;
        public abstract AstNode CallProcessor(ASTProcessor processor);

        public override bool Equals(object obj) {
            if (obj == null || GetType() != obj.GetType())
                return false;

            AstNode other = (AstNode)obj;
            return lineNumber == other.lineNumber;
        }

        public override int GetHashCode() {
            return lineNumber.GetHashCode();
        }

        public string getDebugText() {
            return getDebugText(0, new StringBuilder()).ToString();
        }
        protected StringBuilder addDebugTextHeader(bool includeLineNumber, int lineNumberOverride, int indentLevel, StringBuilder builder) {
            if(includeLineNumber) {
                return addDebugTextHeader(lineNumberOverride, indentLevel, builder);
            }
            return builder.Append(' ', indentLevel * DEBUG_INDENT_COUNT);
        }
        protected StringBuilder addDebugTextHeader(int indentLevel, StringBuilder builder) {
            return addDebugTextHeader(lineNumber, indentLevel, builder);
        }
        protected StringBuilder addDebugTextHeader(int lineNumberOverride, int indentLevel, StringBuilder builder) {
            if(lineNumberOverride < 0) return builder.Append(' ', DEBUG_LINE_NUMBER_LEN+1).Append(' ', indentLevel * DEBUG_INDENT_COUNT);
            return builder.Append(lineNumberOverride.ToString().PadLeft(DEBUG_LINE_NUMBER_LEN, '0')).Append(':').Append(' ', indentLevel * DEBUG_INDENT_COUNT);
        }

        /// <summary>
        /// Returns the byte size of the node and all its children including padding.
        /// </summary>
        /// <returns>The byte size of the node and all its children.</returns>
        public virtual int GetTotalByteSize() {
            return 0;
        }

        /// <summary>
        /// Returns the byte size of the node itself or padding etc. excluding its children.
        /// </summary>
        /// <returns>The byte size of the node.</returns>
        public virtual int GetOwnByteSize() {
            return 0;
        }

        public abstract StringBuilder getDebugText(int indentLevel, StringBuilder builder);
    }
}
