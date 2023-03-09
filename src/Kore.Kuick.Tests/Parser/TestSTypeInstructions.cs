using Kore.AST;
using Kore.RiscMeta;
using NUnit.Framework;

namespace Kore.Kuick.Tests.Parser {
    class TestSTypeInstructions {
        [SetUp]
        public void Setup() {
        }

        [Test]
        public void TestParseSbInstruction() {
            string input =
                ".text\n" +
                "   sb x1, 4(x2)\n";

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

            var sInstruction = textSection.Contents[0];
            Assert.IsInstanceOf<InstructionNodeTypeS>(sInstruction);
            var sbInstruction = (InstructionNodeTypeS)sInstruction;
            Assert.AreEqual(RiscMeta.Instructions.TypeS.sb, sbInstruction.op);
            Assert.AreEqual(Register.x1, sbInstruction.rs2);
            Assert.AreEqual(4, sbInstruction.imm);
            Assert.AreEqual(Register.x2, sbInstruction.rs1);
        }

        [Test]
        public void TestParseShInstruction() {
            string input =
                ".text\n" +
                "   sh  x2, 0(x1)\n";

            var lexer = new Lexer();
            lexer.Load(input);

            var ast = Kore.Kuick.Parser.Parse(lexer);

            Assert.IsInstanceOf<ProgramNode>(ast);
            var programNode = (ProgramNode)ast;
            Assert.AreEqual(1, programNode.Sections.Count);

            var textSection = programNode.Sections[0];
            Assert.IsInstanceOf<SectionNode>(textSection);
            Assert.AreEqual(".text", textSection.Name);

            var shInstruction = textSection.Contents[0];
            Assert.IsInstanceOf<InstructionNodeTypeS>(shInstruction);
            var sh = (InstructionNodeTypeS)shInstruction;
            Assert.AreEqual(RiscMeta.Instructions.TypeS.sh, sh.op);
            Assert.AreEqual(Register.x2, sh.rs2);
            Assert.AreEqual(Register.x1, sh.rs1);
            Assert.AreEqual(0, sh.imm);
        }

        [Test]
        public void TestParseSwInstruction() {
            string input =
                ".text\n" +
                "   sw x1, 4(x2)\n";

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

            var sInstruction = textSection.Contents[0];
            Assert.IsInstanceOf<InstructionNodeTypeS>(sInstruction);
            var swInstruction = (InstructionNodeTypeS)sInstruction;
            Assert.AreEqual(RiscMeta.Instructions.TypeS.sw, swInstruction.op);
            Assert.AreEqual(Register.x1, swInstruction.rs2);
            Assert.AreEqual(Register.x2, swInstruction.rs1);
            Assert.AreEqual(4, swInstruction.imm);
        }

    }
}
