.. _cli-readelf:

riscv32-kuick-elf-readelf
=========================

``riscv32-kuick-elf-readelf`` is the KORE **readelf**-style utility. It reads ELF files and prints information in a **KORE-defined** format. The output is **not** meant to match GNU ``readelf`` byte-for-byte; it is intended to be readable and stable for KORE workflows.

New to ELF or ``.o`` files? Read :doc:`../architecture/elf_format` for a beginner-friendly explanation of what is inside an object file and how RISC-V fits in.

This program uses **Kuick.Elf** to load and format ELF data. For the API (``ElfLoader``, ``ElfObject``, formatters), see :doc:`../development/kuick_elf_library`.

.. list-table:: Implemented options (quick reference)
   :header-rows: 1
   :widths: 28 72

   * - Option
     - Summary
   * - :ref:`--help <readelf-global-options>`
     - Show usage and exit.
   * - :ref:`-v / --version <readelf-global-options>`
     - Print version, copyright, and license, then exit.
   * - :ref:`-h / --file-header / --header <readelf-file-header>`
     - Print **only** the ELF executable (file) header.
   * - :ref:`-l / --program-headers <readelf-program-headers>`
     - Print the **program header table** (segments).
   * - :ref:`-S / --section-headers / --sections <readelf-section-headers>`
     - Print the **section header table** (section descriptors).
   * - :ref:`--include-empty <readelf-other-options>`
     - Include empty tables when applicable.
   * - :ref:`--verbose <readelf-other-options>`
     - Increase verbosity where supported.

Synopsis
--------

.. code-block:: text

   riscv32-kuick-elf-readelf [OPTIONS] <input-path>
   riscv32-kuick-elf-readelf readelf [OPTIONS] <input-path>

You may omit the ``readelf`` subcommand; the same options apply.

.. _readelf-global-options:

Global options
--------------

``--help``
   Show usage and exit. **Note:** On GNU tools, ``-h`` often means “help”; in KORE readelf, **help is only ``--help``** so that ``-h`` remains free for the ELF **file header** option (see below).

``-v`` / ``--version``
   Print version, copyright, and license lines, then exit.

.. _readelf-file-header:

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

.. _readelf-program-headers:

Program headers (``-l`` / ``--program-headers``)
------------------------------------------------

``-l``
   Short form for printing the **program header table** (segment descriptors: loadable regions, interpreter, dynamic linking, etc.).

``--program-headers``
   Long form with the same meaning as ``-l``.

When set **without** ``-h``, only the program-header view is printed (KORE format, not GNU-identical). When combined with ``-h`` / ``--file-header``, the tool prints the ELF file header and then the program headers, and nothing else from the other tables.

Relocatable object files (``.o``) often have **no** program headers (``e_phnum == 0``); in that case the output explains that this is normal. Executables and shared objects usually have one or more ``LOAD`` and other segment types.

Example:

.. code-block:: bash

   ./riscv32-kuick-elf-readelf -l /path/to/a.out

Equivalent:

.. code-block:: bash

   ./riscv32-kuick-elf-readelf --program-headers /path/to/a.out

.. _readelf-section-headers:

Section headers (``-S`` / ``--section-headers`` / ``--sections``)
-----------------------------------------------------------------

``-S``
   Short form for printing the **section header table** (section names, types, offsets, sizes, flags, link/info, etc.).

``--section-headers``
   Long form with the same meaning as ``-S``.

``--sections``
   Alias for ``--section-headers`` (same behavior as GNU ``readelf``).

When set **without** other single-view flags, only the section-header view is printed (KORE format, not GNU-identical). You may combine ``-S`` with ``-h`` and/or ``-l`` to print those views in order; other tables (symbols, relocations, etc.) are omitted when any single-view mode is active.

Example:

.. code-block:: bash

   ./riscv32-kuick-elf-readelf -S /path/to/object.o

Equivalent:

.. code-block:: bash

   ./riscv32-kuick-elf-readelf --section-headers /path/to/object.o

.. _readelf-other-options:

Other options (summary)
-----------------------

``--include-empty``
   When printing tables (sections, symbols, etc.), include empty tables when applicable.

``--verbose``
   Increase verbosity where supported.

Default behavior
----------------

If you do **not** pass any of the “single-view” flags (``-h`` / ``--file-header`` / ``--header``, ``-l`` / ``--program-headers``, ``-S`` / ``--section-headers`` / ``--sections``, or any combination of those), the tool prints a **default** bundle of views (file header, program headers when present, then sections, symbols, etc., as implemented). See ``--help`` for the current list.

See also
--------

* :doc:`../architecture/elf_format` — ELF, object files, and RISC-V (what the tool is inspecting)
* :doc:`../development/kuick_elf_library` — Kuick.Elf library (how this tool loads and formats ELF)
* :ref:`cli-binutils` — KORE binutils overview and other planned tools
