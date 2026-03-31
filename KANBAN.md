How to use KANBAN.md
====================
To use KANBAN.md, you can use the following commands:

Make sure you have a `### Next Commit ({YOUR_NAME})]` line that is inside one of the `## Done {DATE}` sections.
This is where you will put the tasks you have completed but have not yet committed.

NOTE: If there is no `## Done {TODAY'S_DATE}` section, you can create one. If there is a blank `## Done {DATE}` section that is in the past you may delete it.

```markdown
# Done
## Done {TODAY'S_DATE}
### Next Commit ({SOMEONE_ELSE})
- [EXAMPLE] Task completed and committed by someone else.
- [EXAMPLE] Task completed by yourself earlier today and committed.
### Next Commit ({YOUR_NAME})
- [EXAMPLE] Description of the task completed but not yet committed.
```

When you commit your changes move the `- [Next Commit ({YOUR_NAME})]` line to the `## Done {TODAY'S_DATE}` section end of the list.

```markdown
# Done
## Done {TODAY'S_DATE}
### Next Commit ({SOMEONE_ELSE})
- [EXAMPLE] Task completed and committed by someone else.
- [EXAMPLE] Task completed by yourself earlier today and committed.
- [EXAMPLE] Description of the task you completed and just committed.
### Next Commit ({YOUR_NAME})
```

## Working On Stuff

