**********************
Assembly Specification
**********************

Welcome to the RISC-V assembly specification for KORE! This document describes the RISC-V instruction set architecture (ISA) that KORE implements through the KUICK assembler.

For those familiar with x86 or x64 assembly architecture, you may find some differences in the KORE CPU architecture and assembly. Don't worry if you've never done assembly before - we'll explain everything!

* Addresses are either 32 bits (RV32) or 64 bits (RV64) - like having house numbers that go up to about 4 billion or 18 quintillion respectively.
* There are 32 general-purpose registers (think of them as super-fast storage boxes).
* The registers are intended to be used most of the time because they are much more performant than memory.
* Unlike x86, you can't do math directly on memory - you have to load data into registers first.

.. note::
   **What's Assembly Language?** Think of assembly as the most basic language your computer understands. It's like giving very specific, step-by-step instructions: "take this number, add that number, put the result here." Every fancy programming language eventually gets translated down to instructions like these.

RISC-V Overview
===============

RISC-V is a real, open-source instruction set architecture used in actual products worldwide. When you learn RISC-V assembly in KORE, you're learning skills that transfer directly to professional embedded development.

**Key RISC-V Characteristics:**

- **32-bit or 64-bit addresses** (RV32I/RV64I base)
- **32 general-purpose registers** (x0-x31)
- **Load-store architecture** - only load/store instructions access memory
- **Fixed 32-bit instruction size** - no variable-length instructions
- **Simple, orthogonal design** - instructions do one thing well
- **Little-endian byte ordering**

.. note::
   **Plain English**: "Load-store architecture" means you can't do math directly on memory. You have to load data into registers first, do your math, then store the result back. This sounds limiting, but it makes the processor much simpler and faster - and your rockets more reliable!

Data Types
==========

The registers, memory, and operations use the following data types:

.. list-table:: RISC-V Data Types
   :widths: 20 15 15 15 35
   :header-rows: 1

   * - Type
     - Size (in bits)
     - Suffix
     - RV32/RV64
     - Description
   * - Byte
     - 8
     - b
     - Both
     - Signed or unsigned byte
   * - Halfword
     - 16
     - h
     - Both
     - Signed or unsigned half-word
   * - Word
     - 32
     - w
     - Both
     - Signed or unsigned word
   * - Doubleword
     - 64
     - d
     - RV64 only
     - 64-bit value (RV64I extension)

