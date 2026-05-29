PROGRAM [4] Symbols:[1]{
    SYMBOL TABLE {
        Symbol[1]: LOCAL msg, SECTION[2], OFFSET: 0, TYPE: LABEL, REF_COUNT: 2
    }
    SECTION[0] .text [2]{
        RELOC R_RISCV_PCREL_HI20 symbol[1]:msg {
            TypeU auipc RD:x5 IMM:0
        }
        RELOC R_RISCV_PCREL_LO12_I symbol[1]:msg {
            TypeI addi RD:x5 RS:x5 IMM:0
        }
    }
    SECTION[1] .data [1]{
        DIRECTIVE .word INT:1
    }
    SECTION[2] .rodata [2]{
        LABEL msg
        DIRECTIVE .string STRING:"x"
    }
    SECTION[3] .bss [2]{
        COMMENT # Intentional: exercise `.zero` in .bss (GNU-style zero-filled reservation).
        DIRECTIVE .zero INT:4
    }
}
