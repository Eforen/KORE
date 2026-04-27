using Kore.AST;
using NUnit.Framework;

namespace Kore.Kuick.Tests {
    /// <summary>
    /// Repeated section directives with the same name append to one <see cref="SectionNode"/>; <see cref="ParserContext.CurrentSectionIndex"/> tracks the active section.
    /// </summary>
    [TestFixture]
    public class ParserSectionReuseTests {
        [Test]
        public void RepeatedTextDirective_AppendsToSameSection() {
            var lexer = new Lexer();
            lexer.Load(".text\nnop\n.text\nnop\n");
            ProgramNode ast = global::Kore.Kuick.Parser.Parse(lexer);
            Assert.AreEqual(1, ast.Sections.Count);
            Assert.AreEqual(".text", ast.Sections[0].Name);
            Assert.AreEqual(2, ast.Sections[0].Contents.Count);
        }
    }
}
