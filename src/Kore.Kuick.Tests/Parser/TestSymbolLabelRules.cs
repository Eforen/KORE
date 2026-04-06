using System;
using Kore.Kuick;
using NUnit.Framework;

namespace Kore.Kuick.Tests.Parser {
    /// <summary>Parser/symbol-table rules that are not covered by golden <c>getDebugText</c> fixtures.</summary>
    [TestFixture]
    public class TestSymbolLabelRules {
        [Test]
        public void DuplicateGlobalLabelDefinition_Throws() {
            var source = @".text
.global main
main:
    nop
main:
    nop
";
            var lexer = new Lexer();
            lexer.Load(source);
            var ex = Assert.Throws<InvalidOperationException>(() => global::Kore.Kuick.Parser.Parse(lexer));
            Assert.That(ex!.Message, Does.Contain("global label").IgnoreCase);
        }
    }
}
