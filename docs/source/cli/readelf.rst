.. _cli-readelf:

riscv32-kuick-elf-readelf
=========================

``riscv32-kuick-elf-readelf`` is the KORE **readelf**-style utility. It reads ELF files and prints information in a **KORE-defined** format. The output is **not** meant to match GNU ``readelf`` byte-for-byte; it is intended to be readable and stable for KORE workflows.

New to ELF or ``.o`` files? Read :doc:`../architecture/elf_format` for a beginner-friendly explanation of what is inside an object file and how RISC-V fits in.

Synopsis
--------

.. code-block:: text

   riscv32-kuick-elf-readelf [OPTIONS] <input-path>
   riscv32-kuick-elf-readelf readelf [OPTIONS] <input-path>

You may omit the ``readelf`` subcommand; the same options apply.

Global options
--------------

``--help``
   Show usage and exit. **Note:** On GNU tools, ``-h`` often means “help”; in KORE readelf, **help is only ``--help``** so that ``-h`` remains free for the ELF **file header** option (see below).

``-v`` / ``--version``
   Print version, copyright, and license lines, then exit.

ELF file header (``-h`` / ``--file-header``)
--------------------------------------------

``-h``
   Short form for printing **only** the ELF executable header.

``--file-header``
   Long form with the same meaning as ``-h``.

``--header``
   Accepted as an alias for the same behavior (backwards compatibility).

When any of these flags is set, the tool loads the input ELF file and prints a **single** section describing the ELF file header: magic (``e_ident``), class, endianness, ABI, type, machine, entry point, program and section header table locations and sizes, flags (including RISC-V-specific decoding where applicable), and related fields.

Example:

.. code-block:: bash

   ./riscv32-kuick-elf-readelf --file-header /path/to/object.o

Equivalent:

.. code-block:: bash

   ./riscv32-kuick-elf-readelf -h /path/to/object.o

Other options (summary)
-----------------------

``--include-empty``
   When printing tables (sections, symbols, etc.), include empty tables when applicable.

``--verbose``
   Increase verbosity where supported.

Default behavior
----------------

If you do **not** pass ``-h``, ``--file-header``, or ``--header``, the tool may print additional views (sections, symbols, relocations, string tables) as implemented; see the current ``--help`` output for the exact set.

See also
--------

* :doc:`../architecture/elf_format` — ELF, object files, and RISC-V (what the tool is inspecting)
* :ref:`cli-binutils` — KORE binutils overview and other planned tools
