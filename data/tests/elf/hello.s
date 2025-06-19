.section .text
.global _start

_start:
    li a0, 1          # File descriptor 1 (stdout)
    la a1, msg        # Address of message
    li a2, 13         # Length of message
    li a7, 64         # Syscall number for write
    ecall             # Make syscall

    li a0, 0          # Exit status
    li a7, 93         # Syscall number for exit
    ecall             # Make syscall

.section .data
msg:
    .ascii "Hello, World!\n"