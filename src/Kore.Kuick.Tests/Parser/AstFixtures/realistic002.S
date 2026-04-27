# realistic002.S - Comprehensive test program for KUICK assembler
# Demonstrates sections, labels, relocations, alignment, and pseudo-instructions

.file "realistic002.S"               # Tell assembler the source filename for debug

# ====================== GLOBAL SYMBOLS ======================
.globl main                     # Entry point - must be global
.globl my_global_data           # Exported data

# ====================== .text SECTION ======================
.text
.align 4                        # Explicit 4-byte alignment (power of 2)

main:                           # Global function
    la   t0, my_global_data     # la → PCREL_HI20 + PCREL_LO12I
    lw   t1, 0(t0)              # lw with label → Lo12I relocation
    la   t2, .Llocal_buffer     # la to local label in .bss

    call my_function            # call → CallPlt relocation

    li   a0, 42                 # pseudo → li
    ret                         # pseudo → jalr x0, 0(ra)

my_function:                    # Global function
    addi t0, t0, 1
    j    .Lloop                 # j pseudo → jal x0, .Lloop

.Lloop:                         # Local label (starts with .L)
    addi t0, t0, -1
    bnez t0, .Lloop             # Branch with local label
    ret

# ====================== .rodata SECTION ======================
.section .rodata
.align 8
hello_msg:
    .asciz "Hello from KUICK!\n"

const_value:
    .word 0xDEADBEEF

# ====================== .data SECTION ======================
.data
.align 8
my_global_data:
    .word 100
    .word 200
    .dword 0x123456789ABCDEF0

# ====================== .bss SECTION ======================
.bss
.align 8
.Llocal_buffer:                 # Local label in .bss
    .space 64                   # 64 bytes uninitialized

.global_buffer:                 # Global buffer in .bss
    .space 128

# ====================== End of file ======================
