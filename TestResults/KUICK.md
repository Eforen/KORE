# Kuick Test Results Summary

## Overall Statistics

| Metric | Count | Status |
|--------|-------|--------|
| Total Tests | 879 | ℹ️ |
| Executed | 855 | ▶️ |
| Passed | 809 | ✅ |
| Failed | 46 | ❌ |

## Test Method Breakdown

| Status | Details | Test Method |
|--------|---------|-------------|
| ✅ | 105 passed | `PseudoInstructions` |
| ❌ | 11 failed | `sanityCheckOps` |
| ✅ | 14 passed | `TestParseBLabelInstruction` |
| ✅ | 174 passed | `readToken` |
| ✅ | 19 passed | `fullReadToken` |
| ❌ | 1 failed | `MachineCode` |
| ✅ | 1 passed | `TestParseAddAndSubInstructions` |
| ✅ | 1 passed | `TestParseAddiMultiInstruction` |
| ✅ | 1 passed | `TestParseAddInstruction` |
| ✅ | 1 passed | `TestParseSbInstruction` |
| ✅ | 1 passed | `TestParseShInstruction` |
| ✅ | 1 passed | `TestParseSubInstruction` |
| ✅ | 1 passed | `TestParseSwInstruction` |
| ✅ | 1 passed | `tokenPositions` |
| ❌ | 24 failed | `sanityCheckOps` |
| ✅ | 2 passed | `InlineDirectives` |
| ✅ | 2 passed | `noWhiteSpaceReadTokens` |
| ✅ | 32 passed | `TestParseAddiInstruction` |
| ❌ | 34 failed | `PseudoInstructions` |
| ✅ | 3 passed | `TestParseBImmInstruction` |
| ✅ | 3 passed | `TestParseJTypeLabelInstruction` |
| ✅ | 435 passed | `sanityCheckOps` |
| ✅ | 4 passed | `TestParseJTypeImmInstruction` |
| ✅ | 4 passed | `TestParseRInstruction` |
| ✅ | 4 passed | `TestParseUTypeInstruction` |
## Detailed Test Results

