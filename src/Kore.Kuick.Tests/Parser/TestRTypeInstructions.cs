/*
*/
using Kore.AST;
using Kore.RiscMeta;
using NUnit.Framework;

namespace Kore.Kuick.Tests {
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
            //var parser = new Parser();
            var ast = Kore.Kuick.Parser.Parse(lexer);

            // Verify that the AST matches what we expect
            Assert.IsInstanceOf<ProgramNode>(ast);
            var programNode = (ProgramNode)ast;
            Assert.AreEqual(1, programNode.Sections.Count);

            var textSection = programNode.Sections[0];
            Assert.IsInstanceOf<SectionNode>(textSection);
            Assert.AreEqual(".text", textSection.Name);

            /*
            var startLabel = textSection.Contents[0];
            Assert.IsInstanceOf<LabelNode>(startLabel);
            Assert.AreEqual("_start", ((LabelNode)startLabel).Name);
             */

            var rInstruction = textSection.Contents[0];
            Assert.IsInstanceOf<InstructionNodeTypeR>(rInstruction);
            var addInstruction = (InstructionNodeTypeR)rInstruction;
            Assert.AreEqual(RiscMeta.Instructions.TypeR.add, addInstruction.op);
            Assert.AreEqual(Register.x1, addInstruction.rd);
            Assert.AreEqual(Register.x2, addInstruction.rs1);
            Assert.AreEqual(Register.x3, addInstruction.rs2);
        }

    }
}