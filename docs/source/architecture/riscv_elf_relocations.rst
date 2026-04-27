.. _riscv-elf-relocations:

RISC-V ELF relocations (reference)
===================================

This page summarizes **RISC-V ELF relocation types** (``r_type`` values) as used in ``.o`` / shared objects. Authoritative definitions are in the **RISC-V ELF psABI** (same numbering as GNU ``include/elf/riscv.h`` and LLVM ``ELFRelocs/RISCV.def``). KORE mirrors these in ``Kore.RiscMeta.RiscvRelocationType`` and in ``RiscvRelocationArithmetic`` / ``Relocation.ApplyToInstruction32`` for teaching and tooling.

.. contents:: On this page
   :local:
   :depth: 2

Notation (psABI)
----------------

.. list-table::
   :widths: 12 40
   :header-rows: 1

   * - Symbol
     - Meaning
   * - ``S``
     - Value of the symbol referenced by the relocation.
   * - ``A``
     - Addend (from ``r_addend`` or implicit in the instruction slot).
   * - ``P``
     - Address of the relocation (where the patch is applied).
   * - ``B``
     - Load address of the shared object (dynamic).
   * - ``G``, ``GOT``
     - GOT offset / GOT base for ``-fPIC`` access.
   * - ``TP``
     - Thread pointer (TLS local-exec / IE).
   * - ``V``
     - Value already at the patch location (local label / relaxation fragments).

Word and dynamic relocations
----------------------------

``R_RISCV_NONE`` (0)
   No effect.

``R_RISCV_32`` (1)
   32-bit absolute word: **result = S + A**.

``R_RISCV_64`` (2)
   64-bit absolute word: **result = S + A**.

``R_RISCV_RELATIVE`` (3)
   Dynamic: **result = B + A** (adjust link address to load address).

``R_RISCV_COPY`` (4)
   Copy relocation in the executable; resolved by the dynamic loader (not a static constant).

``R_RISCV_JUMP_SLOT`` (5)
   PLT slot: **result = S** (address of the resolved symbol).

``R_RISCV_TLS_DTPMOD32`` / ``R_RISCV_TLS_DTPMOD64`` (6–7)
   General-dynamic TLS: module id for the symbol’s TLS block (runtime).

``R_RISCV_TLS_DTPREL32`` / ``R_RISCV_TLS_DTPREL64`` (8–9)
   **result = S + A - TLS_DTV_OFFSET** (DTP-relative).

``R_RISCV_TLS_TPREL32`` / ``R_RISCV_TLS_TPREL64`` (10–11)
   **result = S + A + TLSOFFSET** (TP-relative, dynamic form).

``R_RISCV_TLSDESC`` (12)
   Dynamic TLS descriptor; resolved at load time.

Control-flow (static)
---------------------

``R_RISCV_BRANCH`` (16)
   B-type immediate: offset **S + A - P** (bytes, 2-byte aligned, 13-bit signed range).

``R_RISCV_JAL`` (17)
   J-type immediate: offset **S + A - P** (21-bit signed range, 2-byte aligned).

``R_RISCV_CALL`` (18) / ``R_RISCV_CALL_PLT`` (19)
   ``auipc`` + ``jalr`` call sequence: **same field math** as ``R_RISCV_PCREL_HI20`` on the ``auipc`` and ``R_RISCV_PCREL_LO12_I`` on the ``jalr`` (PLT variant uses PLT symbol semantics).

PC-relative address building (PIC)
-----------------------------------

``R_RISCV_GOT_HI20`` (20), ``R_RISCV_TLS_GOT_HI20`` (21), ``R_RISCV_TLS_GD_HI20`` (22)
   U-type (``auipc``): high 20 bits of **G + GOT + A - P** (TLS variants use TLS GOT entries).

``R_RISCV_PCREL_HI20`` (23)
   U-type (``auipc``): high 20 bits of **S + A - P**: ``((delta + 0x800) >> 12)`` with ``delta = S + A - P`` (signed 20-bit field).

