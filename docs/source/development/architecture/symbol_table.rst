****************
Symbol Table Architecture
****************

The KORE assembler implements a comprehensive symbol table system that manages labels, variables, and other symbols with proper scope management and address resolution. This document describes the architecture, design decisions, and implementation details of the symbol table system.

Overview
========

The symbol table system provides centralized management of all symbols within RISC-V assembly programs, supporting:

- **Multi-scope symbol management** (Global, Local, Unknown)
- **Forward reference resolution** 
- **Auto-incrementing unique symbol IDs**
- **Symbol type classification** (Label, Data, Function, Section)
- **Address resolution and validation**
- **Cross-compilation unit symbol visibility**

Core Architecture
=================

The symbol table system consists of several key components that work together to provide comprehensive symbol management:

Core Components
===============

Symbol Class
------------

The ``Symbol`` class represents individual symbols with comprehensive metadata:

**Properties:**

.. list-table:: Symbol Properties
    :widths: 20 15 65
    :header-rows: 1

    * - Property
      - Type
      - Description
    * - ``Id``
      - ``int``
      - Unique auto-incrementing identifier
    * - ``Name``
      - ``string``
      - Symbol name (e.g., "main", "loop", "data_var")
    * - ``Scope``
      - ``SymbolScope``
      - Local, Global, or Unknown
    * - ``Type``
      - ``SymbolType``
      - Label, Data, Function, or Section
    * - ``Section``
      - ``string``
      - Section where symbol is defined (.text, .data, etc.)
    * - ``LineNumber``
      - ``int``
      - Source line number where symbol is defined
    * - ``Address``
      - ``uint``
      - Memory address/offset (resolved during assembly)
    * - ``IsDefined``
      - ``bool``
      - Whether symbol has been defined (not just referenced)
    * - ``References``
      - ``List<AstNode>``
      - AST nodes that reference this symbol
    * - ``Metadata``
      - ``Dictionary``
      - Additional symbol-specific metadata

**Key Methods:**

.. code-block:: csharp

    public void Define(int lineNumber, string section = null)
    public void AddReference(AstNode node)
    public override string ToString()

SymbolTable Class
-----------------

The ``SymbolTable`` class provides centralized symbol management:

**Core Features:**

- **Unique ID generation** with auto-incrementing counter
- **Multi-scope organization** with scope-based lookups
- **Comprehensive symbol queries** and validation
- **Forward reference tracking**

**Key Methods:**

.. list-table:: SymbolTable Methods
    :widths: 40 60
    :header-rows: 1

    * - Method
      - Description
    * - ``GetOrCreateSymbol(name, scope, type)``
      - Returns existing symbol or creates new one
    * - ``GetSymbol(name)``
      - Retrieves symbol by name
    * - ``DefineSymbol(name, lineNumber, section)``
      - Defines a symbol at specific location
    * - ``GetSymbolsByScope(scope)``
      - Returns all symbols in given scope
    * - ``GetUndefinedSymbols()``
      - Returns symbols that are referenced but not defined
    * - ``AllSymbolsDefined()``
      - Checks if all symbols have been defined
    * - ``ExportSymbols()``
      - Exports symbol table for debugging/analysis

**Implementation Example:**

.. code-block:: csharp

    public Symbol GetOrCreateSymbol(string name, SymbolScope scope, SymbolType type) {
        if (_symbols.ContainsKey(name)) {
            var existingSymbol = _symbols[name];
            // Promote scope if upgrading from Unknown
            if (existingSymbol.Scope == SymbolScope.Unknown && scope != SymbolScope.Unknown) {
                existingSymbol.Scope = scope;
            }
            return existingSymbol;
        }

        var symbol = new Symbol(++_nextId, name, scope, type);
        _symbols[name] = symbol;
        return symbol;
    }

Symbol Scopes
=============

The system supports three distinct symbol scopes:

Unknown Scope
-------------

**Purpose:** Forward references and symbols used before definition

**Behavior:**
- Created automatically when symbol is first referenced
- Automatically promoted to Local or Global when defined
- Enables proper forward reference resolution

**Example:**

.. code-block:: asm

    # Forward reference creates Unknown scope symbol
    beq x1, x2, end_loop
    addi x1, x1, 1
    
    # Definition promotes to Local scope  
    end_loop:
        nop

Local Scope
-----------

**Purpose:** Symbols visible only within current compilation unit

**Behavior:**
- Default scope for defined symbols
- Prevents external visibility
- Avoids naming conflicts across files

**Example:**

.. code-block:: asm

    .local helper_function
    helper_function:    # Local scope symbol
        ret

Global Scope
------------

**Purpose:** Symbols visible across compilation units

**Behavior:**
- Explicitly declared via ``.global`` directive
- Available for external linkage
- Enables cross-module symbol references

**Example:**

.. code-block:: asm

    .global main
    main:               # Global scope symbol
        ret

