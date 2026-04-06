PROGRAM [1] Symbols:[4]{
    SYMBOL TABLE {
        Symbol[1]: LOCAL main, SECTION: .text, OFFSET: 0, TYPE: LABEL, REF_COUNT: 0
        Symbol[2]: UNKNOWN some_data, SECTION: UNKNOWN, OFFSET: UNKNOWN, TYPE: UNKNOWN, REF_COUNT: 2
        Symbol[3]: UNKNOWN my_function, SECTION: UNKNOWN, OFFSET: UNKNOWN, TYPE: UNKNOWN, REF_COUNT: 2
        Symbol[4]: LOCAL loop, SECTION: .text, OFFSET: 20, TYPE: LABEL, REF_COUNT: 2
    }
    SECTION .text [11]{
        LABEL main
        COMMENT # ← label
        INLINE DIRECTIVE PCREL_HI LABEL:some_data {
            TypeB auipc RD:x5 IMM:0
        }
        INLINE DIRECTIVE PCREL_LO LABEL:some_data {
            TypeI addi RD:x5 RS:x5 IMM:0
        }
        INLINE DIRECTIVE PCREL_HI LABEL:my_function {
            TypeB auipc RD:x1 IMM:0
        }
        INLINE DIRECTIVE PCREL_LO LABEL:my_function {
            TypeI jalr RD:x1 RS:x1 IMM:0
        }
        TypeJ jal RD:x1 LABEL:loop
        LABEL loop
        COMMENT # ← label
        TypeI addi RD:x5 RS:x5 IMM:1
        TypeJ jal RD:x0 LABEL:loop
    }
}
