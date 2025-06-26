Architecture Overview
====================

KUICK (KORE Universal Instruction Compiler Kit) follows a traditional multi-stage compiler architecture adapted for RISC-V assembly language processing. The design emphasizes modularity, extensibility, and performance while maintaining type safety throughout the compilation pipeline.

Core Design Principles
----------------------

**Separation of Concerns:**
Each component has a well-defined responsibility and clean interfaces with other components.

```
Source Code → [Lexer] → Tokens → [Parser] → AST → [Code Generator] → Binary
                ↓                    ↓              ↓
            Error Handling      Symbol Table    Output Formats
```

**Type Safety:**
Strong typing throughout the pipeline catches errors early and provides better debugging information.


**Extensibility:**
The architecture supports future enhancements like additional instruction sets, output formats, and optimization passes.

**Performance:**
Single-pass processing where possible, with efficient data structures and minimal memory allocation.

**Error Recovery:**
Graceful handling of errors with detailed reporting and continuation of processing where feasible.

Component Architecture
----------------------

The KUICK assembler consists of four primary components in a linear pipeline:

```
┌─────────────┐    ┌─────────────┐    ┌─────────────┐    ┌─────────────┐
│    Lexer    │───▶│   Parser    │───▶│ Symbol      │───▶│    Code     │
│             │    │             │    │ Table       │    │ Generator   │
│ (Tokenizer) │    │ (AST        │    │ (Multi-pass │    │ (Binary     │
│             │    │ Builder)    │    │ Resolution) │    │ Output)     │
└─────────────┘    └─────────────┘    └─────────────┘    └─────────────┘
      │                    │                    │                    │
      ▼                    ▼                    ▼                    ▼
   Tokens              AST Nodes           Symbol Info           Machine Code
```

**Data Flow:**
1. **Source Code** → Lexer → **Token Stream**
2. **Token Stream** → Parser → **Abstract Syntax Tree**
3. **AST + Symbol Table** → Symbol Resolution → **Resolved AST**
4. **Resolved AST** → Code Generator → **Binary Output**

Lexer Component
---------------

**Purpose:** Converts character stream to token stream using regex-based pattern matching.

**Key Features:**
- **Regex-Based Tokenization**: Uses `TokenFinder` objects with compiled regex patterns
- **Register Alias Resolution**: Comprehensive mapping of RISC-V register names
- **Position Tracking**: Line and column information for error reporting
- **Comment Handling**: Support for `#`, `//`, and `/* */` comment styles

**Implementation Structure:**

.. code-block:: csharp

    public partial class Lexer {
        public TokenFinder[] Spec = { /* regex patterns */ };
        Replacement[] REGISTER_REPLACEMENTS = { /* register mappings */ };
        
        private string _string;
        private int _cursor;
        
        public TokenData ReadToken(bool ignoreWhitespace = false);
        public TokenData PeakToken(bool ignoreWhitespace = true);
    }

**Token Types:**
The lexer produces tokens for numbers, strings, identifiers, directives, labels, and punctuation.

Parser Component
----------------

**Purpose:** Builds Abstract Syntax Tree from token stream using recursive descent parsing.

**Key Features:**
- **Single-Pass Parsing**: Builds AST in one pass with forward reference tracking
- **Instruction Type Handling**: Separate parsing logic for R, I, S, B, U, J-type instructions
- **Pseudo-Instruction Support**: Expands pseudo-instructions to real instructions
- **Directive Processing**: Handles assembly directives and symbol declarations
- **Error Recovery**: Continues parsing after syntax errors

**Implementation Structure:**

.. code-block:: csharp

    public static class Parser {
        public static AstNode Parse(Lexer lexer);
        
        // Instruction type parsers
        private static AstNode[] ParseRInstruction(TokenData token, Lexer lexer);
        private static AstNode[] ParseIInstruction(TokenData token, Lexer lexer);
        private static AstNode[] ParseBInstruction(TokenData token, Lexer lexer);
        // ... other instruction types
        
        // Directive parsers
        private static AstNode[] ParseDirective(TokenData token, Lexer lexer);
        private static AstNode[] ParseLabel(TokenData token, Lexer lexer);
    }

**AST Node Hierarchy:**
- **ProgramNode**: Root containing sections and symbol table
- **SectionNode**: Contains instructions and directives for a section
- **InstructionNode**: Base class for all instruction types
- **SymbolDirectiveNode**: Represents .global/.local directives
- **LabelNode**: Represents label definitions

Symbol Table Component
----------------------

**Purpose:** Manages symbols across multiple scopes with forward reference resolution.

**Key Features:**
- **Multi-Scope Management**: Global, Local, and Unknown scopes
- **Forward Reference Tracking**: Deferred resolution for undefined symbols
- **Symbol Metadata**: Type, address, section, and reference information
- **Scope Promotion**: Automatic promotion of unknown symbols when defined

