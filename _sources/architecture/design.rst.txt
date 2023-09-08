
*******************
Architecture Design
*******************

.. WARNING:: This is not final and is only the current plans.

.. contents:: Table of Contents

CPU Design
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

+----------+----------+-------------------+-------------------+
| Register | Nickname | Purpose                               |
+          +          +-------------------+-------------------+
|          |          | Primary Purpose   | Secondary Purpose |
+==========+==========+===================+===================+
| x0       | zero     | Hardwired zero                        |
+----------+----------+-------------------+-------------------+
| x1       | ra       | Return Address                        |
+----------+----------+-------------------+-------------------+
| x2       | sp       | Stack Pointer                         |
+----------+----------+-------------------+-------------------+
| x3       | gp       | Global Pointer                        |
+----------+----------+-------------------+-------------------+
| x4       | tp       | Thread Pointer                        |
+----------+----------+-------------------+-------------------+
| x5       | t0       | Temporary                             |
+----------+----------+-------------------+-------------------+
| x6       | t1       | Temporary                             |
+----------+----------+-------------------+-------------------+
| x7       | t2       | Temporary                             |
+----------+----+-----+-------------------+-------------------+
| x8       | s0 | fp  | Saved Register    | Frame Pointer     |
+----------+----+-----+-------------------+-------------------+
| x9       | s1       | Saved Register                        |
+----------+----------+-------------------+-------------------+
| x10      | a0       | Function Argument | Return Value      |
+----------+----------+-------------------+-------------------+
| x11      | a1       | Function Argument | Return Value      |
+----------+----------+-------------------+-------------------+
| x12      | a2       | Function Argument                     |
+----------+----------+-------------------+-------------------+
| x13      | a3       | Function Argument                     |
+----------+----------+-------------------+-------------------+
| x14      | a4       | Function Argument                     |
+----------+----------+-------------------+-------------------+
| x15      | a5       | Function Argument                     |
+----------+----------+-------------------+-------------------+
| x16      | a6       | Function Argument                     |
+----------+----------+-------------------+-------------------+
| x17      | a7       | Function Argument                     |
+----------+----------+-------------------+-------------------+
| x18      | s2       | Saved Register                        |
+----------+----------+-------------------+-------------------+
| x19      | s3       | Saved Register                        |
+----------+----------+-------------------+-------------------+
| x20      | s4       | Saved Register                        |
+----------+----------+-------------------+-------------------+
| x21      | s5       | Saved Register                        |
+----------+----------+-------------------+-------------------+
| x22      | s6       | Saved Register                        |
+----------+----------+-------------------+-------------------+
| x23      | s7       | Saved Register                        |
+----------+----------+-------------------+-------------------+
| x24      | s8       | Saved Register                        |
+----------+----------+-------------------+-------------------+
| x25      | s9       | Saved Register                        |
+----------+----------+-------------------+-------------------+
| x26      | s10      | Saved Register                        |
+----------+----------+-------------------+-------------------+
| x27      | s11      | Saved Register                        |
+----------+----------+-------------------+-------------------+
| x28      | t3       | Temporary                             |
+----------+----------+-------------------+-------------------+
| x29      | t4       | Temporary                             |
+----------+----------+-------------------+-------------------+
| x30      | t5       | Temporary                             |
+----------+----------+-------------------+-------------------+
| x31      | t6       | Temporary                             |
+----------+----------+-------------------+-------------------+