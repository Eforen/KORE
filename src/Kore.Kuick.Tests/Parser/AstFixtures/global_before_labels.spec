PROGRAM [1] Symbols:[2]{
    SYMBOL TABLE {
        Symbol[1]: GLOBAL entry, SECTION: .text, OFFSET: 0, TYPE: LABEL, REF_COUNT: 0
        Symbol[2]: LOCAL after_entry, SECTION: .text, OFFSET: 4, TYPE: LABEL, REF_COUNT: 0
    }
    SECTION .text [5]{
        SYMBOL_DIRECTIVE .global entry -> Symbol[1]
        LABEL entry
        TypeI addi RD:zero RS:zero IMM:0
        LABEL after_entry
        TypeI addi RD:zero RS:zero IMM:0
    }
}
