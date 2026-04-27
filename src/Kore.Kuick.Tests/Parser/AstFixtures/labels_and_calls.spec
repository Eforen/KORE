PROGRAM [1] Symbols:[4]{
    SYMBOL TABLE {
        Symbol[1]: LOCAL main, SECTION[0], OFFSET: 0, TYPE: LABEL, REF_COUNT: 0
        Symbol[2]: UNKNOWN some_data, SECTION[UNKNOWN], OFFSET: UNKNOWN, TYPE: UNKNOWN, REF_COUNT: 2
        Symbol[3]: UNKNOWN my_function, SECTION[UNKNOWN], OFFSET: UNKNOWN, TYPE: UNKNOWN, REF_COUNT: 2
        Symbol[4]: LOCAL loop, SECTION[0], OFFSET: 20, TYPE: LABEL, REF_COUNT: 2
    }
    SECTION[0] .text [11]{
        LABEL main
        COMMENT # ← label
        RELOC R_RISCV_PCREL_HI20 symbol[2]:some_data {
            TypeB auipc RD:x5 IMM:0
        }
        RELOC R_RISCV_PCREL_LO12_I symbol[2]:some_data {
            TypeI addi RD:x5 RS:x5 IMM:0
        }
        RELOC R_RISCV_PCREL_HI20 symbol[3]:my_function {
            TypeB auipc RD:x1 IMM:0
        }
        RELOC R_RISCV_PCREL_LO12_I symbol[3]:my_function {
            TypeI jalr RD:x1 RS:x1 IMM:0
        }
        TypeJ jal RD:x1 LABEL:loop
        LABEL loop
        COMMENT # ← label
        TypeI addi RD:x5 RS:x5 IMM:1
        TypeJ jal RD:x0 LABEL:loop
    }
}
