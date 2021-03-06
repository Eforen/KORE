
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
All though the main registers (excluding IP/SP/BP) are general purpose and can be used for anything, 
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

+----+----+----+----+----+----+----+----+
| 64 | 56 | 48 | 40 | 32 | 24 | 16 |  8 |
+====+====+====+====+====+====+====+====+
|                  R?X                  |
+-------------------+-------------------+
|                   |         E?X       |
+-------------------+---------+---------+
|                             |   ?X    |
+-----------------------------+----+----+
|                             | ?H | ?L |
+-----------------------------+----+----+