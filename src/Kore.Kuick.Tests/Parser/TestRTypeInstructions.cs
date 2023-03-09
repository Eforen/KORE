using Kore.AST;
using Kore.RiscMeta;
using NUnit.Framework;

namespace Kore.Kuick.Tests.Parser {
    public class TestRTypeInstructions {
        [SetUp]
        public void Setup() {
        }

        [Test]
        public void TestParseAddInstruction() {
            string input =
                ".text\n" +
                "   add x1, x2, x3\n";

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
            Assert.IsInstanceOf<InstructionNodeTypeR>(rInstruction);
            var addInstruction = (InstructionNodeTypeR)rInstruction;
            Assert.AreEqual(RiscMeta.Instructions.TypeR.add, addInstruction.op);
            Assert.AreEqual(Register.x1, addInstruction.rd);
            Assert.AreEqual(Register.x2, addInstruction.rs1);
            Assert.AreEqual(Register.x3, addInstruction.rs2);
        }

        [Test]
        public void TestParseSubInstruction() {
            string input =
                ".text\n" +
                "   sub x1, x2, x3\n";

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
            Assert.IsInstanceOf<InstructionNodeTypeR>(rInstruction);
            var subInstruction = (InstructionNodeTypeR)rInstruction;
            Assert.AreEqual(RiscMeta.Instructions.TypeR.sub, subInstruction.op);
            Assert.AreEqual(Register.x1, subInstruction.rd);
            Assert.AreEqual(Register.x2, subInstruction.rs1);
            Assert.AreEqual(Register.x3, subInstruction.rs2);
        }

        [Test]
        public void TestParseAddAndSubInstructions() {
            string input =
                ".text\n" +
                "   add x1, x2, x3\n" +
                "   sub x4, x5, x6\n";

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

            var addInstruction = textSection.Contents[0] as InstructionNodeTypeR;
            Assert.IsNotNull(addInstruction);
            Assert.AreEqual(RiscMeta.Instructions.TypeR.add, addInstruction.op);
            Assert.AreEqual(Register.x1, addInstruction.rd);
            Assert.AreEqual(Register.x2, addInstruction.rs1);
            Assert.AreEqual(Register.x3, addInstruction.rs2);

            var subInstruction = textSection.Contents[1] as InstructionNodeTypeR;
            Assert.IsNotNull(subInstruction);
            Assert.AreEqual(RiscMeta.Instructions.TypeR.sub, subInstruction.op);
            Assert.AreEqual(Register.x4, subInstruction.rd);
            Assert.AreEqual(Register.x5, subInstruction.rs1);
            Assert.AreEqual(Register.x6, subInstruction.rs2);
        }


        [TestCase(RiscMeta.Instructions.TypeR.add, Register.x1, Register.x2, Register.x3)]
        [TestCase(RiscMeta.Instructions.TypeR.sub, Register.x4, Register.x5, Register.x6)]
        [TestCase(RiscMeta.Instructions.TypeR.and, Register.x7, Register.x8, Register.x9)]
        [TestCase(RiscMeta.Instructions.TypeR.or, Register.x10, Register.x11, Register.x12)]
        public void TestParseRInstruction(RiscMeta.Instructions.TypeR op, Register rd, Register rs1, Register rs2) {
            string input = $".text\n   {op.ToString().ToLower()} {rd.ToString().ToLower()}, {rs1.ToString().ToLower()}, {rs2.ToString().ToLower()}\n";

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
            Assert.IsInstanceOf<InstructionNodeTypeR>(rInstruction);
            var parsedInstruction = (InstructionNodeTypeR)rInstruction;
            Assert.AreEqual(op, parsedInstruction.op);
            Assert.AreEqual(rd, parsedInstruction.rd);
            Assert.AreEqual(rs1, parsedInstruction.rs1);
            Assert.AreEqual(rs2, parsedInstruction.rs2);
        }
    }
}
