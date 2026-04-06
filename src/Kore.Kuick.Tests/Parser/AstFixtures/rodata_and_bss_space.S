.text
    la t0, msg
.data
    .word 1
.rodata
msg:
    .string "x"
.bss
    # Intentional: exercise `.space` in .bss (GNU-style reserved bytes).
    .space 4
