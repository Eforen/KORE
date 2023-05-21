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
        [TestCase("bltz x3, 0x00000001", "blt x0, x3, 0x00000001", "Branch if less than zero")]
        [TestCase("bgtz x1, 0x00000003", "blt x0, x1, 0x00000003", "Branch if greater than zero")]
        [TestCase("bgtz x2, 0x00000002", "blt x0, x2, 0x00000002", "Branch if greater than zero")]
        ///////////////////////////////////////////////////////////////////////////////

        public void PseudoInstructions(string pseudoInstruction, string trueInstruction, string description) {

            // Setup the lexer and parse the input into tokens
            var lexer1 = new Lexer();
            lexer1.Load(".text\n" + pseudoInstruction);
            var lexer2 = new Lexer();
            lexer2.Load(".text\n" + trueInstruction);

            // Setup the parser and parse the tokens into an AST
            var ast1 = Kore.Kuick.Parser.Parse(lexer1);
            var ast2 = Kore.Kuick.Parser.Parse(lexer2);

            // Check that the AST of the true instruction matches the AST of the pseudo instruction
            Assert.AreEqual(ast2.Sections[0].Contents[0], ast1.Sections[0].Contents[0]);
            // Fallback Check
            Assert.AreEqual(ast2, ast1);

            // bool checking = true;
            // while(checking) {
            //     checking = false;
            //     Assert.AreEqual(ast2.Sections.Count, ast1.Sections.Count);
            //     Assert.AreEqual(ast2.Sections[0].ToString(), "");
            // }
        }
    }
}