When you start working on a task, you can move it to your own `## Working On ({YOUR_NAME})` section under `# Working On` section [Link](#working-on).
NOTE: You should beable to just alt+arrow move the line to your working on section because the task should ideally already be in the `# Todos` section [Link](#todos).

## Todos Priority
The todos section is actually very straight forward, it is sorted by priority.
The highest priority tasks are at the top of the list.
The lowest priority tasks are at the bottom of the list.

```markdown
# Working On
## Working On ({YOUR_NAME})
- [EXAMPLE] Description of the task you are working on.
```

- [Done](#done)
  - [Done 10/18/2021](#done-10182021)
  - [Done 10/21/2021](#done-10212021)
  - [Done 11/16/2021](#done-11162021)
  - [Done 02/25/2023](#done-02252023)
  - [Done 2023/03/08](#done-20230308)
  - [Done 2023/05/20](#done-20230520)
  - [Done 2023/05/21](#done-20230521)
  - [Done 2023/05/26](#done-20230526)
  - [Done 2024/05/04](#done-20240504)
  - [Done 2025/06/16](#done-20250616)
  - [Done 2025/06/19](#done-20250619)
  - [Done 2025/06/20](#done-20250620)
  - [Done 2025/06/21](#done-20250621)
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
* [Builds] Make build buildable on linux

## Done 2025/06/19
* [IO][External] ELF64 Testing Suite
* [IO][External] ELF64 Parser (Program Headers)
* [IO][External] ELF64 Parser (Sections)
* [IO][External] ELF64 Writer (Headers)
* [IO][External] ELF64 Writer (Program Headers)
* [IO][External] ELF64 Writer (Sections)

## Done 2025/06/20
* [KIUCK][PARSER][TEST] Pseudo Instruction `beqz`
* [KIUCK][PARSER][TEST] Pseudo Instruction `bnez`
* [KIUCK][PARSER][TEST] Pseudo Instruction `blez`
* [KIUCK][PARSER][TEST] Pseudo Instruction `bgez`
* [KIUCK][PARSER][TEST] Pseudo Instruction `bltz`
* [KIUCK][PARSER][TEST] Pseudo Instruction `bgtz`

## Done 2025/06/21
* [KUICK][AST] Maintain Program Node Maintains a Symbols Tables (Multi Scope (Local, Global))
* [KUICK][AST] Symbol Table with Address Assignment in CodeGenerator
* [KUICK][AST] Symbol-based AST Nodes (SymbolReferenceNode, InstructionNodeTypeBSymbol, etc.)
* [KUICK][AST] Symbol Directive Nodes (.global, .local)
* [KUICK][AST] Comprehensive Symbol Table Test Suite (16 tests passing)
* [KUICK][CODEGEN] Symbol Address Assignment and Resolution
* [KUICK][CODEGEN] Multi-pass Assembly with Symbol Cache Miss Handling

## Done 2026/03/31
* [LIB][ELF] new elf reading writing library
* [CLI][READELF] riscv32-kuick-elf-readelf Scaffolding (Basic Command Line Interface)
* [CLI][READELF] Add Versioning system that shows in `--version`
* [CLI][READELF] `-h` / `--file-header` Display the ELF file header info similar to `riscv32-unknown-elf-readelf -h`
* [DOCS][CLI] Create a CLI Documentation Section for CLI Tools used outside of the emulator
* [DOCS][CLI] Create a CLI Documentation Page for binutils with link to the readelf page and placeholders for the other binutils tools
* [DOCS][CLI][READELF] Add Documentation for the Readelf Command Line Interface `-h`
* [BUILD] `make build-docs` runs `docs` container build (`make local` / `Dockerfile.sphinx`) for local preview (`docs/build/html/index.html`)
* [DOCS][ARCHITECTURE][ELF] ELF architecture page (beginner-friendly + RISC-V ELF): `docs/source/architecture/elf_format.rst`
* [DOCS][Development] Kuick.Elf library page + cross-links: `docs/source/development/kuick_elf_library.rst`
### Next Commit (Eforen)

# Working On
## Working on (Eforen)
* [CLI][READELF] Add Help system that shows in `-h` / `--help`
* [CLI][READELF] `-l` / `--program-headers` Display the ELF program headers info similar to `riscv32-unknown-elf-readelf -l`
* [DOCS][CLI][READELF] Add Documentation for the Readelf Command Line Interface `-l`

## Set aside (Eforen)
* [KIUCK][PARSER] Implement Pseudo Instructions
* [KANBAN][KIUCK][PARSER] Add Tasks for remaining Pseudo Instruction implementations
* Confirm that all the tests for the previous Pseudo Instruction implementations exist (Rushed atm)

* Refactor KUICK into its own library
* [KUICK][ASSEMBLER] Rewrite tests to take in AST and output binary
* [KUICK][ASSEMBLER] Write Tests for every Token

# Todos
## Todo
* [CLI][READELF] `-S` / `--section-headers` Display the ELF section headers info similar to `riscv32-unknown-elf-readelf -S`
* [DOCS][CLI][READELF] Add Documentation for the Readelf Command Line Interface `-S`
* [CLI][READELF] `--sections` Simple Alias for `--section-headers`
* [CLI][READELF] `-s` / `--symbols` Display the ELF symbol table info similar to `riscv32-unknown-elf-readelf -s`
* [DOCS][CLI][READELF] Add Documentation for the Readelf Command Line Interface `-s`
* [CLI][READELF] `--syms` Simple Alias for `--symbols`
* [CLI][READELF] `-r` / `--relocations` Display the ELF relocation table info similar to `riscv32-unknown-elf-readelf -r`
* [DOCS][CLI][READELF] Add Documentation for the Readelf Command Line Interface `-r`
* [CLI][READELF] `--relocs` Simple Alias for `--relocations`
* [CLI][READELF] `-d` / `--dynamic-section` Display the ELF dynamic section info similar to `riscv32-unknown-elf-readelf -d`
* [DOCS][CLI][READELF] Add Documentation for the Readelf Command Line Interface `-d`
* [CLI][READELF] `--dynamic` Simple Alias for `--dynamic-section`
* [DOCS][CLI][READELF] Add Documentation for the Readelf Command Line Interface `--dynamic`
* [CLI][READELF] `-V` / `--version-info` Display the ELF version info similar to `riscv32-unknown-elf-readelf -V`
* [DOCS][CLI][READELF] Add Documentation for the Readelf Command Line Interface `-V`
* [CLI][READELF] `-A` / `--arch-specific` Display the ELF architecture specific info similar to `riscv32-unknown-elf-readelf -A`
* [DOCS][CLI][READELF] Add Documentation for the Readelf Command Line Interface `-A`
* [CLI][READELF] `-I` / `--histogram` Display the ELF histogram of bucket list lengths similar to `riscv32-unknown-elf-readelf -I`
* [DOCS][CLI][READELF] Add Documentation for the Readelf Command Line Interface `-I`
* [CLI][READELF] `--got-contents` Display GOT (Global Offset Table) section contents similar to `riscv32-unknown-elf-readelf --got-contents`
* [DOCS][CLI][READELF] Add Documentation for the Readelf Command Line Interface `--got-contents`
* [CLI][READELF] `-a` / `-all` Equivalent to: -h -l -S -s -r -d -V -A -I --got-contents
* [DOCS][CLI][READELF] Add Documentation for the Readelf Command Line Interface `-a`
* [KUICKER] Write simple new 2 pass assembler
* [KUICK][PARSER] Implement Directive `.globl` `(symbol_name)` should emit symbol_name to symbol table (scope GLOBAL) [Symbol Table Foundation Complete]
* [KUICK][PARSER] Implement Directive `.local` `(symbol_name)` should emit symbol_name to symbol table (scope LOCAL) [Symbol Table Foundation Complete] ✅ COMPLETED

* [KUICK][PARSER] Pseudo Instruction `j label` is effectively `jal x0, label` [Symbol Table Foundation Complete]
* [KUICK][PARSER] Pseudo Instruction `jr rs` is effectively `jalr x0, rs, 0` [Symbol Table Foundation Complete]

* [KUICK][PARSER] Implement Text Labels `loop: name` used like `j loop` https://github.com/riscv-non-isa/riscv-asm-manual/blob/main/src/asm-manual.adoc#labels [Symbol Table Foundation Complete]
* [KUICK][PARSER] Implement Numeric Labels `1:` used like `j 1b` or `j 1f` based on if forward or backward refs [Symbol Table Foundation Complete]

* [KUICK] Impliment Directive `.section` `([{.text,.data,.rodata,.bss}])` should emit section (if not present, default .text) and make current
* [KUICK] Impliment Directive `.text` `(emit .text section (if not present) and make current)` should 
* [KUICK] Impliment Directive `.data` `(emit .data section (if not present) and make current)` should 
* [KUICK] Impliment Directive `.rodata` `(emit .rodata section (if not present) and make current)` should 
* [KUICK] Impliment Directive `.bss` `(emit .bss section (if not present) and make current)` should 
* [KUICK] Impliment Directive ` .insn <value>` emit a raw instruction with the given value
* [KUICK] Impliment Directive `.insn <insn_length>, <value>` the same, but also verify that the instruction length has the given value in bytes
* [KUICK] Impliment Directive `.insn <type> <fields>` see: https://github.com/riscv-non-isa/riscv-asm-manual/blob/main/src/asm-manual.adoc#.insn and https://sourceware.org/binutils/docs/as/RISC_002dV_002dFormats.html

* [KUICK] Impliment Directive `.align` `(integer)` should align to power of 2 (alias for .p2align which is preferred - see .align
* [KUICK] Impliment Directive `.p2align` `(p2,[pad_val=0],max)` should align to power of 2
* [KUICK] Impliment Directive `.balign` `(b,[pad_val=0])` should byte align
* [KUICK] Impliment Directive `.file` `("filename")` should emit filename FILE LOCAL symbol table
* [KUICK] Impliment Directive `.comm` `(symbol_name,size,align)` should emit common object to .bss section
* [KUICK] Impliment Directive `.common` `(symbol_name,size,align)` should emit common object to .bss section
* [KUICK] Impliment Directive `.ident` `("string")` should accepted for source compatibility
* [KUICK] Impliment Directive `.size` `(symbol, symbol)` should accepted for source compatibility
* [KUICK] Impliment Directive `.string` `("string")` should emit string
* [KUICK] Impliment Directive `.asciz` `("string")` should emit string (alias for .string)
* [KUICK] Impliment Directive `.equ` `(name, value)` should constant definition
* [KUICK] Impliment Directive `.type` `(symbol, @function)` should accepted for source compatibility
* [KUICK] Impliment Directive `.option` `({arch,rvc,norvc,pic,nopic,relax,norelax,push,pop})` should RISC-V options. Refer to .option for a more detailed description.
* [KUICK] Impliment Directive `.byte` `(expression [, expression]*)` should 8-bit comma separated words
* [KUICK] Impliment Directive `.2byte` `(expression [, expression]*)` should 16-bit comma separated words
* [KUICK] Impliment Directive `.half` `(expression [, expression]*)` should 16-bit comma separated words
* [KUICK] Impliment Directive `.short` `(expression [, expression]*)` should 16-bit comma separated words
* [KUICK] Impliment Directive `.4byte` `(expression [, expression]*)` should 32-bit comma separated words
* [KUICK] Impliment Directive `.word` `(expression [, expression]*)` should 32-bit comma separated words
* [KUICK] Impliment Directive `.long` `(expression [, expression]*)` should 32-bit comma separated words
* [KUICK] Impliment Directive `.8byte` `(expression [, expression]*)` should 64-bit comma separated words
* [KUICK] Impliment Directive `.dword` `(expression [, expression]*)` should 64-bit comma separated words
* [KUICK] Impliment Directive `.quad` `(expression [, expression]*)` should 64-bit comma separated words
* [KUICK] Impliment Directive `.float` `(expression [, expression]*)` should 32-bit floating point values, see Floating-point literals for the value format.
* [KUICK] Impliment Directive `.double` `(expression [, expression]*)` should 64-bit floating point values, see Floating-point literals for the value format.
* [KUICK] Impliment Directive `.quad` `(expression [, expression]*)` should 128-bit floating point values, see Floating-point literals for the value format.
* [KUICK] Impliment Directive `.dtprelword` `(expression [, expression]*)` should 32-bit thread local word
* [KUICK] Impliment Directive `.dtpreldword` `(expression [, expression]*)` should 64-bit thread local word
* [KUICK] Impliment Directive `.sleb128` `(expression)` should signed little endian base 128, DWARF
* [KUICK] Impliment Directive `.uleb128` `(expression)` should unsigned little endian base 128, DWARF
* [KUICK] Impliment Directive `.zero` `(integer)` should zero bytes
* [KUICK] Impliment Directive `.variant_cc` `(symbol_name)` should annotate the symbol with variant calling convention
* [KUICK] Impliment Directive `.attribute` `(name, value)` should RISC-V object attributes, more detailed description see .attribute.
* [KUICK] Impliment Directive `.insn` `(see description)` should emit a custom instruction encoding, see .insn
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
* [KUICK] Impliment Directive `.macro` `(name arg1 [, argn])` should begin macro definition \argname to substitute
* [KUICK] Impliment Directive `.endm` `(end macro definition)` should 
* [Cite] Email SHAKTI Development Team requesting to include their PDF in this repository (RISC-V ASSEMBLY LANGUAGE Programmer Manual - Part 1)[https://shakti.org.in/docs/risc-v-asm-manual.pdf]

## Planned for some time later
## Wishlist