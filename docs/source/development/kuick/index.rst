KUICK Assembler Documentation
=============================

KUICK (KORE Universal Instruction Compiler Kit) is a modern RISC-V assembler designed for the KORE project. It provides a complete toolchain for assembling RISC-V assembly language programs with advanced features like multi-scope symbol management, forward reference resolution, and comprehensive error reporting.

Overview
--------

KUICK is built around a modular architecture that separates concerns across distinct components:

- **Lexer**: Tokenizes assembly source code into meaningful tokens
- **Parser**: Builds an Abstract Syntax Tree (AST) from tokens using recursive descent parsing
- **Symbol Table**: Manages symbols across multiple scopes with forward reference resolution
- **Code Generator**: Transforms the AST into RISC-V machine code

Key Features
------------

**Symbol Management:**
- Multi-scope symbol resolution (global, local, unknown)
- Forward and backward reference handling
- Automatic symbol type inference
- Symbol visibility control

**Parsing Capabilities:**
- Complete RISC-V instruction set support
- Pseudo-instruction expansion
- Directive processing (.global, .local, .text, .data, .bss)
- Comprehensive error reporting with source location information

**Code Generation:**
- All RISC-V instruction formats (R, I, S, B, U, J-type)
- Multiple output formats (ELF, raw binary, Intel HEX)
- Automatic address calculation and symbol resolution
- Optimization passes for size and performance

Architecture Principles
-----------------------

**Type Safety:**
KUICK uses strong typing throughout the compilation pipeline to catch errors early and provide better debugging information.

**Modular Design:**
Each component has a well-defined interface, making the assembler extensible and maintainable.

**Error Recovery:**
The assembler continues processing after encountering errors, providing comprehensive error reports in a single pass.

**Performance:**
Optimized for fast assembly times while maintaining code quality and comprehensive error checking.

Implementation Details
----------------------

The KUICK assembler follows a traditional multi-pass architecture:

1. **Lexical Analysis**: Source code → Token stream
2. **Syntax Analysis**: Token stream → Abstract Syntax Tree
3. **Symbol Resolution**: Symbol table population and resolution
4. **Code Generation**: AST → Machine code

Each pass is designed to be independent and testable, with clear interfaces between components.

.. toctree::
   :maxdepth: 2
   :caption: Core Components

   lexer
   parser
   ast
   symbol_table
   code_generation

.. toctree::
   :maxdepth: 2
   :caption: Implementation Details

   testing
   performance
   architecture 