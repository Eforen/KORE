PROGRAM [1] Symbols:[2]{
    SYMBOL TABLE {
        Symbol[1]: GLOBAL main, SECTION[UNKNOWN], OFFSET: UNKNOWN, TYPE: UNKNOWN, REF_COUNT: 0
        Symbol[2]: GLOBAL other_sym, SECTION[UNKNOWN], OFFSET: UNKNOWN, TYPE: UNKNOWN, REF_COUNT: 0
    }
    PREAMBLE {
        COMMENT # Preamble fixture: comments and file-level directives before .text
        DIRECTIVE .file STRING:"preamble_fixture.S"
        SYMBOL_DIRECTIVE .globl main -> Symbol[1]
        SYMBOL_DIRECTIVE .global other_sym -> Symbol[2]
    }
    SECTION[0] .text [1]{
        TypeI addi RD:x0 RS:x0 IMM:0
    }
}
