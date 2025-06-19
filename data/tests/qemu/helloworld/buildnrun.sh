set -x
riscv64-unknown-elf-as -march=rv64gc -mabi=lp64d -o hello.o hello.S
riscv64-unknown-elf-ld -T link.ld -o hello.elf hello.o
qemu-system-riscv64 -machine sifive_u -nographic -bios none -kernel hello.elf
