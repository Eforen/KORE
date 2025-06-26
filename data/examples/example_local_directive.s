.text
.global main
.local helper_function

main:
    addi x1, x0, 10
    jal x1, helper_function
    addi x2, x1, 5
    ret

helper_function:
    addi x1, x1, 1
    ret 