PROGRAM [1] Symbols:[2]{
    SYMBOL TABLE {
        Symbol[1]: LOCAL block, SECTION: .text, OFFSET: 0, TYPE: LABEL, REF_COUNT: 1
        Symbol[2]: LOCAL 1Lblock, SECTION: .text, OFFSET: 4, TYPE: LABEL, REF_COUNT: 1
    }
    SECTION .text [4]{
        LABEL block
        TypeJ jal RD:zero LABEL:block
        LABEL block
        TypeJ jal RD:zero LABEL:block
    }
}
