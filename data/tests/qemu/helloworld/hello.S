.section .text
    .global _start
    .equ UART_BASE, 0x10000000
    .equ UART_TXDATA, 0x0
    .equ UART_TXCTRL, 0x8

_start:
    # Initialize UART base address
    li a2, UART_BASE
    # Load address of string into a0
    la a0, hello_str

    # Loop through string
loop:
    lb a1, (a0)         # Load byte from string
    beqz a1, done       # If null terminator, exit loop
uart_wait:
    lw t0, UART_TXCTRL(a2)  # Load TXCTRL
    andi t0, t0, 2          # Check TXFULL (bit 1)
    bnez t0, uart_wait      # Wait if TXFULL
    sw a1, UART_TXDATA(a2)  # Write byte to UART
    addi a0, a0, 1      # Move to next character
    j loop

done:
    # Infinite loop
    j .

    .section .data
hello_str:
    .string "hello\n"