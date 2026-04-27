PROGRAM [4] Symbols:[8]{
    SYMBOL TABLE {
        Symbol[1]: GLOBAL main, SECTION[0], OFFSET: 0, TYPE: LABEL, REF_COUNT: 0
        Symbol[2]: GLOBAL my_global_data, SECTION[2], OFFSET: 0, TYPE: LABEL, REF_COUNT: 2
        Symbol[3]: LOCAL .Llocal_buffer, SECTION[3], OFFSET: 0, TYPE: LABEL, REF_COUNT: 2
        Symbol[4]: LOCAL my_function, SECTION[0], OFFSET: 36, TYPE: LABEL, REF_COUNT: 2
        Symbol[5]: LOCAL .Lloop, SECTION[0], OFFSET: 44, TYPE: LABEL, REF_COUNT: 2
        Symbol[6]: LOCAL hello_msg, SECTION[1], OFFSET: 0, TYPE: LABEL, REF_COUNT: 0
        Symbol[7]: LOCAL const_value, SECTION[1], OFFSET: 0, TYPE: LABEL, REF_COUNT: 0
        Symbol[8]: LOCAL .global_buffer, SECTION[3], OFFSET: 0, TYPE: LABEL, REF_COUNT: 0
    }
    PREAMBLE {
        COMMENT # example.s - Comprehensive test program for KUICK assembler
        COMMENT # Demonstrates sections, labels, relocations, alignment, and pseudo-instructions
        DIRECTIVE .file STRING:"example.s"
        COMMENT # ====================== GLOBAL SYMBOLS ======================
        SYMBOL_DIRECTIVE .globl main -> Symbol[1]
        SYMBOL_DIRECTIVE .globl my_global_data -> Symbol[2]
        COMMENT # ====================== .text SECTION ======================
    }
    SECTION[0] .text [22]{
        ALIGN 16 BYTES
        LABEL main
        COMMENT # Global function
        RELOC R_RISCV_PCREL_HI20 symbol[2]:my_global_data {
            TypeB auipc RD:x5 IMM:0
        }
        RELOC R_RISCV_PCREL_LO12_I symbol[2]:my_global_data {
            TypeI addi RD:x5 RS:x5 IMM:0
        }
        TypeI lw RD:x6 RS:x5 IMM:0
        RELOC R_RISCV_PCREL_HI20 symbol[3]:.Llocal_buffer {
            TypeB auipc RD:x7 IMM:0
        }
        RELOC R_RISCV_PCREL_LO12_I symbol[3]:.Llocal_buffer {
            TypeI addi RD:x7 RS:x7 IMM:0
        }
        RELOC R_RISCV_PCREL_HI20 symbol[4]:my_function {
            TypeB auipc RD:x1 IMM:0
        }
        RELOC R_RISCV_PCREL_LO12_I symbol[4]:my_function {
            TypeI jalr RD:x1 RS:x1 IMM:0
        }
        TypeI addi RD:x10 RS:x0 IMM:42
        TypeI jalr RD:x0 RS:x1 IMM:0
        LABEL my_function
        COMMENT # Global function
        TypeI addi RD:x5 RS:x5 IMM:1
        TypeJ jal RD:x0 LABEL:.Lloop
        LABEL .Lloop
        COMMENT # Local label (starts with .L)
        TypeI addi RD:x5 RS:x5 IMM:-1
        TypeB bne RS1:x5 RS2:x0 LABEL:.Lloop
        TypeI jalr RD:x0 RS:x1 IMM:0
        COMMENT # ====================== .rodata SECTION ======================
    }
    SECTION[1] .rodata [6]{
        ALIGN 256 BYTES
        LABEL hello_msg
        DIRECTIVE .asciz STRING:"Hello from KUICK!\n"
        LABEL const_value
        DIRECTIVE .word INT:-559038737
        COMMENT # ====================== .data SECTION ======================
    }
    SECTION[2] .data [6]{
        ALIGN 256 BYTES
        LABEL my_global_data
        DIRECTIVE .word INT:100
        DIRECTIVE .word INT:200
        DIRECTIVE .dword STRING:0x123456789ABCDEF0
        COMMENT # ====================== .bss SECTION ======================
    }
    SECTION[3] .bss [8]{
        ALIGN 256 BYTES
        LABEL .Llocal_buffer
        COMMENT # Local label in .bss
        DIRECTIVE .space INT:64
        LABEL .global_buffer
        COMMENT # Global buffer in .bss
        DIRECTIVE .space INT:128
        COMMENT # ====================== End of file ======================
    }
}
