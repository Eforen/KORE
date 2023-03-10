using Kore.AST;
using Kore.RiscMeta;
using NUnit.Framework;

namespace Kore.Kuick.Tests.Parser {
    public class TestUTypeInstructions {
        [SetUp]
        public void Setup() {
        }

        [Test]
        [TestCase(RiscMeta.Instructions.TypeU.lui, ".text\nlui x1, 0", Register.x1, 0)]
        [TestCase(RiscMeta.Instructions.TypeU.lui, ".text\nlui x1, 0x12345", Register.x1, 0x12345)]
        [TestCase(RiscMeta.Instructions.TypeU.auipc, ".text\nauipc x1, 0", Register.x1, 0)]
        [TestCase(RiscMeta.Instructions.TypeU.auipc, ".text\nauipc x1, 0x52345", Register.x1, 0x52345)]
        public void TestParseUTypeInstruction(RiscMeta.Instructions.TypeU opType, string input, Register expectedRd, int expectedImmediate) {
            // Setup the lexer and parse the input into tokens
            var lexer = new Lexer();
            lexer.Load(input);

            // Setup the parser and parse the tokens into an AST
            var ast = Kore.Kuick.Parser.Parse(lexer);

            // Verify that the AST matches what we expect
            Assert.IsInstanceOf<ProgramNode>(ast);
            var programNode = (ProgramNode)ast;
            Assert.AreEqual(1, programNode.Sections.Count);

            var textSection = programNode.Sections[0];
            Assert.IsInstanceOf<SectionNode>(textSection);
            Assert.AreEqual(".text", textSection.Name);

            var rInstruction = textSection.Contents[0];
            Assert.IsInstanceOf<InstructionNodeTypeU>(rInstruction);
            var instruction = (InstructionNodeTypeU)rInstruction;
            Assert.AreEqual(opType, instruction.op);
            Assert.AreEqual(expectedRd, instruction.rd);
            Assert.AreEqual(expectedImmediate, instruction.imm);
        }


    }
}
