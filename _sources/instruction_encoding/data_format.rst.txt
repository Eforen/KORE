
*******************
RISC V Data Format
*******************

.. WARNING:: This May not be entirely accurate as it only reflects my current understanding of RISC V

.. contents:: Table of Contents

Instruction
==============================================

You may notice that some of these things could be done in other ways that may be simplier,
they have been done this way to make the architecture have a closer parity with real CPUs.

Purpose
----------------------------------------------
All though the main registers are general purpose and can be used for anything, 
they are designed to fulfill the following purposes:

.. list-table:: General Purpose Registers
    :widths: 25 50
    :header-rows: 1
    :stub-columns: 1

    * - Registers
      - Purpose
    * - A
      - Accumulator
    * - B
      - Base index (for use with arrays)
    * - C
      - Counter (for use with loops, and strings, etc)
    * - D
      - General Data Register
    * - SP
      - Stack pointer for top address of the stack
    * - BP
      - Stack Base pointer for holding the address of the current stack frame
    * - IP
      - Instruction pointer. Holds the program counter (the address of the next instruction).

General Purpose Registers (A, B, C and D)
----------------------------------------------

.. rst-class:: center-align-table-cell
+---------+---------+-----------+---------+---------+---------+---------+-----------+---------+---------+
|   OP    | 31      |  30 - 25  | 24 - 21 |   20    | 19 - 15 | 14 - 12 |  11 - 08  |   07    | 06 - 00 |
+=========+=========+===========+=========+=========+=========+=========+===========+=========+=========+
|  R-TYPE | func7               |        rs2        |   rs1   |  func3  |         rd          | opcode  |
+---------+---------+-----------+---------+---------+---------+---------+-----------+---------+---------+
|  I-TYPE |                imm[11:0]                |   rs1   |  func3  | imm[4:1]  | imm[11] | opcode  |
+---------+---------+-----------+---------+---------+---------+---------+-----------+---------+---------+
|  S-TYPE |      imm[11:5]      |        rs2        |   rs1   |  func3  |      imm[4:0]       | opcode  |
+---------+---------+-----------+---------+---------+---------+---------+-----------+---------+---------+
|  B-TYPE | imm[12] | imm[10:5] |        rs2        |   rs1   |  func3  | imm[4:1]  | imm[11] | opcode  |
+---------+---------+-----------+---------+---------+---------+---------+-----------+---------+---------+
|  U-TYPE |                     imm[31:12]                              |         rd          | opcode  |
+---------+---------+-----------+---------+---------+---------+---------+-----------+---------+---------+
|  J-TYPE | imm[20] |      imm[10:1]      | imm[11] |    imm[19:12]     |         rd          | opcode  |
+---------+---------+-----------+---------+---------+---------+---------+-----------+---------+---------+
|  P-TYPE | This is for pseudoinstructions that are not simple replacements*                            |
+---------+---------+-----------+---------+---------+---------+---------+-----------+---------+---------+
| PR-TYPE | This is for pseudoinstructions that are simple replacements*                                |
+---------+---------+-----------+---------+---------+---------+---------+-----------+---------+---------+

\* RISC-V ISA has pseudo-instructions that are processed in the compiler to allow more complex instructions 
to be implemented without complicating the cpu architecture more then necessary. P-TYPE and PR-TYPE are 
not officially types of RISC-V ISA but they represent the types of pseudoinstructions that are part of 
the ISA by breaking them down into 2 types of instructions Simple and Complex. Simple instructions are 
the ones that do not need any logic applied in the complier just a simple replacement like `NOP` that is 
replaced with `ADDI x0, x0, 0`. Complex instructions are things that require processing in the compiler 
like for example moving around the order of registers or even if its a simple switch like `FOO a4, a6` 
to `BAR a4, a6`.