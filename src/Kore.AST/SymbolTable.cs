using System;
using System.Collections.Generic;
using System.Linq;

namespace Kore.AST {
    /// <summary>
    /// Represents the scope of a symbol in the assembly program.
    /// </summary>
    public enum SymbolScope {
        /// <summary>
        /// Symbol is referenced but not yet defined (forward reference).
        /// </summary>
        Unknown,
        /// <summary>
        /// Symbol is local to the current file/section.
        /// </summary>
        Local,
        /// <summary>
        /// Symbol is globally visible across files.
        /// </summary>
        Global
    }

    /// <summary>
    /// Represents the type of symbol.
    /// </summary>
    public enum SymbolType {
        /// <summary>
        /// A jump label (e.g., "loop:", "end:")
        /// </summary>
        Label,
        /// <summary>
        /// A data symbol (e.g., variables, constants)
        /// </summary>
        Data,
        /// <summary>
        /// A function symbol
        /// </summary>
        Function,
        /// <summary>
        /// A section symbol
        /// </summary>
        Section
    }

    /// <summary>
    /// Represents a symbol in the assembly program.
    /// </summary>
    public class Symbol {
        /// <summary>
        /// Unique identifier for this symbol, auto-incremented.
        /// </summary>
        public uint Id { get; }

        /// <summary>
        /// The name of the symbol.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The scope of the symbol (Local, Global, Unknown).
        /// </summary>
        public SymbolScope Scope { get; set; }

        /// <summary>
        /// The type of symbol (Label, Data, Function, Section).
        /// </summary>
        public SymbolType Type { get; set; }

        /// <summary>
        /// The section where this symbol is defined (if applicable).
        /// </summary>
        public string Section { get; set; }

        /// <summary>
        /// The line number where this symbol is defined (-1 if not yet defined).
        /// </summary>
        public int LineNumber { get; set; }

        /// <summary>
        /// The address/offset of this symbol (calculated during assembly).
        /// </summary>
        public long Address { get; set; }

        /// <summary>
        /// Whether this symbol has been defined (not just referenced).
        /// </summary>
        public bool IsDefined { get; set; }

        /// <summary>
        /// List of AST nodes that reference this symbol.
        /// </summary>
        public List<AstNode> References { get; }

        /// <summary>
        /// Additional metadata for the symbol.
        /// </summary>
        public Dictionary<string, object> Metadata { get; }

        public Symbol(uint id, string name, SymbolScope scope = SymbolScope.Unknown, SymbolType type = SymbolType.Label) {
            Id = id;
            Name = name;
            Scope = scope;
            Type = type;
            Section = null;
            LineNumber = -1;
            Address = 0;
            IsDefined = false;
            References = new List<AstNode>();
            Metadata = new Dictionary<string, object>();
        }

        /// <summary>
        /// Marks this symbol as defined at the specified line number.
        /// </summary>
        public void Define(int lineNumber, string section = null) {
            IsDefined = true;
            LineNumber = lineNumber;
            if (section != null) {
                Section = section;
            }
            // If this was an unknown symbol, promote it to local by default
            if (Scope == SymbolScope.Unknown) {
                Scope = SymbolScope.Local;
            }
        }

        /// <summary>
        /// Adds a reference to this symbol from an AST node.
        /// </summary>
        public void AddReference(AstNode node) {
            if (!References.Contains(node)) {
                References.Add(node);
            }
        }

        public override string ToString() {
            return $"Symbol[{Id}]: {Name} ({Scope}, {Type}) - Defined: {IsDefined}, Line: {LineNumber}, Refs: {References.Count}";
        }

        public override bool Equals(object obj) {
            if (obj is Symbol other) {
                return Id == other.Id;
            }
            return false;
        }

        public override int GetHashCode() {
            return Id.GetHashCode();
        }
    }

    /// <summary>
    /// Manages symbols across different scopes in the assembly program.
    /// </summary>
    public class SymbolTable {
        private readonly Dictionary<string, Symbol> _symbols;
        private readonly Dictionary<uint, Symbol> _symbolsById;
        private readonly Dictionary<SymbolScope, List<Symbol>> _symbolsByScope;
        private uint _nextId;

        public SymbolTable() {
            _symbols = new Dictionary<string, Symbol>();
            _symbolsById = new Dictionary<uint, Symbol>();
            _symbolsByScope = new Dictionary<SymbolScope, List<Symbol>>();
            _nextId = 1;
            
            // Initialize scope lists
            foreach (SymbolScope scope in Enum.GetValues(typeof(SymbolScope))) {
                _symbolsByScope[scope] = new List<Symbol>();
            }
        }

        /// <summary>
        /// Resets the symbol ID counter (for testing purposes).
        /// </summary>
        public void ResetIdCounter() {
            _nextId = 1;
        }

