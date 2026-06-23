using System;
using System.IO;
using System.Text;
using Kore.AST;
using NUnit.Framework;

namespace Kore.Kuick.Tests.Assembler {
    /// <summary>
    /// For each <c>Assembler/AssemblyFixtures/*.S</c> file, parses + assembles the source and asserts
    /// that a textual rendering of the resulting object (sections, bytes, relocations) matches the
    /// sibling <c>*.spec</c> file (same base name).
    ///
    /// Spec format (one per fixture):
    /// <code>
    /// OBJECT Sections:[N] Symbols:[M]{
    ///     SECTION[i] {name} Align:{a} Size:{s} Relocs:{r} {
    ///         0000: bb bb bb bb
    ///         0004: bb bb bb bb
    ///         RELOC[k] {Type} OFFSET:0x{off} SYMBOL:{sym} ADDEND:{a}[ RELATED:{label}]
    ///     }
    /// }
    /// </code>
    /// Hex bytes are lowercase, little-endian (i.e. the order they appear on disk).
    /// </summary>
    [TestFixture]
    public class TestAssemblerObjectSpecs {
        private static string AssemblyFixturesDir =>
            Path.Combine(TestContext.CurrentContext.TestDirectory, "Assembler", "AssemblyFixtures");

        // One [TestCase] per fixture base-name. Each value names the pair of files the test
        // will load: <name>.S and <name>.spec, both under Assembler/AssemblyFixtures/.
        // The case label also becomes the test name in the explorer (SpecFileMatchesAssembledObject(...)).
        [TestCase("add_basic")]
        [TestCase("addi_basic")]
        [TestCase("add_then_addi")]
        public void SpecFileMatchesAssembledObject(string fixtureName) {
            var sPath    = Path.Combine(AssemblyFixturesDir, fixtureName + ".S");
            var specPath = Path.Combine(AssemblyFixturesDir, fixtureName + ".spec");

            Assert.That(File.Exists(sPath), Is.True,
                $"Missing assembly fixture: {sPath}");
            Assert.That(File.Exists(specPath), Is.True,
                $"Missing spec for fixture '{fixtureName}': expected {specPath}");

            var source = File.ReadAllText(sPath);
            var expected = NormalizeNewlines(File.ReadAllText(specPath));

            var lexer = new Lexer();
            lexer.Load(source);
            var ast = (ProgramNode)Kore.Kuick.Parser.Parse(lexer);

            // Same-instance pattern: AssemblerContext is mutated in-place by Assemble(),
            // so we can read it back through the same reference we passed in.
            var ctx = new Kore.Kuick.Assembler.AssemblerContext();
            var asm = new Kore.Kuick.Assembler.Assembler(ctx, ast);
            asm.Assemble();

            var actual = NormalizeNewlines(RenderObject(ctx));
            Assert.That(actual, Is.EqualTo(expected),
                $"Assembled object spec mismatch for {fixtureName}.S");
        }

        /// <summary>
        /// Deterministic textual rendering of the assembler's output. Mirrors the section-by-section
        /// shape of the parser's <c>getDebugText</c> goldens but at the byte-emission layer.
        /// </summary>
        private static string RenderObject(Kore.Kuick.Assembler.AssemblerContext ctx) {
            var sb = new StringBuilder();
            sb.Append("OBJECT Sections:[").Append(ctx.Sections.Count)
              .Append("] Symbols:[").Append(ctx.Symbols.Count).Append("]{\n");

            for (int i = 0; i < ctx.Sections.Count; i++) {
                var s = ctx.Sections[i];
                sb.Append("    SECTION[").Append(i).Append("] ").Append(s.Name)
                  .Append(" Align:").Append(s.Alignment)
                  .Append(" Size:").Append(s.MachineCode.Count)
                  .Append(" Relocs:").Append(s.Relocations.Count)
                  .Append(" {\n");

                // Bytes, 4 per row, lowercase hex, with section-relative offset prefix.
                for (int off = 0; off < s.MachineCode.Count; off += 4) {
                    sb.Append("        ").Append(off.ToString("x4")).Append(":");
                    int end = Math.Min(off + 4, s.MachineCode.Count);
                    for (int b = off; b < end; b++) {
                        sb.Append(' ').Append(s.MachineCode[b].ToString("x2"));
                    }
                    sb.Append('\n');
                }

                // Relocations, indexed in declaration order.
                for (int r = 0; r < s.Relocations.Count; r++) {
                    var reloc = s.Relocations[r];
                    sb.Append("        RELOC[").Append(r).Append("] ").Append(reloc.Type)
                      .Append(" OFFSET:0x").Append(reloc.Offset.ToString("x"))
                      .Append(" SYMBOL:").Append(reloc.SymbolName)
                      .Append(" ADDEND:").Append(reloc.Addend);
                    if (!string.IsNullOrEmpty(reloc.RelatedLabel)) {
                        sb.Append(" RELATED:").Append(reloc.RelatedLabel);
                    }
                    sb.Append('\n');
                }

                sb.Append("    }\n");
            }
            sb.Append("}\n");
            return sb.ToString();
        }

        /// <summary>Matches the spec rendering across platforms and ignores trailing blank lines.</summary>
        private static string NormalizeNewlines(string text) =>
            text
              .Replace("\r\n", "\n")
              .Replace("\r", "\n")
              .TrimEnd();
    }
}
