.. _cli-binutils:

KORE Binutils (host)
====================

The **KORE binutils** are command-line tools modeled after the familiar GNU binutils family. They live under ``src/Kuick.Tools/binutils/`` in the repository. The first implemented tool is **readelf** for inspecting ELF objects; other utilities will be added alongside it without reshuffling the tree.

These tools assume you are working with **ELF**-format RISC-V artifacts. If you want a plain-language explanation of what that means (headers, sections, symbols, relocations), read :doc:`../architecture/elf_format` first.

They are built on the **Kuick.Elf** library; for namespaces, loaders, and how to reference the project from your own C# code, see :doc:`../development/kuick_elf_library`.

.. contents::
   :local:
   :depth: 2

Available and planned tools
---------------------------

.. list-table::
   :widths: 25 55 20
   :header-rows: 1

   * - Tool
     - Role
     - Status
   * - :doc:`readelf`
     - Display ELF headers, sections, symbols, relocations, and related views (KORE output style).
     - Available
   * - *objcopy*
     - Copy and translate object files (planned).
     - Placeholder
   * - *objdump*
     - Display object file information and disassembly (planned).
     - Placeholder
   * - *nm*
     - List symbols from object files (planned).
     - Placeholder
   * - *strip*
     - Discard symbols from object files (planned).
     - Placeholder

Building
--------

From the repository root, the readelf binary is built via the Makefile:

.. code-block:: bash

   make build-tools-binutils-readelf CONFIGURATION=Release

Output is written under ``bin/Kuick.Tools/bin/<Configuration>/net8.0/`` as ``riscv32-kuick-elf-readelf`` (see the project settings for the exact layout).

See also
--------

* :doc:`../architecture/elf_format` — ELF, object files, and RISC-V (conceptual background)
* :doc:`../development/kuick_elf_library` — Kuick.Elf .NET API (what these tools use under the hood)
* :ref:`cli-host-tools` — parent section for all host CLI tools