        /// <summary>
        /// Gets or creates a symbol with the specified name and scope.
        /// If the symbol already exists, returns the existing one.
        /// </summary>
        public Symbol GetOrCreateSymbol(string name, SymbolScope scope = SymbolScope.Unknown, SymbolType type = SymbolType.Label) {
            if (_symbols.TryGetValue(name, out Symbol existingSymbol)) {
                // If we're trying to promote an unknown symbol to a known scope, update it
                if (existingSymbol.Scope == SymbolScope.Unknown && scope != SymbolScope.Unknown) {
                    PromoteSymbolScope(existingSymbol, scope);
                }
                return existingSymbol;
            }

            var newSymbol = new Symbol(_nextId++, name, scope, type);
            _symbols[name] = newSymbol;
            _symbolsById[newSymbol.Id] = newSymbol;
            _symbolsByScope[scope].Add(newSymbol);

            return newSymbol;
        }

        /// <summary>
        /// Gets a symbol by name, or null if it doesn't exist.
        /// </summary>
        public Symbol GetSymbol(string name) {
            return _symbols.TryGetValue(name, out Symbol symbol) ? symbol : null;
        }

        /// <summary>
        /// Gets a symbol by ID, or null if it doesn't exist.
        /// </summary>
        public Symbol GetSymbol(uint id) {
            return _symbolsById.TryGetValue(id, out Symbol symbol) ? symbol : null;
        }

        /// <summary>
        /// Defines a symbol at the specified line number and section.
        /// </summary>
        public Symbol DefineSymbol(string name, int lineNumber, string section = null, SymbolScope scope = SymbolScope.Local, SymbolType type = SymbolType.Label) {
            var symbol = GetOrCreateSymbol(name, scope, type);
            symbol.Define(lineNumber, section);
            return symbol;
        }

        /// <summary>
        /// Adds a reference to a symbol from an AST node.
        /// Creates the symbol as Unknown if it doesn't exist.
        /// </summary>
        public Symbol ReferenceSymbol(string name, AstNode referencingNode, SymbolType type = SymbolType.Label) {
            var symbol = GetOrCreateSymbol(name, SymbolScope.Unknown, type);
            symbol.AddReference(referencingNode);
            return symbol;
        }

        /// <summary>
        /// Promotes a symbol from Unknown scope to the specified scope.
        /// </summary>
        private void PromoteSymbolScope(Symbol symbol, SymbolScope newScope) {
            if (symbol.Scope == newScope) return;

            // Remove from old scope list
            _symbolsByScope[symbol.Scope].Remove(symbol);
            
            // Update scope
            symbol.Scope = newScope;
            
            // Add to new scope list
            _symbolsByScope[newScope].Add(symbol);
        }

        /// <summary>
        /// Gets all symbols in a specific scope.
        /// </summary>
        public IReadOnlyList<Symbol> GetSymbolsByScope(SymbolScope scope) {
            return _symbolsByScope[scope].AsReadOnly();
        }

        /// <summary>
        /// Gets all symbols.
        /// </summary>
        public IReadOnlyCollection<Symbol> GetAllSymbols() {
            return _symbols.Values;
        }

        /// <summary>
        /// Gets all undefined symbols (forward references).
        /// </summary>
        public IEnumerable<Symbol> GetUndefinedSymbols() {
            return _symbols.Values.Where(s => !s.IsDefined);
        }

        /// <summary>
        /// Checks if all referenced symbols have been defined.
        /// </summary>
        public bool AllSymbolsDefined() {
            return !GetUndefinedSymbols().Any();
        }

        /// <summary>
        /// Gets symbols that have references but are not defined.
        /// </summary>
        public IEnumerable<Symbol> GetUnresolvedReferences() {
            return _symbols.Values.Where(s => s.References.Count > 0 && !s.IsDefined);
        }

        /// <summary>
        /// Clears all symbols from the table.
        /// </summary>
        public void Clear() {
            _symbols.Clear();
            _symbolsById.Clear();
            foreach (var list in _symbolsByScope.Values) {
                list.Clear();
            }
        }

        /// <summary>
        /// Gets the total number of symbols.
        /// </summary>
        public int Count => _symbols.Count;

        /// <summary>
        /// Exports symbol information for serialization/debugging.
        /// </summary>
        public Dictionary<string, object> ExportSymbolInfo() {
            var export = new Dictionary<string, object>();
            
            foreach (var symbol in _symbols.Values) {
                export[symbol.Name] = new {
                    Id = symbol.Id,
                    Scope = symbol.Scope.ToString(),
                    Type = symbol.Type.ToString(),
                    Section = symbol.Section,
                    LineNumber = symbol.LineNumber,
                    Address = symbol.Address,
                    IsDefined = symbol.IsDefined,
                    ReferenceCount = symbol.References.Count
                };
            }
            
            return export;
        }
    }
} 