Symbol Types
============

The system classifies symbols into distinct types:

.. list-table:: Symbol Types
    :widths: 20 80
    :header-rows: 1

    * - Type
      - Description
    * - ``Label``
      - Jump targets, function names, loop labels
    * - ``Data``
      - Data variables, constants, arrays
    * - ``Function``
      - Function symbols (special case of labels)
    * - ``Section``
      - Section markers (.text, .data, .rodata, .bss)

Integration with AST
====================

Symbol-Based AST Nodes
-----------------------

The symbol table integrates with the AST through specialized node types:

**SymbolReferenceNode:**

.. code-block:: csharp

    public class SymbolReferenceNode : AstNode {
        public int SymbolId { get; set; }
        public Symbol ResolvedSymbol { get; set; }
        
        public bool IsResolved => ResolvedSymbol != null;
        public bool IsForwardReference => !ResolvedSymbol?.IsDefined ?? false;
    }

**Symbol-Based Instruction Nodes:**

- ``InstructionNodeTypeBSymbol``: Branch instructions with symbol references
- ``InstructionNodeTypeJSymbol``: Jump instructions with symbol references

These replace string-based label references with type-safe symbol ID references.

**SymbolDirectiveNode:**

.. code-block:: csharp

    public class SymbolDirectiveNode : AstNode {
        public DirectiveType Type { get; set; }  // Global or Local
        public string SymbolName { get; set; }
        public Symbol Symbol { get; set; }       // Resolved symbol reference
    }

Parser Integration
==================

**Enhanced Parsing Logic:**

The parser integrates with the symbol table through:

1. **Symbol directive processing** during parsing
2. **Automatic symbol creation** for label references
3. **Post-processing integration** with symbol table
4. **Forward reference resolution**

**Key Integration Points:**

.. code-block:: csharp

    // During parsing - create symbol references
    var symbol = programNode.SymbolTable.GetOrCreateSymbol(
        labelName, SymbolScope.Unknown, SymbolType.Label);
    
    // Post-processing - integrate directives
    ProcessSymbolDirectives(programNode, currentSection);

**Extension Methods:**

The ``ParserExtensions`` class provides helper methods for parser integration:

.. code-block:: csharp

    public static SymbolDirectiveNode ProcessLocalDirective(
        this ProgramNode program, string symbolName)
    public static SymbolDirectiveNode ProcessGlobalDirective(
        this ProgramNode program, string symbolName)
    public static Symbol DefineLabel(
        this ProgramNode program, string labelName, int lineNumber, string section)

Forward Reference Resolution
============================

**Problem:**

Assembly programs frequently reference symbols before they are defined:

.. code-block:: asm

    main:
        beq x1, x2, cleanup    # Forward reference to 'cleanup'
        addi x1, x1, 1
    cleanup:
        ret

**Solution:**

The symbol table handles forward references through a two-phase approach:

1. **First Pass**: Create symbols with ``Unknown`` scope
2. **Resolution**: Promote to appropriate scope when definition is encountered

**Implementation:**

.. code-block:: csharp

    // Phase 1: Create unknown symbol for forward reference
    var symbol = symbolTable.GetOrCreateSymbol("cleanup", SymbolScope.Unknown, SymbolType.Label);
    
    // Phase 2: Promote when definition found
    symbol.Define(lineNumber, currentSection);
    if (hasGlobalDirective) {
        symbol.Scope = SymbolScope.Global;
    } else {
        symbol.Scope = SymbolScope.Local;
    }

Benefits and Design Goals
=========================

**Type Safety:**

- Symbol references use unique IDs instead of error-prone strings
- Compile-time checking of symbol operations
- Prevents common assembly programming errors

**Performance:**

- O(1) symbol lookup by name
- Efficient scope-based queries
- Minimal memory overhead

**Extensibility:**

- Easy addition of new symbol types
- Pluggable metadata system
- Framework for advanced symbol analysis

**Debugging Support:**

- Rich symbol metadata for debugging
- Symbol table export for analysis
- Reference tracking for impact analysis

Implementation Status
=====================

**Completed Features:**

- ✅ Core symbol table implementation
- ✅ Multi-scope symbol management  
- ✅ Forward reference resolution
- ✅ Parser integration via extension methods
- ✅ Symbol directive support (.global/.local)
- ✅ Comprehensive test suite (18 tests)
- ✅ AST node integration

**Current Status:**

The symbol table foundation is complete and fully integrated with the parser. All core functionality is implemented and tested, providing a solid foundation for advanced assembly language features.

**Next Steps:**

1. Enhanced code generator integration
2. Symbol import/export for multi-file assembly
3. Advanced symbol analysis and validation
4. Performance optimization for large symbol tables

This symbol table architecture provides a robust foundation for advanced assembly language features while maintaining simplicity and performance for basic use cases. 