**Implementation Structure:**

.. code-block:: csharp

    public class SymbolTable {
        private Dictionary<string, Symbol> _symbols;
        private Dictionary<SymbolScope, List<Symbol>> _symbolsByScope;
        private uint _nextId;
        
        public Symbol GetOrCreateSymbol(string name, SymbolScope scope);
        public Symbol DefineSymbol(string name, int lineNumber, string section);
        public IEnumerable<Symbol> GetUndefinedSymbols();
    }

**Symbol Representation:**

.. code-block:: csharp

    public class Symbol {
        public uint Id { get; }
        public string Name { get; set; }
        public SymbolScope Scope { get; set; }
        public SymbolType Type { get; set; }
        public long Address { get; set; }
        public bool IsDefined { get; set; }
        public List<AstNode> References { get; }
    }

Code Generator Component
------------------------

**Purpose:** Transforms AST into RISC-V machine code with multi-pass symbol resolution.

**Key Features:**
- **Multi-Pass Strategy**: Handles forward references through multiple assembly passes
- **Instruction Encoding**: Direct encoding of RISC-V instruction formats
- **Symbol Address Assignment**: Calculates and assigns symbol addresses
- **Cache Miss Handling**: Efficiently detects and handles unresolved symbols

**Implementation Structure:**

.. code-block:: csharp

    public class CodeGenerator : ASTProcessor {
        public enum GeneratorPass {
            PsudoCode,        // Convert pseudo-instructions
            LineNumber,       // Assign line numbers
            LineNumberCleanup,// Handle cache misses
            GenerateCode      // Generate final binary
        }
        
        public byte[] Generate(ProgramNode node);
        
        // Visitor pattern for AST processing
        public AstNode ProcessASTNode(InstructionNodeTypeR node);
        public AstNode ProcessASTNode(SymbolDirectiveNode node);
        // ... other node types
    }

**Multi-Pass Strategy:**
1. **Pseudo-Code Pass**: Expand pseudo-instructions and prepare structure
2. **Line Number Pass**: Assign addresses and create symbol table entries
3. **Cleanup Pass**: Resolve any cache misses from forward references
4. **Code Generation Pass**: Generate final machine code with resolved symbols

Integration Points
------------------

**Lexer ↔ Parser:**
- Parser consumes tokens from lexer using `ReadToken()` and `PeakToken()`
- Error reporting includes position information from lexer
- Register aliases resolved during lexing for parser consumption

**Parser ↔ Symbol Table:**
- Parser creates symbol references and definitions during AST construction
- Symbol table integrated into `ProgramNode` for centralized management
- Directives processed to update symbol scope and metadata

**Parser ↔ AST:**
- Direct AST node creation during parsing
- Type-safe node construction with compile-time checking
- Forward references handled through symbol-based AST nodes

**Symbol Table ↔ Code Generator:**
- Code generator assigns addresses and resolves symbol references
- Multi-pass strategy handles forward references efficiently
- Symbol cache miss detection enables minimal pass requirements

Error Handling Strategy
-----------------------

**Lexical Errors:**
- Invalid characters captured by broad identifier pattern
- Position tracking for precise error location
- Graceful handling of incomplete tokens

**Syntax Errors:**
- Parser continues after errors when possible
- Detailed error messages with expected token information
- Recovery strategies for common error patterns

**Semantic Errors:**
- Undefined symbol detection and reporting
- Type mismatch detection in instruction operands
- Scope violation detection for symbol references

**Runtime Errors:**
- Assembly-time errors for invalid instruction combinations
- Address resolution failures for malformed programs
- Resource exhaustion handling for large programs

Testing Integration
-------------------

**Component Testing:**
Each component includes comprehensive unit tests with isolated dependencies.

**Integration Testing:**
End-to-end tests verify the complete pipeline functionality.

**Test Infrastructure:**
- Mock objects for component isolation
- Test utilities for AST comparison and validation
- Performance benchmarks integrated into test suite

**Error Testing:**
- Comprehensive error scenario coverage
- Recovery behavior validation
- Error message accuracy verification

Extension Points
----------------

**New Instruction Types:**
- Add parser methods for new instruction formats
- Extend AST node hierarchy for new instruction types
- Update code generator with encoding logic

**Additional Output Formats:**
- Extend code generator with format-specific writers
- Maintain existing AST and symbol table interfaces
- Add format-specific optimization passes

**Custom Directives:**
- Extend directive parsing in parser component
- Add new AST node types for custom directives
- Implement processing logic in code generator

**Optimization Passes:**
- Insert optimization phases between parsing and code generation
- Maintain AST structure for transparent optimization
- Add performance monitoring for optimization effectiveness

The KUICK architecture provides a solid foundation for RISC-V assembly processing while remaining extensible for future enhancements and optimizations. 