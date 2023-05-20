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
        [Test]
        [TestCase("nop", "addi x0, x0, 0", "No operation")]
        [TestCase("neg x1, x2", "sub x1, x0, x2", "Two's complement negation")]
        [TestCase("neg x2, x1", "sub x2, x0, x1", "Two's complement negation")]
        [TestCase("neg x3, x3", "sub x3, x0, x3", "Two's complement negation")]
        [TestCase("negw x1, x2", "subw x1, x0, x2", "Two's complement word negation")]
        [TestCase("negw x2, x1", "subw x2, x0, x1", "Two's complement word negation")]
        [TestCase("negw x3, x3", "subw x3, x0, x3", "Two's complement word negation")]
        [TestCase("snez x1, x2", "sltu x1, x0, x2", "Set if not equal zero")]
        [TestCase("snez x2, x1", "sltu x2, x0, x1", "Set if not equal zero")]
        [TestCase("snez x3, x3", "sltu x3, x0, x3", "Set if not equal zero")]
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
