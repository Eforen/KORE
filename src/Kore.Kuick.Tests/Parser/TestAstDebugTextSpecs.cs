using System.IO;
using System.Linq;
using Kore.AST;
using NUnit.Framework;

namespace Kore.Kuick.Tests.Parser {
    /// <summary>
    /// For each <c>Parser/AstFixtures/*.S</c> file, parses the assembly and asserts that
    /// <see cref="AstNode.getDebugText"/> matches the sibling <c>*.spec</c> file (same base name).
    /// </summary>
    [TestFixture]
    public class TestAstDebugTextSpecs {
        private static string AstFixturesDir =>
            Path.Combine(TestContext.CurrentContext.TestDirectory, "Parser", "AstFixtures");

        [Test]
        public void AllSpecFilesMatchParsedDebugText() {
            Assert.That(Directory.Exists(AstFixturesDir), Is.True, $"Missing fixtures directory: {AstFixturesDir}");

            var assemblyFiles = Directory.GetFiles(AstFixturesDir, "*.S", SearchOption.AllDirectories);
            Assert.That(assemblyFiles.Length, Is.GreaterThan(0), "Expected at least one .S file under Parser/AstFixtures");

            // Stable order so failures are deterministic when multiple fixtures exist.
            foreach (var sPath in assemblyFiles.OrderBy(p => p, System.StringComparer.Ordinal)) {
                var dir = Path.GetDirectoryName(sPath)!;
                var baseName = Path.GetFileNameWithoutExtension(sPath);
                var specPath = Path.Combine(dir, baseName + ".spec");

                Assert.That(File.Exists(specPath), Is.True, $"Missing spec for {sPath}: expected {specPath}");

                var source = File.ReadAllText(sPath);
                // Golden files may use CRLF on Windows; normalize before compare.
                var expected = NormalizeNewlines(File.ReadAllText(specPath));

                var lexer = new Lexer();
                lexer.Load(source);
                var ast = Kore.Kuick.Parser.Parse(lexer);

                // Debug print the AST class name
                System.Console.WriteLine($"AST class name: {ast.GetType().Name}");

                var actual = NormalizeNewlines(ast.getDebugText());
                Assert.That(actual, Is.EqualTo(expected), $"AstNode.getDebugText mismatch for {Path.GetFileName(sPath)}");
            }
        }

        /// <summary>Matches <c>getDebugText</c> output across platforms and ignores trailing blank lines.</summary>
        private static string NormalizeNewlines(string text) =>
            text
              .Replace("\r\n", "\n")
              .Replace("\r", "\n")
              .TrimEnd();
    }
}
