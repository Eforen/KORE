*******************
Assembly Structural Elements
*******************

Assembly structural elements provide control over symbol visibility, scope, and organization within RISC-V assembly programs. These directives help manage the symbol table and control which symbols are accessible across different compilation units.

Symbol Visibility Directives
=============================

The KORE KUICK assembler supports directives for controlling symbol visibility and scope within assembly programs. These directives manage how symbols are exposed in the symbol table and their accessibility across different compilation units.

.global Directive
-----------------

The ``.global`` directive (also spelled ``.globl``) makes a symbol visible to other compilation units by adding it to the global symbol table scope.

**Syntax:**

.. code-block:: asm

    .global symbol_name
    .globl symbol_name

**Parameters:**

.. list-table:: .global Parameters
    :widths: 25 75
    :header-rows: 1

    * - Parameter
      - Description
    * - symbol_name
      - The name of the symbol to be made globally visible

**Description:**

The ``.global`` directive instructs the assembler to emit the specified symbol to the global symbol table, making it accessible from other compilation units during the linking phase. This is essential for creating functions, variables, and labels that need to be referenced across multiple source files.

**Usage Examples:**

.. code-block:: asm

    # Make a function globally accessible
    .global main
    main:
        addi x1, x0, 42
        ret

    # Make a data label globally accessible
    .global message
    message:
        .string "Hello, World!"

    # Multiple symbols can be declared global
    .global func1
    .global func2
    .global data_array

**Symbol Table Impact:**

When a symbol is declared with ``.global``, the assembler:

1. Adds the symbol to the **GLOBAL** scope in the symbol table
2. Makes the symbol available for external linkage
3. Allows other compilation units to reference this symbol
4. Enables the linker to resolve cross-module symbol references

.local Directive
----------------

The ``.local`` directive restricts a symbol's visibility to the current compilation unit by adding it to the local symbol table scope.

**Syntax:**

.. code-block:: asm

    .local symbol_name

**Parameters:**

.. list-table:: .local Parameters
    :widths: 25 75
    :header-rows: 1

    * - Parameter
      - Description
    * - symbol_name
      - The name of the symbol to be kept locally scoped

**Description:**

The ``.local`` directive instructs the assembler to emit the specified symbol to the local symbol table, restricting its visibility to the current compilation unit. This prevents the symbol from being accessible during external linkage and helps avoid naming conflicts across different source files.

**Usage Examples:**

.. code-block:: asm

    # Keep a helper function local to this file
    .local helper_function
    helper_function:
        addi x2, x1, 1
        ret

    # Keep internal data structures local
    .local internal_buffer
    internal_buffer:
        .zero 256

    # Local constants
    .local INTERNAL_CONSTANT
    INTERNAL_CONSTANT = 0x1000

**Symbol Table Impact:**

When a symbol is declared with ``.local``, the assembler:

1. Adds the symbol to the **LOCAL** scope in the symbol table
2. Restricts the symbol to internal linkage only
3. Prevents other compilation units from referencing this symbol
4. Helps avoid symbol name collisions across different modules

Symbol Scope Behavior
======================

The KORE assembler maintains a multi-scope symbol table system that manages both global and local symbol visibility:

**Default Behavior:**

- Symbols without explicit scope directives are treated according to their context
- Labels and functions are typically local by default unless explicitly made global
- Data symbols may have different default behaviors depending on the section

**Scope Precedence:**

.. list-table:: Symbol Scope Precedence
    :widths: 30 70
    :header-rows: 1

    * - Scope Type
      - Visibility and Linkage
    * - Global (``.global``)
      - Visible across all compilation units, external linkage
    * - Local (``.local``)
      - Visible only within current compilation unit, internal linkage

**Best Practices:**

1. **Explicitly declare scope** for all symbols that will be referenced externally
2. **Use ``.local``** for internal helper functions and private data structures
3. **Use ``.global``** for public API functions and shared data
4. **Consistent naming** helps avoid conflicts between local symbols in different files

**Example Program Structure:**

.. code-block:: asm

    # Public API function
    .global calculate_result
    calculate_result:
        call .local_helper
        ret

    # Private helper function
    .local local_helper
    local_helper:
        addi x1, x0, 100
        ret

    # Shared data
    .global shared_data
    shared_data:
        .word 0x12345678

    # Private data
    .local private_buffer
    private_buffer:
        .zero 64

