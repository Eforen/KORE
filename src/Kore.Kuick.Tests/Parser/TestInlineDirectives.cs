using Kore.AST;
using Kore.RiscMeta;
using NUnit.Framework;

namespace Kore.Kuick.Tests.Parser {
    class TestInlineDirectives {

        [Test]
        [TestCase("hi", Kore.AST.InlineDirectiveNode.InlineDirectiveType.HI, "lo", Kore.AST.InlineDirectiveNode.InlineDirectiveType.LO, "myVar", RiscvRelocationType.R_RISCV_HI20, RiscvRelocationType.R_RISCV_LO12_I)]
        [TestCase("pcrel_hi", Kore.AST.InlineDirectiveNode.InlineDirectiveType.PCREL_HI, "pcrel_lo", Kore.AST.InlineDirectiveNode.InlineDirectiveType.PCREL_LO, "myVar", RiscvRelocationType.R_RISCV_PCREL_HI20, RiscvRelocationType.R_RISCV_PCREL_LO12_I)]
        public void InlineDirectives(string var1, Kore.AST.InlineDirectiveNode.InlineDirectiveType type1, string var2, Kore.AST.InlineDirectiveNode.InlineDirectiveType type2, string label, RiscvRelocationType expectedHi, RiscvRelocationType expectedLo) {
            // lower example ".text\nauipc t1, %hi(myVar)\nld t1, %lo(myVar)(t1)"
            RunInlineTest($".text\nauipc t1, %{var1.ToLower()}({label})\nld t1, %{var2.ToLower()}({label})(t1)", type1, type2, label, expectedHi, expectedLo);
            // upper example ".text\nauipc t1, %HI(myVar)\nld t1, %LO(myVar)(t1)"
            RunInlineTest($".text\nauipc t1, %{var1.ToUpper()}({label})\nld t1, %{var2.ToUpper()}({label})(t1)", type1, type2, label, expectedHi, expectedLo);
            // 1st example ".text\nauipc t1, %hi(myVar)\nld t1, %LO(myVar)(t1)"
            RunInlineTest($".text\nauipc t1, %{var1.ToLower()}({label})\nld t1, %{var2.ToUpper()}({label})(t1)", type1, type2, label, expectedHi, expectedLo);
            // 2nd example ".text\nauipc t1, %HI(myVar)\nld t1, %lo(myVar)(t1)"
            RunInlineTest($".text\nauipc t1, %{var1.ToUpper()}({label})\nld t1, %{var2.ToLower()}({label})(t1)", type1, type2, label, expectedHi, expectedLo);
        }

        public void RunInlineTest(string test, Kore.AST.InlineDirectiveNode.InlineDirectiveType primary, Kore.AST.InlineDirectiveNode.InlineDirectiveType secondary, string label, RiscvRelocationType expectedHi, RiscvRelocationType expectedLo){
            // Setup the lexer and parse the input into tokens
            var lexer = new Lexer();
            lexer.Load(test);

            // Setup the parser and parse the tokens into an AST
            var ast = Kore.Kuick.Parser.Parse(lexer);

            // Verify that the AST matches what we expect
            Assert.IsInstanceOf<ProgramNode>(ast);
            var programNode = (ProgramNode)ast;
            Assert.AreEqual(1, programNode.Sections.Count);

            var textSection = programNode.Sections[0];
            Assert.IsInstanceOf<SectionNode>(textSection);
            Assert.AreEqual(".text", textSection.Name);

            // Check first instruction (eg: `auipc t1, %hi(myVar)`)
            var auipcWrapper = textSection.Contents[0];
            Assert.IsInstanceOf<RelocationInstructionNode<InstructionNodeTypeU>>(auipcWrapper);
            var instruction = (RelocationInstructionNode<InstructionNodeTypeU>)auipcWrapper;
            Assert.AreEqual(expectedHi, instruction.RelocationKind);
            Assert.AreEqual(label, instruction.SymbolName);
            Assert.AreEqual(1u, instruction.SymbolId);

            // Check the wrapped instruction
            Assert.IsInstanceOf<InstructionNodeTypeU>(instruction.WrappedInstruction);
            var wrappedInstruction = (InstructionNodeTypeU)instruction.WrappedInstruction;
            Assert.AreEqual(Kore.RiscMeta.Instructions.TypeU.auipc, wrappedInstruction.op);
            Assert.AreEqual(Kore.RiscMeta.Register.t1, wrappedInstruction.rd);
            Assert.AreEqual(0, wrappedInstruction.imm);

            // Check second instruction (eg: `ld t1, %lo(myVar)(t1)`)
            var ldWrapper = textSection.Contents[1];
            Assert.IsInstanceOf<RelocationInstructionNode<InstructionNodeTypeI>>(ldWrapper);
            var ldInstruction = (RelocationInstructionNode<InstructionNodeTypeI>)ldWrapper;
            Assert.AreEqual(expectedLo, ldInstruction.RelocationKind);
            Assert.AreEqual(label, ldInstruction.SymbolName);
            Assert.AreEqual(1u, ldInstruction.SymbolId);

            // Check the wrapped instruction
            Assert.IsInstanceOf<InstructionNodeTypeI>(ldInstruction.WrappedInstruction);
            var wrappedLdInstruction = (InstructionNodeTypeI)ldInstruction.WrappedInstruction;
            Assert.AreEqual(Kore.RiscMeta.Instructions.TypeI.ld, wrappedLdInstruction.op);
            Assert.AreEqual(Kore.RiscMeta.Register.t1, wrappedLdInstruction.rd);
            Assert.AreEqual(Kore.RiscMeta.Register.t1, wrappedLdInstruction.rs);
            Assert.AreEqual(0, wrappedLdInstruction.immediate);

        }
    }
}
