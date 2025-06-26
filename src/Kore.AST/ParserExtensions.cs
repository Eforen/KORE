using System;
using System.Collections.Generic;
using System.Linq;

namespace Kore.AST {
    /// <summary>
    /// Extension methods and helper functions for integrating the symbol table system with the parser.
    /// These methods show how the existing parser would be modified to use the new symbol table.
    /// </summary>
    public static class ParserExtensions {
        /// <summary>
        /// Helper method to create a symbol reference from a label name during parsing.
        /// This would be used in place of directly storing string labels in instruction nodes.
        /// </summary>
        public static SymbolReferenceNode CreateSymbolReference(this ProgramNode program, string labelName, AstNode referencingNode) {
            // Get or create the symbol in the symbol table
            var symbol = program.SymbolTable.ReferenceSymbol(labelName, referencingNode, SymbolType.Label);
            
            // Create and return the symbol reference node
            return new SymbolReferenceNode(symbol.Id, symbol.Name, program.SymbolTable);
        }

        /// <summary>
        /// Helper method to define a label symbol during parsing.
        /// This would be called when a LabelNode is encountered.
        /// </summary>
        public static Symbol DefineLabel(this ProgramNode program, string labelName, int lineNumber, string currentSection) {
            return program.SymbolTable.DefineSymbol(labelName, lineNumber, currentSection, SymbolScope.Local, SymbolType.Label);
        }

        /// <summary>
        /// Helper method to process .global directive during parsing.
        /// </summary>
        public static SymbolDirectiveNode ProcessGlobalDirective(this ProgramNode program, string symbolName) {
            // Get or create the symbol and set it to global scope
            var symbol = program.SymbolTable.GetOrCreateSymbol(symbolName, SymbolScope.Global, SymbolType.Label);
            
            // Create the directive node
            var directive = new SymbolDirectiveNode(SymbolDirectiveNode.DirectiveType.Global, symbolName) {
                Symbol = symbol
            };
            
            return directive;
        }

        /// <summary>
        /// Helper method to process .local directive during parsing.
        /// </summary>
        public static SymbolDirectiveNode ProcessLocalDirective(this ProgramNode program, string symbolName) {
            // Get or create the symbol and set it to local scope
            var symbol = program.SymbolTable.GetOrCreateSymbol(symbolName, SymbolScope.Local, SymbolType.Label);
            
            // Create the directive node
            var directive = new SymbolDirectiveNode(SymbolDirectiveNode.DirectiveType.Local, symbolName) {
                Symbol = symbol
            };
            
            return directive;
        }

        /// <summary>
        /// Validates that all referenced symbols have been defined.
        /// This would be called after parsing is complete.
        /// </summary>
        public static List<string> ValidateSymbols(this ProgramNode program) {
            var errors = new List<string>();
            
            var undefinedSymbols = program.SymbolTable.GetUnresolvedReferences();
            foreach (var symbol in undefinedSymbols) {
                errors.Add($"Undefined symbol '{symbol.Name}' referenced at {symbol.References.Count} location(s)");
            }
            
            return errors;
        }

        /// <summary>
        /// Gets symbol usage statistics for debugging/analysis.
        /// </summary>
        public static Dictionary<string, object> GetSymbolStatistics(this ProgramNode program) {
            var stats = new Dictionary<string, object>();
            var symbolTable = program.SymbolTable;
            
            stats["TotalSymbols"] = symbolTable.Count;
            stats["GlobalSymbols"] = symbolTable.GetSymbolsByScope(SymbolScope.Global).Count;
            stats["LocalSymbols"] = symbolTable.GetSymbolsByScope(SymbolScope.Local).Count;
            stats["UnknownSymbols"] = symbolTable.GetSymbolsByScope(SymbolScope.Unknown).Count;
            stats["UndefinedSymbols"] = symbolTable.GetUndefinedSymbols().Count();
            stats["UnresolvedReferences"] = symbolTable.GetUnresolvedReferences().Count();
            
            // Group by symbol type
            var byType = symbolTable.GetAllSymbols().GroupBy(s => s.Type).ToDictionary(g => g.Key.ToString(), g => g.Count());
            stats["SymbolsByType"] = byType;
            
            return stats;
        }
    }

    /// <summary>
    /// Example of how the existing parser methods would be modified to use the symbol table.
    /// This shows the pattern for updating instruction parsing methods.
    /// </summary>
    public static class ParserModificationExamples {
        /// <summary>
        /// Example of how ParseBInstruction would be modified to use symbols instead of strings.
        /// This replaces the creation of InstructionNodeTypeBLabel with InstructionNodeTypeBSymbol.
        /// </summary>
        public static AstNode[] ParseBInstructionWithSymbols(ProgramNode program, string currentSection) {
            // ... existing parsing logic for opcode and registers ...
            
            // When we encounter an identifier (label), instead of creating InstructionNodeTypeBLabel:
            string labelName = "example_label"; // This would come from the lexer
            
            // Create a symbol reference
            var symbolRef = program.CreateSymbolReference(labelName, null); // null will be replaced with the actual instruction node
            
            // Create the new symbol-based instruction
            var instruction = new InstructionNodeTypeBSymbol(
                Kore.RiscMeta.Instructions.TypeB.beq, // example
                Kore.RiscMeta.Register.x1, // example
                Kore.RiscMeta.Register.x2, // example
                symbolRef
            );
            
            // Update the symbol reference to point to this instruction
            symbolRef.SymbolTable.ReferenceSymbol(labelName, instruction);
            
            return new AstNode[] { instruction };
        }

        /// <summary>
        /// Example of how label definition would be handled in the new system.
        /// </summary>
        public static AstNode[] ParseLabelWithSymbols(ProgramNode program, string labelName, int lineNumber, string currentSection) {
            // Define the symbol in the symbol table
            var symbol = program.DefineLabel(labelName, lineNumber, currentSection);
            
            // Create the label node (this could be enhanced to reference the symbol)
            var labelNode = new LabelNode(labelName) {
                lineNumber = lineNumber
            };
            
            return new AstNode[] { labelNode };
        }

        /// <summary>
        /// Example of how directive parsing would be enhanced for .global/.local.
        /// </summary>
        public static AstNode[] ParseSymbolDirective(ProgramNode program, string directiveName, string symbolName) {
            switch (directiveName.ToLower()) {
                case ".global":
                case ".globl":
                    return new AstNode[] { program.ProcessGlobalDirective(symbolName) };
                    
                case ".local":
                    return new AstNode[] { program.ProcessLocalDirective(symbolName) };
                    
                default:
                    throw new ArgumentException($"Unknown symbol directive: {directiveName}");
            }
        }
    }
} 