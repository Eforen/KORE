- [Done](#done)
  - [Done 10/18/2021](#done-10182021)
  - [Done 10/21/2021](#done-10212021)
  - [Done 02/25/2023](#done-02252023)
  - [Done 2023/03/08](#done-20230308)
    - [Next Commit (Eforen)](#next-commit-eforen)
- [Working On](#working-on)
  - [Working on (Eforen)](#working-on-eforen)
- [Todos](#todos)
  - [Todo](#todo)
  - [Planned for some time later](#planned-for-some-time-later)
  - [Wishlist](#wishlist)

# Done
## Done 10/18/2021
* [KANBAN] Initialize
* [KUICK][TOKENIZER] Write tests to specificity test tokenizer
* [KUICK][PARSER] refactor still pass tests
* [KUICK][TOKENIZER] change whitespace and comments to their own tokens
* [KUICK][TOKENIZER][TOKEN] implement NUMBER_FLOAT
* [KUICK][TOKENIZER][TOKEN] implement NUMBER_DOUBLE
* [KUICK][TOKENIZER][TOKEN] implement NUMBER_HEX

## Done 10/21/2021
* [KUICK][TOKENIZER] Write Tests for every Token
* [KUICK][TOKENIZER] Implement all failing tests

## Done 02/25/2023
* [KUICK][LEXER] Refactor switch the name of the parser and lexer because I had the definitions backwards
* [KUICK][PARSER] Change to using a parser that makes an intermediary AST (Abstract Syntax Tree)
* [KUICK][PARSER] Write Test for R Type Instruction `ADD`

## Done 2023/03/08
* [KUICK][PARSER] Write Test for I Type Instruction `ADDI`
* [KUICK][PARSER] Write Test for S Type Instruction `SW`
* [KUICK][PARSER] Write Test for S Type Instruction `SH`
* [KUICK][PARSER] Write Test for S Type Instruction `SB`
* [KUICK][PARSER] Refactor ParseIInstruction to use the new ParseOP, ParseRegister, and ParseImmediate functions
* [KUICK][PARSER] Refactor ParseRInstruction to use the new ParseOP, ParseRegister, and ParseImmediate functions
### Next Commit (Eforen)
* [KUICK][PARSER] Write Test for R Type Instruction `SUB`
* [KUICK][PARSER] Write Test for R Type Instruction `AND`
* [KUICK][PARSER] Write Test for R Type Instruction `OR`

# Working On
## Working on (Eforen)
* Refactor KUICK into its own library
* [KUICK][PARSER] Rewrite tests to take in string and output AST
* [KUICK][ASSEMBLER] Rewrite tests to take in AST and output binary
* [KUICK][ASSEMBLER] Write Tests for every Token

# Todos
* [KUICK][PARSER] Refactor ParseNodeSection to use a switch statement instead of ifs
* [KUICK][PARSER] Refactor ParseNodeSection to Reduce code dupliaction in the difrent OP types
* [KUICK][PARSER] Write Test for B Type Instruction `BEQ`
* [KUICK][PARSER] Write Test for B Type Instruction `BNE`
* [KUICK][PARSER] Write Test for B Type Instruction `BLT`
* [KUICK][PARSER] Write Test for B Type Instruction `BGE`
* [KUICK][PARSER] Write Test for U Type Instruction `LUI`
* [KUICK][PARSER] Write Test for U Type Instruction `AUIPC`
* [KUICK][PARSER] Write Test for J Type Instruction `JAL`
* [KUICK][PARSER] Write Test for J Type Instruction `JALR`
* [KUICK][PARSER] Write Test for F Type Instruction `FADD.S`
* [KUICK][PARSER] Write Test for F Type Instruction `FMUL.D`
* [KUICK][PARSER] Write Test for I Type Instruction `SLTI`
* [KUICK][PARSER] Write Test for I Type Instruction `XORI`
* [KUICK][PARSER] Write Test for I Type Instruction `SLLI`
* [KUICK][PARSER] Write Test for RV32I/E Instruction Set Instruction `LW`
* [KUICK][PARSER] Write Test for RV64I/E Instruction Set Instruction `ADDIW`
* [KUICK][PARSER] Write Test for RV64I/E Instruction Set Instruction `LD`
* [KUICK][PARSER] Write Test for RV64I/E Instruction Set Instruction `SD`
* [KUICK][ASSEMBLER] Implement all failing tests
* Implement all RISC-V ASM directives in (RISC-V ASSEMBLY LANGUAGE Programmer Manual- Part 1)[https://shakti.org.in/docs/risc-v-asm-manual.pdf]
* [Cite] Email SHAKTI Development Team requesting to include their PDF in this repository (RISC-V ASSEMBLY LANGUAGE Programmer Manual- Part 1)[https://shakti.org.in/docs/risc-v-asm-manual.pdf]
## Todo
## Planned for some time later
## Wishlist