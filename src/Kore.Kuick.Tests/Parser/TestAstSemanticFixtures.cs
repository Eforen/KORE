using System.Collections.Generic;
using System.IO;
using System.Linq;
using Kore.AST;
using Kore.Kuick;
using Kore.RiscMeta;
using Kore.RiscMeta.Instructions;
using NUnit.Framework;
using KuickParser = Kore.Kuick.Parser;

namespace Kore.Kuick.Tests.Parser {
    /// <summary>
    /// Structural checks on parsed AST independent of full <see cref="AstNode.getDebugText"/> goldens.
    /// Complements semantic review per <c>AstFixtures/REVIEW_PROCESS.md</c>.
    /// </summary>
    [TestFixture]
    public class TestAstSemanticFixtures {
        private static string FixtureSourcePath(string basename) =>
            Path.Combine(TestContext.CurrentContext.TestDirectory, "Parser", "AstFixtures", basename + ".S");

        private static ProgramNode ParseFixture(string basename) {
            var lexer = new Lexer();
            lexer.Load(File.ReadAllText(FixtureSourcePath(basename)));
            return KuickParser.Parse(lexer);
        }

        private static IEnumerable<RelocationInstructionNode> CollectRelocations(ProgramNode program) =>
            program.Sections.SelectMany(s => s.Contents).OfType<RelocationInstructionNode>();

        [Test]
        public void Alignment_directives_StoresAlignBytes_AlignExponent_P2Align_And_BalignAbsolute() {
            var program = ParseFixture("alignment_directives");

            Assert.That(program.Sections, Has.Count.EqualTo(1));
            Assert.That(program.Sections[0].Name, Is.EqualTo(".text"));

            var boundaries = program.Sections[0].Contents.OfType<AlignmentNode>().Select(a => a.Bytes).ToArray();
            // `.align 3` / `.p2align 2` → 2^n bytes; `.balign 2` → absolute 2-byte boundary (parser docs)
            Assert.That(boundaries, Is.EqualTo(new[] { 8, 4, 2 }));
        }

        [Test]
        public void Preamble_fixture_HasFileLevelDirectivesThenTextSection() {
            var program = ParseFixture("preamble_fixture");

            Assert.That(program.Preamble.Any(n => n is DirectiveNode || n is CommentNode || n is SymbolDirectiveNode));
            Assert.That(program.Sections, Has.Count.EqualTo(1));
            Assert.That(program.Sections[0].Name, Is.EqualTo(".text"));
        }

        [Test]
        public void Realistic002_AstStructureMatchesRealistic001ExceptSourceStrings() {
            var a = ParseFixture("realistic001");
            var b = ParseFixture("realistic002");

            Assert.That(b.Sections.Select(s => s.Name).ToArray(), Is.EqualTo(a.Sections.Select(s => s.Name).ToArray()));
            Assert.That(b.SymbolTable.Count, Is.EqualTo(a.SymbolTable.Count));

            static string NormalizeDump(ProgramNode p) {
                var t = p.getDebugText();
                t = t.Replace("example.s", "SHARED", System.StringComparison.Ordinal)
                     .Replace("realistic002.S", "SHARED", System.StringComparison.Ordinal)
                     .Replace("# example.s - Comprehensive", "# SHARED header", System.StringComparison.Ordinal)
                     .Replace("# realistic002.S - Comprehensive", "# SHARED header", System.StringComparison.Ordinal);
                return t;
            }

            Assert.That(NormalizeDump(b), Is.EqualTo(NormalizeDump(a)));
        }

        [Test]
        public void Csr_instructions_CSR_immediates_match_privileged_numbers() {
            var program = ParseFixture("csr_instructions");

            Assert.That(program.Sections, Has.Count.EqualTo(1));
            var instrs = program.Sections[0].Contents.OfType<InstructionNodeTypeI>().ToArray();
            Assert.That(instrs, Has.Length.EqualTo(5));

            Assert.Multiple(() => {
                Assert.That(instrs[0].op, Is.EqualTo(TypeI.csrrs));
                Assert.That(instrs[0].immediate, Is.EqualTo(3072)); // cycle

                Assert.That(instrs[1].op, Is.EqualTo(TypeI.csrrs));
                Assert.That(instrs[1].immediate, Is.EqualTo(3073)); // time

                Assert.That(instrs[2].op, Is.EqualTo(TypeI.csrrs));
                Assert.That(instrs[2].immediate, Is.EqualTo(3201)); // timeh

                Assert.That(instrs[3].op, Is.EqualTo(TypeI.csrrwi));
                Assert.That(instrs[3].immediate, Is.EqualTo(3072));

                Assert.That(instrs[4].op, Is.EqualTo(TypeI.csrrs));
                Assert.That(instrs[4].immediate, Is.EqualTo(3072));
            });
        }

