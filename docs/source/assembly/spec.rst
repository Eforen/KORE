
**********************
Assembly Specification
**********************

For those familiar with x84 or x64 assembly architecture you may find some defrences in the KORE Cpu architecture or assembly.
* Addresses are 32 bits.
* There is direct hardware support for arithmatic and logical operations on 64-bit integers.
* There is direct hardware support for floating point operations
* There are 16 64-bit general purpose registers like many x64 processors (instead of the 8 32-bit registers you often find in x84 CPUs)
* The Register are intended to be used most because they are more preformant.

Data Types
==============================
The registers, memory and operations use the following data types:

+------------------------+--------+--------------+
| Date Type              | Suffix | Size (bytes) |
+========================+========+==============+
| byte                   | b      | 1            |
+------------------------+--------+--------------+
| word                   | w      | 2            |
+------------------------+--------+--------------+
| double word            | l      | 4            |
+------------------------+--------+--------------+
| long word              | l      | 4            |
+------------------------+--------+--------------+
| quad word              |      q |            8 |
+------------------------+--------+--------------+
| single precision float |      s |            4 |
+------------------------+--------+--------------+
| double precision float |      d |            8 |
+------------------------+--------+--------------+

Notes:
* The "suffix" column above shows the letter used by the assembler to specify appropriately-sized variants of instructions.
* The machine is byte-addressed. It is a "little-endian" mechine.
* Addresses are 32 bits. A single step is a single byte thus the address 0x00000003 would refrence the 3rd byte, where 0x00000004 would refrence the 4th byte.

Registers and Stack
==============================

There are 16 Registers if you want to see them check the `Architecture Specification <../architecture/structure.html#map-of-registers>`_, 16 64-bit "general-purpose" registers;
the low-order 32, 16, and 8 bits of each register can be accessed independently under other names, as shown in the `Map of Registers <../architecture/structure.html#map-of-registers>`_

In principle, almost any register can be used to hold any data, but some have special or restricted uses.
Using these extra registers can have unexpected behavior.



Addressing Mode
==============================

Register :math:`R_2 \leftarrow R_2 + R_1`

.. code-block:: kasm

    ADD R1, R2

Direct :math:`Mem[200] \leftarrow Mem[200] + R_1`

.. code-block:: kasm

    ADD R1, [200]

Indirect :math:`Mem[R_1] \leftarrow Mem[R_1] + A`

.. code-block:: kasm

    ADD A, [R1]

Immediate :math:`R_1 \leftarrow R_1 + 69`

.. code-block:: kasm

    ADI 69, R1

Instructions
==============================

Data Transfer Instructions
------------------------------

.. list-table:: Key
    :widths: 10 50
    :header-rows: 1

    * - Placeholder
      - Denotes
    * - s
      - immediate, register, or memory Address
    * - d
      - a register or memory address
    * - r
      - a register
    * - imm
      - immediate

Most transfers use the mov instruction which works between to registers or between registers and a memory address.

.. note::
    Data Transfers from one memory address to another memory address is not supported.
    If you need to do this a register or the stack would need to be used instead.

.. list-table:: Data transfer instructions
    :widths: 50 50
    :header-rows: 0

    * - mov [b|w|l|q] s,d
      - move s to d
    * - movs [bw|bl|bq|wl|wq|lq] s,d
      - move with sign extension
    * - movz [bw|bl|bq|wl|wq] s,d
      - move with zero extension
    * - movabsq imm, r
      - move absolute quad word (64-bits)
    * - pushq s
      - push onto the stack
    * - popq s
      - pop from the stack stack

.. note::
    It may be desirable at a later point in time to make some instructions for direct memory to memory mov ops to increase preformance of memcopy commands if users are using that frequently.

.. warning::
    Remember that the stack must stay 8 byte aligned at all times thus remember to pad your data if you need to push less then 8 byte.

Integer Arithmetic and Logical Operations
-----------------------------------------

Condition Codes
-----------------------------------------

Flow Control Transfers
-----------------------------------------

Floating Point Arithmetic
-----------------------------------------