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

        /// <summary>
        /// The symbol table that maintains all symbols (labels, globals, locals) with their scopes.
        /// </summary>
        public SymbolTable SymbolTable { get; }

        public ProgramNode() {
            Sections = new List<SectionNode>();
            SymbolTable = new SymbolTable();
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
            addDebugTextHeader(indentLevel, builder).AppendLine($"PROGRAM [{Sections.Count}] Symbols:[{SymbolTable.Count}]{{");
            
            // Add symbol table debug info
            if (SymbolTable.Count > 0) {
                addDebugTextHeader(indentLevel + 1, builder).AppendLine("SYMBOL TABLE {");
                foreach (var symbol in SymbolTable.GetAllSymbols()) {
                    addDebugTextHeader(indentLevel + 2, builder).AppendLine(symbol.ToString());
                }
                addDebugTextHeader(indentLevel + 1, builder).AppendLine("}");
            }
            
            foreach(var section in Sections) {
                section.getDebugText(indentLevel+1, builder);
            }
            return addDebugTextHeader(indentLevel, builder).AppendLine("}");
        }

        /// <summary>
        /// Helper method to process a .local directive and create the corresponding AST node.
        /// </summary>
        /// <param name="symbolName">The name of the symbol to mark as local</param>
        /// <returns>The created SymbolDirectiveNode</returns>
        public SymbolDirectiveNode ProcessLocalDirective(string symbolName) {
            var directive = new SymbolDirectiveNode(SymbolDirectiveNode.DirectiveType.Local, symbolName);
            
            // Add the symbol to the symbol table with local scope
            var symbol = SymbolTable.GetOrCreateSymbol(symbolName, SymbolScope.Local);
            directive.Symbol = symbol; // Link the symbol to the directive
            
            return directive;
        }

        /// <summary>
        /// Helper method to process a .global directive and create the corresponding AST node.
        /// </summary>
        /// <param name="symbolName">The name of the symbol to mark as global</param>
        /// <returns>The created SymbolDirectiveNode</returns>
        public SymbolDirectiveNode ProcessGlobalDirective(string symbolName) {
            var directive = new SymbolDirectiveNode(SymbolDirectiveNode.DirectiveType.Global, symbolName);
            
            // Add the symbol to the symbol table with global scope
            var symbol = SymbolTable.GetOrCreateSymbol(symbolName, SymbolScope.Global);
            directive.Symbol = symbol; // Link the symbol to the directive
            
            return directive;
        }

        /// <summary>
        /// Helper method to define a label in the symbol table.
        /// </summary>
        /// <param name="labelName">The name of the label</param>
        /// <param name="lineNumber">The line number where the label is defined</param>
        /// <param name="section">The section where the label is defined</param>
        /// <returns>The defined symbol</returns>
        public Symbol DefineLabel(string labelName, int lineNumber, string section) {
            return SymbolTable.DefineSymbol(labelName, lineNumber, section);
        }
    }
}

