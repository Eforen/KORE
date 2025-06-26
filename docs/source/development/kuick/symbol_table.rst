Symbol Table Implementation
============================

The KUICK assembler uses a sophisticated symbol table system to manage symbols across multiple scopes and handle forward references. This document details the implementation within the assembler context.

Overview
--------

The symbol table is integrated into the KUICK parser and maintains:

- **Multi-scope management**: Global, Local, and Unknown scopes
- **Forward reference resolution**: Two-pass symbol resolution
- **Symbol metadata**: Type, size, and section information
- **Integration with AST**: Seamless AST node integration

For detailed architecture information, see :doc:`../architecture/symbol_table`.

Parser Integration
------------------

The symbol table is tightly integrated with the KUICK parser:

**Symbol Creation:**

.. code-block:: csharp

    // During parsing, symbols are created with initial scope
    var symbol = symbolTable.AddSymbol(name, SymbolScope.Unknown);
    
    // Directives later promote scope
    if (directiveType == SymbolDirectiveNode.DirectiveType.Global) {
        symbol.Scope = SymbolScope.Global;
    }

**Symbol Resolution:**

.. code-block:: csharp

    // Post-processing integrates symbol directives
    ProcessSymbolDirectives(programNode, currentSection);
    
    // Forward references are resolved in second pass
    ResolveForwardReferences(programNode);

Multi-Pass Assembly
-------------------

The symbol table enables multi-pass assembly for handling forward references:

**Pass 1: Initial Processing**
- Create symbols with Unknown scope for forward references
- Build complete AST structure
- Process symbol directives

**Pass 2: Symbol Resolution**
- Promote Unknown scope symbols to declared scope
- Resolve all forward references
- Validate symbol consistency

**Example Forward Reference Handling:**

.. code-block:: asm

    # Forward reference - creates Unknown scope symbol
    jal x1, helper_function
    
    # Later directive promotes to Local scope
    .local helper_function
    helper_function:
        ret

Symbol Scope Management
-----------------------

The symbol table maintains three distinct scopes:

**Global Scope:**
- Symbols visible across compilation units
- Used for public API functions and shared data
- Created by ``.global`` directive

**Local Scope:**
- Symbols visible only within current compilation unit
- Used for private functions and internal data
- Created by ``.local`` directive

**Unknown Scope:**
- Temporary scope for forward references
- Promoted to appropriate scope when directive is encountered
- Enables single-pass parsing with deferred resolution

Code Generation Integration
---------------------------

The symbol table integrates with code generation:

**Address Assignment:**

.. code-block:: csharp

    // Symbols receive addresses during code generation
    foreach (var symbol in symbolTable.GetSymbols()) {
        if (symbol.Type == SymbolType.Label) {
            symbol.Address = currentAddress;
        }
    }

**Reference Resolution:**

.. code-block:: csharp

    // Symbol references are resolved to addresses
    var targetSymbol = symbolTable.GetSymbol(symbolName);
    var offset = targetSymbol.Address - currentAddress;

Testing and Validation
----------------------

The symbol table implementation includes comprehensive testing:

**Test Coverage:**
- 18+ passing tests covering all symbol table functionality
- Symbol directive integration tests
- Forward reference resolution tests
- Multi-scope behavior validation

**Key Test Scenarios:**

.. code-block:: csharp

    [Test]
    public void TestLocalDirectiveIntegration() {
        // Verify .local directive creates Local scope symbol
    }
    
    [Test] 
    public void TestForwardReferenceResolution() {
        // Verify forward references are correctly resolved
    }
    
    [Test]
    public void TestSymbolScopePromotion() {
        // Verify Unknown scope symbols are promoted correctly
    }

Performance Considerations
--------------------------

The symbol table is optimized for assembler performance:

**Efficient Lookups:**
- Hash-based symbol resolution
- O(1) average case symbol access
- Minimal memory overhead

**Memory Management:**
- Symbols are allocated once and reused
- Efficient storage of symbol metadata
- Garbage collection friendly design

**Cache Locality:**
- Symbols grouped by scope for better cache performance
- Minimized pointer chasing during resolution

Integration Points
------------------

The symbol table integrates with multiple KUICK components:

**With Parser:**
- Symbol creation during directive parsing
- Forward reference tracking
- AST node symbol association

**With Code Generator:**
- Address assignment during assembly
- Reference resolution for instructions
- Section-aware symbol handling

**With AST:**
- Symbol nodes contain symbol table references
- Bidirectional symbol-node relationships
- Efficient symbol metadata access

Error Handling
--------------

The symbol table provides comprehensive error detection:

**Duplicate Symbol Detection:**
- Prevents multiple definitions in same scope
- Provides meaningful error messages
- Suggests alternative names when appropriate

**Undefined Symbol Detection:**
- Identifies unresolved forward references
- Reports line numbers and context
- Helps debug symbol visibility issues

**Scope Violation Detection:**
- Prevents access to local symbols from other units
- Validates symbol usage patterns
- Enforces visibility rules 