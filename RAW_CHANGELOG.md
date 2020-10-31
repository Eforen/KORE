# Planned
* Replace conditional negitives and replace with bitwise shifting for handelling of sign extensions (if tests show they are faster)
* Program JType Precom Tests
* Program UType Precom Tests
* For speed make RamController imidiately drop out if op is nop
* Write simple compiler for Risc-V ASM Type B
* Write simple compiler for Risc-V ASM Type U
* Write simple compiler for Risc-V ASM Type J
* [Note] There are about 255 riscv instructions (not all are true instructions some are aliases)
* [Note] There are 47 instructions in Risc-V RV32I
* [Tests][CPU][RV32I] lui
* [Tests][CPU][RV32I] auipc
* [Tests][CPU][RV32I] jal
* [Tests][CPU][RV32I] jalr
* [Tests][CPU][RV32I] beq
* [Tests][CPU][RV32I] blt
* [Tests][CPU][RV32I] lb
* [Tests][CPU][RV32I] lw
* [Tests][CPU][RV32I] lbu
* [Tests][CPU][RV32I] sb
* [Tests][CPU][RV32I] slti
* [Tests][CPU][RV32I] sltiu
* [Tests][CPU][RV32I] xori
* [Tests][CPU][RV32I] ori
* [Tests][CPU][RV32I] andi
* [Tests][CPU][RV32I] slli
* [Tests][CPU][RV32I] srli
* [Tests][CPU][RV32I] srai
* [Tests][CPU][RV32I] add
* [Tests][CPU][RV32I] sub
* [Tests][CPU][RV32I] sll
* [Tests][CPU][RV32I] slt
* [Tests][CPU][RV32I] sltu
* [Tests][CPU][RV32I] xor
* [Tests][CPU][RV32I] srl
* [Tests][CPU][RV32I] sra
* [Tests][CPU][RV32I] or
* [Tests][CPU][RV32I] and
* [Tests][CPU][RV32I] fence
* [Tests][CPU][RV32I] fence.i
* [Tests][CPU][RV32I] ecall
* [Tests][CPU][RV32I] ebreak
* [Tests][CPU][RV32I] csrrw
* [Tests][CPU][RV32I] csrrs
* [Tests][CPU][RV32I] carrc
* [Tests][CPU][RV32I] csrrwi
* [Tests][CPU][RV32I] csrrsi
* [Tests][CPU][RV32I] csrrci

# Working On
* Write tests for all SType Store Instructions
* Write simple compiler for Risc-V ASM called Kuick Compiler
* Write simple compiler for Risc-V ASM Type S
* Write simple compiler for Risc-V ASM Type I

# Finished
* [Tests][CPU][RV32I] ld
* [Tests][CPU][RV32I] lh
* [Tests][CPU][RV32I] lhu
* [Tests][CPU][RV32I] sh
* [Tests][CPU][RV32I] sd
* [Tests][CPU][RV32I] sw
* [Tests][CPU][RV32I] addi
* [Tests][CPU][RV32I] bgeu
* [Tests][CPU][RV32I] nop
* [Tests][CPU][RV32I] bge
* [Tests][CPU][RV32I] bne
* [Tests][CPU][RV32I] bltu
* [Tests][CPU][RV?] beqz
* Write simple compiler for Risc-V ASM Type R
* Change B32_ADD to B32_OP
* Fixed stuff I broke
* Impliment Assembly OP SW
* Refactor Write Cycle to Generalize
* Program BType Full Range Tests
* Program RType Full Range Tests
* Program UType Full Range Tests
* Program JType Full Range Tests
* Program IType Full Range Tests
* Expand S Type Full range test to include registers
* Refactored Names of Type Struct Tests to have Precompiled tests use suffix `_Precomp` 