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
        protected StringBuilder addDebugTextHeader(int indentLevel, StringBuilder builder) {
            return addDebugTextHeader(lineNumber, indentLevel, builder);
        }
        protected StringBuilder addDebugTextHeader(int lineNumberOverride, int indentLevel, StringBuilder builder) {
            if(lineNumberOverride < 0) return builder.Append(' ', DEBUG_LINE_NUMBER_LEN+1).Append(' ', indentLevel * DEBUG_INDENT_COUNT);
            return builder.Append(lineNumberOverride.ToString().PadLeft(DEBUG_LINE_NUMBER_LEN, '0')).Append(':').Append(' ', indentLevel * DEBUG_INDENT_COUNT);
        }

        public abstract StringBuilder getDebugText(int indentLevel, StringBuilder builder);
    }
}
