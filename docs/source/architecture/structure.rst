Architecture Structural Elements
*******************************************************************************

.. _Map_of_Registers:

Map of Registers
================

+-----+------+------+-----+-------+
|     | 63   | 31   | 15  | 7     |
+=====+======+======+=====+=======+
|     | %rax | %eax               |
+     +------+------+-------------+
| R1  |             | %ax         |
+     +-------------+-----+-------+
|     |             | %ah | %al   |
+-----+------+------+-----+-------+
|     | %rbx | %ebx               |
+     +------+------+-------------+
| R2  |             | %bx         |
+     +-------------+-----+-------+
|     |             | %bh | %bl   |
+-----+------+------+-----+-------+
|     | %rcx | %ecx               |
+     +------+------+-------------+
| R3  |             | %cx         |
+     +-------------+-----+-------+
|     |             | %ch | %cl   |
+-----+------+------+-----+-------+
|     | %rdx | %edx               |
+     +------+------+-------------+
| R4  |             | %dx         |
+     +-------------+-----+-------+
|     |             | %dh | %dl   |
+-----+------+------+-----+-------+
|     | %rsi | %esi               |
+     +------+------+-------------+
| R5  |             | %si         |
+     +-------------+-----+-------+
|     |                   | %sil  |
+-----+------+------------+-------+
|     | %rdi | %edi               |
+     +------+------+-------------+
| R6  |             | %di         |
+     +-------------+-----+-------+
|     |                   | %dil  |
+-----+------+------------+-------+
|     | %rbp | %ebp               |
+     +------+------+-------------+
| R7  |             | %bp         |
+     +-------------+-----+-------+
|     |                   | %bpl  |
+-----+------+------------+-------+
|     | %rsp | %esp               |
+     +------+------+-------------+
| R8  |             | %sp         |
+     +-------------+-----+-------+
|     |                   | %spl  |
+-----+------+------------+-------+
|     | %r8  | %r8d               |
+     +------+------+-------------+
| R9  |             | %r8w        |
+     +-------------+-----+-------+
|     |                   | %r8b  |
+-----+------+------------+-------+
|     | %r9  | %r9d               |
+     +------+------+-------------+
| R10 |             | %r9w        |
+     +-------------+-----+-------+
|     |                   | %r9b  |
+-----+------+------------+-------+
|     | %r10  | %r10d             |
+     +------+------+-------------+
| R11 |             | %r10w       |
+     +-------------+-----+-------+
|     |                   | %r10b |
+-----+------+------------+-------+
|     | %r11  | %r11d             |
+     +------+------+-------------+
| R12 |             | %r11w       |
+     +-------------+-----+-------+
|     |                   | %r11b |
+-----+------+------------+-------+
|     | %r12  | %r12d             |
+     +------+------+-------------+
| R13 |             | %r12w       |
+     +-------------+-----+-------+
|     |                   | %r12b |
+-----+------+------------+-------+
|     | %r13  | %r13d             |
+     +------+------+-------------+
| R14 |             | %r13w       |
+     +-------------+-----+-------+
|     |                   | %r13b |
+-----+------+------------+-------+
|     | %r14  | %r14d             |
+     +------+------+-------------+
| R15 |             | %r14w       |
+     +-------------+-----+-------+
|     |                   | %r14b |
+-----+------+------------+-------+
|     | %r15  | %r15d             |
+     +------+------+-------------+
| R16 |             | %r15w       |
+     +-------------+-----+-------+
|     |                   | %r15b |
+-----+------+------------+-------+