**Integration with Symbol Table:**

The KORE assembler's Program Node maintains a multi-scope symbol table that tracks:

- **Global scope**: Symbols accessible across compilation units
- **Local scope**: Symbols restricted to the current compilation unit
- **Section context**: Current active section (`.text`, `.data`, `.rodata`, `.bss`)
- **Symbol metadata**: Type information, size, and alignment requirements

This symbol table system enables proper linking behavior and helps prevent common assembly programming errors related to symbol visibility and scope management.

Plain English
-------------

**Why do I care about .local when I can just use a label?**

Think of assembly labels like street addresses (:term:`symbol names`). When you create a label in your code, it's like putting up a street sign that anyone can see and use to find your house. By default, these "addresses" (:term:`symbol`) are visible to everyone in your entire project (:term:`global scope`).

**The Problem:**

Imagine you're working on a big project with multiple files (:term:`compilation unit`), and you create a helper function called ``cleanup`` in your file. But your teammate also creates a function called ``cleanup`` in their file. When the :term:`linker` tries to put everything together, it gets confused - which ``cleanup`` function should it use? This causes a naming conflict (:term:`symbol collision`).

**The Solution:**

The ``.local`` directive is like putting up a sign that says "This address is only for people in this building" (:term:`file scope`). It keeps your symbols private to your file (:term:`local linkage`), preventing conflicts with other files (:term:`external linkage` conflicts).

**Real World Example:**

.. code-block:: asm

    # Without .local - this could conflict with other files
    helper_function:
        addi x1, x0, 42
        ret

    # With .local - this stays private to this file
    .local helper_function
    helper_function:
        addi x1, x0, 42
        ret

**When to use each:**

- **Use regular labels** for things other files need to access (:term:`external linkage`) - like your main function
- **Use ``.local``** for internal helper functions, temporary labels, or anything that's just for your file (:term:`internal linkage`)
- **Use ``.global``** when you explicitly want to share something with other files (:term:`export symbols`)

Think of it like the difference between your home address (global :term:`symbol` - :term:`external linkage`) and your bedroom (local :term:`symbol` - :term:`internal linkage`). The ``.local`` directive helps you keep your internal "rooms" (:term:`private symbols`) private while still allowing access to your public "front door" (:term:`public API`).

**If all labels are global by default, why would I ever use .global?**

Great question! Even though the KORE KUICK assembler currently makes all labels globally visible by default, there are several important reasons to explicitly use ``.global``:

**Future Compatibility:**

The KORE project may add compiler configuration options that change the default behavior. We're considering adding a compiler directive that would make labels local by default (like many other assemblers do). If you explicitly mark your symbols with ``.global``, your code will continue to work regardless of future default changes.

**Good Programming Habits:**

Being explicit about your intentions makes your code more readable and maintainable. When someone reads your code and sees ``.global main``, they immediately know that ``main`` is meant to be accessible from other files. Without the ``.global``, they have to remember or look up what the default behavior is.

**External Compatibility:**

If you ever want to compile your KORE assembly code with other RISC-V assemblers (like GNU's ``as`` or LLVM's assembler), many of them make labels local by default. Your code will be more portable if you explicitly declare which symbols should be global.

**Team Development:**

When working with a team, explicit declarations prevent confusion. New team members don't need to memorize the default behavior - they can see your intentions directly in the code.

**Real World Example:**

.. code-block:: asm

    # This works in KUICK but might not work elsewhere
    main:
        addi x1, x0, 42
        ret

    # This is explicit and works everywhere
    .global main
    main:
        addi x1, x0, 42
        ret

**Professional Standards:**

Most professional assembly code explicitly declares symbol visibility. Following this convention makes your code look more professional and easier for experienced developers to understand.

Think of it like using turn signals when driving - even if the road is empty, it's good practice that becomes an unconscious automatic habit and helps when the situation is more complex and your focus is on other things than your blinkers.

**Integration with Symbol Table:**

The KORE assembler's Program Node maintains a multi-scope symbol table that tracks:

- **Global scope**: Symbols accessible across compilation units
- **Local scope**: Symbols restricted to the current compilation unit
- **Section context**: Current active section (`.text`, `.data`, `.rodata`, `.bss`)
- **Symbol metadata**: Type information, size, and alignment requirements

This symbol table system enables proper linking behavior and helps prevent common assembly programming errors related to symbol visibility and scope management.

