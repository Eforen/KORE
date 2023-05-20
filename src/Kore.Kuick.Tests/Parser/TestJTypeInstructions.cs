using Kore.AST;
using Kore.RiscMeta;
using NUnit.Framework;

namespace Kore.Kuick.Tests.Parser {
    [TestFixture]
    public class TestJTypeInstructions {
        [SetUp]
        public void Setup() {
        }

        [Test]
        [TestCase(RiscMeta.Instructions.TypeJ.jal, ".text\njal x1, some_label", Register.x1, "some_label")]
        [TestCase(RiscMeta.Instructions.TypeJ.jal, ".text\njal x2, some_label2", Register.x2, "some_label2")]
        [TestCase(RiscMeta.Instructions.TypeJ.jal, ".text\njal x3, some_label5", Register.x3, "some_label5")]
        public void TestParseJTypeLabelInstruction(RiscMeta.Instructions.TypeJ opType, string input, Register expectedRd, string expectedLabel) {
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
            Assert.IsInstanceOf<InstructionNodeTypeJLabel>(rInstruction);
            var instruction = (InstructionNodeTypeJLabel)rInstruction;
            Assert.AreEqual(opType, instruction.op);
            Assert.AreEqual(expectedRd, instruction.rd);
            Assert.AreEqual(expectedLabel, instruction.label);
        }



        [Test]
        [TestCase(RiscMeta.Instructions.TypeJ.jal, ".text\njal x1, 0x50", Register.x1, 0x50)]
        [TestCase(RiscMeta.Instructions.TypeJ.jal, ".text\njal x1, 5", Register.x1, 5)]
        [TestCase(RiscMeta.Instructions.TypeJ.jal, ".text\njal x2, 0", Register.x2, 0)]
        [TestCase(RiscMeta.Instructions.TypeJ.jal, ".text\njal x3, -8", Register.x3, -8)]
        public void TestParseJTypeImmInstruction(RiscMeta.Instructions.TypeJ opType, string input, Register expectedRd, int expected) {
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
            Assert.IsInstanceOf<InstructionNodeTypeJImmediate>(rInstruction);
            var instruction = (InstructionNodeTypeJImmediate)rInstruction;
            Assert.AreEqual(opType, instruction.op);
            Assert.AreEqual(expectedRd, instruction.rd);
            Assert.AreEqual(expected, instruction.imm);
        }
    }
}
