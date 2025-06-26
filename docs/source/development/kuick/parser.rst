Parser Implementation
====================

The KUICK parser is responsible for converting tokens from the lexer into an Abstract Syntax Tree (AST). It handles all RISC-V instructions, directives, and symbols.

Overview
--------

The parser follows a recursive descent approach and builds specialized AST nodes for different language constructs:

- **Instruction Nodes**: R-Type, I-Type, S-Type, B-Type, U-Type, J-Type
- **Directive Nodes**: Symbol directives, section directives, data directives
- **Symbol Nodes**: Labels, references, and forward declarations

Enhanced Directive Parsing
---------------------------

The parser includes enhanced support for symbol visibility directives:

**Symbol Directive Handling:**

.. code-block:: csharp

    // Enhanced ParseNodeDirective method
    if (name == ".global" || name == ".local") {
        if (currentToken.token == Lexer.Token.IDENTIFIER) {
            var symbolName = currentToken.value;
            var directiveType = name == ".global" ? 
                SymbolDirectiveNode.DirectiveType.Global : 
                SymbolDirectiveNode.DirectiveType.Local;
            
            return ComposeInstructionArray(expectReturnEOL(
                new SymbolDirectiveNode(directiveType, symbolName), lexer));
        }
    }

**Symbol Table Integration:**

The parser performs post-processing to integrate symbol directives with the program's symbol table:

.. code-block:: csharp

    // Post-process to integrate symbol directives with symbol table
    ProcessSymbolDirectives(programNode, currentSection);

Forward Reference Resolution
----------------------------

The parser handles forward references through a multi-pass approach:

**Initial Pass:**
- Creates symbols with **Unknown** scope for forward references
- Builds the complete AST structure
- Defers symbol resolution to later passes

**Symbol Resolution Pass:**
- Processes all symbol directives
- Promotes Unknown scope symbols to their declared scope
- Validates symbol consistency across the program

**Example Forward Reference:**

.. code-block:: asm

    # Forward reference - initially Unknown scope
    jal x1, helper_function
    
    # Later directive promotes to Local scope  
    .local helper_function
    helper_function:
        ret

Instruction Parsing
-------------------

The parser supports all standard RISC-V instruction formats:

**R-Type Instructions:**
- Three register operands
- Examples: ``add``, ``sub``, ``and``, ``or``

**I-Type Instructions:**
- Two registers and immediate value
- Examples: ``addi``, ``lw``, ``jalr``

**S-Type Instructions:**
- Store instructions with immediate offset
- Examples: ``sw``, ``sh``, ``sb``

**B-Type Instructions:**
- Branch instructions with labels
- Examples: ``beq``, ``bne``, ``blt``

**U-Type Instructions:**
- Upper immediate instructions
- Examples: ``lui``, ``auipc``

**J-Type Instructions:**
- Jump instructions with large immediate
- Examples: ``jal``

Pseudo-Instruction Support
--------------------------

The parser includes comprehensive support for RISC-V pseudo-instructions:

**Zero Operand:**
- ``nop`` → ``addi x0, x0, 0``

**Single Operand:**
- ``neg rd, rs`` → ``sub rd, x0, rs``
- ``seqz rd, rs`` → ``sltiu rd, rs, 1``
- ``snez rd, rs`` → ``sltu rd, x0, rs``

**Branch Pseudo-Instructions:**
- ``beqz rs, offset`` → ``beq rs, x0, offset``
- ``bnez rs, offset`` → ``bne rs, x0, offset``
- ``blez rs, offset`` → ``bge x0, rs, offset``
- ``bgtz rs, offset`` → ``blt x0, rs, offset``

Error Handling
--------------

The parser includes comprehensive error handling:

- **Syntax Errors**: Invalid instruction formats
- **Semantic Errors**: Invalid register names or immediate values
- **Symbol Errors**: Undefined symbols or multiple definitions
- **Directive Errors**: Invalid directive parameters

**Error Recovery:**
- Continues parsing after errors when possible
- Provides meaningful error messages with line numbers
- Suggests corrections for common mistakes

Testing
-------

The parser includes extensive test coverage:

- **Instruction Tests**: Every supported instruction format
- **Directive Tests**: All assembler directives
- **Pseudo-Instruction Tests**: Complete pseudo-instruction set
- **Error Tests**: Various error conditions and recovery
- **Integration Tests**: Complex assembly programs

**Test Statistics:**
- 100+ instruction parsing tests
- 18+ symbol table integration tests
- 50+ pseudo-instruction tests
- Comprehensive error handling coverage 