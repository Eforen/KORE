- [Done](#done)
  - [Done 10/18/2021](#done-10182021)
  - [Done 10/21/2021](#done-10212021)
  - [Done 11/16/2021](#done-11162021)
  - [Done 02/25/2023](#done-02252023)
  - [Done 2023/03/08](#done-20230308)
  - [Done 2023/05/20](#done-20230520)
  - [Done 2023/05/21](#done-20230521)
  - [Done 2023/05/26](#done-20230526)
    - [Next Commit (Eforen)](#next-commit-eforen)
  - [Done 2024/05/04](#done-20240504)
  - [Done 2025/06/16](#done-20250616)
    - [Next Commit (Eforen)](#next-commit-eforen-1)
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

## Done 11/16/2021
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
* [KUICK][PARSER] Write Test for R Type Instruction `SUB`
* [KUICK][PARSER] Write Test for R Type Instruction `AND`
* [KUICK][PARSER] Write Test for R Type Instruction `OR`
* [KUICK][PARSER] Write Test for I Type Instruction `SLTI`
* [KUICK][PARSER] Write Test for I Type Instruction `XORI`
* [KUICK][PARSER] Write Test for I Type Instruction `ORI`
* [KUICK][PARSER] Write Test for I Type Instruction `SLLI`
* [KUICK][PARSER] Write Test for I Type Instruction `SRLI`
* [KUICK][PARSER] Write Test for B Type Instruction `BEQ`
* [KUICK][PARSER] Write Test for B Type Instruction `BNE`
* [KUICK][PARSER] Write Test for B Type Instruction `BLT`
* [KUICK][PARSER] Write Test for B Type Instruction `BGE`
* [KUICK][PARSER] Refactor ParseNodeSection to use a switch statement instead of ifs
* [KUICK][PARSER] Refactor ParseNodeSection to Reduce code dupliaction in the difrent OP types
* [KUICK][PARSER] Write Test for U Type Instruction `LUI`
* [KUICK][PARSER] Write Test for U Type Instruction `AUIPC`
* [KUICK][PARSER] Write Test for J Type Instruction `JAL`
* [KUICK][PARSER] Rewrite tests to take in string and output AST
## Done 2023/05/20
* [KUICK][PARSER] Implement Pseudo Instruction: `nop`
* [KUICK][PARSER] Implement Pseudo Instruction: `neg`
* [KUICK][PARSER] Implement Pseudo Instruction: `seqz`
* [KUICK][PARSER] Implement Pseudo Instruction: `snez`
* [KUICK][PARSER] Implement Pseudo Instruction: `sltz`
* [KUICK][PARSER] Implement Pseudo Instruction: `sgtz`
* [KUICK][PARSER] Implement Pseudo Instruction: `beqz`
* [KUICK][PARSER] Implement Pseudo Instruction: `bnez`
* [KUICK][PARSER] Implement Pseudo Instruction: `blez`
* [KUICK][PARSER] Implement Pseudo Instruction: `bgez`
* [KUICK][PARSER] Implement Pseudo Instruction: `bltz`
* [KUICK][PARSER] Implement Pseudo Instruction: `bgtz`

## Done 2023/05/21
* [KIUCK][PARSER][BUG] `BEQZ` type instructions not working for some reason. Figure out why and fix it.
* [KIUCK][PARSER][BUG] Fix test case for `bltz x3, 0x00000001`

## Done 2023/05/26
### Next Commit (Eforen)
* [KUICK][LEXER] Implement Binary Literals
* [KUICK][PARSER] Implement Inline Directive `%hi`
* [KUICK][PARSER] Implement Inline Directive `%lo`
* [KUICK][PARSER] Implement Inline Directive `%pcrel_hi`
* [KUICK][PARSER] Implement Inline Directive `%pcrel_lo`
* [KUICK][PARSER] Implement `LB`
* [KUICK][PARSER] Implement `LBU`
* [KUICK][PARSER] Implement `LH`
* [KUICK][PARSER] Implement `LHU`
* [KUICK][PARSER] Implement `LW`
* [KUICK][PARSER] Implement `LWU`
* [KUICK][PARSER] Implement `LD`

## Done 2024/05/04
* [IO][External] ELF64 Parser (Headers)

## Done 2025/06/16
### Next Commit (Eforen)
* [Builds] Make build buildable on linux

# Working On
## Working on (Eforen)
* [IO][External] ELF64 Parser (Program Headers)
* [IO][External] ELF64 Parser (Sections)
* [IO][External] ELF64 Writer (Headers)
* [IO][External] ELF64 Writer (Program Headers)
* [IO][External] ELF64 Writer (Sections)
* [KIUCK][PARSER] Implement Pseudo Instructions
* [KIUCK][PARSER][TEST] Pseudo Instruction `beqz`
* [KIUCK][PARSER][TEST] Pseudo Instruction `bnez`
* [KIUCK][PARSER][TEST] Pseudo Instruction `blez`
* [KIUCK][PARSER][TEST] Pseudo Instruction `bgez`
* [KIUCK][PARSER][TEST] Pseudo Instruction `bltz`
* [KIUCK][PARSER][TEST] Pseudo Instruction `bgtz`
* [KANBAN][KIUCK][PARSER] Add Tasks for remaining Pseudo Instruction implementations
* Confirm that all the tests for the previous Pseudo Instruction implementations exist (Rushed atm)

* Refactor KUICK into its own library
* [KUICK][ASSEMBLER] Rewrite tests to take in AST and output binary
* [KUICK][ASSEMBLER] Write Tests for every Token

# Todos
## Todo
* [KUICK][PARSER] Implement `%hi(msg)`
* [KUICK][PARSER] Implement `%lo(msg)`
* [KUICK][PARSER] Implement `msg: .string "Hello World\n"`
* [KUICK][ASSEMBLER] Implement all failing tests
* Implement all RISC-V ASM directives in (RISC-V ASSEMBLY LANGUAGE Programmer Manual - Part 1)[https://shakti.org.in/docs/risc-v-asm-manual.pdf] (contact @ shakti[dot]iitm[@]gmail[dot]com)
* [KUICK][PARSER] Write Test for F Type Instruction `FADD.S`
* [KUICK][PARSER] Write Test for F Type Instruction `FMUL.D`
* [KUICK][PARSER] Write Test for RV32I/E Instruction Set Instruction `LW`
* [KUICK][PARSER] Write Test for RV64I/E Instruction Set Instruction `ADDIW`
* [KUICK][PARSER] Write Test for RV64I/E Instruction Set Instruction `LD`
* [KUICK][PARSER] Write Test for RV64I/E Instruction Set Instruction `SD`
* [Cite] Email SHAKTI Development Team requesting to include their PDF in this repository (RISC-V ASSEMBLY LANGUAGE Programmer Manual - Part 1)[https://shakti.org.in/docs/risc-v-asm-manual.pdf]
## Planned for some time later
## Wishlist