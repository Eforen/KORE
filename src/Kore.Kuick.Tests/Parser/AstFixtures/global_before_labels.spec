PROGRAM [1] Symbols:[2]{
    SYMBOL TABLE {
        Symbol[1]: GLOBAL entry, SECTION[0], OFFSET: 0, TYPE: LABEL, REF_COUNT: 0
        Symbol[2]: LOCAL after_entry, SECTION[0], OFFSET: 4, TYPE: LABEL, REF_COUNT: 0
    }
    SECTION[0] .text [5]{
        SYMBOL_DIRECTIVE .global entry -> Symbol[1]
        LABEL entry
        TypeI addi RD:x0 RS:x0 IMM:0
        LABEL after_entry
        TypeI addi RD:x0 RS:x0 IMM:0
    }
}
