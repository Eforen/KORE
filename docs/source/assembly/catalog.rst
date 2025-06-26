********************
Assembly Directive Catalog
********************

This document provides a comprehensive catalog of all supported assembly directives in the KORE RISC-V assembler.

Symbol Visibility Directives
=============================

.global / .globl
-----------------

**Purpose:** Make symbols visible across compilation units

**Syntax:**

.. code-block:: asm

    .global symbol_name
    .globl symbol_name    # Alternative spelling

**Description:**

The ``.global`` directive (also spelled ``.globl``) declares a symbol as globally visible, making it accessible from other compilation units during linking. This is essential for functions and data that need to be shared between different source files.

**Examples:**

.. code-block:: asm

    .global main
    main:
        addi x1, x0, 42
        ret

    .global shared_function
    shared_function:
        # Function implementation
        ret

    .global data_array
    data_array:
        .word 1, 2, 3, 4

.local
------

**Purpose:** Restrict symbol visibility to current compilation unit

**Syntax:**

.. code-block:: asm

    .local symbol_name

**Description:**

The ``.local`` directive restricts a symbol's visibility to the current compilation unit, preventing external access and avoiding naming conflicts across different source files.

**Examples:**

.. code-block:: asm

    .local helper_function
    helper_function:
        addi x2, x1, 1
        ret

    .local internal_buffer
    internal_buffer:
        .zero 256

    .local INTERNAL_CONSTANT
    INTERNAL_CONSTANT = 0x1000

Section Directives
==================

.text
-----

**Purpose:** Switch to the text (code) section

**Syntax:**

.. code-block:: asm

    .text

**Description:**

Switches the current assembly context to the text section, where executable instructions are placed.

.data
-----

**Purpose:** Switch to the data section

**Syntax:**

.. code-block:: asm

    .data

**Description:**

Switches the current assembly context to the data section, where initialized data is placed.

.rodata
-------

**Purpose:** Switch to the read-only data section

**Syntax:**

.. code-block:: asm

    .rodata

**Description:**

Switches the current assembly context to the read-only data section for constants and string literals.

.bss
----

**Purpose:** Switch to the BSS (uninitialized data) section

**Syntax:**

.. code-block:: asm

    .bss

**Description:**

Switches the current assembly context to the BSS section for uninitialized data that will be zero-initialized at program start.

Data Declaration Directives
============================

.word
-----

**Purpose:** Declare 32-bit words

**Syntax:**

.. code-block:: asm

    .word value [, value, ...]

**Examples:**

.. code-block:: asm

    .word 0x12345678
    .word 42, 100, 200
    
.byte
-----

**Purpose:** Declare 8-bit bytes

**Syntax:**

.. code-block:: asm

    .byte value [, value, ...]

**Examples:**

.. code-block:: asm

    .byte 0xFF
    .byte 1, 2, 3, 4

.string / .asciz
----------------

**Purpose:** Declare null-terminated strings

**Syntax:**

.. code-block:: asm

    .string "text"
    .asciz "text"    # Alternative

**Examples:**

.. code-block:: asm

    .string "Hello, World!"
    .asciz "Null-terminated string"

.zero
-----

**Purpose:** Reserve and zero-initialize memory

**Syntax:**

.. code-block:: asm

    .zero size

**Examples:**

.. code-block:: asm

    .zero 64      # Reserve 64 zero bytes
    .zero 1024    # Reserve 1KB of zero memory

Alignment Directives
====================

.align
------

**Purpose:** Align to power-of-2 boundary

**Syntax:**

.. code-block:: asm

    .align power

**Examples:**

.. code-block:: asm

    .align 2      # Align to 4-byte boundary (2^2)
    .align 3      # Align to 8-byte boundary (2^3)

Example Assembly Program
========================

Here's a complete example showing various directives:

.. code-block:: asm

    # Text section with global and local functions
    .text
    .global main
    .local helper

    main:
        addi x1, x0, 10
        jal x1, helper
        ret

    helper:
        addi x1, x1, 1
        ret

    # Data section with various data types
    .data
    .global message
    .local buffer

    message:
        .string "Hello, RISC-V!"

    numbers:
        .word 1, 2, 3, 4, 5

    buffer:
        .zero 256

    # Read-only data section
    .rodata
    .global constants

    constants:
        .word 0x12345678
        .word 42

    PI_APPROX:
        .word 0x40490FDB    # Approximate value of π as float

Implementation Status
=====================

.. list-table:: Directive Implementation Status
    :widths: 30 20 50
    :header-rows: 1

    * - Directive
      - Status
      - Notes
    * - ``.global``
      - ✅ Complete
      - Full symbol table integration
    * - ``.local``
      - ✅ Complete
      - Full symbol table integration
    * - ``.text``
      - ✅ Complete
      - Section switching supported
    * - ``.data``
      - ✅ Complete
      - Section switching supported
    * - ``.rodata``
      - ✅ Complete
      - Section switching supported
    * - ``.bss``
      - ✅ Complete
      - Section switching supported
    * - ``.word``
      - 🚧 Partial
      - Parser support, code generation pending
    * - ``.byte``
      - 🚧 Partial
      - Parser support, code generation pending
    * - ``.string``
      - 🚧 Partial
      - Parser support, code generation pending
    * - ``.zero``
      - 🚧 Partial
      - Parser support, code generation pending
    * - ``.align``
      - ❌ Planned
      - Not yet implemented

For a complete list of planned directives, see the :doc:`../contributing` guide and the KANBAN.md file in the project repository. 