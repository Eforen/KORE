using System.Text;

namespace Kore.AST {
    /// <summary>
    /// Represents a reference to a symbol by its unique ID.
    /// This is used in instructions and other contexts where a symbol needs to be referenced.
    /// </summary>
    public class SymbolReferenceNode : AstNode {
        /// <summary>
        /// The unique ID of the symbol being referenced.
        /// </summary>
        public uint SymbolId { get; set; }

        /// <summary>
        /// The name of the symbol (cached for debugging/display purposes).
        /// </summary>
        public string SymbolName { get; set; }

        /// <summary>
        /// Reference to the symbol table to resolve the symbol.
        /// </summary>
        public SymbolTable SymbolTable { get; set; }

        public SymbolReferenceNode(uint symbolId, string symbolName, SymbolTable symbolTable) {
            SymbolId = symbolId;
            SymbolName = symbolName;
            SymbolTable = symbolTable;
        }

        /// <summary>
        /// Gets the actual symbol object from the symbol table.
        /// </summary>
        public Symbol GetSymbol() {
            return SymbolTable?.GetSymbol(SymbolId);
        }

        /// <summary>
        /// Checks if the referenced symbol is defined.
        /// </summary>
        public bool IsSymbolDefined() {
            var symbol = GetSymbol();
            return symbol?.IsDefined ?? false;
        }

        /// <summary>
        /// Gets the address/value of the referenced symbol.
        /// Returns null if the symbol is not defined.
        /// </summary>
        public long? GetSymbolAddress() {
            var symbol = GetSymbol();
            return symbol?.IsDefined == true ? (long?)symbol.Address : null;
        }

        public override AstNode CallProcessor(ASTProcessor processor) {
            return processor.ProcessASTNode(this);
        }

        public override bool Equals(object obj) {
            if (obj == null || GetType() != obj.GetType())
                return false;

            SymbolReferenceNode other = (SymbolReferenceNode)obj;
            return SymbolId == other.SymbolId && base.Equals(other);
        }

        public override int GetHashCode() {
            unchecked {
                int hash = base.GetHashCode();
                hash = (hash * 397) ^ SymbolId.GetHashCode();
                return hash;
            }
        }

        public override StringBuilder getDebugText(int indentLevel, StringBuilder builder) {
            var symbol = GetSymbol();
            var status = symbol?.IsDefined == true ? "DEFINED" : "UNDEFINED";
            return addDebugTextHeader(indentLevel, builder).AppendLine($"SYMBOL_REF [{SymbolId}] {SymbolName} ({status})");
        }
    }
} 