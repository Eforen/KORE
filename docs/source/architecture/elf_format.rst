.. _architecture-elf-riscv:

ELF, object files, and RISC-V
=============================

.. admonition:: Who this page is for
   :class: tip

   You do **not** need any background to start reading. If you already know what compilers and linkers do, you can skip to :ref:`elf-riscv-technical` or skim the :ref:`elf-riscv-parts` section. If words like “ELF” or “``.o``” are new, start at :ref:`elf-big-picture`.

.. contents:: On this page
   :local:
   :depth: 2

.. _elf-big-picture:

The big picture (no prior knowledge)
--------------------------------------

When people write a program, they usually write **text** that humans understand. A **toolchain** (assembler, compiler, linker) turns that text into **bytes** that a **CPU** can run. Those bytes have to live **somewhere**: usually in **files** on disk.

Think of a program as a **story** you want a machine to follow:

- The **source** is the first draft (your ``.s`` assembly or other source).
- An **object file** is like a **chapter** that is finished enough to file away, but the **whole book** (final runnable program) might still combine several chapters.
- The computer also needs a **table of contents**, **page numbers**, and **cross-references** (“see Chapter 3”) so it can find everything. **ELF** is one standard way to pack the bytes *and* that bookkeeping into a single file format.

So: **ELF is not a programming language**. It is a **file format**—an agreed-upon way to arrange machine code, data, and metadata so many tools (assemblers, linkers, debuggers, loaders) can cooperate.

.. _elf-what-is-o:

What is a “``.o``” file?
-------------------------

You might see filenames ending in ``.o`` (the letter **o**, short for **object**).

- An **object file** is **one compiled chunk** of a program: machine instructions and data produced from one source file (or one step of the build), **before** (or **without**) the final step that builds a full executable or library.
- It is **not** “object-oriented programming” (a different use of the word “object”).
- It **is** “an object **artifact**”: a concrete file sitting on disk that the **linker** can combine with other object files.

If you have never seen one: when you build a RISC-V project, the assembler often emits ``something.o``. That file is almost always **ELF** on modern Unix-like systems.

.. _elf-name:

What “ELF” means
----------------

**ELF** stands for **Executable and Linkable Format**.

- **Executable** — the same family of formats can describe a **finished program** you can run (subject to OS rules).
- **Linkable** — it can also describe **pieces** (object files, shared libraries) meant to be **linked** together: the linker stitches pieces and fixes up references.

So ELF is deliberately **flexible**: one format for “chunk” and “whole” and “shared library,” with different **details** in the file header telling the tools which case it is.

.. _elf-riscv-parts:

What is *inside* an ELF file? (simple mental model)
---------------------------------------------------

Imagine a **labeled storage box**:

1. **The label on the outside** — **ELF header**  
   A small block at the beginning of the file that answers basic questions: Is this 32-bit or 64-bit? What **CPU family** is this for? Where in the file do the “tables of contents” start? How big are they?

2. **Chapters** — **sections**  
   The file is divided into **sections** with names like ``.text`` (machine instructions), ``.data`` (initialized data), ``.bss`` (zero-initialized data), ``.rodata`` (read-only constants), and more. Not every file has every section, but the idea is: **different kinds of bytes** live in **different named bins**.

3. **The index** — **symbol table**  
   Human-readable names (like **labels** or **function names**) map to **addresses** or **offsets**. Tools use this to **debug**, to **link**, and to show you meaningful names instead of raw numbers.

4. **Sticky notes that say “fix this later”** — **relocations**  
   When one piece of code refers to an address that is **not known yet** (e.g. another file will define it), the object file records **where** to patch the bytes later and **how** to compute the patch. The **linker** resolves this when it builds the final program.

That is the whole story. Everything else is **detail** on how those pieces are encoded as bytes.

.. admonition:: Tiny glossary (plain language)
   :class: note

   **Section**  
      A named chunk inside the file (code, data, debug info, etc.).

   **Symbol**  
      A **name** tied to a place (function, variable, label).

   **Relocation**  
      A **record** that says “at this offset, fill in the real address later.”

   **Linking**  
      Combining object files (and libraries) and applying relocations to produce an executable or library.

.. _elf-riscv-technical:

RISC-V and ELF (for readers who know the basics)
------------------------------------------------

RISC-V is an **ISA** (instruction set architecture): it defines what instructions mean and how registers work. **ELF** does **not** define RISC-V instructions; it only **carries** RISC-V machine code in a **standard container**.

In the ELF **header**, important fields include:

- **Machine type** — for RISC-V, tools use **EM_RISCV** (numeric value **0xF3** in the ELF header). This tells loaders and tools “the instructions in this file are meant for a RISC-V CPU.”
- **Class** — **ELFCLASS32** vs **ELFCLASS64** (32-bit vs 64-bit addresses and sizes in the file layout).
- **Endianness** — whether multi-byte numbers are stored least-significant-byte first (little-endian) or not.
- **ELF flags** (for RISC-V) — can record **optional ISA extensions** and **ABI** choices (for example compressed instructions, floating-point ABI). The exact bits follow the **RISC-V ELF psABI** documents; your toolchain sets them when it emits the file.

So: **RISC-V tells you what the instructions *are***; **ELF tells you how they are *filed*** in the object file and how the linker should finish the job.

.. admonition:: Relationship to “System V ABI”
   :class: note

   On many Unix-like systems, ELF follows conventions from the **System V ABI**, extended by **processor-specific** supplements (including RISC-V). You do not need those documents to understand the mental model above, but they are the authoritative references for **bit-exact** layout.

.. _elf-kore:

How KORE uses ELF
-----------------

KORE’s toolchain ultimately **targets RISC-V** and produces ELF **object files** and related artifacts. The **Kuick.Elf** library in this repository is responsible for **reading and writing** ELF data in a structured way (headers, sections, symbols, relocations, and so on). The **readelf**-style host tool (`riscv32-kuick-elf-readelf`) is a **friendly viewer** for inspecting those files on your development machine.

For command-line usage, see :doc:`../cli/readelf` and the :doc:`../cli/binutils` overview.

Reading order
-------------

1. **New to the topic** — read from :ref:`elf-big-picture` through :ref:`elf-riscv-parts`.
2. **Comfortable with linkers** — skim the simple sections, read :ref:`elf-riscv-technical`, then use tools and specs for details.
3. **Implementing or debugging** — use this page for orientation, then **ELF** and **RISC-V psABI** references for exact layouts.