| Status | Error Details | Test Name |
|--------|---------------|-----------|
| ✅ | N/A | `fullReadToken(".byte 0xFF, 0xf2, 0x02, 0x85, 0x05",[DIRECTIVE, WHITESPACE, NUMBER_HEX, WHITESPACE, NUMBER_HEX, ...],[".byte", " ", "0xFF", ", ", "0xf2", ...])` |
| ✅ | N/A | `fullReadToken(".byte 0xFF, 0xf2, 0x02, 0x85, 0x05",[DIRECTIVE, WHITESPACE, NUMBER_HEX, WHITESPACE, NUMBER_HEX, ...],[".byte", " ", "0xFF", ", ", "0xf2", ...])` |
| ✅ | N/A | `fullReadToken(".double 0.1d, 4.5d, 2.4d, 2414.125125d",[DIRECTIVE, WHITESPACE, NUMBER_DOUBLE, WHITESPACE, NUMBER_DOUBLE, ...],[".double", " ", "0.1d", ", ", "4.5d", ...])` |
| ✅ | N/A | `fullReadToken(".dword 0xFAB3EA2363424253, 0x634242532535A244",[DIRECTIVE, WHITESPACE, NUMBER_HEX, WHITESPACE, NUMBER_HEX, ...],[".dword", " ", "0xFAB3EA2363424253", ", ", "0x634242532535A244", ...])` |
| ✅ | N/A | `fullReadToken(".float 0.1f, 42.2f, 151326.52562f",[DIRECTIVE, WHITESPACE, NUMBER_FLOAT, WHITESPACE, NUMBER_FLOAT, ...],[".float", " ", "0.1f", ", ", "42.2f", ...])` |
| ✅ | N/A | `fullReadToken(".half 0xFFf2, 0x0285, 0x0563",[DIRECTIVE, WHITESPACE, NUMBER_HEX, WHITESPACE, NUMBER_HEX, ...],[".half", " ", "0xFFf2", ", ", "0x0285", ...])` |
| ✅ | N/A | `fullReadToken(".option nopic",[DIRECTIVE, WHITESPACE, IDENTIFIER, EOF, EOF, ...],[".option", " ", "nopic", null, null, ...])` |
| ✅ | N/A | `fullReadToken(".option norelax",[DIRECTIVE, WHITESPACE, IDENTIFIER, EOF, EOF, ...],[".option", " ", "norelax", null, null, ...])` |
| ✅ | N/A | `fullReadToken(".option norvc",[DIRECTIVE, WHITESPACE, IDENTIFIER, EOF, EOF, ...],[".option", " ", "norvc", null, null, ...])` |
| ✅ | N/A | `fullReadToken(".option pic",[DIRECTIVE, WHITESPACE, IDENTIFIER, EOF, EOF, ...],[".option", " ", "pic", null, null, ...])` |
| ✅ | N/A | `fullReadToken(".option push",[DIRECTIVE, WHITESPACE, IDENTIFIER, EOF, EOF, ...],[".option", " ", "push", null, null, ...])` |
| ✅ | N/A | `fullReadToken(".option relax",[DIRECTIVE, WHITESPACE, IDENTIFIER, EOF, EOF, ...],[".option", " ", "relax", null, null, ...])` |
| ✅ | N/A | `fullReadToken(".option rvc",[DIRECTIVE, WHITESPACE, IDENTIFIER, EOF, EOF, ...],[".option", " ", "rvc", null, null, ...])` |
| ✅ | N/A | `fullReadToken(".string \"str\"",[DIRECTIVE, WHITESPACE, STRING, EOF, EOF, ...],[".string", " ", "\"str\"", null, null, ...])` |
| ✅ | N/A | `fullReadToken(".text",[DIRECTIVE, EOF, EOF, EOF],[".text", null, null, null])` |
| ✅ | N/A | `fullReadToken("     .text",[WHITESPACE, DIRECTIVE, EOF, EOF, EOF],["     ", ".text", null, null, null])` |
| ✅ | N/A | `fullReadToken(".word 0xFAB3EA23, 0x63424253, 0x2535A244",[DIRECTIVE, WHITESPACE, NUMBER_HEX, WHITESPACE, NUMBER_HEX, ...],[".word", " ", "0xFAB3EA23", ", ", "0x63424253", ...])` |
| ✅ | N/A | `fullReadToken("yolo",[IDENTIFIER, EOF, EOF, EOF],["yolo", null, null, null])` |
| ✅ | N/A | `fullReadToken("yolo:",[LABEL, EOF, EOF, EOF],["yolo", null, null, null])` |
| ✅ | N/A | `InlineDirectives("hi",HI,"lo",LO,"myVar")` |
| ✅ | N/A | `InlineDirectives("pcrel_hi",PCREL_HI,"pcrel_lo",PCREL_LO,"myVar")` |
| ✅ | N/A | `PseudoInstructions("beqz x1, 0x00000003","beq x1, x0, 0x00000003","Branch if equal zero")` |
| ✅ | N/A | `PseudoInstructions("beqz x2, 0x00000002","beq x2, x0, 0x00000002","Branch if equal zero")` |
| ✅ | N/A | `PseudoInstructions("beqz x3, 0x00000001","beq x3, x0, 0x00000001","Branch if equal zero")` |
| ✅ | N/A | `PseudoInstructions("bgez x1, 0x00000003","bge x1, x0, 0x00000003","Branch if greater than or equal zero")` |
| ✅ | N/A | `PseudoInstructions("bgez x2, 0x00000002","bge x2, x0, 0x00000002","Branch if greater than or equal zero")` |
| ✅ | N/A | `PseudoInstructions("bgez x3, 0x00000001","bge x3, x0, 0x00000001","Branch if greater than or equal zero")` |
| ✅ | N/A | `PseudoInstructions("bgtz x1, 0x00000003","blt x0, x1, 0x00000003","Branch if greater than zero")` |
| ✅ | N/A | `PseudoInstructions("bgtz x2, 0x00000002","blt x0, x2, 0x00000002","Branch if greater than zero")` |
| ✅ | N/A | `PseudoInstructions("blez x1, 0x00000003","bge x0, x1, 0x00000003","Branch if less than or equal zero")` |
| ✅ | N/A | `PseudoInstructions("blez x2, 0x00000002","bge x0, x2, 0x00000002","Branch if less than or equal zero")` |
| ✅ | N/A | `PseudoInstructions("blez x3, 0x00000001","bge x0, x3, 0x00000001","Branch if less than or equal zero")` |
| ✅ | N/A | `PseudoInstructions("bltz x1, 0x00000003","blt x1, x0, 0x00000003","Branch if less than zero")` |
| ✅ | N/A | `PseudoInstructions("bltz x2, 0x00000002","blt x2, x0, 0x00000002","Branch if less than zero")` |
| ✅ | N/A | `PseudoInstructions("bltz x3, 0x00000001","blt x3, x0, 0x00000001","Branch if less than zero")` |
| ✅ | N/A | `PseudoInstructions("bnez x1, 0x00000003","bne x1, x0, 0x00000003","Branch if not equal zero")` |
| ✅ | N/A | `PseudoInstructions("bnez x2, 0x00000002","bne x2, x0, 0x00000002","Branch if not equal zero")` |
| ✅ | N/A | `PseudoInstructions("bnez x3, 0x00000001","bne x3, x0, 0x00000001","Branch if not equal zero")` |
| ✅ | N/A | `PseudoInstructions("csrc 0x7C0, x1","csrrc x0, 0x7C0, x1","Clear custom CSR")` |
| ❌ | Test failed - see full TRX for details | `PseudoInstructions("csrc cycle, x3","csrrc x0, cycle, x3","Clear cycle count CSR")` |
| ✅ | N/A | `PseudoInstructions("csrci 0x7C0, 0x1","csrrci x0, 0x7C0, 0x1","Clear custom CSR (immediate)")` |
| ✅ | N/A | `PseudoInstructions("csrci cycle, 0x3","csrrci x0, cycle, 0x3","Clear cycle count CSR (immediate)")` |
| ✅ | N/A | `PseudoInstructions("csrci instret, 0x2","csrrci x0, instret, 0x2","Clear instruction count CSR (immediate)")` |
| ❌ | Test failed - see full TRX for details | `PseudoInstructions("csrc instret, x2","csrrc x0, instret, x2","Clear instruction count CSR")` |
| ✅ | N/A | `PseudoInstructions("csrci time, 0x4","csrrci x0, time, 0x4","Clear time CSR (immediate)")` |
| ❌ | Test failed - see full TRX for details | `PseudoInstructions("csrc time, x4","csrrc x0, time, x4","Clear time CSR")` |
| ✅ | N/A | `PseudoInstructions("csrr x1, 0x7C0","csrrs x1, 0x7C0, x0","Read custom CSR")` |
| ✅ | N/A | `PseudoInstructions("csrr x2, instret","csrrs x2, instret, x0","Read instruction count CSR")` |
| ✅ | N/A | `PseudoInstructions("csrr x3, cycle","csrrs x3, cycle, x0","Read cycle count CSR")` |
| ✅ | N/A | `PseudoInstructions("csrr x4, time","csrrs x4, time, x0","Read time CSR")` |
| ✅ | N/A | `PseudoInstructions("csrr x5, 0x7C1","csrrs x5, 0x7C1, x0","Read custom CSR")` |
| ✅ | N/A | `PseudoInstructions("csrr x6, instreth","csrrs x6, instreth, x0","Read instruction count CSR")` |
| ✅ | N/A | `PseudoInstructions("csrr x7, cycleh","csrrs x7, cycleh, x0","Read cycle count CSR")` |
| ✅ | N/A | `PseudoInstructions("csrr x8, timeh","csrrs x8, timeh, x0","Read time CSR")` |
| ✅ | N/A | `PseudoInstructions("csrs 0x7C0, x5","csrrs x0, 0x7C0, x5","Set custom CSR")` |
| ✅ | N/A | `PseudoInstructions("csrs cycle, x7","csrrs x0, cycle, x7","Set cycle count CSR")` |
| ✅ | N/A | `PseudoInstructions("csrsi 0x7C0, 0x5","csrrsi x0, 0x7C0, 0x5","Set custom CSR (immediate)")` |
| ✅ | N/A | `PseudoInstructions("csrsi cycle, 0x7","csrrsi x0, cycle, 0x7","Set cycle count CSR (immediate)")` |
| ✅ | N/A | `PseudoInstructions("csrsi instret, 0x6","csrrsi x0, instret, 0x6","Set instruction count CSR (immediate)")` |
| ✅ | N/A | `PseudoInstructions("csrs instret, x6","csrrs x0, instret, x6","Set instruction count CSR")` |
| ✅ | N/A | `PseudoInstructions("csrsi time, 0x8","csrrsi x0, time, 0x8","Set time CSR (immediate)")` |
| ✅ | N/A | `PseudoInstructions("csrs time, x8","csrrs x0, time, x8","Set time CSR")` |
| ✅ | N/A | `PseudoInstructions("csrw 0x7C0, x1","csrrw x0, 0x7C0, x1","Write custom CSR")` |
| ❌ | Test failed - see full TRX for details | `PseudoInstructions("csrw cycle, x3","csrrw x0, cycle, x3","Write cycle count CSR")` |
| ✅ | N/A | `PseudoInstructions("csrwi 0x7C0, 0x1","csrrwi x0, 0x7C0, 0x1","Write custom CSR (immediate)")` |
| ✅ | N/A | `PseudoInstructions("csrwi cycle, 0x3","csrrwi x0, cycle, 0x3","Write cycle count CSR (immediate)")` |
| ✅ | N/A | `PseudoInstructions("csrwi instret, 0x2","csrrwi x0, instret, 0x2","Write instruction count CSR (immediate)")` |
| ❌ | Test failed - see full TRX for details | `PseudoInstructions("csrw instret, x2","csrrw x0, instret, x2","Write instruction count CSR")` |
| ✅ | N/A | `PseudoInstructions("csrwi time, 0x4","csrrwi x0, time, 0x4","Write time CSR (immediate)")` |
| ❌ | Test failed - see full TRX for details | `PseudoInstructions("csrw time, x4","csrrw x0, time, x4","Write time CSR")` |
| ❌ | Test failed - see full TRX for details | `PseudoInstructions("fld f1, myVar","auipc x1, %pcrel_hi(myVar)\n   fld f1, %pcrel_lo(myVar)(x1)","Load doubleword")` |
| ❌ | Test failed - see full TRX for details | `PseudoInstructions("fld f2, myVar1","auipc x2, %pcrel_hi(myVar1)\n  fld f2, %pcrel_lo(myVar1)(x2)","Load doubleword")` |
| ❌ | Test failed - see full TRX for details | `PseudoInstructions("fld f3, myVar152","auipc x3, %pcrel_hi(myVar152)\nfld f3, %pcrel_lo(myVar152)(x3)","Load doubleword")` |
| ❌ | Test failed - see full TRX for details | `PseudoInstructions("flw f1, myVar","auipc x1, %pcrel_hi(myVar)\n   flw f1, %pcrel_lo(myVar)(x1)","Load word")` |
| ❌ | Test failed - see full TRX for details | `PseudoInstructions("flw f2, myVar1","auipc x2, %pcrel_hi(myVar1)\n  flw f2, %pcrel_lo(myVar1)(x2)","Load word")` |
| ❌ | Test failed - see full TRX for details | `PseudoInstructions("flw f3, myVar152","auipc x3, %pcrel_hi(myVar152)\nflw f3, %pcrel_lo(myVar152)(x3)","Load word")` |
| ✅ | N/A | `PseudoInstructions("frcsr x1","csrrs x1, fcsr, x0","Read floating point control/status register")` |
| ✅ | N/A | `PseudoInstructions("frflags x5","csrrs x5, fflags, x0","Read floating point flags CSR")` |
| ✅ | N/A | `PseudoInstructions("frrm x3","csrrs x3, frm, x0","Read floating point rounding mode CSR")` |
| ✅ | N/A | `PseudoInstructions("fscsr x2","csrrs x0, fcsr, x2","Set floating point control/status register")` |
| ❌ | Test failed - see full TRX for details | `PseudoInstructions("fsd f1, myVar","auipc x1, %pcrel_hi(myVar)\n   fsd f1, %pcrel_lo(myVar)(x1)","Store doubleword")` |
| ❌ | Test failed - see full TRX for details | `PseudoInstructions("fsd f2, myVar1","auipc x2, %pcrel_hi(myVar1)\n  fsd f2, %pcrel_lo(myVar1)(x2)","Store doubleword")` |
| ❌ | Test failed - see full TRX for details | `PseudoInstructions("fsd f3, myVar152","auipc x3, %pcrel_hi(myVar152)\nfsd f3, %pcrel_lo(myVar152)(x3)","Store doubleword")` |
| ✅ | N/A | `PseudoInstructions("fsflags x6","csrrs x0, fflags, x6","Set floating point flags CSR")` |
| ✅ | N/A | `PseudoInstructions("fsrm x4","csrrs x0, frm, x4","Set floating point rounding mode CSR")` |
| ❌ | Test failed - see full TRX for details | `PseudoInstructions("fsw f1, myVar","auipc x1, %pcrel_hi(myVar)\n   fsw f1, %pcrel_lo(myVar)(x1)","Store word")` |
| ❌ | Test failed - see full TRX for details | `PseudoInstructions("fsw f2, myVar1","auipc x2, %pcrel_hi(myVar1)\n  fsw f2, %pcrel_lo(myVar1)(x2)","Store word")` |
| ❌ | Test failed - see full TRX for details | `PseudoInstructions("fsw f3, myVar152","auipc x3, %pcrel_hi(myVar152)\nfsw f3, %pcrel_lo(myVar152)(x3)","Store word")` |
| ❌ | Test failed - see full TRX for details | `PseudoInstructions("j 0x16","jal x0, 0x8","Jump 16 bytes")` |
| ✅ | N/A | `PseudoInstructions("j 0x8","jal x0, 0x8","Jump 8 bytes")` |
| ❌ | Test failed - see full TRX for details | `PseudoInstructions("jr x1","jalr x0, 0(x1)","Jump register")` |
| ❌ | Test failed - see full TRX for details | `PseudoInstructions("jr x7","jalr x0, 0(x7)","Jump register")` |
| ❌ | Test failed - see full TRX for details | `PseudoInstructions("la x1, myVar","auipc x1, %pcrel_hi(myVar)\n   addi x1, x1, %pcrel_lo(myVar)","Load address into x1")` |
| ❌ | Test failed - see full TRX for details | `PseudoInstructions("la x2, myVar1","auipc x2, %pcrel_hi(myVar1)\n  addi x2, x2, %pcrel_lo(myVar1)","Load address into x2")` |
| ❌ | Test failed - see full TRX for details | `PseudoInstructions("la x3, myVar152","auipc x3, %pcrel_hi(myVar152)\naddi x3, x3, %pcrel_lo(myVar152)","Load address into x3")` |
| ✅ | N/A | `PseudoInstructions("lb x1, myVar","auipc x1, %pcrel_hi(myVar)\n   lb x1, %pcrel_lo(myVar)(x1)","Load byte")` |
| ✅ | N/A | `PseudoInstructions("lb x2, myVar1","auipc x2, %pcrel_hi(myVar1)\n  lb x2, %pcrel_lo(myVar1)(x2)","Load byte")` |
| ✅ | N/A | `PseudoInstructions("lb x3, myVar152","auipc x3, %pcrel_hi(myVar152)\nlb x3, %pcrel_lo(myVar152)(x3)","Load byte")` |
| ✅ | N/A | `PseudoInstructions("ld x1, myVar","auipc x1, %pcrel_hi(myVar)\n   ld x1, %pcrel_lo(myVar)(x1)","Load doubleword")` |
| ✅ | N/A | `PseudoInstructions("ld x2, myVar1","auipc x2, %pcrel_hi(myVar1)\n  ld x2, %pcrel_lo(myVar1)(x2)","Load doubleword")` |
| ✅ | N/A | `PseudoInstructions("ld x3, myVar152","auipc x3, %pcrel_hi(myVar152)\nld x3, %pcrel_lo(myVar152)(x3)","Load doubleword")` |
| ✅ | N/A | `PseudoInstructions("lh x1, myVar","auipc x1, %pcrel_hi(myVar)\n   lh x1, %pcrel_lo(myVar)(x1)","Load halfword")` |
| ✅ | N/A | `PseudoInstructions("lh x2, myVar1","auipc x2, %pcrel_hi(myVar1)\n  lh x2, %pcrel_lo(myVar1)(x2)","Load halfword")` |
| ✅ | N/A | `PseudoInstructions("lh x3, myVar152","auipc x3, %pcrel_hi(myVar152)\nlh x3, %pcrel_lo(myVar152)(x3)","Load halfword")` |
| ❌ | Test failed - see full TRX for details | `PseudoInstructions("li x1, 0","addi x1, x0, 0","Load immediate")` |
| ❌ | Test failed - see full TRX for details | `PseudoInstructions("li x2, 5","addi x2, x0, 5","Load immediate")` |
| ❌ | Test failed - see full TRX for details | `PseudoInstructions("li x3, 2047","addi x3, x0, 2047","Load immediate")` |
| ❌ | Test failed - see full TRX for details | `PseudoInstructions("lla x1, myVar","auipc x1, %pcrel_hi(myVar)\naddi x1, x1, %pcrel_lo(myVar)","Load address into x1")` |
| ❌ | Test failed - see full TRX for details | `PseudoInstructions("lla x2, myVar1","auipc x2, %pcrel_hi(myVar1)\naddi x2, x2, %pcrel_lo(myVar1)","Load address into x2")` |
| ❌ | Test failed - see full TRX for details | `PseudoInstructions("lla x3, myVar152","auipc x3, %pcrel_hi(myVar152)\naddi x3, x3, %pcrel_lo(myVar152)","Load address into x3")` |
| ✅ | N/A | `PseudoInstructions("lw x1, myVar","auipc x1, %pcrel_hi(myVar)\n   lw x1, %pcrel_lo(myVar)(x1)","Load word")` |
| ✅ | N/A | `PseudoInstructions("lw x2, myVar1","auipc x2, %pcrel_hi(myVar1)\n  lw x2, %pcrel_lo(myVar1)(x2)","Load word")` |
| ✅ | N/A | `PseudoInstructions("lw x3, myVar152","auipc x3, %pcrel_hi(myVar152)\nlw x3, %pcrel_lo(myVar152)(x3)","Load word")` |
| ✅ | N/A | `PseudoInstructions("neg x1, x2","sub x1, x0, x2","Two's complement negation")` |
| ✅ | N/A | `PseudoInstructions("neg x2, x1","sub x2, x0, x1","Two's complement negation")` |
| ✅ | N/A | `PseudoInstructions("neg x3, x3","sub x3, x0, x3","Two's complement negation")` |
| ✅ | N/A | `PseudoInstructions("nop","addi x0, x0, 0","No operation")` |
| ✅ | N/A | `PseudoInstructions("rdcycleh x1","csrrs x1, cycleh, x0","Read cycle count")` |
| ✅ | N/A | `PseudoInstructions("rdcycleh x2","csrrs x2, cycleh, x0","Read cycle count")` |
| ✅ | N/A | `PseudoInstructions("rdcycleh x3","csrrs x3, cycleh, x0","Read cycle count")` |
| ✅ | N/A | `PseudoInstructions("rdcycle x1","csrrs x1, cycle, x0","Read cycle count")` |
| ✅ | N/A | `PseudoInstructions("rdcycle x2","csrrs x2, cycle, x0","Read cycle count")` |
| ✅ | N/A | `PseudoInstructions("rdcycle x3","csrrs x3, cycle, x0","Read cycle count")` |
| ✅ | N/A | `PseudoInstructions("rdinstreth x1","csrrs x1, instreth, x0","Read instruction count")` |
| ✅ | N/A | `PseudoInstructions("rdinstreth x2","csrrs x2, instreth, x0","Read instruction count")` |
| ✅ | N/A | `PseudoInstructions("rdinstreth x3","csrrs x3, instreth, x0","Read instruction count")` |
| ✅ | N/A | `PseudoInstructions("rdinstret x1","csrrs x1, instret, x0","Read instruction count")` |
| ✅ | N/A | `PseudoInstructions("rdinstret x2","csrrs x2, instret, x0","Read instruction count")` |
| ✅ | N/A | `PseudoInstructions("rdinstret x3","csrrs x3, instret, x0","Read instruction count")` |
| ✅ | N/A | `PseudoInstructions("rdtimeh x1","csrrs x1, timeh, x0","Read time")` |
| ✅ | N/A | `PseudoInstructions("rdtimeh x2","csrrs x2, timeh, x0","Read time")` |
| ✅ | N/A | `PseudoInstructions("rdtimeh x3","csrrs x3, timeh, x0","Read time")` |
| ✅ | N/A | `PseudoInstructions("rdtime x1","csrrs x1, time, x0","Read time")` |
| ✅ | N/A | `PseudoInstructions("rdtime x2","csrrs x2, time, x0","Read time")` |
| ✅ | N/A | `PseudoInstructions("rdtime x3","csrrs x3, time, x0","Read time")` |
| ❌ | Test failed - see full TRX for details | `PseudoInstructions("ret","jalr x0, 0(x1)","Return from subroutine")` |
| ✅ | N/A | `PseudoInstructions("sb x1, myVar, t1","auipc t1, %pcrel_hi(myVar)\n   sb x1, %pcrel_lo(myVar)(t1)","Store byte")` |
| ✅ | N/A | `PseudoInstructions("sb x2, myVar1, t2","auipc t2, %pcrel_hi(myVar1)\n  sb x2, %pcrel_lo(myVar1)(t2)","Store byte")` |
| ✅ | N/A | `PseudoInstructions("sb x3, myVar152, t3","auipc t3, %pcrel_hi(myVar152)\nsb x3, %pcrel_lo(myVar152)(t3)","Store byte")` |
| ✅ | N/A | `PseudoInstructions("sd x1, myVar, t1","auipc t1, %pcrel_hi(myVar)\n   sd x1, %pcrel_lo(myVar)(t1)","Store doubleword")` |
| ✅ | N/A | `PseudoInstructions("sd x2, myVar1, t2","auipc t2, %pcrel_hi(myVar1)\n  sd x2, %pcrel_lo(myVar1)(t2)","Store doubleword")` |
| ✅ | N/A | `PseudoInstructions("sd x3, myVar152, t3","auipc t3, %pcrel_hi(myVar152)\nsd x3, %pcrel_lo(myVar152)(t3)","Store doubleword")` |
| ✅ | N/A | `PseudoInstructions("sgtz x1, x2","slt x1, x0, x2","Set if greater than zero")` |
| ✅ | N/A | `PseudoInstructions("sgtz x2, x1","slt x2, x0, x1","Set if greater than zero")` |
| ✅ | N/A | `PseudoInstructions("sgtz x3, x3","slt x3, x0, x3","Set if greater than zero")` |
| ✅ | N/A | `PseudoInstructions("sh x1, myVar, t1","auipc t1, %pcrel_hi(myVar)\n   sh x1, %pcrel_lo(myVar)(t1)","Store halfword")` |
| ✅ | N/A | `PseudoInstructions("sh x2, myVar1, t2","auipc t2, %pcrel_hi(myVar1)\n  sh x2, %pcrel_lo(myVar1)(t2)","Store halfword")` |
| ✅ | N/A | `PseudoInstructions("sh x3, myVar152, t3","auipc t3, %pcrel_hi(myVar152)\nsh x3, %pcrel_lo(myVar152)(t3)","Store halfword")` |
| ✅ | N/A | `PseudoInstructions("sltz x1, x2","slt x1, x2, x0","Set if less than zero")` |
| ✅ | N/A | `PseudoInstructions("sltz x2, x1","slt x2, x1, x0","Set if less than zero")` |
| ✅ | N/A | `PseudoInstructions("sltz x3, x3","slt x3, x3, x0","Set if less than zero")` |
| ✅ | N/A | `PseudoInstructions("snez x1, x2","sltu x1, x0, x2","Set if not equal zero")` |
| ✅ | N/A | `PseudoInstructions("snez x2, x1","sltu x2, x0, x1","Set if not equal zero")` |
| ✅ | N/A | `PseudoInstructions("snez x3, x3","sltu x3, x0, x3","Set if not equal zero")` |
| ✅ | N/A | `PseudoInstructions("sw x1, myVar, t1","auipc t1, %pcrel_hi(myVar)\n   sw x1, %pcrel_lo(myVar)(t1)","Store word")` |
| ✅ | N/A | `PseudoInstructions("sw x2, myVar1, t2","auipc t2, %pcrel_hi(myVar1)\n  sw x2, %pcrel_lo(myVar1)(t2)","Store word")` |
| ✅ | N/A | `PseudoInstructions("sw x3, myVar152, t3","auipc t3, %pcrel_hi(myVar152)\nsw x3, %pcrel_lo(myVar152)(t3)","Store word")` |
| ❌ | Test failed - see full TRX for details | `PseudoInstructions("tail 0x4262fb3f","auipc x6, 0x4262f000\n    jal x0, 0xb3f","Tail call arr-away subroutine")` |
| ❌ | Test failed - see full TRX for details | `PseudoInstructions("tail 0x74543765","auipc x6, 0x74543000\n    jal x0, 0x765","Tail call arr-away subroutine")` |
| ❌ | Test failed - see full TRX for details | `PseudoInstructions("tail 0x81C45C2C","auipc x6, 0x81C45000\n    jal x0, 0xC2C","Tail call arr-away subroutine")` |
| ✅ | N/A | `readToken("0.0f",NUMBER_FLOAT,"0.0f")` |
| ✅ | N/A | `readToken("0.1d, 4.5d, 2.4d, 2414.125125d",NUMBER_DOUBLE,"0.1d")` |
| ✅ | N/A | `readToken("0.1f, 42.2f, 151326.52562f",NUMBER_FLOAT,"0.1f")` |
| ✅ | N/A | `readToken("0b0",NUMBER_BIN,"0b0")` |
| ✅ | N/A | `readToken("0b1000",NUMBER_BIN,"0b1000")` |
| ✅ | N/A | `readToken("0b1001",NUMBER_BIN,"0b1001")` |
| ✅ | N/A | `readToken("0b100",NUMBER_BIN,"0b100")` |
| ✅ | N/A | `readToken("0b101",NUMBER_BIN,"0b101")` |
| ✅ | N/A | `readToken("0b10",NUMBER_BIN,"0b10")` |
| ✅ | N/A | `readToken("0b110",NUMBER_BIN,"0b110")` |
| ✅ | N/A | `readToken("0b111",NUMBER_BIN,"0b111")` |
| ✅ | N/A | `readToken("0b11",NUMBER_BIN,"0b11")` |
| ✅ | N/A | `readToken("0b1",NUMBER_BIN,"0b1")` |
| ✅ | N/A | `readToken("0",NUMBER_INT,"0")` |
| ✅ | N/A | `readToken("0x02, 0x85, 0x05",NUMBER_HEX,"0x02")` |
| ✅ | N/A | `readToken("0x05",NUMBER_HEX,"0x05")` |
| ✅ | N/A | `readToken("0x0",NUMBER_HEX,"0x0")` |
| ✅ | N/A | `readToken("0x10",NUMBER_HEX,"0x10")` |
| ✅ | N/A | `readToken("0x11",NUMBER_HEX,"0x11")` |
| ✅ | N/A | `readToken("0x1",NUMBER_HEX,"0x1")` |
| ✅ | N/A | `readToken("0x2",NUMBER_HEX,"0x2")` |
| ✅ | N/A | `readToken("0x3",NUMBER_HEX,"0x3")` |
| ✅ | N/A | `readToken("0x4",NUMBER_HEX,"0x4")` |
| ✅ | N/A | `readToken("0x5",NUMBER_HEX,"0x5")` |
| ✅ | N/A | `readToken("0x6",NUMBER_HEX,"0x6")` |
| ✅ | N/A | `readToken("0x7",NUMBER_HEX,"0x7")` |
| ✅ | N/A | `readToken("0x85, 0x05",NUMBER_HEX,"0x85")` |
| ✅ | N/A | `readToken("0x8",NUMBER_HEX,"0x8")` |
| ✅ | N/A | `readToken("0x9",NUMBER_HEX,"0x9")` |
| ✅ | N/A | `readToken("0xa",NUMBER_HEX,"0xa")` |
| ✅ | N/A | `readToken("0xb",NUMBER_HEX,"0xb")` |
| ✅ | N/A | `readToken("0xc",NUMBER_HEX,"0xc")` |
| ✅ | N/A | `readToken("0xd",NUMBER_HEX,"0xd")` |
| ✅ | N/A | `readToken("0xe",NUMBER_HEX,"0xe")` |
| ✅ | N/A | `readToken("0xf2, 0x02, 0x85, 0x05",NUMBER_HEX,"0xf2")` |
| ✅ | N/A | `readToken("0xFF, 0xf2, 0x02, 0x85, 0x05",NUMBER_HEX,"0xFF")` |
| ✅ | N/A | `readToken("0xf",NUMBER_HEX,"0xf")` |
| ✅ | N/A | `readToken("10.0f",NUMBER_FLOAT,"10.0f")` |
| ✅ | N/A | `readToken("1.0f",NUMBER_FLOAT,"1.0f")` |
| ✅ | N/A | `readToken("10",NUMBER_INT,"10")` |
| ✅ | N/A | `readToken("11.0f",NUMBER_FLOAT,"11.0f")` |
| ✅ | N/A | `readToken("11",NUMBER_INT,"11")` |
| ✅ | N/A | `readToken("12.0f",NUMBER_FLOAT,"12.0f")` |
| ✅ | N/A | `readToken("12",NUMBER_INT,"12")` |
| ✅ | N/A | `readToken("13.0f",NUMBER_FLOAT,"13.0f")` |
| ✅ | N/A | `readToken("13",NUMBER_INT,"13")` |
| ✅ | N/A | `readToken("14.0f",NUMBER_FLOAT,"14.0f")` |
| ✅ | N/A | `readToken("14",NUMBER_INT,"14")` |
| ✅ | N/A | `readToken("151326.52562f",NUMBER_FLOAT,"151326.52562f")` |
| ✅ | N/A | `readToken("1",NUMBER_INT,"1")` |
| ✅ | N/A | `readToken("2.0f",NUMBER_FLOAT,"2.0f")` |
| ✅ | N/A | `readToken("2414.125125d",NUMBER_DOUBLE,"2414.125125d")` |
| ✅ | N/A | `readToken("-242",NUMBER_INT,"-242")` |
| ✅ | N/A | `readToken("242",NUMBER_INT,"242")` |
| ✅ | N/A | `readToken("-2.4d, 2414.125125d",NUMBER_DOUBLE,"-2.4d")` |
| ✅ | N/A | `readToken("2.4d, 2414.125125d",NUMBER_DOUBLE,"2.4d")` |
| ✅ | N/A | `readToken("2",NUMBER_INT,"2")` |
| ✅ | N/A | `readToken("3.0f",NUMBER_FLOAT,"3.0f")` |
| ✅ | N/A | `readToken("3",NUMBER_INT,"3")` |
| ✅ | N/A | `readToken("4.0f",NUMBER_FLOAT,"4.0f")` |
| ✅ | N/A | `readToken("-42.2f, 151326.52562f",NUMBER_FLOAT,"-42.2f")` |
| ✅ | N/A | `readToken("42.2f, 151326.52562f",NUMBER_FLOAT,"42.2f")` |
| ✅ | N/A | `readToken("4.5d, 2.4d, 2414.125125d",NUMBER_DOUBLE,"4.5d")` |
| ✅ | N/A | `readToken("4",NUMBER_INT,"4")` |
| ✅ | N/A | `readToken("5.0f",NUMBER_FLOAT,"5.0f")` |
| ✅ | N/A | `readToken("5",NUMBER_INT,"5")` |
| ✅ | N/A | `readToken("6.0f",NUMBER_FLOAT,"6.0f")` |
| ✅ | N/A | `readToken("6",NUMBER_INT,"6")` |
| ✅ | N/A | `readToken("7.0f",NUMBER_FLOAT,"7.0f")` |
| ✅ | N/A | `readToken("7",NUMBER_INT,"7")` |
| ✅ | N/A | `readToken("8.0f",NUMBER_FLOAT,"8.0f")` |
| ✅ | N/A | `readToken("8",NUMBER_INT,"8")` |
| ✅ | N/A | `readToken("9.0f",NUMBER_FLOAT,"9.0f")` |
| ✅ | N/A | `readToken("9",NUMBER_INT,"9")` |
| ✅ | N/A | `readToken("a00",REGISTER,"x10")` |
| ✅ | N/A | `readToken("a01",REGISTER,"x11")` |
| ✅ | N/A | `readToken("a02",REGISTER,"x12")` |
| ✅ | N/A | `readToken("a03",REGISTER,"x13")` |
| ✅ | N/A | `readToken("a04",REGISTER,"x14")` |
| ✅ | N/A | `readToken("a05",REGISTER,"x15")` |
| ✅ | N/A | `readToken("a06",REGISTER,"x16")` |
| ✅ | N/A | `readToken("a07",REGISTER,"x17")` |
| ✅ | N/A | `readToken("a0",REGISTER,"x10")` |
| ✅ | N/A | `readToken("a1",REGISTER,"x11")` |
| ✅ | N/A | `readToken("a2",REGISTER,"x12")` |
| ✅ | N/A | `readToken("a3",REGISTER,"x13")` |
| ✅ | N/A | `readToken("a4",REGISTER,"x14")` |
| ✅ | N/A | `readToken("a5",REGISTER,"x15")` |
| ✅ | N/A | `readToken("a6",REGISTER,"x16")` |
| ✅ | N/A | `readToken("a7",REGISTER,"x17")` |
| ✅ | N/A | `readToken(".byte 0xFF, 0xf2, 0x02, 0x85, 0x05",DIRECTIVE,".byte")` |
| ✅ | N/A | `readToken(".double 0.1d, 4.5d, 2.4d, 2414.125125d",DIRECTIVE,".double")` |
| ✅ | N/A | `readToken(".dword 0xFAB3EA2363424253, 0x634242532535A244",DIRECTIVE,".dword")` |
| ✅ | N/A | `readToken(".float 0.1f, 42.2f, 151326.52562f",DIRECTIVE,".float")` |
| ✅ | N/A | `readToken("fp",REGISTER,"x8")` |
| ✅ | N/A | `readToken("%got(myVar)",INLINE_DIRECTIVE,"got")` |
| ✅ | N/A | `readToken("%got_pcrel_hi(myVar)",INLINE_DIRECTIVE,"got_pcrel_hi")` |
| ✅ | N/A | `readToken("%got_pcrel_lo(myVar)",INLINE_DIRECTIVE,"got_pcrel_lo")` |
| ✅ | N/A | `readToken("%got_pcrel(myVar)",INLINE_DIRECTIVE,"got_pcrel")` |
| ✅ | N/A | `readToken("gp",REGISTER,"x3")` |
| ✅ | N/A | `readToken(".half 0xFFf2, 0x0285, 0x0563",DIRECTIVE,".half")` |
| ✅ | N/A | `readToken("%hi(myVar)",INLINE_DIRECTIVE,"hi")` |
| ✅ | N/A | `readToken("%lo(myVar)",INLINE_DIRECTIVE,"lo")` |
| ✅ | N/A | `readToken("nopic",IDENTIFIER,"nopic")` |
| ✅ | N/A | `readToken("norelax",IDENTIFIER,"norelax")` |
| ✅ | N/A | `readToken("norvc",IDENTIFIER,"norvc")` |
| ✅ | N/A | `readToken(".option nopic",DIRECTIVE,".option")` |
| ✅ | N/A | `readToken(".option norelax",DIRECTIVE,".option")` |
| ✅ | N/A | `readToken(".option norvc",DIRECTIVE,".option")` |
| ✅ | N/A | `readToken(".option pic",DIRECTIVE,".option")` |
| ✅ | N/A | `readToken(".option push",DIRECTIVE,".option")` |
| ✅ | N/A | `readToken(".option relax",DIRECTIVE,".option")` |
| ✅ | N/A | `readToken(".option rvc",DIRECTIVE,".option")` |
| ✅ | N/A | `readToken("%pcrel_hi(myVar)",INLINE_DIRECTIVE,"pcrel_hi")` |
| ✅ | N/A | `readToken("%pcrel_lo(myVar)",INLINE_DIRECTIVE,"pcrel_lo")` |
| ✅ | N/A | `readToken("pic",IDENTIFIER,"pic")` |
| ✅ | N/A | `readToken("push",IDENTIFIER,"push")` |
| ✅ | N/A | `readToken("ra",REGISTER,"x1")` |
| ✅ | N/A | `readToken("relax",IDENTIFIER,"relax")` |
| ✅ | N/A | `readToken("rvc",IDENTIFIER,"rvc")` |
| ✅ | N/A | `readToken("s01",REGISTER,"x9")` |
| ✅ | N/A | `readToken("s02",REGISTER,"x18")` |
| ✅ | N/A | `readToken("s03",REGISTER,"x19")` |
| ✅ | N/A | `readToken("s04",REGISTER,"x20")` |
| ✅ | N/A | `readToken("s05",REGISTER,"x21")` |
| ✅ | N/A | `readToken("s06",REGISTER,"x22")` |
| ✅ | N/A | `readToken("s07",REGISTER,"x23")` |
| ✅ | N/A | `readToken("s08",REGISTER,"x24")` |
| ✅ | N/A | `readToken("s09",REGISTER,"x25")` |
| ✅ | N/A | `readToken("s0",REGISTER,"x8")` |
| ✅ | N/A | `readToken("s10",REGISTER,"x26")` |
| ✅ | N/A | `readToken("s11",REGISTER,"x27")` |
| ✅ | N/A | `readToken("s1",REGISTER,"x9")` |
| ✅ | N/A | `readToken("s2",REGISTER,"x18")` |
| ✅ | N/A | `readToken("s3",REGISTER,"x19")` |
| ✅ | N/A | `readToken("s4",REGISTER,"x20")` |
| ✅ | N/A | `readToken("s5",REGISTER,"x21")` |
| ✅ | N/A | `readToken("s6",REGISTER,"x22")` |
| ✅ | N/A | `readToken("s7",REGISTER,"x23")` |
| ✅ | N/A | `readToken("s8",REGISTER,"x24")` |
| ✅ | N/A | `readToken("s9",REGISTER,"x25")` |
| ✅ | N/A | `readToken("SLL ,",OP_R,"SLL")` |
| ✅ | N/A | `readToken(", SLL",WHITESPACE,", ")` |
| ✅ | N/A | `readToken("sp",REGISTER,"x2")` |
| ✅ | N/A | `readToken(".string \"str\"",DIRECTIVE,".string")` |
| ✅ | N/A | `readToken("\"str\"",STRING,"\"str\"")` |
| ✅ | N/A | `readToken("t03",REGISTER,"x28")` |
| ✅ | N/A | `readToken("t04",REGISTER,"x29")` |
| ✅ | N/A | `readToken("t05",REGISTER,"x30")` |
| ✅ | N/A | `readToken("t06",REGISTER,"x31")` |
| ✅ | N/A | `readToken("t0",REGISTER,"x5")` |
| ✅ | N/A | `readToken("t1",REGISTER,"x6")` |
| ✅ | N/A | `readToken("t2",REGISTER,"x7")` |
| ✅ | N/A | `readToken("t3",REGISTER,"x28")` |
| ✅ | N/A | `readToken("t4",REGISTER,"x29")` |
| ✅ | N/A | `readToken("t5",REGISTER,"x30")` |
| ✅ | N/A | `readToken("t6",REGISTER,"x31")` |
| ✅ | N/A | `readToken(".text",DIRECTIVE,".text")` |
| ✅ | N/A | `readToken("     .text",WHITESPACE,"     ")` |
| ✅ | N/A | `readToken("tp",REGISTER,"x4")` |
| ✅ | N/A | `readToken(".word 0xFAB3EA23, 0x63424253, 0x2535A244",DIRECTIVE,".word")` |
| ✅ | N/A | `readToken("x00",REGISTER,"x0")` |
| ✅ | N/A | `readToken("x01",REGISTER,"x1")` |
| ✅ | N/A | `readToken("x02",REGISTER,"x2")` |
| ✅ | N/A | `readToken("x03",REGISTER,"x3")` |
| ✅ | N/A | `readToken("x04",REGISTER,"x4")` |
| ✅ | N/A | `readToken("x05",REGISTER,"x5")` |
| ✅ | N/A | `readToken("x06",REGISTER,"x6")` |
| ✅ | N/A | `readToken("x07",REGISTER,"x7")` |
| ✅ | N/A | `readToken("x08",REGISTER,"x8")` |
| ✅ | N/A | `readToken("x09",REGISTER,"x9")` |
| ✅ | N/A | `readToken("yolo",IDENTIFIER,"yolo")` |
| ✅ | N/A | `readToken("yolo:",LABEL,"yolo")` |
| ✅ | N/A | `readToken("zero",REGISTER,"x0")` |
| ✅ | N/A | `sanityCheckOps("ADDI",OP_I)` |
| ✅ | N/A | `sanityCheckOps("ADDIW",OP_I)` |
| ✅ | N/A | `sanityCheckOps("ADD",OP_R)` |
| ✅ | N/A | `sanityCheckOps("ADDW",OP_R)` |
| ✅ | N/A | `sanityCheckOps("AMOADD.D",OP_R)` |
| ✅ | N/A | `sanityCheckOps("AMOADD",OP_R)` |
| ✅ | N/A | `sanityCheckOps("AMOADD.W",OP_R)` |
| ✅ | N/A | `sanityCheckOps("AMOAND.D",OP_R)` |
| ✅ | N/A | `sanityCheckOps("AMOAND",OP_R)` |
| ✅ | N/A | `sanityCheckOps("AMOAND.W",OP_R)` |
| ✅ | N/A | `sanityCheckOps("AMOMAX.D",OP_R)` |
| ✅ | N/A | `sanityCheckOps("AMOMAX",OP_R)` |
| ✅ | N/A | `sanityCheckOps("AMOMAXU.D",OP_R)` |
| ✅ | N/A | `sanityCheckOps("AMOMAXU.W",OP_R)` |
| ✅ | N/A | `sanityCheckOps("AMOMAX.W",OP_R)` |
| ✅ | N/A | `sanityCheckOps("AMOMIN.D",OP_R)` |
| ✅ | N/A | `sanityCheckOps("AMOMIN",OP_R)` |
| ✅ | N/A | `sanityCheckOps("AMOMINU.D",OP_R)` |
| ✅ | N/A | `sanityCheckOps("AMOMINU.W",OP_R)` |
| ✅ | N/A | `sanityCheckOps("AMOMIN.W",OP_R)` |
| ✅ | N/A | `sanityCheckOps("AMOOR.D",OP_R)` |
| ✅ | N/A | `sanityCheckOps("AMOOR",OP_R)` |
| ✅ | N/A | `sanityCheckOps("AMOOR.W",OP_R)` |
| ✅ | N/A | `sanityCheckOps("AMOSWAP.D",OP_R)` |
| ✅ | N/A | `sanityCheckOps("AMOSWAP",OP_R)` |
| ✅ | N/A | `sanityCheckOps("AMOSWAP.W",OP_R)` |
| ✅ | N/A | `sanityCheckOps("AMOXOR.D",OP_R)` |
| ✅ | N/A | `sanityCheckOps("AMOXOR",OP_R)` |
| ✅ | N/A | `sanityCheckOps("AMOXOR.W",OP_R)` |
| ✅ | N/A | `sanityCheckOps("ANDI",OP_I)` |
| ✅ | N/A | `sanityCheckOps("AND",OP_R)` |
| ✅ | N/A | `sanityCheckOps("AUIPC",OP_U)` |
| ✅ | N/A | `sanityCheckOps("BEQ",OP_B)` |
| ✅ | N/A | `sanityCheckOps("BEQZ",OP_PSEUDO)` |
| ✅ | N/A | `sanityCheckOps("BGE",OP_B)` |
| ✅ | N/A | `sanityCheckOps("BGEU",OP_B)` |
| ✅ | N/A | `sanityCheckOps("BGEZ",OP_PSEUDO)` |
| ✅ | N/A | `sanityCheckOps("bgt",OP_PSEUDO)` |
| ✅ | N/A | `sanityCheckOps("bgtu",OP_PSEUDO)` |
| ✅ | N/A | `sanityCheckOps("BGTZ",OP_PSEUDO)` |
| ✅ | N/A | `sanityCheckOps("ble",OP_PSEUDO)` |
| ✅ | N/A | `sanityCheckOps("bleu",OP_PSEUDO)` |
| ✅ | N/A | `sanityCheckOps("BLEZ",OP_PSEUDO)` |
| ✅ | N/A | `sanityCheckOps("BLT",OP_B)` |
| ✅ | N/A | `sanityCheckOps("BLTU",OP_B)` |
| ✅ | N/A | `sanityCheckOps("BLTZ",OP_PSEUDO)` |
| ✅ | N/A | `sanityCheckOps("BNE",OP_B)` |
| ✅ | N/A | `sanityCheckOps("BNEZ",OP_PSEUDO)` |
| ✅ | N/A | `sanityCheckOps("bsatp",CSR)` |
| ✅ | N/A | `sanityCheckOps("C.ADDI16SP",OP_CI)` |
| ✅ | N/A | `sanityCheckOps("C.ADDI4SPN",OP_CIW)` |
| ✅ | N/A | `sanityCheckOps("C.ADDI",OP_CI)` |
| ✅ | N/A | `sanityCheckOps("C.ADDIW",OP_CI)` |
| ✅ | N/A | `sanityCheckOps("C.ADD",OP_CR)` |
| ✅ | N/A | `sanityCheckOps("C.ADDW",OP_CR)` |
| ✅ | N/A | `sanityCheckOps("call",OP_PSEUDO)` |
| ✅ | N/A | `sanityCheckOps("C.ANDI",OP_CI)` |
| ✅ | N/A | `sanityCheckOps("C.AND",OP_CR)` |
| ✅ | N/A | `sanityCheckOps("C.BEQZ",OP_CB)` |
| ✅ | N/A | `sanityCheckOps("C.BNEZ",OP_CB)` |
| ✅ | N/A | `sanityCheckOps("C.EBRAKE",OP_CI)` |
| ✅ | N/A | `sanityCheckOps("C.FLD",OP_CL)` |
| ✅ | N/A | `sanityCheckOps("C.FLDSP",OP_CI)` |
| ✅ | N/A | `sanityCheckOps("C.FLW",OP_CL)` |
| ✅ | N/A | `sanityCheckOps("C.FLWSP",OP_CI)` |
| ✅ | N/A | `sanityCheckOps("C.FSD",OP_CS)` |
| ✅ | N/A | `sanityCheckOps("C.FSDSP",OP_CSS)` |
| ✅ | N/A | `sanityCheckOps("C.FSW",OP_CS)` |
| ✅ | N/A | `sanityCheckOps("C.FSWSP",OP_CSS)` |
| ✅ | N/A | `sanityCheckOps("C.JAL",OP_CJ)` |
| ✅ | N/A | `sanityCheckOps("C.JALR",OP_CR)` |
| ✅ | N/A | `sanityCheckOps("C.J",OP_CJ)` |
| ✅ | N/A | `sanityCheckOps("C.JR",OP_CR)` |
| ✅ | N/A | `sanityCheckOps("C.LD",OP_CL)` |
| ✅ | N/A | `sanityCheckOps("C.LDSP",OP_CI)` |
| ✅ | N/A | `sanityCheckOps("C.LI",OP_CI)` |
| ✅ | N/A | `sanityCheckOps("C.LUI",OP_CI)` |
| ✅ | N/A | `sanityCheckOps("C.LW",OP_CL)` |
| ✅ | N/A | `sanityCheckOps("C.LWSP",OP_CI)` |
| ✅ | N/A | `sanityCheckOps("C.MV",OP_CR)` |
| ✅ | N/A | `sanityCheckOps("C.OR",OP_CR)` |
| ✅ | N/A | `sanityCheckOps("C.SD",OP_CS)` |
| ✅ | N/A | `sanityCheckOps("C.SDSP",OP_CSS)` |
| ✅ | N/A | `sanityCheckOps("C.SLLI",OP_CI)` |
| ✅ | N/A | `sanityCheckOps("C.SRAI",OP_CI)` |
| ✅ | N/A | `sanityCheckOps("CSRCI",OP_PSEUDO)` |
| ✅ | N/A | `sanityCheckOps("csrc",OP_PSEUDO)` |
| ✅ | N/A | `sanityCheckOps("C.SRLI",OP_CI)` |
| ✅ | N/A | `sanityCheckOps("CSRRCI",OP_I)` |
| ✅ | N/A | `sanityCheckOps("CSRRC",OP_I)` |
| ✅ | N/A | `sanityCheckOps("csrr",OP_PSEUDO)` |
| ✅ | N/A | `sanityCheckOps("CSRRSI",OP_I)` |
| ✅ | N/A | `sanityCheckOps("CSRRS",OP_I)` |
| ✅ | N/A | `sanityCheckOps("CSRRWI",OP_I)` |
| ✅ | N/A | `sanityCheckOps("CSRRW",OP_I)` |
| ✅ | N/A | `sanityCheckOps("CSRSI",OP_PSEUDO)` |
| ✅ | N/A | `sanityCheckOps("csrs",OP_PSEUDO)` |
| ✅ | N/A | `sanityCheckOps("CSRWI",OP_PSEUDO)` |
| ✅ | N/A | `sanityCheckOps("csrw",OP_PSEUDO)` |
| ✅ | N/A | `sanityCheckOps("C.SUB",OP_CR)` |
| ✅ | N/A | `sanityCheckOps("C.SUBW",OP_CR)` |
| ✅ | N/A | `sanityCheckOps("C.SW",OP_CS)` |
| ✅ | N/A | `sanityCheckOps("C.SWSP",OP_CSS)` |
| ✅ | N/A | `sanityCheckOps("C.XOR",OP_CR)` |
| ✅ | N/A | `sanityCheckOps("cycle",CSR)` |
| ✅ | N/A | `sanityCheckOps("cycle",CSR)` |
| ✅ | N/A | `sanityCheckOps("cycleh",CSR)` |
| ✅ | N/A | `sanityCheckOps("cycleh",CSR)` |
| ✅ | N/A | `sanityCheckOps("dcsr",CSR)` |
| ✅ | N/A | `sanityCheckOps("DIV",OP_R)` |
| ✅ | N/A | `sanityCheckOps("DIVU",OP_R)` |
| ✅ | N/A | `sanityCheckOps("DIVW",OP_R)` |
| ✅ | N/A | `sanityCheckOps("dpc",CSR)` |
| ✅ | N/A | `sanityCheckOps("dscratch0",CSR)` |
| ✅ | N/A | `sanityCheckOps("dscratch1",CSR)` |
| ✅ | N/A | `sanityCheckOps("EBREAK",OP_I)` |
| ✅ | N/A | `sanityCheckOps("ECALL",OP_I)` |
| ✅ | N/A | `sanityCheckOps("fabs.d",OP_PSEUDO)` |
| ✅ | N/A | `sanityCheckOps("fabs.s",OP_PSEUDO)` |
| ✅ | N/A | `sanityCheckOps("FADD.D",OP_R)` |
| ✅ | N/A | `sanityCheckOps("FADD.S",OP_R)` |
| ✅ | N/A | `sanityCheckOps("FCLASS.D",OP_R)` |
| ✅ | N/A | `sanityCheckOps("FCLASS.S",OP_R)` |
| ✅ | N/A | `sanityCheckOps("fcsr",CSR)` |
| ✅ | N/A | `sanityCheckOps("FCVT.D.L",OP_R)` |
| ✅ | N/A | `sanityCheckOps("FCVT.D.LU",OP_R)` |
| ✅ | N/A | `sanityCheckOps("FCVT.D.W",OP_R)` |
| ✅ | N/A | `sanityCheckOps("FCVT.D.WU",OP_R)` |
| ✅ | N/A | `sanityCheckOps("FCVT.L.D",OP_R)` |
| ✅ | N/A | `sanityCheckOps("FCVT.L.S",OP_R)` |
| ✅ | N/A | `sanityCheckOps("FCVT.LU.D",OP_R)` |
| ✅ | N/A | `sanityCheckOps("FCVT.LU.S",OP_R)` |
| ✅ | N/A | `sanityCheckOps("FCVT.S.L",OP_R)` |
| ✅ | N/A | `sanityCheckOps("FCVT.S.LU",OP_R)` |
| ✅ | N/A | `sanityCheckOps("FCVT.S.W",OP_R)` |
| ✅ | N/A | `sanityCheckOps("FCVT.S.WU",OP_R)` |
| ✅ | N/A | `sanityCheckOps("FCVT.W.D",OP_R)` |
| ✅ | N/A | `sanityCheckOps("FCVT.W.S",OP_R)` |
| ✅ | N/A | `sanityCheckOps("FCVT.WU.D",OP_R)` |
| ✅ | N/A | `sanityCheckOps("FCVT.WU.S",OP_R)` |
| ✅ | N/A | `sanityCheckOps("FDIV.D",OP_R)` |
| ✅ | N/A | `sanityCheckOps("FDIV.S",OP_R)` |
| ✅ | N/A | `sanityCheckOps("FENCE.I",OP_I)` |
| ✅ | N/A | `sanityCheckOps("FENCE",OP_I)` |
| ❌ | Test failed - see full TRX for details | `sanityCheckOps("fence",OP_PSEUDO)` |
| ✅ | N/A | `sanityCheckOps("FEQ.D",OP_R)` |
| ✅ | N/A | `sanityCheckOps("FEQ.S",OP_R)` |
| ✅ | N/A | `sanityCheckOps("fflags",CSR)` |
| ✅ | N/A | `sanityCheckOps("FLD",OP_I)` |
| ❌ | Test failed - see full TRX for details | `sanityCheckOps("fld",OP_PSEUDO)` |
| ✅ | N/A | `sanityCheckOps("FLE.D",OP_R)` |
| ✅ | N/A | `sanityCheckOps("FLE.S",OP_R)` |
| ✅ | N/A | `sanityCheckOps("FLT.D",OP_R)` |
| ✅ | N/A | `sanityCheckOps("FLT.S",OP_R)` |
| ✅ | N/A | `sanityCheckOps("FLW",OP_I)` |
| ❌ | Test failed - see full TRX for details | `sanityCheckOps("flw",OP_PSEUDO)` |
| ✅ | N/A | `sanityCheckOps("FMADD.D",OP_R)` |
| ✅ | N/A | `sanityCheckOps("FMADD.S",OP_R)` |
| ✅ | N/A | `sanityCheckOps("FMAX.D",OP_R)` |
| ✅ | N/A | `sanityCheckOps("FMAX.S",OP_R)` |
| ✅ | N/A | `sanityCheckOps("FMIN.D",OP_R)` |
| ✅ | N/A | `sanityCheckOps("FMIN.S",OP_R)` |
| ✅ | N/A | `sanityCheckOps("FMSUB.D",OP_R)` |
| ✅ | N/A | `sanityCheckOps("FMSUB.S",OP_R)` |
| ✅ | N/A | `sanityCheckOps("FMUL.D",OP_R)` |
| ✅ | N/A | `sanityCheckOps("FMUL.S",OP_R)` |
| ✅ | N/A | `sanityCheckOps("fmv.d",OP_PSEUDO)` |
| ✅ | N/A | `sanityCheckOps("FMV.D.X",OP_R)` |
| ✅ | N/A | `sanityCheckOps("fmv.s",OP_PSEUDO)` |
| ✅ | N/A | `sanityCheckOps("FMV.W.X",OP_R)` |
| ✅ | N/A | `sanityCheckOps("FMV.X.D",OP_R)` |
| ✅ | N/A | `sanityCheckOps("FMV.X.W",OP_R)` |
| ✅ | N/A | `sanityCheckOps("fneg.d",OP_PSEUDO)` |
| ✅ | N/A | `sanityCheckOps("fneg.s",OP_PSEUDO)` |
| ✅ | N/A | `sanityCheckOps("FNMADD.D",OP_R)` |
| ✅ | N/A | `sanityCheckOps("FNMADD.S",OP_R)` |
| ✅ | N/A | `sanityCheckOps("FNMSUB.D",OP_R)` |
| ✅ | N/A | `sanityCheckOps("FNMSUB.S",OP_R)` |
| ❌ | Test failed - see full TRX for details | `sanityCheckOps("FRCSR",OP_PSEUDO)` |
| ❌ | Test failed - see full TRX for details | `sanityCheckOps("FRCSR",OP_R)` |
| ❌ | Test failed - see full TRX for details | `sanityCheckOps("FRFLAGS",OP_PSEUDO)` |
| ❌ | Test failed - see full TRX for details | `sanityCheckOps("FRFLAGS",OP_R)` |
| ✅ | N/A | `sanityCheckOps("frm",CSR)` |
| ❌ | Test failed - see full TRX for details | `sanityCheckOps("FRRM",OP_PSEUDO)` |
| ❌ | Test failed - see full TRX for details | `sanityCheckOps("FRRM",OP_R)` |
| ❌ | Test failed - see full TRX for details | `sanityCheckOps("fscsr",OP_PSEUDO)` |
| ❌ | Test failed - see full TRX for details | `sanityCheckOps("FSCSR",OP_PSEUDO)` |
| ❌ | Test failed - see full TRX for details | `sanityCheckOps("FSCSR",OP_R)` |
| ❌ | Test failed - see full TRX for details | `sanityCheckOps("fsd",OP_PSEUDO)` |
| ✅ | N/A | `sanityCheckOps("FSD",OP_S)` |
| ❌ | Test failed - see full TRX for details | `sanityCheckOps("FSFLAGSI",OP_R)` |
| ❌ | Test failed - see full TRX for details | `sanityCheckOps("fsflags",OP_PSEUDO)` |
| ❌ | Test failed - see full TRX for details | `sanityCheckOps("FSFLAGS",OP_PSEUDO)` |
| ❌ | Test failed - see full TRX for details | `sanityCheckOps("FSFLAGS",OP_R)` |
| ✅ | N/A | `sanityCheckOps("FSGNJ.D",OP_R)` |
| ✅ | N/A | `sanityCheckOps("FSGNJN.D",OP_R)` |
| ✅ | N/A | `sanityCheckOps("FSGNJN.S",OP_R)` |
| ✅ | N/A | `sanityCheckOps("FSGNJ.S",OP_R)` |
| ✅ | N/A | `sanityCheckOps("FSGNJX.D",OP_R)` |
| ✅ | N/A | `sanityCheckOps("FSGNJX.S",OP_R)` |
| ✅ | N/A | `sanityCheckOps("FSQRT.D",OP_R)` |
| ✅ | N/A | `sanityCheckOps("FSQRT.S",OP_R)` |
| ❌ | Test failed - see full TRX for details | `sanityCheckOps("FSRMI",OP_R)` |
| ❌ | Test failed - see full TRX for details | `sanityCheckOps("fsrm",OP_PSEUDO)` |
| ❌ | Test failed - see full TRX for details | `sanityCheckOps("FSRM",OP_PSEUDO)` |
| ❌ | Test failed - see full TRX for details | `sanityCheckOps("FSRM",OP_R)` |
| ✅ | N/A | `sanityCheckOps("FSUB.D",OP_R)` |
| ✅ | N/A | `sanityCheckOps("FSUB.S",OP_R)` |
| ❌ | Test failed - see full TRX for details | `sanityCheckOps("fsw",OP_PSEUDO)` |
| ✅ | N/A | `sanityCheckOps("FSW",OP_S)` |
| ✅ | N/A | `sanityCheckOps("hcause",CSR)` |
| ✅ | N/A | `sanityCheckOps("hcontext",CSR)` |
| ✅ | N/A | `sanityCheckOps("hcounteren",CSR)` |
| ✅ | N/A | `sanityCheckOps("hedeleg",CSR)` |
| ✅ | N/A | `sanityCheckOps("hgatp",CSR)` |
| ✅ | N/A | `sanityCheckOps("hgeip",CSR)` |
| ✅ | N/A | `sanityCheckOps("hideleg",CSR)` |
| ✅ | N/A | `sanityCheckOps("hie",CSR)` |
| ✅ | N/A | `sanityCheckOps("hip",CSR)` |
| ✅ | N/A | `sanityCheckOps("hip",CSR)` |
| ✅ | N/A | `sanityCheckOps("hpmcounter31",CSR)` |
| ✅ | N/A | `sanityCheckOps("hpmcounter31",CSR)` |
| ✅ | N/A | `sanityCheckOps("hpmcounter31h",CSR)` |
| ✅ | N/A | `sanityCheckOps("hpmcounter31h",CSR)` |
| ✅ | N/A | `sanityCheckOps("hpmcounter3",CSR)` |
| ✅ | N/A | `sanityCheckOps("hpmcounter3",CSR)` |
| ✅ | N/A | `sanityCheckOps("hpmcounter3h",CSR)` |
| ✅ | N/A | `sanityCheckOps("hpmcounter3h",CSR)` |
| ✅ | N/A | `sanityCheckOps("hpmcounter4",CSR)` |
| ✅ | N/A | `sanityCheckOps("hpmcounter4",CSR)` |
| ✅ | N/A | `sanityCheckOps("hpmcounter4h",CSR)` |
| ✅ | N/A | `sanityCheckOps("hpmcounter4h",CSR)` |
| ✅ | N/A | `sanityCheckOps("hstatus",CSR)` |
| ✅ | N/A | `sanityCheckOps("htegie",CSR)` |
| ✅ | N/A | `sanityCheckOps("htimedelta",CSR)` |
| ✅ | N/A | `sanityCheckOps("htimedeltah",CSR)` |
| ✅ | N/A | `sanityCheckOps("htinst",CSR)` |
| ✅ | N/A | `sanityCheckOps("htinst",CSR)` |
| ✅ | N/A | `sanityCheckOps("htval2",CSR)` |
| ✅ | N/A | `sanityCheckOps("htval",CSR)` |
| ✅ | N/A | `sanityCheckOps("htval",CSR)` |
| ✅ | N/A | `sanityCheckOps("hvip",CSR)` |
| ✅ | N/A | `sanityCheckOps("instret",CSR)` |
| ✅ | N/A | `sanityCheckOps("instret",CSR)` |
| ✅ | N/A | `sanityCheckOps("instreth",CSR)` |
| ✅ | N/A | `sanityCheckOps("instreth",CSR)` |
| ✅ | N/A | `sanityCheckOps("JAL",OP_J)` |
| ❌ | Test failed - see full TRX for details | `sanityCheckOps("jal",OP_PSEUDO)` |
| ✅ | N/A | `sanityCheckOps("JALR",OP_I)` |
| ❌ | Test failed - see full TRX for details | `sanityCheckOps("jalr",OP_PSEUDO)` |
| ✅ | N/A | `sanityCheckOps("J",OP_PSEUDO)` |
| ✅ | N/A | `sanityCheckOps("JR",OP_PSEUDO)` |
| ✅ | N/A | `sanityCheckOps("LA",OP_PSEUDO)` |
| ✅ | N/A | `sanityCheckOps("LB",OP_I)` |
| ❌ | Test failed - see full TRX for details | `sanityCheckOps("LB",OP_PSEUDO)` |
| ✅ | N/A | `sanityCheckOps("LBU",OP_I)` |
| ✅ | N/A | `sanityCheckOps("LD",OP_I)` |
| ❌ | Test failed - see full TRX for details | `sanityCheckOps("LD",OP_PSEUDO)` |
| ✅ | N/A | `sanityCheckOps("LH",OP_I)` |
| ❌ | Test failed - see full TRX for details | `sanityCheckOps("LH",OP_PSEUDO)` |
| ✅ | N/A | `sanityCheckOps("LHU",OP_I)` |
| ✅ | N/A | `sanityCheckOps("li",OP_PSEUDO)` |
| ✅ | N/A | `sanityCheckOps("LLA",OP_PSEUDO)` |
| ✅ | N/A | `sanityCheckOps("LR.D",OP_R)` |
| ✅ | N/A | `sanityCheckOps("LR.W",OP_R)` |
| ✅ | N/A | `sanityCheckOps("LUI",OP_U)` |
| ✅ | N/A | `sanityCheckOps("LW",OP_I)` |
| ❌ | Test failed - see full TRX for details | `sanityCheckOps("LW",OP_PSEUDO)` |
| ✅ | N/A | `sanityCheckOps("LWU",OP_I)` |
| ✅ | N/A | `sanityCheckOps("marchid",CSR)` |
| ✅ | N/A | `sanityCheckOps("mcontext",CSR)` |
| ✅ | N/A | `sanityCheckOps("mcounteren",CSR)` |
| ✅ | N/A | `sanityCheckOps("mcountinhibit",CSR)` |
| ✅ | N/A | `sanityCheckOps("medeleg",CSR)` |
| ✅ | N/A | `sanityCheckOps("mepc",CSR)` |
| ✅ | N/A | `sanityCheckOps("mhartid",CSR)` |
| ✅ | N/A | `sanityCheckOps("mhpmevent31",CSR)` |
| ✅ | N/A | `sanityCheckOps("mhpmevent3",CSR)` |
| ✅ | N/A | `sanityCheckOps("mhpmevent4",CSR)` |
| ✅ | N/A | `sanityCheckOps("mideleg",CSR)` |
| ✅ | N/A | `sanityCheckOps("mie",CSR)` |
| ✅ | N/A | `sanityCheckOps("mimpid",CSR)` |
| ✅ | N/A | `sanityCheckOps("misa",CSR)` |
| ✅ | N/A | `sanityCheckOps("MRET",OP_R)` |
| ✅ | N/A | `sanityCheckOps("mscratch",CSR)` |
| ✅ | N/A | `sanityCheckOps("mstatus",CSR)` |
| ✅ | N/A | `sanityCheckOps("mstatush",CSR)` |
| ✅ | N/A | `sanityCheckOps("mtvec",CSR)` |
| ✅ | N/A | `sanityCheckOps("MULH",OP_R)` |
| ✅ | N/A | `sanityCheckOps("MULHSU",OP_R)` |
| ✅ | N/A | `sanityCheckOps("MULHU",OP_R)` |
| ✅ | N/A | `sanityCheckOps("MUL",OP_R)` |
| ✅ | N/A | `sanityCheckOps("MULW",OP_R)` |
| ✅ | N/A | `sanityCheckOps("mvendorid",CSR)` |
| ✅ | N/A | `sanityCheckOps("mv",OP_PSEUDO)` |
| ✅ | N/A | `sanityCheckOps("NEG",OP_PSEUDO)` |
| ✅ | N/A | `sanityCheckOps("NEGW",OP_PSEUDO)` |
| ✅ | N/A | `sanityCheckOps("NOP",OP_PSEUDO)` |
| ✅ | N/A | `sanityCheckOps("not",OP_PSEUDO)` |
| ✅ | N/A | `sanityCheckOps("ORI",OP_I)` |
| ✅ | N/A | `sanityCheckOps("OR",OP_R)` |
| ✅ | N/A | `sanityCheckOps("pmpaddr0",CSR)` |
| ✅ | N/A | `sanityCheckOps("pmpaddr1",CSR)` |
| ✅ | N/A | `sanityCheckOps("pmpaddr63",CSR)` |
| ✅ | N/A | `sanityCheckOps("pmpcfg0",CSR)` |
| ✅ | N/A | `sanityCheckOps("pmpcfg14",CSR)` |
| ✅ | N/A | `sanityCheckOps("pmpcfg15",CSR)` |
| ✅ | N/A | `sanityCheckOps("pmpcfg1",CSR)` |
| ✅ | N/A | `sanityCheckOps("pmpcfg2",CSR)` |
| ✅ | N/A | `sanityCheckOps("pmpcfg3",CSR)` |
| ❌ | Test failed - see full TRX for details | `sanityCheckOps("RDCYCLE[H]",OP_PSEUDO)` |
| ❌ | Test failed - see full TRX for details | `sanityCheckOps("RDINSTRET[H]",OP_PSEUDO)` |
| ❌ | Test failed - see full TRX for details | `sanityCheckOps("RDTIME[H]",OP_PSEUDO)` |
| ✅ | N/A | `sanityCheckOps("REM",OP_R)` |
| ✅ | N/A | `sanityCheckOps("REMU",OP_R)` |
| ✅ | N/A | `sanityCheckOps("REMUW",OP_R)` |
| ✅ | N/A | `sanityCheckOps("REMW",OP_R)` |
| ✅ | N/A | `sanityCheckOps("RET",OP_PSEUDO)` |
| ✅ | N/A | `sanityCheckOps("satp",CSR)` |
| ❌ | Test failed - see full TRX for details | `sanityCheckOps("SB",OP_PSEUDO)` |
| ✅ | N/A | `sanityCheckOps("SB",OP_S)` |
| ✅ | N/A | `sanityCheckOps("scause",CSR)` |
| ✅ | N/A | `sanityCheckOps("SC.D",OP_R)` |
| ✅ | N/A | `sanityCheckOps("scontext",CSR)` |
| ✅ | N/A | `sanityCheckOps("scounteren",CSR)` |
| ✅ | N/A | `sanityCheckOps("SC.W",OP_R)` |
| ❌ | Test failed - see full TRX for details | `sanityCheckOps("SD",OP_PSEUDO)` |
| ✅ | N/A | `sanityCheckOps("SD",OP_S)` |
| ✅ | N/A | `sanityCheckOps("sedeleg",CSR)` |
| ✅ | N/A | `sanityCheckOps("sepc",CSR)` |
| ✅ | N/A | `sanityCheckOps("seqz",OP_PSEUDO)` |
| ✅ | N/A | `sanityCheckOps("SETVL",OP_R)` |
| ✅ | N/A | `sanityCheckOps("sext.w",OP_PSEUDO)` |
| ✅ | N/A | `sanityCheckOps("SFENCE.VMA",OP_R)` |
| ✅ | N/A | `sanityCheckOps("SGTZ",OP_PSEUDO)` |
| ❌ | Test failed - see full TRX for details | `sanityCheckOps("SH",OP_PSEUDO)` |
| ✅ | N/A | `sanityCheckOps("SH",OP_S)` |
| ✅ | N/A | `sanityCheckOps("sideleg",CSR)` |
| ✅ | N/A | `sanityCheckOps("sie",CSR)` |
| ✅ | N/A | `sanityCheckOps("sip",CSR)` |
| ✅ | N/A | `sanityCheckOps("SLLI",OP_I)` |
| ✅ | N/A | `sanityCheckOps("SLLIW",OP_I)` |
| ✅ | N/A | `sanityCheckOps("SLL",OP_R)` |
| ✅ | N/A | `sanityCheckOps("SLLW",OP_R)` |
| ✅ | N/A | `sanityCheckOps("SLTI",OP_I)` |
| ✅ | N/A | `sanityCheckOps("SLTIU",OP_I)` |
| ✅ | N/A | `sanityCheckOps("SLT",OP_R)` |
| ✅ | N/A | `sanityCheckOps("SLTU",OP_R)` |
| ✅ | N/A | `sanityCheckOps("SLTZ",OP_PSEUDO)` |
| ✅ | N/A | `sanityCheckOps("SNEZ",OP_PSEUDO)` |
| ✅ | N/A | `sanityCheckOps("SRAI",OP_I)` |
| ✅ | N/A | `sanityCheckOps("SRAIW",OP_I)` |
| ✅ | N/A | `sanityCheckOps("SRA",OP_R)` |
| ✅ | N/A | `sanityCheckOps("SRAW",OP_R)` |
| ✅ | N/A | `sanityCheckOps("SRET",OP_R)` |
| ✅ | N/A | `sanityCheckOps("SRLI",OP_I)` |
| ✅ | N/A | `sanityCheckOps("SRLIW",OP_I)` |
| ✅ | N/A | `sanityCheckOps("SRL",OP_R)` |
| ✅ | N/A | `sanityCheckOps("SRLW",OP_R)` |
| ✅ | N/A | `sanityCheckOps("sscratch",CSR)` |
| ✅ | N/A | `sanityCheckOps("sstatus",CSR)` |
| ✅ | N/A | `sanityCheckOps("stval",CSR)` |
| ✅ | N/A | `sanityCheckOps("stvec",CSR)` |
| ✅ | N/A | `sanityCheckOps("SUB",OP_R)` |
| ✅ | N/A | `sanityCheckOps("SUBW",OP_R)` |
| ❌ | Test failed - see full TRX for details | `sanityCheckOps("SW",OP_PSEUDO)` |
| ✅ | N/A | `sanityCheckOps("SW",OP_S)` |
| ✅ | N/A | `sanityCheckOps("TAIL",OP_PSEUDO)` |
| ✅ | N/A | `sanityCheckOps("tdata1",CSR)` |
| ✅ | N/A | `sanityCheckOps("tdata2",CSR)` |
| ✅ | N/A | `sanityCheckOps("tdata3",CSR)` |
| ✅ | N/A | `sanityCheckOps("time",CSR)` |
| ✅ | N/A | `sanityCheckOps("timeh",CSR)` |
| ✅ | N/A | `sanityCheckOps("tselect",CSR)` |
| ✅ | N/A | `sanityCheckOps("ucause",CSR)` |
| ✅ | N/A | `sanityCheckOps("uepc",CSR)` |
| ✅ | N/A | `sanityCheckOps("uie",CSR)` |
| ✅ | N/A | `sanityCheckOps("uip",CSR)` |
| ✅ | N/A | `sanityCheckOps("uscratch",CSR)` |
| ✅ | N/A | `sanityCheckOps("ustatus",CSR)` |
| ✅ | N/A | `sanityCheckOps("utval",CSR)` |
| ✅ | N/A | `sanityCheckOps("utvec",CSR)` |
| ✅ | N/A | `sanityCheckOps("VADD",OP_R)` |
| ✅ | N/A | `sanityCheckOps("VAND",OP_R)` |
| ✅ | N/A | `sanityCheckOps("VCLASS",OP_R)` |
| ✅ | N/A | `sanityCheckOps("VCVT",OP_R)` |
| ✅ | N/A | `sanityCheckOps("VDIV",OP_R)` |
| ✅ | N/A | `sanityCheckOps("VEXTRACT",OP_R)` |
| ✅ | N/A | `sanityCheckOps("VFMADD",OP_R)` |
| ✅ | N/A | `sanityCheckOps("VFMSUB",OP_R)` |
| ✅ | N/A | `sanityCheckOps("VFNMADD",OP_R)` |
| ✅ | N/A | `sanityCheckOps("VFNMSUB",OP_R)` |
| ✅ | N/A | `sanityCheckOps("VLD",OP_I)` |
| ✅ | N/A | `sanityCheckOps("VLDS",OP_R)` |
| ✅ | N/A | `sanityCheckOps("VLDX",OP_R)` |
| ✅ | N/A | `sanityCheckOps("VMAX",OP_R)` |
| ✅ | N/A | `sanityCheckOps("VMERGE",OP_R)` |
| ✅ | N/A | `sanityCheckOps("VMIN",OP_R)` |
| ✅ | N/A | `sanityCheckOps("VMOV",OP_R)` |
| ✅ | N/A | `sanityCheckOps("VMULH",OP_R)` |
| ✅ | N/A | `sanityCheckOps("VMUL",OP_R)` |
| ✅ | N/A | `sanityCheckOps("VOR",OP_R)` |
| ✅ | N/A | `sanityCheckOps("VPANDN",OP_R)` |
| ✅ | N/A | `sanityCheckOps("VPAND",OP_R)` |
| ✅ | N/A | `sanityCheckOps("VPEQ",OP_R)` |
| ✅ | N/A | `sanityCheckOps("VPGE",OP_R)` |
| ✅ | N/A | `sanityCheckOps("VPLT",OP_R)` |
| ✅ | N/A | `sanityCheckOps("VPNE",OP_R)` |
| ✅ | N/A | `sanityCheckOps("VPNOT",OP_R)` |
| ✅ | N/A | `sanityCheckOps("VPOR",OP_R)` |
| ✅ | N/A | `sanityCheckOps("VPSWAP",OP_R)` |
| ✅ | N/A | `sanityCheckOps("VPXOR",OP_R)` |
| ✅ | N/A | `sanityCheckOps("VREM",OP_R)` |
| ✅ | N/A | `sanityCheckOps("vscause",CSR)` |
| ✅ | N/A | `sanityCheckOps("VSELECT",OP_R)` |
| ✅ | N/A | `sanityCheckOps("vsepc",CSR)` |
| ✅ | N/A | `sanityCheckOps("VSETDCFG",OP_R)` |
| ✅ | N/A | `sanityCheckOps("VSGNJN",OP_R)` |
| ✅ | N/A | `sanityCheckOps("VSGNJ",OP_R)` |
| ✅ | N/A | `sanityCheckOps("VSGNJX",OP_R)` |
| ✅ | N/A | `sanityCheckOps("vsie",CSR)` |
| ✅ | N/A | `sanityCheckOps("vsip",CSR)` |
| ✅ | N/A | `sanityCheckOps("VSLL",OP_R)` |
| ✅ | N/A | `sanityCheckOps("VSQRET",OP_R)` |
| ✅ | N/A | `sanityCheckOps("VSRA",OP_R)` |
| ✅ | N/A | `sanityCheckOps("VSRL",OP_R)` |
| ✅ | N/A | `sanityCheckOps("vsscratch",CSR)` |
| ✅ | N/A | `sanityCheckOps("vsstatus",CSR)` |
| ✅ | N/A | `sanityCheckOps("VST",OP_S)` |
| ✅ | N/A | `sanityCheckOps("VSTS",OP_R)` |
| ✅ | N/A | `sanityCheckOps("vstval",CSR)` |
| ✅ | N/A | `sanityCheckOps("vstvec",CSR)` |
| ✅ | N/A | `sanityCheckOps("VSTX",OP_R)` |
| ✅ | N/A | `sanityCheckOps("VSUB",OP_R)` |
| ✅ | N/A | `sanityCheckOps("VXOR",OP_R)` |
| ✅ | N/A | `sanityCheckOps("WFI",OP_R)` |
| ✅ | N/A | `sanityCheckOps("x0",REGISTER)` |
| ✅ | N/A | `sanityCheckOps("x10",REGISTER)` |
| ✅ | N/A | `sanityCheckOps("x11",REGISTER)` |
| ✅ | N/A | `sanityCheckOps("x12",REGISTER)` |
| ✅ | N/A | `sanityCheckOps("x13",REGISTER)` |
| ✅ | N/A | `sanityCheckOps("x14",REGISTER)` |
| ✅ | N/A | `sanityCheckOps("x15",REGISTER)` |
| ✅ | N/A | `sanityCheckOps("x16",REGISTER)` |
| ✅ | N/A | `sanityCheckOps("x17",REGISTER)` |
| ✅ | N/A | `sanityCheckOps("x18",REGISTER)` |
| ✅ | N/A | `sanityCheckOps("x19",REGISTER)` |
| ✅ | N/A | `sanityCheckOps("x1",REGISTER)` |
| ✅ | N/A | `sanityCheckOps("x20",REGISTER)` |
| ✅ | N/A | `sanityCheckOps("x21",REGISTER)` |
| ✅ | N/A | `sanityCheckOps("x22",REGISTER)` |
| ✅ | N/A | `sanityCheckOps("x23",REGISTER)` |
| ✅ | N/A | `sanityCheckOps("x24",REGISTER)` |
| ✅ | N/A | `sanityCheckOps("x25",REGISTER)` |
| ✅ | N/A | `sanityCheckOps("x26",REGISTER)` |
| ✅ | N/A | `sanityCheckOps("x27",REGISTER)` |
| ✅ | N/A | `sanityCheckOps("x28",REGISTER)` |
| ✅ | N/A | `sanityCheckOps("x29",REGISTER)` |
| ✅ | N/A | `sanityCheckOps("x2",REGISTER)` |
| ✅ | N/A | `sanityCheckOps("x30",REGISTER)` |
| ✅ | N/A | `sanityCheckOps("x31",REGISTER)` |
| ✅ | N/A | `sanityCheckOps("x3",REGISTER)` |
| ✅ | N/A | `sanityCheckOps("x4",REGISTER)` |
| ✅ | N/A | `sanityCheckOps("x5",REGISTER)` |
| ✅ | N/A | `sanityCheckOps("x6",REGISTER)` |
| ✅ | N/A | `sanityCheckOps("x7",REGISTER)` |
| ✅ | N/A | `sanityCheckOps("x8",REGISTER)` |
| ✅ | N/A | `sanityCheckOps("x9",REGISTER)` |
| ✅ | N/A | `sanityCheckOps("XORI",OP_I)` |
| ✅ | N/A | `sanityCheckOps("XOR",OP_R)` |
| ✅ | N/A | `],["SH", "x2", "0x10", "(", "x1", ...])` |
| ✅ | N/A | `],["SH", "x2", "0", "(", "x1", ...])` |
| ✅ | N/A | `TestParseAddAndSubInstructions` |
| ✅ | N/A | `TestParseAddiInstruction(addi,".text\naddi x1, x0, 0",x1,zero,0)` |
| ✅ | N/A | `TestParseAddiInstruction(addi,".text\naddi x1, x0, 1",x1,zero,1)` |
| ✅ | N/A | `TestParseAddiInstruction(addi,".text\naddi x2, x0, -1",x2,zero,-1)` |
| ✅ | N/A | `TestParseAddiInstruction(addi,".text\naddi x3, x0, 0x7f",x3,zero,127)` |
| ✅ | N/A | `TestParseAddiInstruction(addi,".text\naddi x4, x0, 0x80",tp,zero,128)` |
| ✅ | N/A | `TestParseAddiInstruction(andi,".text\nandi x1, x0, 0",x1,zero,0)` |
| ✅ | N/A | `TestParseAddiInstruction(andi,".text\nandi x1, x0, -1",x1,zero,-1)` |
| ✅ | N/A | `TestParseAddiInstruction(andi,".text\nandi x2, x3, 10",x2,x3,10)` |
| ✅ | N/A | `TestParseAddiInstruction(ori,".text\nori x1, x0, 0",x1,zero,0)` |
| ✅ | N/A | `TestParseAddiInstruction(ori,".text\nori x1, x0, -1",x1,zero,-1)` |
| ✅ | N/A | `TestParseAddiInstruction(ori,".text\nori x2, x3, 10",x2,x3,10)` |
| ✅ | N/A | `TestParseAddiInstruction(slli,".text\nslli x1, x0, 0",x1,zero,0)` |
| ✅ | N/A | `TestParseAddiInstruction(slli,".text\nslli x1, x0, 5",x1,zero,5)` |
| ✅ | N/A | `TestParseAddiInstruction(slli,".text\nslli x2, x3, 10",x2,x3,10)` |
| ✅ | N/A | `TestParseAddiInstruction(slti,".text\nslti x1, x0, 0",x1,zero,0)` |
| ✅ | N/A | `TestParseAddiInstruction(slti,".text\nslti x1, x0, -1",x1,zero,-1)` |
| ✅ | N/A | `TestParseAddiInstruction(slti,".text\nslti x2, x3, 10",x2,x3,10)` |
| ✅ | N/A | `TestParseAddiInstruction(slti,".text\nslti x4, x5, 0xff",tp,x5,255)` |
| ✅ | N/A | `TestParseAddiInstruction(slti,".text\nslti x5, x6, 0x100",x5,t1,256)` |
| ✅ | N/A | `TestParseAddiInstruction(sltiu,".text\nsltiu x1, x0, 0",x1,zero,0)` |
| ✅ | N/A | `TestParseAddiInstruction(sltiu,".text\nsltiu x1, x0, -1",x1,zero,-1)` |
| ✅ | N/A | `TestParseAddiInstruction(sltiu,".text\nsltiu x2, x3, 10",x2,x3,10)` |
| ✅ | N/A | `TestParseAddiInstruction(srli,".text\nsrli x1, x0, 0",x1,zero,0)` |
| ✅ | N/A | `TestParseAddiInstruction(srli,".text\nsrli x1, x0, 5",x1,zero,5)` |
| ✅ | N/A | `TestParseAddiInstruction(srli,".text\nsrli x2, x3, 10",x2,x3,10)` |
| ✅ | N/A | `TestParseAddiInstruction(xori,".text\nxori x1, x0, 0",x1,zero,0)` |
| ✅ | N/A | `TestParseAddiInstruction(xori,".text\nxori x1, x0, -1",x1,zero,-1)` |
| ✅ | N/A | `TestParseAddiInstruction(xori,".text\nxori x2, x3, 10",x2,x3,10)` |
| ✅ | N/A | `TestParseAddiMultiInstruction` |
| ✅ | N/A | `TestParseAddInstruction` |
| ✅ | N/A | `TestParseBImmInstruction(bne,".text\nbne x1, x2, 0",x1,x2,0)` |
| ✅ | N/A | `TestParseBImmInstruction(bne,".text\nbne x2, x3, 4",x2,x3,4)` |
| ✅ | N/A | `TestParseBImmInstruction(bne,".text\nbne x3, x4, -8",x3,tp,-8)` |
| ✅ | N/A | `TestParseBLabelInstruction(beq,".text\nbeq x1, x2, label",x1,x2,"label")` |
| ✅ | N/A | `TestParseBLabelInstruction(beq,".text\nbeq x2, x3, other_label",x2,x3,"other_label")` |
| ✅ | N/A | `TestParseBLabelInstruction(bge,".text\nbge x0, x1, some_label",zero,x1,"some_label")` |
| ✅ | N/A | `TestParseBLabelInstruction(bge,".text\nbge x1, x2, some_label",x1,x2,"some_label")` |
| ✅ | N/A | `TestParseBLabelInstruction(bge,".text\nbge x2, x3, some_label",x2,x3,"some_label")` |
| ✅ | N/A | `TestParseBLabelInstruction(bgeu,".text\nbgeu x0, x1, some_label",zero,x1,"some_label")` |
| ✅ | N/A | `TestParseBLabelInstruction(bgeu,".text\nbgeu x1, x2, some_label",x1,x2,"some_label")` |
| ✅ | N/A | `TestParseBLabelInstruction(bgeu,".text\nbgeu x2, x3, some_label",x2,x3,"some_label")` |
| ✅ | N/A | `TestParseBLabelInstruction(blt,".text\nblt x0, x1, some_label",zero,x1,"some_label")` |
| ✅ | N/A | `TestParseBLabelInstruction(blt,".text\nblt x1, x2, some_label",x1,x2,"some_label")` |
| ✅ | N/A | `TestParseBLabelInstruction(blt,".text\nblt x2, x3, some_label",x2,x3,"some_label")` |
| ✅ | N/A | `TestParseBLabelInstruction(bltu,".text\nbltu x0, x1, some_label",zero,x1,"some_label")` |
| ✅ | N/A | `TestParseBLabelInstruction(bltu,".text\nbltu x1, x2, some_label",x1,x2,"some_label")` |
| ✅ | N/A | `TestParseBLabelInstruction(bltu,".text\nbltu x2, x3, some_label",x2,x3,"some_label")` |
| ✅ | N/A | `TestParseJTypeImmInstruction(jal,".text\njal x1, 0x50",x1,80)` |
| ✅ | N/A | `TestParseJTypeImmInstruction(jal,".text\njal x1, 5",x1,5)` |
| ✅ | N/A | `TestParseJTypeImmInstruction(jal,".text\njal x2, 0",x2,0)` |
| ✅ | N/A | `TestParseJTypeImmInstruction(jal,".text\njal x3, -8",x3,-8)` |
| ✅ | N/A | `TestParseJTypeLabelInstruction(jal,".text\njal x1, some_label",x1,"some_label")` |
| ✅ | N/A | `TestParseJTypeLabelInstruction(jal,".text\njal x2, some_label2",x2,"some_label2")` |
| ✅ | N/A | `TestParseJTypeLabelInstruction(jal,".text\njal x3, some_label5",x3,"some_label5")` |
| ✅ | N/A | `TestParseRInstruction(add,x1,x2,x3)` |
| ✅ | N/A | `TestParseRInstruction(and,t2,fp,s1)` |
| ✅ | N/A | `TestParseRInstruction(or,a0,a1,x12)` |
| ✅ | N/A | `TestParseRInstruction(sub,tp,x5,t1)` |
| ✅ | N/A | `TestParseSbInstruction` |
| ✅ | N/A | `TestParseShInstruction` |
| ✅ | N/A | `TestParseSubInstruction` |
| ✅ | N/A | `TestParseSwInstruction` |
| ✅ | N/A | `TestParseUTypeInstruction(auipc,".text\nauipc x1, 0",x1,0)` |
| ✅ | N/A | `TestParseUTypeInstruction(auipc,".text\nauipc x1, 0x52345",x1,336709)` |
| ✅ | N/A | `TestParseUTypeInstruction(lui,".text\nlui x1, 0",x1,0)` |
| ✅ | N/A | `TestParseUTypeInstruction(lui,".text\nlui x1, 0x12345",x1,74565)` |
| ✅ | N/A | `text\nld x1, 0(x0)",x1,zero,0)` |
| ✅ | N/A | `text\nld x1, 0x7ff(x0)",x1,zero,2047)` |
| ✅ | N/A | `text\nld x1, 0x7f(x0)",x1,zero,127)` |
| ✅ | N/A | `text\nld x1, 0x80(x0)",x1,zero,128)` |
| ❌ | Test failed - see full TRX for details | `text\n    li      a5,0      # should result in 0x00000793u\n    lui     a0,10     # should result in 0x00010537u\n    ret               # should result in 0x00008067u\n    auipc   gp,0x2    # should result in 0x00002197u\n    sub     a2,a2,a0  # should result in 0x40a60633u\n    li      a1,0      # should result in 0x00000593u\n    auipc   a0,0      # should result in 0x00000517u\n    lw      a0,0(sp)  # should result in 0x00012503u\n    addi    a1,sp,8   # should result in 0x00810593u\n    li      a2,0      # should result in 0x00000613u\n    addi    sp,sp,-16 # should result in 0xff010113u\n    sd      s0,0(sp)  # should result in 0x00813023u\n",[1939, 66871, 32871, 8599, 1084622387, ...])` |
| ✅ | N/A | `tokenPositions(".option push\n.option nopic",[DIRECTIVE, WHITESPACE, IDENTIFIER, EOL, DIRECTIVE, ...],[0, 0, 0, 0, 1, ...],[0, 7, 8, 12, 0, ...],[".option", " ", "push", "\n", ".option", ...])` |

---
*Generated on Fri Jun 20 01:17:38 AM CDT 2025*

📁 **Full results**: [Kuick.xml](./Kuick.xml)
