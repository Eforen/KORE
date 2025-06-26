Lexer Implementation
====================

The KUICK lexer (also called tokenizer) performs lexical analysis on RISC-V assembly source code, converting the character stream into a sequence of tokens using regex-based pattern matching.

Overview
--------

The lexer recognizes and tokenizes:

- **Numbers**: Decimal, hexadecimal, binary, float, and double literals
- **Identifiers**: Labels, symbol names, and instruction mnemonics
- **Directives**: Assembly directives (prefixed with ``.``)
- **Inline Directives**: Preprocessor-style directives (prefixed with ``%``)
- **Labels**: Symbol definitions (suffixed with ``:``)
- **Strings**: Quoted string literals
- **Comments**: Line comments (``#`` and ``//``) and block comments (``/* */``)
- **Punctuation**: Parentheses for addressing modes
- **Whitespace**: Spaces, tabs, newlines, and commas

Token Types
-----------

The lexer produces the following token types:

**Compiler Logic Tokens:**
- ``COMPILER_LOGIC`` - Preprocessor directives (#define, #ifdef, etc.)

**Control Tokens:**
- ``EOL`` - End of line (newline characters)
- ``WHITESPACE`` - Spaces, tabs, and commas
- ``COMMENT`` - All comment types

**Numeric Literals:**
- ``NUMBER_INT`` - Decimal integers (e.g., ``123``, ``-456``)
- ``NUMBER_HEX`` - Hexadecimal literals (e.g., ``0x1A2B``, ``0xDEADBEEF``)
- ``NUMBER_BIN`` - Binary literals (e.g., ``0b1010``, ``0b11110000``)
- ``NUMBER_FLOAT`` - Single-precision floats (e.g., ``3.14f``, ``-2.5f``)
- ``NUMBER_DOUBLE`` - Double-precision floats (e.g., ``3.14``, ``-2.5e10``)

**String Literals:**
- ``STRING`` - Double or single-quoted strings (e.g., ``"Hello"``, ``'World'``)

**Assembly Constructs:**
- ``DIRECTIVE`` - Assembly directives (e.g., ``.text``, ``.global``, ``.local``)
- ``INLINE_DIRECTIVE`` - Preprocessor-style directives (e.g., ``%hi``, ``%lo``)
- ``LABEL`` - Symbol definitions (e.g., ``main:``, ``loop:``)
- ``IDENTIFIER`` - Symbol names and instruction mnemonics

**Punctuation:**
- ``PARREN_OPEN`` - Opening parenthesis ``(``
- ``PARREN_CLOSE`` - Closing parenthesis ``)``

Register Handling
-----------------

The lexer includes comprehensive register alias support:

**Integer Registers:**
- Numeric format: ``x0`` - ``x31``
- Zero-padded format: ``x00`` - ``x31``
- ABI aliases: ``zero``, ``ra``, ``sp``, ``gp``, ``tp``, ``fp``
- Temporary registers: ``t0`` - ``t6``
- Saved registers: ``s0`` - ``s11``
- Argument registers: ``a0`` - ``a7``

**Floating-Point Registers:**
- Numeric format: ``f0`` - ``f31``
- ABI aliases: ``ft0`` - ``ft7``, ``fs0`` - ``fs9``, ``fa0`` - ``fa7``

Lexical Rules
-------------

**Token Priority:**
The lexer processes tokens in order of specificity:

1. Compiler logic directives
2. Control characters (EOL, whitespace)
3. Comments
4. Numeric literals (most specific to least specific)
5. String literals
6. Assembly constructs
7. Identifiers and punctuation

**Number Parsing:**

- **Decimal**: ``123``, ``-456`` (regex: ``^-?\d+``)
- **Hexadecimal**: ``0x1A2B``, ``0xDEADBEEF`` (regex: ``^0x[\da-fA-F]+``)
- **Binary**: ``0b1010``, ``0b11110000`` (regex: ``^0b[01]+``)
- **Float**: ``3.14f``, ``-2.5F`` (regex: ``^-?\d+\.?\d*[fF]``)
- **Double**: ``3.14d``, ``-2.5e10`` (regex: ``^-?\d+\.?\d*[dD]`` or ``^-?\d+\.\d+``)

**String Literals:**

- Double-quoted: ``"Hello, World!"``
- Single-quoted: ``'Character'``
- Regex pattern: ``^"[^"]*"`` and ``^'[^']*'``

**Comments:**

- Line comments: ``# This is a comment`` or ``// This is a comment``
- Block comments: ``/* Multi-line comment */``

**Labels and Identifiers:**

- Labels: Must end with colon (``main:``, ``loop_1:``)
- Identifiers: Can include letters, numbers, underscores, dots, and brackets
- Regex: ``^[a-zA-Z_][a-zA-Z0-9_]*:`` for labels, ``^[\w\[\]\._]+`` for identifiers

Implementation Details
----------------------

**TokenFinder Structure:**
Each token type is defined by a ``TokenFinder`` object containing:

- Regular expression pattern for matching
- Associated token type enum value
- Processing priority (determined by array order)

**Lexer State:**
The lexer maintains:

- Current position in source string
- Line and column tracking for error reporting
- Token buffer for lookahead operations
- EOF detection

**Error Handling:**
- Unrecognized characters are typically captured by the broad identifier pattern
- Position tracking enables precise error location reporting
- Graceful handling of incomplete tokens at EOF

Testing
-------

The lexer includes extensive test coverage:

- **Token Recognition Tests**: Verify correct tokenization
- **Number Format Tests**: All supported number formats
- **String Parsing Tests**: Various string literal formats
- **Error Handling Tests**: Invalid input scenarios
- **Performance Tests**: Large file tokenization

**Test Statistics:**
- 50+ tokenization tests
- 20+ number format tests
- 15+ string parsing tests
- 25+ error handling tests

Implementation Notes
--------------------

**Performance Optimizations:**
- Single-pass tokenization
- Efficient character lookahead
- Minimal memory allocations
- Fast string interning for identifiers

**Thread Safety:**
- Lexer state is isolated per instance
- No shared mutable state
- Safe for concurrent use

**Extensibility:**
- Easy to add new token types
- Configurable keyword recognition
- Pluggable number parsing 