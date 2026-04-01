# KORE readelf CLI test fixture.
# The undefined call produces relocations, PLT, and .got.plt when linked as a shared object (as + ld.lld -shared).
# Regenerate Golden/*.txt after changing this file (see scripts/regenerate-readelf-golden.sh).
.text
.globl readelf_test_entry
readelf_test_entry:
    call readelf_undefined_reloc_target