        [Test]
        public void Labels_and_calls_Relocation_sequence_matches_la_and_call_expansion() {
            var program = ParseFixture("labels_and_calls");

            var relocs = CollectRelocations(program).ToArray();
            Assert.That(relocs, Has.Length.EqualTo(4));

            Assert.That(relocs.Select(r => r.RelocationKind).ToArray(), Is.EqualTo(new[] {
                RiscvRelocationType.R_RISCV_PCREL_HI20,
                RiscvRelocationType.R_RISCV_PCREL_LO12_I,
                RiscvRelocationType.R_RISCV_PCREL_HI20,
                RiscvRelocationType.R_RISCV_PCREL_LO12_I,
            }));

            Assert.That(relocs.Select(r => r.SymbolName).ToArray(), Is.EqualTo(new[] {
                "some_data",
                "some_data",
                "my_function",
                "my_function",
            }));

            Assert.That(program.SymbolTable.Count, Is.EqualTo(4));
        }

        [TestCase("rodata_and_bss", ".word", 0)]
        [TestCase("rodata_and_bss_space", ".space", 4)]
        [TestCase("rodata_and_bss_zero", ".zero", 4)]
        public void Rodata_bss_trilogy_Shared_layout_last_bss_directive_differs(string basename, string expectedDirective, int expectedIntArg) {
            var program = ParseFixture(basename);

            Assert.That(program.Sections.Select(s => s.Name).ToArray(),
                Is.EqualTo(new[] { ".text", ".data", ".rodata", ".bss" }));

            var bss = program.Sections.Single(s => s.Name == ".bss");
            var last = bss.Contents.OfType<DirectiveNode>().LastOrDefault();
            Assert.That(last, Is.Not.Null);
            Assert.That(last!.Name, Is.EqualTo(expectedDirective));

            var intDir = last as IntDirectiveNode;
            Assert.That(intDir, Is.Not.Null);
            Assert.That(intDir!.Value, Is.EqualTo(expectedIntArg));
        }

        [Test]
        public void Preamble_fixture_Global_symbols_from_globl_and_global() {
            var program = ParseFixture("preamble_fixture");

            var globals = program.Preamble.OfType<SymbolDirectiveNode>().Select(s => s.SymbolName).ToArray();
            Assert.That(globals, Is.EqualTo(new[] { "main", "other_sym" }));

            Assert.That(program.SymbolTable.GetSymbol("main").Scope, Is.EqualTo(SymbolScope.Global));
            Assert.That(program.SymbolTable.GetSymbol("other_sym").Scope, Is.EqualTo(SymbolScope.Global));
        }

        [Test]
        public void Data_section_basic_Text_then_data_words_and_string() {
            var program = ParseFixture("data_section_basic");

            Assert.That(program.Sections.Select(s => s.Name).ToArray(),
                Is.EqualTo(new[] { ".text", ".data" }));

            var data = program.Sections[1];
            var words = data.Contents.OfType<IntDirectiveNode>().Where(d => d.Name == ".word").Select(d => d.Value).ToArray();
            Assert.That(words, Is.EqualTo(new[] { 42, 4096 }));
            Assert.That(data.Contents.OfType<StringDirectiveNode>().Count, Is.EqualTo(1));
        }

        [Test]
        public void Global_before_labels_Global_directive_before_label_definitions() {
            var program = ParseFixture("global_before_labels");

            Assert.That(program.Sections[0].Contents[0], Is.InstanceOf<SymbolDirectiveNode>());
            var sd = (SymbolDirectiveNode)program.Sections[0].Contents[0];
            Assert.That(sd.Type, Is.EqualTo(SymbolDirectiveNode.DirectiveType.Global));
            Assert.That(sd.SymbolName, Is.EqualTo("entry"));

            Assert.That(program.SymbolTable.GetSymbol("entry").Scope, Is.EqualTo(SymbolScope.Global));
            Assert.That(program.SymbolTable.GetSymbol("after_entry").Scope, Is.EqualTo(SymbolScope.Local));
        }

        [Test]
        public void Duplicate_local_labels_Second_block_renamed_in_symbol_table() {
            var program = ParseFixture("duplicate_local_labels");

            var names = program.SymbolTable.GetAllSymbols().Select(s => s.Name).OrderBy(n => n, System.StringComparer.Ordinal).ToArray();
            Assert.That(names, Is.EqualTo(new[] { "1Lblock", "block" }));

            var jals = program.Sections[0].Contents.OfType<InstructionNodeTypeJLabel>().ToArray();
            Assert.That(jals, Has.Length.EqualTo(2));
            Assert.That(jals.All(j => j.op == TypeJ.jal && j.rd == Register.x0 && j.label == "block"), Is.True);
        }

        [Test]
        public void Rodata_and_bss_La_msg_emits_pcrel_hi20_and_lo12_i() {
            var program = ParseFixture("rodata_and_bss");

            var relocs = CollectRelocations(program).ToArray();
            Assert.That(relocs, Has.Length.EqualTo(2));
            Assert.That(relocs[0].RelocationKind, Is.EqualTo(RiscvRelocationType.R_RISCV_PCREL_HI20));
            Assert.That(relocs[1].RelocationKind, Is.EqualTo(RiscvRelocationType.R_RISCV_PCREL_LO12_I));
            Assert.That(relocs.Select(r => r.SymbolName).Distinct().Single(), Is.EqualTo("msg"));
        }
    }
}
