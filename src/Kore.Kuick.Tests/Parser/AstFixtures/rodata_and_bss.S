.text
    la t0, msg
.data
    .word 1
.rodata
msg:
    .string "x"
.bss
    # Intentional: .word in .bss is uncommon in GNU as (people often use .space/.zero), but we keep it
    # here because firmware/BIOS or hand-written blobs sometimes emit initialized-sized slots this way.
    .word 0
