PROGRAM [5] Symbols:[16]{
    SYMBOL TABLE {
        Symbol[1]: GLOBAL main, SECTION[0], OFFSET: 0, TYPE: LABEL, REF_COUNT: 0
        Symbol[2]: GLOBAL _start, SECTION[0], OFFSET: 0, TYPE: LABEL, REF_COUNT: 0
        Symbol[3]: UNKNOWN _stack_top, SECTION[UNKNOWN], OFFSET: UNKNOWN, TYPE: UNKNOWN, REF_COUNT: 2
        Symbol[4]: UNKNOWN _bss_start, SECTION[UNKNOWN], OFFSET: UNKNOWN, TYPE: UNKNOWN, REF_COUNT: 2
        Symbol[5]: UNKNOWN _bss_end, SECTION[UNKNOWN], OFFSET: UNKNOWN, TYPE: UNKNOWN, REF_COUNT: 2
        Symbol[6]: UNKNOWN 1f, SECTION[UNKNOWN], OFFSET: UNKNOWN, TYPE: UNKNOWN, REF_COUNT: 1
        Symbol[7]: LOCAL 0, SECTION[0], OFFSET: 28, TYPE: LABEL, REF_COUNT: 0
        Symbol[8]: UNKNOWN 0b, SECTION[UNKNOWN], OFFSET: UNKNOWN, TYPE: UNKNOWN, REF_COUNT: 1
        Symbol[9]: LOCAL 1, SECTION[0], OFFSET: 40, TYPE: LABEL, REF_COUNT: 0
        Symbol[10]: LOCAL user_main, SECTION[1], OFFSET: 0, TYPE: LABEL, REF_COUNT: 2
        Symbol[11]: LOCAL 2, SECTION[0], OFFSET: 48, TYPE: LABEL, REF_COUNT: 0
        Symbol[12]: UNKNOWN 2b, SECTION[UNKNOWN], OFFSET: UNKNOWN, TYPE: UNKNOWN, REF_COUNT: 1
        Symbol[13]: LOCAL loop, SECTION[1], OFFSET: 4, TYPE: LABEL, REF_COUNT: 1
        Symbol[14]: LOCAL hello_string, SECTION[2], OFFSET: 0, TYPE: LABEL, REF_COUNT: 2
        Symbol[15]: LOCAL global_counter, SECTION[3], OFFSET: 0, TYPE: LABEL, REF_COUNT: 0
        Symbol[16]: LOCAL buffer, SECTION[4], OFFSET: 0, TYPE: LABEL, REF_COUNT: 0
    }
    PREAMBLE {
        COMMENT # realistic002.S - Simple bare-metal RISC-V startup for your linker script
        COMMENT # This file demonstrates how to work with the linker script you posted
        DIRECTIVE .file STRING:"realistic002.S"
        COMMENT # Make 'main' visible to the linker (required by ENTRY(main) in the linker script)
        SYMBOL_DIRECTIVE .globl main -> Symbol[1]
        SYMBOL_DIRECTIVE .globl _start -> Symbol[2]
        COMMENT # Optional: Put startup code in a special section so it can be placed first
    }
    SECTION[0] .text.init [26]{
        ALIGN 16 BYTES
        COMMENT # Entry point (the linker script's ENTRY(main) will point here)
        LABEL _start
        LABEL main
        COMMENT # Initialize stack pointer (stack is defined at the end of RAM in your linker script)
        RELOC R_RISCV_PCREL_HI20 symbol[3]:_stack_top {
            TypeB auipc RD:x2 IMM:0
        }
        RELOC R_RISCV_PCREL_LO12_I symbol[3]:_stack_top {
            TypeI addi RD:x2 RS:x2 IMM:0
        }
        COMMENT # Zero out .bss section (very common in bare-metal)
        RELOC R_RISCV_PCREL_HI20 symbol[4]:_bss_start {
            TypeB auipc RD:x10 IMM:0
        }
        RELOC R_RISCV_PCREL_LO12_I symbol[4]:_bss_start {
            TypeI addi RD:x10 RS:x10 IMM:0
        }
        RELOC R_RISCV_PCREL_HI20 symbol[5]:_bss_end {
            TypeB auipc RD:x11 IMM:0
        }
        RELOC R_RISCV_PCREL_LO12_I symbol[5]:_bss_end {
            TypeI addi RD:x11 RS:x11 IMM:0
        }
        TypeB bge RS1:x10 RS2:x11 LABEL:1f
        LABEL 0
        TypeS sw RS1:x10 RS2:x0 IMM:0
        TypeI addi RD:x10 RS:x10 IMM:4
        TypeB blt RS1:x10 RS2:x11 LABEL:0b
        LABEL 1
        COMMENT # Call the user's main() function
        RELOC R_RISCV_PCREL_HI20 symbol[10]:user_main {
            TypeB auipc RD:x1 IMM:0
        }
        RELOC R_RISCV_PCREL_LO12_I symbol[10]:user_main {
            TypeI jalr RD:x1 RS:x1 IMM:0
        }
        COMMENT # If main returns, loop forever (or wfi)
        LABEL 2
        TypeMisc wfi
        TypeJ jal RD:x0 LABEL:2b
        COMMENT # ====================== User Code Section ======================
    }
    SECTION[1] .text [14]{
        ALIGN 16 BYTES
        COMMENT # This is where your actual program logic goes
        LABEL user_main
        COMMENT # Simple example: infinite counter
        TypeI addi RD:x5 RS:x0 IMM:0
        LABEL loop
        TypeI addi RD:x5 RS:x5 IMM:1
        COMMENT # Example: load from .rodata
        RELOC R_RISCV_PCREL_HI20 symbol[14]:hello_string {
            TypeB auipc RD:x10 IMM:0
        }
        RELOC R_RISCV_PCREL_LO12_I symbol[14]:hello_string {
            TypeI addi RD:x10 RS:x10 IMM:0
        }
        COMMENT # (you could call a putchar here if you had UART code)
        TypeJ jal RD:x0 LABEL:loop
        TypeI jalr RD:x0 RS:x1 IMM:0
        COMMENT # ====================== Read-only Data ======================
    }
    SECTION[2] .rodata [4]{
        ALIGN 256 BYTES
        LABEL hello_string
        DIRECTIVE .asciz STRING:"Hello from KUICK bare-metal!\n"
        COMMENT # ====================== Initialized Data ======================
    }
    SECTION[3] .data [4]{
        ALIGN 256 BYTES
        LABEL global_counter
        DIRECTIVE .word INT:4660
        COMMENT # ====================== Uninitialized Data (BSS) ======================
    }
    SECTION[4] .bss [3]{
        ALIGN 256 BYTES
        LABEL buffer
        DIRECTIVE .space INT:256
    }
}
