using Kore.AST;
using Kore.RiscMeta;
using NUnit.Framework;

namespace Kore.Kuick.Tests.Parser {
    public class TestITypeInstructions {
        [SetUp]
        public void Setup() {
        }

        [Test]
        public void TestParseAddiInstruction() {
            string input =
                ".text\n" +
                "   addi x1, x2, 123\n" + //Inst 1
                "   addi x1, x2, 0x123\n"; //Inst 2

            // Setup the lexer and parse the input into tokens
            var lexer = new Lexer();
            lexer.Load(input);

            // Setup the parser and parse the tokens into an AST
            //var parser = new Parser();
            var ast = Kore.Kuick.Parser.Parse(lexer);

            // Verify that the AST matches what we expect
            Assert.IsInstanceOf<ProgramNode>(ast);
            var programNode = (ProgramNode)ast;
            Assert.AreEqual(1, programNode.Sections.Count);

            var textSection = programNode.Sections[0];
            Assert.IsInstanceOf<SectionNode>(textSection);
            Assert.AreEqual(".text", textSection.Name);

            // Inst 1
            var iInstruction1 = textSection.Contents[0];
            Assert.IsInstanceOf<InstructionNodeTypeI>(iInstruction1);
            var addiInstruction1 = (InstructionNodeTypeI)iInstruction1;
            Assert.AreEqual(RiscMeta.Instructions.TypeI.addi, addiInstruction1.op);
            Assert.AreEqual(Register.x1, addiInstruction1.rd);
            Assert.AreEqual(Register.x2, addiInstruction1.rs);
            Assert.AreEqual(123, addiInstruction1.immediate);

            // Inst 2
            var iInstruction2 = textSection.Contents[0];
            Assert.IsInstanceOf<InstructionNodeTypeI>(iInstruction2);
            var addiInstruction2 = (InstructionNodeTypeI)iInstruction2;
            Assert.AreEqual(RiscMeta.Instructions.TypeI.addi, addiInstruction2.op);
            Assert.AreEqual(Register.x1, addiInstruction2.rd);
            Assert.AreEqual(Register.x2, addiInstruction2.rs);
            Assert.AreEqual(123, addiInstruction2.immediate);
        }

    }
}
