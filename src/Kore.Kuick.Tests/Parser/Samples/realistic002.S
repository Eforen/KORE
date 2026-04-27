# realistic002.S - Simple bare-metal RISC-V startup for your linker script
# This file demonstrates how to work with the linker script you posted

.file "realistic002.S"

# Make 'main' visible to the linker (required by ENTRY(main) in the linker script)
.globl main
.globl _start                     # Some linkers expect _start as entry

# Optional: Put startup code in a special section so it can be placed first
.section .text.init
.align 4

# Entry point (the linker script's ENTRY(main) will point here)
_start:
main:
    # Initialize stack pointer (stack is defined at the end of RAM in your linker script)
    la   sp, _stack_top

    # Zero out .bss section (very common in bare-metal)
    la   a0, _bss_start
    la   a1, _bss_end
    bge  a0, a1, 1f               # Skip if no bss
0:
    sw   zero, 0(a0)
    addi a0, a0, 4
    blt  a0, a1, 0b
1:

    # Call the user's main() function
    call user_main

    # If main returns, loop forever (or wfi)
    2:
        wfi
        j 2b

# ====================== User Code Section ======================
.text
.align 4

# This is where your actual program logic goes
user_main:
    # Simple example: infinite counter
    li   t0, 0

loop:
    addi t0, t0, 1

    # Example: load from .rodata
    la   a0, hello_string
    # (you could call a putchar here if you had UART code)

    j    loop

    ret                           # Should never reach here

# ====================== Read-only Data ======================
.section .rodata
.align 8

hello_string:
    .asciz "Hello from KUICK bare-metal!\n"

# ====================== Initialized Data ======================
.data
.align 8

global_counter:
    .word 0x1234

# ====================== Uninitialized Data (BSS) ======================
.bss
.align 8

buffer:
    .space 256                    # 256 bytes of zeroed memory
