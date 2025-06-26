using System.Text;

namespace Kore.AST {
    /// <summary>
    /// Represents a symbol directive such as .global or .local that affects symbol visibility.
    /// </summary>
    public class SymbolDirectiveNode : DirectiveNode {
        /// <summary>
        /// The type of symbol directive.
        /// </summary>
        public enum DirectiveType {
            Global,
            Local
        }

        /// <summary>
        /// The type of this directive.
        /// </summary>
        public DirectiveType Type { get; set; }

        /// <summary>
        /// The name of the symbol being affected by this directive.
        /// </summary>
        public string SymbolName { get; set; }

        /// <summary>
        /// Reference to the symbol in the symbol table (set during processing).
        /// </summary>
        public Symbol Symbol { get; set; }

        public SymbolDirectiveNode(DirectiveType type, string symbolName) {
            Type = type;
            SymbolName = symbolName;
            Name = type == DirectiveType.Global ? ".global" : ".local";
        }

        public override AstNode CallProcessor(ASTProcessor processor) {
            return processor.ProcessASTNode(this);
        }

        public override bool Equals(object obj) {
            if (obj == null || GetType() != obj.GetType())
                return false;

            SymbolDirectiveNode other = (SymbolDirectiveNode)obj;
            return Type == other.Type && SymbolName == other.SymbolName && base.Equals(other);
        }

        public override int GetHashCode() {
            unchecked {
                int hash = base.GetHashCode();
                hash = (hash * 397) ^ Type.GetHashCode();
                hash = (hash * 397) ^ (SymbolName?.GetHashCode() ?? 0);
                return hash;
            }
        }

        public override StringBuilder getDebugText(int indentLevel, StringBuilder builder) {
            var symbolInfo = Symbol != null ? $" -> Symbol[{Symbol.Id}]" : "";
            return addDebugTextHeader(indentLevel, builder).AppendLine($"SYMBOL_DIRECTIVE {Name} {SymbolName}{symbolInfo}");
        }
    }
} 