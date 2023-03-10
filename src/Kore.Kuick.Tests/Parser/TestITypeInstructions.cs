using Kore.AST;
using Kore.RiscMeta;
using NUnit.Framework;

namespace Kore.Kuick.Tests.Parser {
    public class TestITypeInstructions {
        [SetUp]
        public void Setup() {
        }

        [Test]
        [TestCase(RiscMeta.Instructions.TypeI.addi, ".text\naddi x1, x0, 0", Register.x1, Register.x0, 0)]
        [TestCase(RiscMeta.Instructions.TypeI.addi, ".text\naddi x1, x0, 1", Register.x1, Register.x0, 1)]
        [TestCase(RiscMeta.Instructions.TypeI.addi, ".text\naddi x2, x0, -1", Register.x2, Register.x0, -1)]
        [TestCase(RiscMeta.Instructions.TypeI.addi, ".text\naddi x3, x0, 0x7f", Register.x3, Register.x0, 0x7f)]
        [TestCase(RiscMeta.Instructions.TypeI.addi, ".text\naddi x4, x0, 0x80", Register.x4, Register.x0, 0x80)]
        [TestCase(RiscMeta.Instructions.TypeI.slti, ".text\nslti x1, x0, 0", Register.x1, Register.x0, 0)]
        [TestCase(RiscMeta.Instructions.TypeI.slti, ".text\nslti x1, x0, -1", Register.x1, Register.x0, -1)]
        [TestCase(RiscMeta.Instructions.TypeI.slti, ".text\nslti x2, x3, 10", Register.x2, Register.x3, 10)]
        [TestCase(RiscMeta.Instructions.TypeI.slti, ".text\nslti x4, x5, 0xff", Register.x4, Register.x5, 0xff)]
        [TestCase(RiscMeta.Instructions.TypeI.slti, ".text\nslti x5, x6, 0x100", Register.x5, Register.x6, 0x100)]
        [TestCase(RiscMeta.Instructions.TypeI.sltiu, ".text\nsltiu x1, x0, 0", Register.x1, Register.x0, 0)]
        [TestCase(RiscMeta.Instructions.TypeI.sltiu, ".text\nsltiu x1, x0, -1", Register.x1, Register.x0, -1)]
        [TestCase(RiscMeta.Instructions.TypeI.sltiu, ".text\nsltiu x2, x3, 10", Register.x2, Register.x3, 10)]
        [TestCase(RiscMeta.Instructions.TypeI.xori, ".text\nxori x1, x0, 0", Register.x1, Register.x0, 0)]
        [TestCase(RiscMeta.Instructions.TypeI.xori, ".text\nxori x1, x0, -1", Register.x1, Register.x0, -1)]
        [TestCase(RiscMeta.Instructions.TypeI.xori, ".text\nxori x2, x3, 10", Register.x2, Register.x3, 10)]
        [TestCase(RiscMeta.Instructions.TypeI.ori, ".text\nori x1, x0, 0", Register.x1, Register.x0, 0)]
        [TestCase(RiscMeta.Instructions.TypeI.ori, ".text\nori x1, x0, -1", Register.x1, Register.x0, -1)]
        [TestCase(RiscMeta.Instructions.TypeI.ori, ".text\nori x2, x3, 10", Register.x2, Register.x3, 10)]
        [TestCase(RiscMeta.Instructions.TypeI.andi, ".text\nandi x1, x0, 0", Register.x1, Register.x0, 0)]
        [TestCase(RiscMeta.Instructions.TypeI.andi, ".text\nandi x1, x0, -1", Register.x1, Register.x0, -1)]
        [TestCase(RiscMeta.Instructions.TypeI.andi, ".text\nandi x2, x3, 10", Register.x2, Register.x3, 10)]
        [TestCase(RiscMeta.Instructions.TypeI.slli, ".text\nslli x1, x0, 0", Register.x1, Register.x0, 0)]
        [TestCase(RiscMeta.Instructions.TypeI.slli, ".text\nslli x1, x0, 5", Register.x1, Register.x0, 5)]
        [TestCase(RiscMeta.Instructions.TypeI.slli, ".text\nslli x2, x3, 10", Register.x2, Register.x3, 10)]
        [TestCase(RiscMeta.Instructions.TypeI.srli, ".text\nsrli x1, x0, 0", Register.x1, Register.x0, 0)]
        [TestCase(RiscMeta.Instructions.TypeI.srli, ".text\nsrli x1, x0, 5", Register.x1, Register.x0, 5)]
        [TestCase(RiscMeta.Instructions.TypeI.srli, ".text\nsrli x2, x3, 10", Register.x2, Register.x3, 10)]

        public void TestParseAddiInstruction(RiscMeta.Instructions.TypeI opType, string input, Register expectedRd, Register expectedRs1, int expectedImmediate) {
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
            Assert.IsInstanceOf<InstructionNodeTypeI>(rInstruction);
            var instruction = (InstructionNodeTypeI)rInstruction;
            Assert.AreEqual(opType, instruction.op);
            Assert.AreEqual(expectedRd, instruction.rd);
            Assert.AreEqual(expectedRs1, instruction.rs);
            Assert.AreEqual(expectedImmediate, instruction.immediate);
        }


        [Test]
        public void TestParseAddiMultiInstruction() {
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
