PROGRAM [4] Symbols:[1]{
    SYMBOL TABLE {
        Symbol[1]: LOCAL msg, SECTION: .rodata, OFFSET: 0, TYPE: LABEL, REF_COUNT: 2
    }
    SECTION .text [2]{
        INLINE DIRECTIVE PCREL_HI LABEL:msg {
            TypeB auipc RD:x5 IMM:0
        }
        INLINE DIRECTIVE PCREL_LO LABEL:msg {
            TypeI addi RD:x5 RS:x5 IMM:0
        }
    }
    SECTION .data [1]{
        DIRECTIVE .word INT:1
    }
    SECTION .rodata [2]{
        LABEL msg
        DIRECTIVE .string STRING:"x"
    }
    SECTION .bss [3]{
        COMMENT # Intentional: .word in .bss is uncommon in GNU as (people often use .space/.zero), but we keep it
        COMMENT # here because firmware/BIOS or hand-written blobs sometimes emit initialized-sized slots this way.
        DIRECTIVE .word INT:0
    }
}
