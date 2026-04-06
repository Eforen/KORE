.text
main:                   # ← label
    la   t0, some_data
    call my_function
    jal  ra, loop

loop:                   # ← label
    addi t0, t0, 1
    j    loop
