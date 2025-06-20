using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kore.AST;
using Kore.RiscMeta;
using NUnit.Framework;

namespace Kore.Kuick.Tests.Parser {

    class TestPsudoInstructions {

        // Test Pseudo Instructions in code generator.
        /*
        * [KUICK][PARSER] Implement Pseudo Instruction: `nop`
        * [KUICK][PARSER] Implement Pseudo Instruction: `neg`

        * [KUICK][PARSER] Implement Pseudo Instruction: `snez`
        * [KUICK][PARSER] Implement Pseudo Instruction: `sltz`
        * [KUICK][PARSER] Implement Pseudo Instruction: `sgtz`

        * [KUICK][PARSER] Implement Pseudo Instruction: `beqz`
        * [KUICK][PARSER] Implement Pseudo Instruction: `bnez`
        * [KUICK][PARSER] Implement Pseudo Instruction: `blez`
        * [KUICK][PARSER] Implement Pseudo Instruction: `bgez`
        * [KUICK][PARSER] Implement Pseudo Instruction: `bltz`
        * [KUICK][PARSER] Implement Pseudo Instruction: `bgtz`

        * [KUICK][PARSER] Implement Pseudo Instruction: `seqz`
        */
        [Test]
        [TestCase("nop", "addi x0, x0, 0", "No operation")]
        [TestCase("neg x1, x2", "sub x1, x0, x2", "Two's complement negation")]
        [TestCase("neg x2, x1", "sub x2, x0, x1", "Two's complement negation")]
        [TestCase("neg x3, x3", "sub x3, x0, x3", "Two's complement negation")]
        // TODO: RV64I
        // [TestCase("negw x1, x2", "subw x1, x0, x2", "Two's complement word negation")]
        // [TestCase("negw x2, x1", "subw x2, x0, x1", "Two's complement word negation")]
        // [TestCase("negw x3, x3", "subw x3, x0, x3", "Two's complement word negation")]
        ///////////////////////////////////////////////////////////////////////////////
        [TestCase("snez x1, x2", "sltu x1, x0, x2", "Set if not equal zero")]
        [TestCase("snez x2, x1", "sltu x2, x0, x1", "Set if not equal zero")]
        [TestCase("snez x3, x3", "sltu x3, x0, x3", "Set if not equal zero")]
        [TestCase("sltz x1, x2", "slt x1, x2, x0", "Set if less than zero")]
        [TestCase("sltz x2, x1", "slt x2, x1, x0", "Set if less than zero")]
        [TestCase("sltz x3, x3", "slt x3, x3, x0", "Set if less than zero")]
        [TestCase("sgtz x1, x2", "slt x1, x0, x2", "Set if greater than zero")]
        [TestCase("sgtz x2, x1", "slt x2, x0, x1", "Set if greater than zero")]
        [TestCase("sgtz x3, x3", "slt x3, x0, x3", "Set if greater than zero")]
        ///////////////////////////////////////////////////////////////////////////////
        [TestCase("beqz x1, 0x00000003", "beq x1, x0, 0x00000003", "Branch if equal zero")]
        [TestCase("beqz x2, 0x00000002", "beq x2, x0, 0x00000002", "Branch if equal zero")]
        [TestCase("beqz x3, 0x00000001", "beq x3, x0, 0x00000001", "Branch if equal zero")]
        [TestCase("bnez x1, 0x00000003", "bne x1, x0, 0x00000003", "Branch if not equal zero")]
        [TestCase("bnez x2, 0x00000002", "bne x2, x0, 0x00000002", "Branch if not equal zero")]
        [TestCase("bnez x3, 0x00000001", "bne x3, x0, 0x00000001", "Branch if not equal zero")]
        [TestCase("blez x1, 0x00000003", "bge x0, x1, 0x00000003", "Branch if less than or equal zero")]
        [TestCase("blez x2, 0x00000002", "bge x0, x2, 0x00000002", "Branch if less than or equal zero")]
        [TestCase("blez x3, 0x00000001", "bge x0, x3, 0x00000001", "Branch if less than or equal zero")]
        [TestCase("bgez x1, 0x00000003", "bge x1, x0, 0x00000003", "Branch if greater than or equal zero")]
        [TestCase("bgez x2, 0x00000002", "bge x2, x0, 0x00000002", "Branch if greater than or equal zero")]
        [TestCase("bgez x3, 0x00000001", "bge x3, x0, 0x00000001", "Branch if greater than or equal zero")]
        [TestCase("bltz x1, 0x00000003", "blt x1, x0, 0x00000003", "Branch if less than zero")]
        [TestCase("bltz x2, 0x00000002", "blt x2, x0, 0x00000002", "Branch if less than zero")]
        [TestCase("bltz x3, 0x00000001", "blt x3, x0, 0x00000001", "Branch if less than zero")]
        [TestCase("bgtz x1, 0x00000003", "blt x0, x1, 0x00000003", "Branch if greater than zero")]
        [TestCase("bgtz x2, 0x00000002", "blt x0, x2, 0x00000002", "Branch if greater than zero")]
        ///////////////////////////////////////////////////////////////////////////////
        [TestCase("j 0x8", "jal x0, 0x8", "Jump 8 bytes")]
        [TestCase("j 0x16", "jal x0, 0x8", "Jump 16 bytes")]
        [TestCase("jr x1", "jalr x0, 0(x1)", "Jump register")]
        [TestCase("jr x7", "jalr x0, 0(x7)", "Jump register")]
        [TestCase("ret", "jalr x0, 0(x1)", "Return from subroutine")]
        ///////////////////////////////////////////////////////////////////////////////
        [TestCase("tail 0x74543765", "auipc x6, 0x74543000\n    jal x0, 0x765", "Tail call arr-away subroutine")] // 0xFFFFF000 is offset[31:12] and 0x00000FFF is offset[11:0]
        [TestCase("tail 0x4262fb3f", "auipc x6, 0x4262f000\n    jal x0, 0xb3f", "Tail call arr-away subroutine")] // 0xFFFFF000 is offset[31:12] and 0x00000FFF is offset[11:0]
        [TestCase("tail 0x81C45C2C", "auipc x6, 0x81C45000\n    jal x0, 0xC2C", "Tail call arr-away subroutine")] // 0xFFFFF000 is offset[31:12] and 0x00000FFF is offset[11:0]
        ///////////////////////////////////////////////////////////////////////////////
        [TestCase("rdinstret x1", "csrrs x1, instret, x0", "Read instruction count")]
        [TestCase("rdinstret x2", "csrrs x2, instret, x0", "Read instruction count")]
        [TestCase("rdinstret x3", "csrrs x3, instret, x0", "Read instruction count")]
        [TestCase("rdinstreth x1", "csrrs x1, instreth, x0", "Read instruction count")]
        [TestCase("rdinstreth x2", "csrrs x2, instreth, x0", "Read instruction count")]
        [TestCase("rdinstreth x3", "csrrs x3, instreth, x0", "Read instruction count")]
        [TestCase("rdcycle x1", "csrrs x1, cycle, x0", "Read cycle count")]
        [TestCase("rdcycle x2", "csrrs x2, cycle, x0", "Read cycle count")]
        [TestCase("rdcycle x3", "csrrs x3, cycle, x0", "Read cycle count")]
        [TestCase("rdcycleh x1", "csrrs x1, cycleh, x0", "Read cycle count")]
        [TestCase("rdcycleh x2", "csrrs x2, cycleh, x0", "Read cycle count")]
        [TestCase("rdcycleh x3", "csrrs x3, cycleh, x0", "Read cycle count")]
        [TestCase("rdtime x1", "csrrs x1, time, x0", "Read time")]
        [TestCase("rdtime x2", "csrrs x2, time, x0", "Read time")]
        [TestCase("rdtime x3", "csrrs x3, time, x0", "Read time")]
        [TestCase("rdtimeh x1", "csrrs x1, timeh, x0", "Read time")]
        [TestCase("rdtimeh x2", "csrrs x2, timeh, x0", "Read time")]
        [TestCase("rdtimeh x3", "csrrs x3, timeh, x0", "Read time")]
        ///////////////////////////////////////////////////////////////////////////////
        [TestCase("csrr x1, 0x7C0",    "csrrs x1, 0x7C0, x0",    "Read custom CSR")]
        [TestCase("csrr x2, instret",  "csrrs x2, instret, x0",  "Read instruction count CSR")]
        [TestCase("csrr x3, cycle",    "csrrs x3, cycle, x0",    "Read cycle count CSR")]
        [TestCase("csrr x4, time",     "csrrs x4, time, x0",     "Read time CSR")]
        [TestCase("csrr x5, 0x7C1",    "csrrs x5, 0x7C1, x0",    "Read custom CSR")]
        [TestCase("csrr x6, instreth", "csrrs x6, instreth, x0", "Read instruction count CSR")]
        [TestCase("csrr x7, cycleh",   "csrrs x7, cycleh, x0",   "Read cycle count CSR")]
        [TestCase("csrr x8, timeh",    "csrrs x8, timeh, x0",    "Read time CSR")]
        [TestCase("csrw 0x7C0, x1",    "csrrw x0, 0x7C0, x1",    "Write custom CSR")]
        [TestCase("csrw instret, x2",  "csrrw x0, instret, x2",  "Write instruction count CSR")]
        [TestCase("csrw cycle, x3",    "csrrw x0, cycle, x3",    "Write cycle count CSR")]
        [TestCase("csrw time, x4",     "csrrw x0, time, x4",     "Write time CSR")]
        [TestCase("csrs 0x7C0, x5",    "csrrs x0, 0x7C0, x5",    "Set custom CSR")]
        [TestCase("csrs instret, x6",  "csrrs x0, instret, x6",  "Set instruction count CSR")]
        [TestCase("csrs cycle, x7",    "csrrs x0, cycle, x7",    "Set cycle count CSR")]
        [TestCase("csrs time, x8",     "csrrs x0, time, x8",     "Set time CSR")]
        [TestCase("csrc 0x7C0, x1",    "csrrc x0, 0x7C0, x1",    "Clear custom CSR")]
        [TestCase("csrc instret, x2",  "csrrc x0, instret, x2",  "Clear instruction count CSR")]
        [TestCase("csrc cycle, x3",    "csrrc x0, cycle, x3",    "Clear cycle count CSR")]
        [TestCase("csrc time, x4",     "csrrc x0, time, x4",     "Clear time CSR")]
        ///////////////////////////////////////////////////////////////////////////////
        [TestCase("csrwi 0x7C0, 0x1",    "csrrwi x0, 0x7C0, 0x1",    "Write custom CSR (immediate)")]
        [TestCase("csrwi instret, 0x2",  "csrrwi x0, instret, 0x2",  "Write instruction count CSR (immediate)")]
        [TestCase("csrwi cycle, 0x3",    "csrrwi x0, cycle, 0x3",    "Write cycle count CSR (immediate)")]
        [TestCase("csrwi time, 0x4",     "csrrwi x0, time, 0x4",     "Write time CSR (immediate)")]
        [TestCase("csrsi 0x7C0, 0x5",    "csrrsi x0, 0x7C0, 0x5",    "Set custom CSR (immediate)")]
        [TestCase("csrsi instret, 0x6",  "csrrsi x0, instret, 0x6",  "Set instruction count CSR (immediate)")]
        [TestCase("csrsi cycle, 0x7",    "csrrsi x0, cycle, 0x7",    "Set cycle count CSR (immediate)")]
        [TestCase("csrsi time, 0x8",     "csrrsi x0, time, 0x8",     "Set time CSR (immediate)")]
        [TestCase("csrci 0x7C0, 0x1",    "csrrci x0, 0x7C0, 0x1",    "Clear custom CSR (immediate)")]
        [TestCase("csrci instret, 0x2",  "csrrci x0, instret, 0x2",  "Clear instruction count CSR (immediate)")]
        [TestCase("csrci cycle, 0x3",    "csrrci x0, cycle, 0x3",    "Clear cycle count CSR (immediate)")]
        [TestCase("csrci time, 0x4",     "csrrci x0, time, 0x4",     "Clear time CSR (immediate)")]
        ///////////////////////////////////////////////////////////////////////////////
        [TestCase("frcsr x1", "csrrs x1, fcsr, x0", "Read floating point control/status register")]
        [TestCase("fscsr x2", "csrrs x0, fcsr, x2", "Set floating point control/status register")]
        ///////////////////////////////////////////////////////////////////////////////
        [TestCase("frrm x3",  "csrrs x3, frm, x0",    "Read floating point rounding mode CSR")]
        [TestCase("fsrm x4",  "csrrs x0, frm, x4",    "Set floating point rounding mode CSR")]
        ///////////////////////////////////////////////////////////////////////////////
        [TestCase("frflags x5", "csrrs x5, fflags, x0", "Read floating point flags CSR")]
        [TestCase("fsflags x6", "csrrs x0, fflags, x6", "Set floating point flags CSR")]
        ///////////////////////////////////////////////////////////////////////////////
        // TODO: This whole section is wrong and needs symbol support
        [TestCase("lla x1, myVar", "auipc x1, %pcrel_hi(myVar)\naddi x1, x1, %pcrel_lo(myVar)", "Load address into x1")]
        [TestCase("lla x2, myVar1", "auipc x2, %pcrel_hi(myVar1)\naddi x2, x2, %pcrel_lo(myVar1)", "Load address into x2")]
        [TestCase("lla x3, myVar152", "auipc x3, %pcrel_hi(myVar152)\naddi x3, x3, %pcrel_lo(myVar152)", "Load address into x3")]
        ///////////////////////////////////////////////////////////////////////////////
        // la rd, symbol has 2 options one is for Position Independent Code (PIC) and the other is for Position Dependent Code (PDC or Non-PIC)
        // Because the Kuick Compiler currently does not support a linker, it cannot know if the code is PIC or PDC, so it will always use the PDC version for now
        // The PDC version of this instruction is the same as the lla instruction
        [TestCase("la x1, myVar",    "auipc x1, %pcrel_hi(myVar)\n   addi x1, x1, %pcrel_lo(myVar)",    "Load address into x1")]
        [TestCase("la x2, myVar1",   "auipc x2, %pcrel_hi(myVar1)\n  addi x2, x2, %pcrel_lo(myVar1)",   "Load address into x2")]
        [TestCase("la x3, myVar152", "auipc x3, %pcrel_hi(myVar152)\naddi x3, x3, %pcrel_lo(myVar152)", "Load address into x3")]
        ///////////////////////////////////////////////////////////////////////////////
        [TestCase("lb x1, myVar",    "auipc x1, %pcrel_hi(myVar)\n   lb x1, %pcrel_lo(myVar)(x1)",      "Load byte")]
        [TestCase("lb x2, myVar1",   "auipc x2, %pcrel_hi(myVar1)\n  lb x2, %pcrel_lo(myVar1)(x2)",     "Load byte")]
        [TestCase("lb x3, myVar152", "auipc x3, %pcrel_hi(myVar152)\nlb x3, %pcrel_lo(myVar152)(x3)",   "Load byte")]
        [TestCase("lh x1, myVar",    "auipc x1, %pcrel_hi(myVar)\n   lh x1, %pcrel_lo(myVar)(x1)",      "Load halfword")]
        [TestCase("lh x2, myVar1",   "auipc x2, %pcrel_hi(myVar1)\n  lh x2, %pcrel_lo(myVar1)(x2)",     "Load halfword")]
        [TestCase("lh x3, myVar152", "auipc x3, %pcrel_hi(myVar152)\nlh x3, %pcrel_lo(myVar152)(x3)",   "Load halfword")]
        [TestCase("lw x1, myVar",    "auipc x1, %pcrel_hi(myVar)\n   lw x1, %pcrel_lo(myVar)(x1)",      "Load word")]
        [TestCase("lw x2, myVar1",   "auipc x2, %pcrel_hi(myVar1)\n  lw x2, %pcrel_lo(myVar1)(x2)",     "Load word")]
        [TestCase("lw x3, myVar152", "auipc x3, %pcrel_hi(myVar152)\nlw x3, %pcrel_lo(myVar152)(x3)",   "Load word")]
        [TestCase("ld x1, myVar",    "auipc x1, %pcrel_hi(myVar)\n   ld x1, %pcrel_lo(myVar)(x1)",      "Load doubleword")]
        [TestCase("ld x2, myVar1",   "auipc x2, %pcrel_hi(myVar1)\n  ld x2, %pcrel_lo(myVar1)(x2)",     "Load doubleword")]
        [TestCase("ld x3, myVar152", "auipc x3, %pcrel_hi(myVar152)\nld x3, %pcrel_lo(myVar152)(x3)",   "Load doubleword")]
        ///////////////////////////////////////////////////////////////////////////////
        [TestCase("sb x1, myVar, t1",    "auipc t1, %pcrel_hi(myVar)\n   sb x1, %pcrel_lo(myVar)(t1)",      "Store byte")]
        [TestCase("sb x2, myVar1, t2",   "auipc t2, %pcrel_hi(myVar1)\n  sb x2, %pcrel_lo(myVar1)(t2)",     "Store byte")]
        [TestCase("sb x3, myVar152, t3", "auipc t3, %pcrel_hi(myVar152)\nsb x3, %pcrel_lo(myVar152)(t3)",   "Store byte")]
        [TestCase("sh x1, myVar, t1",    "auipc t1, %pcrel_hi(myVar)\n   sh x1, %pcrel_lo(myVar)(t1)",      "Store halfword")]
        [TestCase("sh x2, myVar1, t2",   "auipc t2, %pcrel_hi(myVar1)\n  sh x2, %pcrel_lo(myVar1)(t2)",     "Store halfword")]
        [TestCase("sh x3, myVar152, t3", "auipc t3, %pcrel_hi(myVar152)\nsh x3, %pcrel_lo(myVar152)(t3)",   "Store halfword")]
        [TestCase("sw x1, myVar, t1",    "auipc t1, %pcrel_hi(myVar)\n   sw x1, %pcrel_lo(myVar)(t1)",      "Store word")]
        [TestCase("sw x2, myVar1, t2",   "auipc t2, %pcrel_hi(myVar1)\n  sw x2, %pcrel_lo(myVar1)(t2)",     "Store word")]
        [TestCase("sw x3, myVar152, t3", "auipc t3, %pcrel_hi(myVar152)\nsw x3, %pcrel_lo(myVar152)(t3)",   "Store word")]
        [TestCase("sd x1, myVar, t1",    "auipc t1, %pcrel_hi(myVar)\n   sd x1, %pcrel_lo(myVar)(t1)",      "Store doubleword")]
        [TestCase("sd x2, myVar1, t2",   "auipc t2, %pcrel_hi(myVar1)\n  sd x2, %pcrel_lo(myVar1)(t2)",     "Store doubleword")]
        [TestCase("sd x3, myVar152, t3", "auipc t3, %pcrel_hi(myVar152)\nsd x3, %pcrel_lo(myVar152)(t3)",   "Store doubleword")]
        ///////////////////////////////////////////////////////////////////////////////
        [TestCase("flw f1, myVar",    "auipc x1, %pcrel_hi(myVar)\n   flw f1, %pcrel_lo(myVar)(x1)",    "Load word")]
        [TestCase("flw f2, myVar1",   "auipc x2, %pcrel_hi(myVar1)\n  flw f2, %pcrel_lo(myVar1)(x2)",   "Load word")]
        [TestCase("flw f3, myVar152", "auipc x3, %pcrel_hi(myVar152)\nflw f3, %pcrel_lo(myVar152)(x3)", "Load word")]
        [TestCase("fld f1, myVar",    "auipc x1, %pcrel_hi(myVar)\n   fld f1, %pcrel_lo(myVar)(x1)",    "Load doubleword")]
        [TestCase("fld f2, myVar1",   "auipc x2, %pcrel_hi(myVar1)\n  fld f2, %pcrel_lo(myVar1)(x2)",   "Load doubleword")]
        [TestCase("fld f3, myVar152", "auipc x3, %pcrel_hi(myVar152)\nfld f3, %pcrel_lo(myVar152)(x3)", "Load doubleword")]
        ///////////////////////////////////////////////////////////////////////////////
        [TestCase("fsw f1, myVar",    "auipc x1, %pcrel_hi(myVar)\n   fsw f1, %pcrel_lo(myVar)(x1)",    "Store word")]
        [TestCase("fsw f2, myVar1",   "auipc x2, %pcrel_hi(myVar1)\n  fsw f2, %pcrel_lo(myVar1)(x2)",   "Store word")]
        [TestCase("fsw f3, myVar152", "auipc x3, %pcrel_hi(myVar152)\nfsw f3, %pcrel_lo(myVar152)(x3)", "Store word")]
        [TestCase("fsd f1, myVar",    "auipc x1, %pcrel_hi(myVar)\n   fsd f1, %pcrel_lo(myVar)(x1)",    "Store doubleword")]
        [TestCase("fsd f2, myVar1",   "auipc x2, %pcrel_hi(myVar1)\n  fsd f2, %pcrel_lo(myVar1)(x2)",   "Store doubleword")]
        [TestCase("fsd f3, myVar152", "auipc x3, %pcrel_hi(myVar152)\nfsd f3, %pcrel_lo(myVar152)(x3)", "Store doubleword")]
        ///////////////////////////////////////////////////////////////////////////////
        [TestCase("li x1, 0", "addi x1, x0, 0", "Load immediate")]
        [TestCase("li x2, 5", "addi x2, x0, 5", "Load immediate")]
        [TestCase("li x3, 2047", "addi x3, x0, 2047", "Load immediate")]

        public void PseudoInstructions(string pseudoInstruction, string trueInstruction, string description) {

            // Setup the lexer and parse the input into tokens
            var lexer1 = new Lexer();
            lexer1.Load(".text\n" + pseudoInstruction);
            var lexer2 = new Lexer();
            lexer2.Load(".text\n" + trueInstruction);

            // Setup the parser and parse the tokens into an AST
            var ast1 = Kore.Kuick.Parser.Parse(lexer1);
            var ast2 = Kore.Kuick.Parser.Parse(lexer2);

            Assert.AreEqual(ast1.getDebugText(), ast2.getDebugText());

            // Check that the AST of the true instruction matches the AST of the pseudo instruction
            //Assert.AreEqual(ast2.Sections[0].Contents[0], ast1.Sections[0].Contents[0]);
            // Fallback Check
            //Assert.AreEqual(ast2, ast1);

            // bool checking = true;
            // while(checking) {
            //     checking = false;
            //     Assert.AreEqual(ast2.Sections.Count, ast1.Sections.Count);
            //     Assert.AreEqual(ast2.Sections[0].ToString(), "");
            // }
        }
    }
}