``R_RISCV_PCREL_LO12_I`` (24) / ``R_RISCV_PCREL_LO12_S`` (25)
   I-type / S-type: low **12** bits (signed) of **S + A - P_hi**, where **P_hi** is the PC of the paired ``auipc``.

Absolute address building (medlow)
----------------------------------

``R_RISCV_HI20`` (26)
   U-type (``lui``): high 20 bits of RV32 **S + A**, split so ``(sign_extend(hi)<<12) + sign_extend(lo12) == (S+A)``.

``R_RISCV_LO12_I`` (27) / ``R_RISCV_LO12_S`` (28)
   Low 12 bits of **S + A** (signed) for ``addi`` / loads / stores after ``lui``.

TLS local-exec / descriptors (overview)
---------------------------------------

``R_RISCV_TPREL_HI20`` / ``TPREL_LO12_*`` / ``TPREL_ADD`` (29–32)
   Thread-pointer-relative layout: same **HI20/LO12 split** pattern as absolute, applied to **(S - TP + A)**-style values per psABI pairing.

``R_RISCV_TLSDESC_HI20`` (62), ``TLSDESC_LOAD_LO12`` (63), ``TLSDESC_ADD_LO12`` (64), ``TLSDESC_CALL`` (65)
   Descriptor protocol: HI/LO PC-relative to descriptor; ``CALL`` annotates the resolver branch for relaxation.

Add / subtract fragments
------------------------

``R_RISCV_ADD8`` … ``R_RISCV_ADD64`` (33–36)
   **V + S + A** on 8/16/32/64-bit fields.

``R_RISCV_SUB8`` … ``R_RISCV_SUB64`` (37–40)
   **V - S - A**.

``R_RISCV_GOT32_PCREL`` (41)
   Word: **G + GOT + A - P**.

Linker metadata and compressed
-------------------------------

``R_RISCV_ALIGN`` (43)
   Linker-only: alignment padding for relaxation; **addend** describes padding size.

``R_RISCV_RVC_BRANCH`` (44) / ``R_RISCV_RVC_JUMP`` (45)
   Compressed encodings: **S + A - P** in CB/CJ immediate ranges.

``R_RISCV_RELAX`` (51)
   Paired hint at the **same** offset as a real relocation; linker may shrink instructions.

``R_RISCV_SUB6`` (52), ``R_RISCV_SET6`` … ``SET32`` (53–56)
   Local label / narrow-field assignment and subtraction.

More words and PLT
------------------

``R_RISCV_32_PCREL`` (57)
   Word: **S + A - P**.

``R_RISCV_IRELATIVE`` (58)
   Dynamic ifunc: **ifunc_resolver(B + A)**.

``R_RISCV_PLT32`` (59)
   Word: **S + A - P** (offset to target or PLT).

ULEB128
-------

``R_RISCV_SET_ULEB128`` (60) / ``R_RISCV_SUB_ULEB128`` (61)
   Paired ULEB128 updates: **S + A** vs **V - S - A** per psABI pairing rules.

Vendor
------

``R_RISCV_VENDOR`` (191)
   Must appear immediately before a vendor-specific relocation; identifies the vendor.

KORE implementation
---------------------

* **Enum:** ``Kore.RiscMeta.RiscvRelocationType``
* **Closed-form math (tests + tools):** ``Kore.RiscMeta.RiscvRelocationArithmetic`` — static helpers matching the psABI “Calculation” column where a closed form exists.
* **32-bit instruction patching:** ``Relocation.ApplyToInstruction32`` (subset of types; TLS/linker-only kinds are documented-only or need extra runtime parameters).
* **Regression / teaching tests:** ``src/Kore.Kuick.Tests/RiscvRelocationTypeFormulaTests.cs`` — one NUnit test per enum member, with comments showing numeric examples.

See also
--------

* :doc:`elf_format` — ELF mental model for KORE
* :doc:`../development/kuick_elf_library` — Kuick.Elf library
* RISC-V ELF psABI (official relocation table)
