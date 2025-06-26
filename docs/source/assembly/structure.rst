Assembly Program Structure
==========================

This document explains how to structure RISC-V assembly programs in KORE, focusing on symbol visibility and program organization.

Symbol Visibility Directives
=============================

KORE assembly supports two primary directives for controlling symbol visibility:

- ``.global`` - Makes symbols visible across compilation units
- ``.local`` - Restricts symbols to the current compilation unit

.global Directive
-----------------

The ``.global`` directive makes a symbol visible to other compilation units, enabling external linkage.

**Syntax:**

.. code-block:: asm

    .global symbol_name

**Parameters:**

- ``symbol_name``: The name of the symbol to make globally visible

**Description:**

When you declare a symbol with ``.global``, it becomes available for use by other source files in your project. This is essential for creating libraries and sharing functions between modules.

**Usage Examples:**

.. code-block:: asm

    # Declare a global function
    .global main
    main:
        addi x1, x0, 42
        ret

    # Declare global data
    .global shared_buffer
    shared_buffer:
        .zero 1024

**When to use .global:**

- Public API functions that other files will call
- Shared data structures
- Entry points (like ``main``)
- Library functions

.local Directive
----------------

The ``.local`` directive restricts a symbol to the current compilation unit, providing internal linkage.

**Syntax:**

.. code-block:: asm

    .local symbol_name

**Parameters:**

- ``symbol_name``: The name of the symbol to make locally visible only

**Description:**

Symbols declared with ``.local`` are only visible within the current source file. This prevents naming conflicts and keeps internal implementation details private.

**Usage Examples:**

.. code-block:: asm

    # Declare a local helper function
    .local helper_function
    helper_function:
        addi x2, x1, 1
        ret

    # Keep internal data structures local
    .local internal_buffer
    internal_buffer:
        .zero 256

**When to use .local:**

- Helper functions used only within the current file
- Internal data structures
- Private constants and variables
- Implementation details that shouldn't be exposed

Symbol Scope Behavior
======================

**Default Behavior:**

- Symbols without explicit scope directives are treated as local by default
- Labels and functions are typically local unless explicitly made global
- This helps prevent accidental symbol conflicts

**Scope Precedence:**

.. list-table:: Symbol Scope Comparison
    :widths: 30 35 35
    :header-rows: 1

    * - Aspect
      - ``.global``
      - ``.local``
    * - Visibility
      - Across all files
      - Current file only
    * - Linkage
      - External
      - Internal
    * - Name Conflicts
      - Can cause conflicts
      - Prevents conflicts

**Best Practices:**

1. **Explicitly declare scope** for all symbols that will be referenced externally
2. **Use ``.local``** for internal helper functions and private data structures
3. **Use ``.global``** for public API functions and shared data
4. **Consistent naming** helps avoid conflicts between local symbols in different files

Example Program Structure
=========================

Here's a well-structured assembly program demonstrating proper use of symbol directives:

.. code-block:: asm

    # File: calculator.s
    
    # Public API function - accessible from other files
    .global calculate_result
    calculate_result:
        call add_numbers
        call multiply_by_factor
        ret

    # Private helper function - only used within this file
    .local add_numbers
    add_numbers:
        add x1, x1, x2
        ret

    # Another private helper
    .local multiply_by_factor
    multiply_by_factor:
        slli x1, x1, 1  # multiply by 2
        ret

    # Shared data - accessible from other files
    .global calculation_result
    calculation_result:
        .word 0

    # Private data - only used within this file
    .local temp_storage
    temp_storage:
        .zero 64

Understanding Symbol Conflicts
===============================

**The Problem:**

When multiple files define symbols with the same name, the linker can't determine which one to use:

.. code-block:: asm

    # File 1: navigation.s
    cleanup:        # No directive = local by default
        # Clean up navigation data
        ret

    # File 2: telemetry.s  
    cleanup:        # No directive = local by default
        # Clean up telemetry data
        ret

This works fine because both ``cleanup`` functions are local by default.

**But this causes problems:**

.. code-block:: asm

    # File 1: navigation.s
    .global cleanup  # Made global
    cleanup:
        ret

    # File 2: telemetry.s  
    .global cleanup  # Made global - CONFLICT!
    cleanup:
        ret

**The Solution:**

Keep internal functions local and only expose what needs to be shared:

.. code-block:: asm

    # File 1: navigation.s
    .global nav_cleanup  # Descriptive global name
    nav_cleanup:
        call nav_internal_cleanup
        ret
    
    .local nav_internal_cleanup  # Private implementation
    nav_internal_cleanup:
        ret

    # File 2: telemetry.s  
    .global tel_cleanup  # Different descriptive global name
    tel_cleanup:
        call tel_internal_cleanup
        ret
    
    .local tel_internal_cleanup  # Private implementation
    tel_internal_cleanup:
        ret

Plain English Explanation
==========================

**Why do I care about .local and .global?**

Think of assembly symbols like street addresses. When you create a label in your code, it's like putting up a street sign. By default, these "addresses" are only visible in your neighborhood (your source file).

**The Problem:**

Imagine you're working on a big project with multiple files, and you create a helper function called ``cleanup`` in your file. But your teammate also creates a function called ``cleanup`` in their file. If both are made global, the linker gets confused - which ``cleanup`` function should it use?

**The Solution:**

- Use ``.local cleanup`` to keep your function private to your file
- Use ``.global api_function`` only for functions other files need to call

**Example:**

.. code-block:: asm

    # Functions other files can call
    .global launch_sequence
    .global abort_sequence
    
    # Private helper functions
    .local check_fuel
    .local validate_systems
    
    launch_sequence:
        call check_fuel      # Private function
        call validate_systems # Private function
        # ... launch logic
        ret

This way, other files can call ``launch_sequence`` but can't accidentally interfere with your internal ``check_fuel`` and ``validate_systems`` functions.

