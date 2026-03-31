.. _development-kuick-elf-library:

Kuick.Elf library
=================

The **Kuick.Elf** assembly is a .NET library for **reading and writing** RISC-V (and generic) **ELF** files inside the KORE solution. Host tools such as **readelf** use it to load object files and print views of the data; other projects can reference it to inspect or emit ELF without shelling out to GNU binutils.

.. admonition:: Background vs API
   :class: tip

   * **Concepts** — what ELF is, what ``.o`` files are, and how RISC-V fits in: :doc:`../architecture/elf_format`.
   * **This page** — namespaces, main types, and how to reference the library from C#.
   * **Command-line** — :doc:`../cli/binutils` and :doc:`../cli/readelf` (the readelf tool is built on Kuick.Elf).

.. contents:: On this page
   :local:
   :depth: 2

Location and project
--------------------

* **Path:** ``src/Kuick.Elf/Kuick.Elf.csproj``
* **Target framework:** ``net8.0`` (adjust if the project file changes)
* **Solution:** listed as **Kuick.Elf** in ``KorePlatform.sln``
* **Tests:** ``src/Kuick.Elf.Tests/`` (NUnit)

Namespaces (overview)
---------------------

``Kuick.Elf.Models``
   In-memory representation: ``ElfObject`` (aggregate), ``ElfHeader``, ``Section``, ``Symbol``, ``RelocationEntry``. These types describe what you *have* after loading (or what you *want* before writing).

``Kuick.Elf.IO``
   Binary I/O: ``ElfLoader`` reads a path into an ``ElfObject``; ``ElfWriter`` writes an ``ElfObject`` to a path. The loader validates the ELF magic and parses the **executable header** for ELF32 and ELF64; additional tables (sections, symbols, …) are represented on ``ElfObject`` and can be filled as the implementation grows.

``Kuick.Elf.Formatting``
   Text output helpers: ``FormatterOptions``, ``IElfFormatter``, and **ReadelfFormatters** (e.g. header, section, symbol, relocation, string-table formatters) used by the readelf-style CLI to turn an ``ElfObject`` into printable strings without tying core parsing to one console style.

Typical usage (read a file)
-----------------------------

.. code-block:: csharp

   using Kuick.Elf.IO;
   using Kuick.Elf.Models;

   var loader = new ElfLoader();
   ElfObject elf = loader.Load("/path/to/file.o");

   // Header is populated after Load; other lists may be empty until the loader fills them.
   ElfHeader h = elf.Header;  // e_ident, machine, offsets, etc.

Typical usage (write a file)
----------------------------

.. code-block:: csharp

   using Kuick.Elf.IO;
   using Kuick.Elf.Models;

   var myHeader = new ElfHeader { /* Ident, Type, Machine, … */ };
   var obj = new ElfObject { Header = myHeader };
   new ElfWriter().Write(obj, "/path/to/out.o");

How the readelf tool uses it
----------------------------

The **riscv32-kuick-elf-readelf** program (under ``src/Kuick.Tools/binutils/readelf/``) constructs an ``ElfLoader``, calls ``Load`` with the user’s path, then passes the ``ElfObject`` to a small **CLI formatter** layer that delegates to Kuick.Elf’s **ReadelfFormatters** and ``FormatterOptions``. That keeps transport (argv, exit codes) in ``Kuick.Tools`` and ELF semantics in **Kuick.Elf**.

For flags and behavior, see :doc:`../cli/readelf`.

Implementation notes
--------------------

* **Parsing depth** — The loader focuses on a correct **ELF header** parse (including program/section header *offsets and counts* in the header). Populating ``Sections``, ``Symbols``, ``Relocations``, and string tables from the section header table is the natural next step for features like ``readelf -S`` / ``-s``; the **model** types are already present on ``ElfObject`` for that work.
* **Output style** — Formatters under ``Kuick.Elf.Formatting`` are **KORE-specific** text; they are not guaranteed to match GNU readelf.

See also
--------

* :doc:`../architecture/elf_format` — ELF and RISC-V concepts (on-disk format and terminology)
* :doc:`../cli/binutils` — KORE binutils overview (host tools that consume this library)
* :doc:`../cli/readelf` — readelf CLI reference
