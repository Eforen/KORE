using System.Linq;
using Kore.AST;
using NUnit.Framework;

namespace Kore.Kuick.Tests {
    /// <summary>
    /// Tests for <see cref="SymbolTable.GetLabelRef"/>, <see cref="SymbolTable.DefineLabelRef"/>,
    /// <see cref="SymbolTable.MakeGlobal"/>, and <see cref="SymbolTable.MakeLocal"/>.
    /// </summary>
    [TestFixture]
    public class SymbolTableLabelRefAndScopeTests {
        private SymbolTable _table;

        [SetUp]
        public void SetUp() {
            _table = new SymbolTable();
            _table.ResetIdCounter();
        }

        [Test]
        public void DefineLabelRef_BeforeGetLabelRef_SameSymbolAndId() {
            var defined = _table.DefineLabelRef("entry", 1, 0, 0);
            var n1 = new LabelNode("use");
            var fromRef = _table.GetLabelRef("entry", n1);

            Assert.AreSame(defined, fromRef);
            Assert.AreEqual(defined.Id, fromRef.Id);
            Assert.IsTrue(defined.IsDefined);
            Assert.AreEqual(1, defined.References.Count);
        }

        [Test]
        public void GetLabelRef_MultipleTimesBeforeDefineLabelRef_SingleSymbol_AccumulatesReferences() {
            var a = new LabelNode("a");
            var b = new LabelNode("b");
            var c = new LabelNode("c");

            var r1 = _table.GetLabelRef("later", a);
            var r2 = _table.GetLabelRef("later", b);
            var r3 = _table.GetLabelRef("later", c);

            Assert.AreSame(r1, r2);
            Assert.AreSame(r2, r3);
            Assert.AreEqual(SymbolScope.Unknown, r1.Scope);
            Assert.IsFalse(r1.IsDefined);
            Assert.AreEqual(3, r1.References.Count);

            _table.DefineLabelRef("later", 10, 0, 4);

            Assert.IsTrue(r1.IsDefined);
            Assert.AreEqual(SymbolScope.Local, r1.Scope);
            Assert.AreEqual(10, r1.LineNumber);
            Assert.AreEqual(0, r1.SectionIndex);
            Assert.AreEqual(4, r1.Address);
        }

        [Test]
        public void GetLabelRef_BackwardLocalLabel_UsesActiveDefinition() {
            _table.DefineLabelRef("L1", 1, 0, 0);
            var n = new LabelNode("n");
            var sym = _table.GetLabelRef("L1", n);

            Assert.IsTrue(sym.IsDefined);
            Assert.AreEqual(1, sym.References.Count);
        }

        [Test]
        public void MakeGlobal_NewSymbol_IsGlobalUndefinedLabel() {
            var s = _table.MakeGlobal("g");

            Assert.AreEqual("g", s.Name);
            Assert.AreEqual(SymbolScope.Global, s.Scope);
            Assert.AreEqual(SymbolType.Label, s.Type);
            Assert.IsFalse(s.IsDefined);
            Assert.AreSame(s, _table.GetSymbol("g"));
            Assert.IsTrue(_table.GetSymbolsByScope(SymbolScope.Global).Contains(s));
        }

        [Test]
        public void MakeLocal_NewSymbol_IsLocalUndefinedLabel() {
            var s = _table.MakeLocal("l");

            Assert.AreEqual("l", s.Name);
            Assert.AreEqual(SymbolScope.Local, s.Scope);
            Assert.IsFalse(s.IsDefined);
            Assert.IsTrue(_table.GetSymbolsByScope(SymbolScope.Local).Contains(s));
        }

        [Test]
        public void MakeGlobal_OnExistingUnknown_PromotesToGlobal() {
            var u = _table.GetOrCreateSymbol("x", SymbolScope.Unknown, SymbolType.Label);
            var g = _table.MakeGlobal("x");

            Assert.AreSame(u, g);
            Assert.AreEqual(SymbolScope.Global, g.Scope);
            Assert.IsFalse(_table.GetSymbolsByScope(SymbolScope.Unknown).Contains(g));
            Assert.IsTrue(_table.GetSymbolsByScope(SymbolScope.Global).Contains(g));
        }

        [Test]
        public void MakeLocal_OnExistingUnknown_PromotesToLocal() {
            var u = _table.GetOrCreateSymbol("y", SymbolScope.Unknown, SymbolType.Label);
            var l = _table.MakeLocal("y");

            Assert.AreSame(u, l);
            Assert.AreEqual(SymbolScope.Local, l.Scope);
        }

        [Test]
        public void MakeGlobal_ThenMakeLocal_ScopeBecomesLocal() {
            var g = _table.MakeGlobal("flip");
            Assert.AreEqual(SymbolScope.Global, g.Scope);

            var l = _table.MakeLocal("flip");

            Assert.AreSame(g, l);
            Assert.AreEqual(SymbolScope.Local, l.Scope);
            Assert.IsTrue(_table.GetSymbolsByScope(SymbolScope.Local).Contains(l));
            Assert.IsFalse(_table.GetSymbolsByScope(SymbolScope.Global).Contains(l));
        }

        [Test]
        public void MakeLocal_ThenMakeGlobal_ScopeBecomesGlobal() {
            var l = _table.MakeLocal("flip2");
            Assert.AreEqual(SymbolScope.Local, l.Scope);

            var g = _table.MakeGlobal("flip2");

            Assert.AreSame(l, g);
            Assert.AreEqual(SymbolScope.Global, g.Scope);
        }

        [Test]
        public void MakeGlobal_Idempotent_AlreadyGlobal() {
            var a = _table.MakeGlobal("same");
            var b = _table.MakeGlobal("same");
            Assert.AreSame(a, b);
            Assert.AreEqual(SymbolScope.Global, b.Scope);
        }

        [Test]
        public void GetLabelRef_AfterMakeGlobal_UsesGlobalSymbolForForwardRef() {
            _table.MakeGlobal("ext");
            var n = new LabelNode("n");
            var sym = _table.GetLabelRef("ext", n);

            Assert.AreEqual(SymbolScope.Global, sym.Scope);
            Assert.AreEqual(1, sym.References.Count);
        }
    }
}
