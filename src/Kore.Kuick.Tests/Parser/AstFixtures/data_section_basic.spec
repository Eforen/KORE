PROGRAM [2] Symbols:[2]{
    SYMBOL TABLE {
        Symbol[1]: LOCAL myword, SECTION: .data, OFFSET: 0, TYPE: LABEL, REF_COUNT: 0
        Symbol[2]: LOCAL strlab, SECTION: .data, OFFSET: 0, TYPE: LABEL, REF_COUNT: 0
    }
    SECTION .text [1]{
        TypeI addi RD:x0 RS:x0 IMM:0
    }
    SECTION .data [5]{
        LABEL myword
        DIRECTIVE .word INT:42
        DIRECTIVE .word INT:4096
        LABEL strlab
        DIRECTIVE .string STRING:"hello"
    }
}
