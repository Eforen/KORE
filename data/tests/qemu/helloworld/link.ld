OUTPUT_ARCH(riscv)
ENTRY(_start)

PHDRS
{
    text PT_LOAD FLAGS(5); /* RX: Readable + Executable */
    data PT_LOAD FLAGS(6); /* RW: Readable + Writable */
}

SECTIONS
{
    . = 0x80000000;
    .text : {
        *(.text)
    } :text
    .data : {
        *(.data)
    } :data
    .bss : {
        *(.bss)
    } :data
}