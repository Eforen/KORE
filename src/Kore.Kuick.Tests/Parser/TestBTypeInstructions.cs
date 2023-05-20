using Kore.AST;
using Kore.RiscMeta;
using NUnit.Framework;

namespace Kore.Kuick.Tests.Parser {
    public class TestBTypeInstructions {
        [SetUp]
        public void Setup() {
        }

        [Test]
        [TestCase(RiscMeta.Instructions.TypeB.beq, ".text\nbeq x1, x2, label", Register.x1, Register.x2, "label")]
        [TestCase(RiscMeta.Instructions.TypeB.beq, ".text\nbeq x2, x3, other_label", Register.x2, Register.x3, "other_label")]
        [TestCase(RiscMeta.Instructions.TypeB.blt, ".text\nblt x0, x1, some_label", Register.x0, Register.x1, "some_label")]
        [TestCase(RiscMeta.Instructions.TypeB.blt, ".text\nblt x1, x2, some_label", Register.x1, Register.x2, "some_label")]
        [TestCase(RiscMeta.Instructions.TypeB.blt, ".text\nblt x2, x3, some_label", Register.x2, Register.x3, "some_label")]
        [TestCase(RiscMeta.Instructions.TypeB.bge, ".text\nbge x0, x1, some_label", Register.x0, Register.x1, "some_label")]
        [TestCase(RiscMeta.Instructions.TypeB.bge, ".text\nbge x1, x2, some_label", Register.x1, Register.x2, "some_label")]
        [TestCase(RiscMeta.Instructions.TypeB.bge, ".text\nbge x2, x3, some_label", Register.x2, Register.x3, "some_label")]
        [TestCase(RiscMeta.Instructions.TypeB.bltu, ".text\nbltu x0, x1, some_label", Register.x0, Register.x1, "some_label")]
        [TestCase(RiscMeta.Instructions.TypeB.bltu, ".text\nbltu x1, x2, some_label", Register.x1, Register.x2, "some_label")]
        [TestCase(RiscMeta.Instructions.TypeB.bltu, ".text\nbltu x2, x3, some_label", Register.x2, Register.x3, "some_label")]
        [TestCase(RiscMeta.Instructions.TypeB.bgeu, ".text\nbgeu x0, x1, some_label", Register.x0, Register.x1, "some_label")]
        [TestCase(RiscMeta.Instructions.TypeB.bgeu, ".text\nbgeu x1, x2, some_label", Register.x1, Register.x2, "some_label")]
        [TestCase(RiscMeta.Instructions.TypeB.bgeu, ".text\nbgeu x2, x3, some_label", Register.x2, Register.x3, "some_label")]
        public void TestParseBLabelInstruction(RiscMeta.Instructions.TypeB opType, string input, Register expectedRs1, Register expectedRs2, string expectedLabel) {
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
            Assert.IsInstanceOf<InstructionNodeTypeBLabel>(rInstruction);
            var instruction = (InstructionNodeTypeBLabel)rInstruction;
            Assert.AreEqual(opType, instruction.op);
            Assert.AreEqual(expectedRs1, instruction.rs1);
            Assert.AreEqual(expectedRs2, instruction.rs2);
            Assert.AreEqual(expectedLabel, instruction.label);
        }

        [Test]
        [TestCase(RiscMeta.Instructions.TypeB.bne, ".text\nbne x1, x2, 0", Register.x1, Register.x2, 0)]
        [TestCase(RiscMeta.Instructions.TypeB.bne, ".text\nbne x2, x3, 4", Register.x2, Register.x3, 4)]
        [TestCase(RiscMeta.Instructions.TypeB.bne, ".text\nbne x3, x4, -8", Register.x3, Register.x4, -8)]
        public void TestParseBImmInstruction(RiscMeta.Instructions.TypeB opType, string input, Register expectedRs1, Register expectedRs2, int expectedImm) {
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
            Assert.IsInstanceOf<InstructionNodeTypeBImmediate>(rInstruction);
            var instruction = (InstructionNodeTypeBImmediate)rInstruction;
            Assert.AreEqual(opType, instruction.op);
            Assert.AreEqual(expectedRs1, instruction.rs1);
            Assert.AreEqual(expectedRs2, instruction.rs2);
            Assert.AreEqual(expectedImm, instruction.imm);
        }

    }
}