Notes:
* The "suffix" column shows the letter used by the assembler to specify different-sized variants of instructions.
* The machine is byte-addressed and "little-endian" (don't worry about what that means for now).
* In RV32: Addresses are 32 bits. In RV64: Addresses are 64 bits, but you can still use 32-bit addressing. (This can be confusing, so we may need to explain this better.)
* A single step is a single byte, so address 0x00000003 would reference the 3rd byte, and 0x00000004 would reference the 4th byte.

.. note::
   **Plain English**: Think of memory like a giant apartment building where each apartment has a number (address). A "byte" is like a single mailbox, a "halfword" is like 2 mailboxes side by side, and so on. The "suffix" is just a way to tell the computer how many mailboxes you want to work with at once.

   **Little-endian** means the least significant byte comes first in memory. If you store the number 0x12345678, it gets stored as 78 56 34 12 in memory bytes. Most modern processors work this way.

Registers
=========

RISC-V has 32 general-purpose registers. In RV32I, each register is 32 bits wide. In RV64I, each register is 64 bits wide. Each register has both a number (x0-x31) and a conventional name that describes its typical use.

.. list-table:: RISC-V Register Map
   :widths: 10 15 15 50
   :header-rows: 1

   * - Number
     - ABI Name
     - Saved?
     - Description
   * - x0
     - zero
     - —
     - Hardwired to 0, writes ignored
   * - x1
     - ra
     - No
     - Return address
   * - x2
     - sp
     - Yes
     - Stack pointer
   * - x3
     - gp
     - —
     - Global pointer
   * - x4
     - tp
     - —
     - Thread pointer
   * - x5
     - t0
     - No
     - Temporary register 0
   * - x6-x7
     - t1-t2
     - No
     - Temporary registers 1-2
   * - x8
     - s0/fp
     - Yes
     - Saved register 0 / Frame pointer
   * - x9
     - s1
     - Yes
     - Saved register 1
   * - x10-x11
     - a0-a1
     - No
     - Function arguments/return values
   * - x12-x17
     - a2-a7
     - No
     - Function arguments
   * - x18-x27
     - s2-s11
     - Yes
     - Saved registers 2-11
   * - x28-x31
     - t3-t6
     - No
     - Temporary registers 3-6

.. note::
   **Plain English**: The "ABI Name" is what you'll likely actually use in your code, but both are available and are actually the same thing. It can get confusing going bach and forth so it is best practice to pick one and stick with it in a file. Often people choose the ABI name. Think of `x0` vs `zero` like how you might say "first street" instead of "1st street" - they mean the same thing, but one is more descriptive.

**Special Register Behaviors:**
In principle, you can use most registers to hold any data, but some have special purposes. Using them for the wrong thing can cause unexpected behavior (like your rocket exploding).

- **x0 (zero)**: Always reads as 0, writes are ignored. Great for comparisons and discarding results!
- **x1 (ra)**: Return address for function calls
- **x2 (sp)**: Stack pointer - points to the top of the stack
- **x8 (s0/fp)**: Can be used as frame pointer for debugging

.. note::
   **Plain English**: The `x0` register is special - it's hardwired to always contain zero, and any writes to it are simply ignored. This makes it perfect for when you need a zero value or when you want to discard a result you don't care about. It's like having a magic box that always contains the number 0. This is incredibly useful! Need to compare something to zero? Use x0. Need to throw away a result you don't care about? Store it in x0 (it'll just disappear).

.. warning::
   Just like you wouldn't use your car's steering wheel as a cup holder, don't use special-purpose registers for random stuff. The `sp` register controls your stack - mess with it carelessly and your program will crash!

Instruction Formats
==================

RISC-V uses six basic instruction formats. All instructions are exactly 32 bits wide, which makes decoding simple and fast.

**R-Type (Register-Register)**

Used for arithmetic operations between registers:

.. code-block:: text

   add rd, rs1, rs2    # rd = rs1 + rs2

**I-Type (Immediate)**

Used for operations with immediate values and loads:

.. code-block:: text

   addi rd, rs1, imm   # rd = rs1 + imm
   lw rd, imm(rs1)     # rd = memory[rs1 + imm]

**S-Type (Store)**

Used for store operations:

.. code-block:: text

   sw rs2, imm(rs1)    # memory[rs1 + imm] = rs2

**B-Type (Branch)**

Used for conditional branches:

.. code-block:: text

   beq rs1, rs2, label # if rs1 == rs2, jump to label

**U-Type (Upper Immediate)**

Used for loading large constants:

.. code-block:: text

   lui rd, imm         # rd = imm << 12

**J-Type (Jump)**

Used for unconditional jumps:

.. code-block:: text

   jal rd, label       # rd = pc + 4, jump to label

Addressing Modes
================

This is how you tell the computer where to find the data you want to work with:

**Register Direct** - Use register contents directly
:math:`t0 \leftarrow t1 + t2`

.. code-block:: asm

   add t0, t1, t2

**Immediate** - Use a number you type directly into the instruction
:math:`t0 \leftarrow t1 + 100`

.. code-block:: asm

   addi t0, t1, 100 # 100 in decimal
   addi t0, t1, 0x64 # 0x64 is 100 in decimal
   addi t0, t1, 0b1100100 # 0b1100100 is 100 in decimal

**Base + Offset** - Load from memory using a base address plus an offset
:math:`t0 \leftarrow Memory[sp + 8]`

.. code-block:: asm

   lw t0, 8(sp) # Load word from memory at address sp + 8 into t0
   lh t0, 16(sp) # Load halfword from memory at address sp + 16 into t0
   lh t0, -16(sp) # Load halfword from memory at address sp - 16 into t0
   lh t0, 0x10(sp) # Also Load halfword from memory at address sp + 16 into t0
   lh t0, -0x10(sp) # Also Load halfword from memory at address sp - 16 into t0


**PC-Relative (for branches and jumps)** - Branch or jump relative to the current program counter
:math:`if (t0 == t1) \rightarrow PC \leftarrow PC + 8`

.. code-block:: asm

   beq t0, t1, loop # Branch to loop if t0 equals t1

That's it! No complex addressing modes like x86's `[base + index*scale + displacement]`.

.. note::
   **Plain English**: Think of "base + offset" like finding an apartment. The base address (`sp`) is like the building address, and the offset (`8`) is like the apartment number. So `8(sp)` means "go to building `sp`, then go to apartment number 8."

.. note::
   Unlike x86 assembly, you can't do math directly on memory locations. If you want to add two numbers that are in memory, you have to load them into registers first, do the math, then store the result back if needed.

   The simplicity is a feature, not a limitation. Simple addressing modes mean the processor can be faster and more power-efficient. For complex address calculations, you just use regular arithmetic instructions.

Core Instructions
================

Integer Arithmetic
------------------

**Register-Register Operations (R-Type)**

.. list-table:: R-Type Instructions
   :widths: 20 30 50
   :header-rows: 1

   * - Instruction
     - Syntax
     - Description
   * - add
     - `add rd, rs1, rs2`
     - rd = rs1 + rs2
   * - sub
     - `sub rd, rs1, rs2`
     - rd = rs1 - rs2
   * - and
     - `and rd, rs1, rs2`
     - rd = rs1 & rs2 (bitwise AND)
   * - or
     - `or rd, rs1, rs2`
     - rd = rs1 | rs2 (bitwise OR)
   * - xor
     - `xor rd, rs1, rs2`
     - rd = rs1 ^ rs2 (bitwise XOR)
   * - sll
     - `sll rd, rs1, rs2`
     - rd = rs1 << rs2 (shift left logical)
   * - srl
     - `srl rd, rs1, rs2`
     - rd = rs1 >> rs2 (shift right logical)
   * - sra
     - `sra rd, rs1, rs2`
     - rd = rs1 >> rs2 (shift right arithmetic)
   * - slt
     - `slt rd, rs1, rs2`
     - rd = (rs1 < rs2) ? 1 : 0 (signed)
   * - sltu
     - `sltu rd, rs1, rs2`
     - rd = (rs1 < rs2) ? 1 : 0 (unsigned)

**Immediate Operations (I-Type)**

.. list-table:: I-Type Instructions
   :widths: 20 30 50
   :header-rows: 1

   * - Instruction
     - Syntax
     - Description
   * - addi
     - `addi rd, rs1, imm`
     - rd = rs1 + imm
   * - andi
     - `andi rd, rs1, imm`
     - rd = rs1 & imm
   * - ori
     - `ori rd, rs1, imm`
     - rd = rs1 | imm
   * - xori
     - `xori rd, rs1, imm`
     - rd = rs1 ^ imm
   * - slli
     - `slli rd, rs1, imm`
     - rd = rs1 << imm
   * - srli
     - `srli rd, rs1, imm`
     - rd = rs1 >> imm (logical)
   * - srai
     - `srai rd, rs1, imm`
     - rd = rs1 >> imm (arithmetic)
   * - slti
     - `slti rd, rs1, imm`
     - rd = (rs1 < imm) ? 1 : 0 (signed)
   * - sltiu
     - `sltiu rd, rs1, imm`
     - rd = (rs1 < imm) ? 1 : 0 (unsigned)

.. note::
   A very common trick is to zero a register by using `xor t0, t0, t0` - anything XOR'd with itself equals zero!

.. note::
   **Plain English**: "Bitwise" operations work on individual bits. Think of each number as a series of on/off switches. AND means "both switches must be on", OR means "at least one switch must be on", and XOR means "exactly one switch must be on, but not both."

Memory Operations (Data Movement Instructions)
----------------------------------------------

These instructions move data around - from memory to registers, from registers to memory, from registers to memory mapped IO, etc.

**Load Instructions (I-Type)**

.. list-table:: Load Instructions
   :widths: 20 30 50
   :header-rows: 1

   * - Instruction
     - Syntax
     - Description
   * - lb
     - `lb rd, imm(rs1)`
     - Load signed byte
   * - lbu
     - `lbu rd, imm(rs1)`
     - Load unsigned byte
   * - lh
     - `lh rd, imm(rs1)`
     - Load signed halfword
   * - lhu
     - `lhu rd, imm(rs1)`
     - Load unsigned halfword
   * - lw
     - `lw rd, imm(rs1)`
     - Load word
   * - lwu
     - `lwu rd, imm(rs1)`
     - Load unsigned word (RV64I only)
   * - ld
     - `ld rd, imm(rs1)`
     - Load doubleword (RV64I only)

**Store Instructions (S-Type)**

.. list-table:: Store Instructions
   :widths: 20 30 50
   :header-rows: 1

   * - Instruction
     - Syntax
     - Description
   * - sb
     - `sb rs2, imm(rs1)`
     - Store byte
   * - sh
     - `sh rs2, imm(rs1)`
     - Store halfword
   * - sw
     - `sw rs2, imm(rs1)`
     - Store word
   * - sd
     - `sd rs2, imm(rs1)`
     - Store doubleword (RV64I only)

.. note::
   **Plain English**: The syntax `imm(rs1)` means "base + offset" addressing. So `lw t0, 8(sp)` loads a word from memory address `sp + 8`. Think of it like "8 bytes above the stack pointer."

   "Sign-extended" means if you load a small negative number, it stays negative when put in a bigger register. "Zero-extended" means the extra space gets filled with zeros. It's like the difference between "-5" staying "-5" versus becoming "000-5".

.. warning::
   Remember that you can't move data directly from one memory location to another. If you need to copy something from one place in memory to another, you have to load it into a register first, then store it to the new location.

Control Flow
-----------

**Branch Instructions (B-Type)**

.. list-table:: Branch Instructions
   :widths: 20 30 50
   :header-rows: 1

   * - Instruction
     - Syntax
     - Description
   * - beq
     - `beq rs1, rs2, label`
     - Branch if rs1 == rs2
   * - bne
     - `bne rs1, rs2, label`
     - Branch if rs1 != rs2
   * - blt
     - `blt rs1, rs2, label`
     - Branch if rs1 < rs2 (signed)
   * - bltu
     - `bltu rs1, rs2, label`
     - Branch if rs1 < rs2 (unsigned)
   * - bge
     - `bge rs1, rs2, label`
     - Branch if rs1 >= rs2 (signed)
   * - bgeu
     - `bgeu rs1, rs2, label`
     - Branch if rs1 >= rs2 (unsigned)

.. note::
   **Plain English**: The comparison instructions are like asking a yes/no question and getting a 1 for "yes" or 0 for "no". The branch instructions are like saying "if this condition is true, jump to that label."

**Jump Instructions**

These instructions change where your program is running - like jumping to different parts of your code.

.. list-table:: Jump Instructions
   :widths: 20 30 50
   :header-rows: 1

   * - Instruction
     - Syntax
     - Description
   * - jal
     - `jal rd, label`
     - Jump and link (rd = pc + 4)
   * - jalr
     - `jalr rd, rs1, imm`
     - Jump and link register

.. note::
   **Plain English**: `jal` is used for function calls. It saves the return address in `rd` (usually `ra`) and jumps to the label. `jalr` is used for returns and indirect calls.

   **Using x0 to Discard Return Address**
   Here's a clever trick: If you don't need the return address (like for unconditional jumps), you can use `x0` as the destination register. Since `x0` always reads as zero and ignores writes, `jal x0, label` effectively becomes a simple jump without saving anything. This is exactly how the `j label` pseudo-instruction works!

   Examples:
   - `jal ra, function` - Call function, save return address in `ra`
   - `jal x0, loop` - Jump to loop, don't save return address (same as `j loop`)
   - `jalr x0, ra, 0` - Jump to address in `ra`, don't save return address (same as `ret`)

Upper Immediate Instructions
---------------------------

.. list-table:: Upper Immediate Instructions
   :widths: 20 30 50
   :header-rows: 1

   * - Instruction
     - Syntax
     - Description
   * - lui
     - `lui rd, imm`
     - Load upper immediate (rd = imm << 12)
   * - auipc
     - `auipc rd, imm`
     - Add upper immediate to PC

.. note::
   **Plain English**: `lui` is used to load large constants. Since immediate values are limited to 12 bits in most instructions, `lui` loads the upper 20 bits, then you can use `addi` to set the lower 12 bits.

RV64I Extensions
---------------

When running in 64-bit mode, RISC-V adds word-sized operations that operate on 32-bit values within 64-bit registers:

.. list-table:: RV64I Word Operations
   :widths: 20 30 50
   :header-rows: 1

   * - Instruction
     - Syntax
     - Description
   * - addw
     - `addw rd, rs1, rs2`
     - rd = (rs1 + rs2)[31:0] (32-bit add, sign-extend result)
   * - subw
     - `subw rd, rs1, rs2`
     - rd = (rs1 - rs2)[31:0] (32-bit subtract, sign-extend result)
   * - sllw
     - `sllw rd, rs1, rs2`
     - rd = (rs1 << rs2[4:0])[31:0] (32-bit shift left)
   * - srlw
     - `srlw rd, rs1, rs2`
     - rd = (rs1[31:0] >> rs2[4:0]) (32-bit logical right shift)
   * - sraw
     - `sraw rd, rs1, rs2`
     - rd = (rs1[31:0] >> rs2[4:0]) (32-bit arithmetic right shift)
   * - addiw
     - `addiw rd, rs1, imm`
     - rd = (rs1 + imm)[31:0] (32-bit add immediate, sign-extend)
   * - slliw
     - `slliw rd, rs1, imm`
     - rd = (rs1 << imm)[31:0] (32-bit shift left immediate)
   * - srliw
     - `srliw rd, rs1, imm`
     - rd = (rs1[31:0] >> imm) (32-bit logical right shift immediate)
   * - sraiw
     - `sraiw rd, rs1, imm`
     - rd = (rs1[31:0] >> imm) (32-bit arithmetic right shift immediate)

.. note::
   **Plain English**: The "W" suffix stands for "Word" (32-bit). These instructions are only available in RV64I and they operate on the lower 32 bits of 64-bit registers, then sign-extend the result to fill the full 64-bit register. This is useful when you want to do 32-bit math even though you're on a 64-bit processor.

Pseudo-Instructions
==================

RISC-V assembly includes many pseudo-instructions that make code more readable. These are translated by the assembler into real instructions.

.. note::
   **Plain English**: Pseudo-instructions are like shortcuts. Writing `mv t0, t1` is clearer than `addi t0, t1, 0`, even though they do the same thing. The assembler handles the translation for you.

.. note::
   **Plain English**: The `nop` pseudo-instruction is like a do-nothing instruction. It's useful when you need a place to put code without actually doing anything.

.. note::
   It's important to note that these are not a part of the actual opcodes of the RISC-V architecture. They are just part of the assembler. They are something it does to make your life easier.
   when you type `nop` in your code, the assembler will actually treat it as if you typed `addi x0, x0, 0` instead.
   Similarly, `mv t0, t1` is actually `addi t0, t1, 0` and `not rd, rs` is actually `xori rd, rs, -1`.

   But more importantly, because they are just a convenience feature of the assembler, at points in the game we may not give you full access to a complete assembler and thus you may not be able to use these pseudo-instructions in those cases early game.
   We do this for a few reasons:
   - Gameplay advancement reasons, we want you to have a progression curve and not just give you full access to the assembler from the start.
   - To make it easier to understand the actual instructions and opcodes.
   - Give you more appritiation for the aid that the assembler is giving you.

.. list-table:: Common Pseudo-Instructions
   :widths: 25 25 50
   :header-rows: 1

   * - Pseudo-Instruction
     - Real Instruction
     - Description
   * - `nop`
     - `addi x0, x0, 0`
     - No operation
   * - `mv rd, rs`
     - `addi rd, rs, 0`
     - Copy register
   * - `not rd, rs`
     - `xori rd, rs, -1`
     - Bitwise NOT
   * - `neg rd, rs`
     - `sub rd, x0, rs`
     - Negate
   * - `negw rd, rs`
     - `subw rd, x0, rs`
     - Negate word (RV64I only)
   * - `sext.w rd, rs`
     - `addiw rd, rs, 0`
     - Sign-extend word (RV64I only)
   * - `seqz rd, rs`
     - `sltiu rd, rs, 1`
     - Set if equal to zero
   * - `snez rd, rs`
     - `sltu rd, x0, rs`
     - Set if not equal to zero
   * - `sltz rd, rs`
     - `slt rd, rs, x0`
     - Set if less than zero
   * - `sgtz rd, rs`
     - `slt rd, x0, rs`
     - Set if greater than zero
   * - `li rd, imm`
     - Various
     - Load immediate
   * - `la rd, label`
     - Various
     - Load address
   * - `j label`
     - `jal x0, label`
     - Jump
   * - `jr rs`
     - `jalr x0, rs, 0`
     - Jump register
   * - `ret`
     - `jalr x0, ra, 0`
     - Return from function
   * - `call label`
     - Various
     - Call function
   * - `tail label`
     - Various
     - Tail call
   * - `beqz rs, label`
     - `beq rs, x0, label`
     - Branch if equal to zero
   * - `bnez rs, label`
     - `bne rs, x0, label`
     - Branch if not equal to zero
   * - `blez rs, label`
     - `bge x0, rs, label`
     - Branch if less than or equal to zero
   * - `bgez rs, label`
     - `bge rs, x0, label`
     - Branch if greater than or equal to zero
   * - `bltz rs, label`
     - `blt rs, x0, label`
     - Branch if less than zero
   * - `bgtz rs, label`
     - `blt x0, rs, label`
     - Branch if greater than zero

.. note::
   **Plain English**: Pseudo-instructions are like shortcuts. Writing `mv t0, t1` is clearer than `addi t0, t1, 0`, even though they do the same thing. The assembler handles the translation for you.

   **Jump Pseudo-Instructions Explained**: The `j` and `jr` pseudo-instructions are particularly clever examples of how RISC-V uses the `x0` register:

   - `j label` becomes `jal x0, label` - Jump to label, but throw away the return address by "saving" it to `x0` (which ignores writes)
   - `jr rs` becomes `jalr x0, rs, 0` - Jump to the address in register `rs`, but don't save a return address
   - `ret` becomes `jalr x0, ra, 0` - Jump to the address in `ra` (return address), don't save anything

   This shows the elegance of RISC-V design - instead of having separate jump and call instructions, there's just one instruction (`jal`/`jalr`) that can do both depending on which register you use for the return address!

Assembly Directives
==================

Directives are instructions to the assembler itself, not the processor. They control how your program is assembled and linked.

**Section Directives**

.. list-table:: Section Directives
   :widths: 20 80
   :header-rows: 1

   * - Directive
     - Description
   * - `.text`
     - Switch to text section (executable code)
   * - `.data`
     - Switch to data section (initialized data)
   * - `.bss`
     - Switch to BSS section (uninitialized data)
   * - `.rodata`
     - Switch to read-only data section

**Symbol Directives**

.. list-table:: Symbol Directives
   :widths: 20 80
   :header-rows: 1

   * - Directive
     - Description
   * - `.global symbol`
     - Make symbol visible to other files
   * - `.local symbol`
     - Keep symbol local to this file

**Data Directives**

.. list-table:: Data Directives
   :widths: 20 80
   :header-rows: 1

   * - Directive
     - Description
   * - `.byte value`
     - Emit 8-bit value
   * - `.half value`
     - Emit 16-bit value
   * - `.word value`
     - Emit 32-bit value
   * - `.dword value`
     - Emit 64-bit value
   * - `.string "text"`
     - Emit null-terminated string
   * - `.ascii "text"`
     - Emit string without null terminator

**Alignment Directives**

.. list-table:: Alignment Directives
   :widths: 20 80
   :header-rows: 1

   * - Directive
     - Description
   * - `.align n`
     - Align to 2^n byte boundary
   * - `.p2align n`
     - Align to 2^n byte boundary (preferred)

.. note::
   **Plain English**: Think of directives as instructions to the person building your program, not instructions for the computer to run. They're like saying "put this code in the main program section" or "reserve some space for this data."

Example Programs
===============

**Simple Addition**

.. code-block:: asm

   .text
   .global _start
   
   _start:
       addi t0, x0, 5      # t0 = 5
       addi t1, x0, 3      # t1 = 3
       add t2, t0, t1      # t2 = t0 + t1 = 8
       # t2 now contains 8

**Simple Loop (Count to 10)**

.. code-block:: asm

   .text
   .global _start
   
   _start:
       addi t0, x0, 0      # counter = 0
       addi t1, x0, 10     # limit = 10
   
   loop:
       beq t0, t1, done    # if counter == limit, exit loop
       addi t0, t0, 1      # counter = counter + 1
       j loop              # jump back to start of loop
   
   done:
       # t0 now contains 10
       nop

**Function Call Example**

.. code-block:: asm

   .text
   .global _start
   
   _start:
       addi a0, x0, 5      # first argument = 5
       addi a1, x0, 3      # second argument = 3
       jal ra, add_func    # call function, save return address
       # result is now in a0
       j done
   
   add_func:
       add a0, a0, a1      # a0 = a0 + a1
       jr ra               # return to caller
   
   done:
       # a0 now contains 8
       nop

**64-bit Example (RV64I)**

.. code-block:: asm

   .text
   .global _start
   
   _start:
       # Load a large 64-bit constant
       lui t0, %hi(0x123456789ABCDEF0)
       addi t0, t0, %lo(0x123456789ABCDEF0)
       
       # Do 32-bit arithmetic that sign-extends to 64-bit
       addiw t1, x0, -1    # t1 = 0xFFFFFFFFFFFFFFFF (sign-extended)
       
       # Do 64-bit arithmetic
       add t2, t0, t1      # Full 64-bit addition
       
       nop

.. note::
   **Plain English**: This shows the basic calling convention. Arguments go in `a0`, `a1`, etc. The return value goes in `a0`. `jal` saves the return address in `ra`, and `jr ra` returns to the caller.

   The 64-bit example shows how to work with large constants and the difference between 32-bit operations (which sign-extend) and full 64-bit operations.

Calling Convention
=================

RISC-V uses a standard calling convention that all programs follow:

**Function Arguments**
   - `a0-a7` (x10-x17): First 8 arguments
   - Additional arguments passed on stack

**Return Values**
   - `a0-a1` (x10-x11): Return values

**Saved Registers**
   - `s0-s11` (x8-x9, x18-x27): Callee must preserve
   - `ra` (x1): Return address
   - `sp` (x2): Stack pointer

**Temporary Registers**
   - `t0-t6` (x5-x7, x28-x31): Caller must save if needed
   - `a0-a7` (x10-x17): Caller must save if needed

.. note::
   **Plain English**: "Callee must preserve" means if a function uses these registers, it must save and restore them. "Caller must save" means if you need these values after a function call, save them before calling.

This specification covers the core RISC-V instruction set that KORE implements. For more advanced features like floating-point operations, atomic instructions, or custom extensions, see the specific documentation sections.

Remember: RISC-V is designed to be simple and consistent. Once you understand the basic patterns, the rest follows naturally.

That's the basics of RISC-V assembly! Don't worry if it seems like a lot - you'll pick it up as you use it. The most important thing to remember is that assembly is just a way of giving very specific, step-by-step instructions to the computer. Start with simple programs and work your way up to more complex ones.

.. note::
   **Remember**: Every high-level programming language eventually gets translated down to instructions like these. When you write `x = y + z` in a language like C, the compiler turns it into something like `add t0, t1, t2`. Assembly just lets you see and control exactly what the computer is doing!