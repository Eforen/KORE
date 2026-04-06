.text
    la t0, msg
.data
    .word 1
.rodata
msg:
    .string "x"
.bss
    # Intentional: exercise `.zero` in .bss (GNU-style zero-filled reservation).
    .zero 4
