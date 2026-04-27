PROGRAM [1] Symbols:[2]{
    SYMBOL TABLE {
        Symbol[1]: LOCAL block, SECTION[0], OFFSET: 0, TYPE: LABEL, REF_COUNT: 1
        Symbol[2]: LOCAL 1Lblock, SECTION[0], OFFSET: 4, TYPE: LABEL, REF_COUNT: 1
    }
    SECTION[0] .text [4]{
        LABEL block
        TypeJ jal RD:x0 LABEL:block
        LABEL block
        TypeJ jal RD:x0 LABEL:block
    }
}
