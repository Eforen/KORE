using System;
using System.Linq;
using NUnit.Framework;

namespace Kore.AST.Test {
    [TestFixture]
    public class SymbolTableTests {
        private SymbolTable symbolTable;
        private ProgramNode program;

        [SetUp]
        public void Setup() {
            symbolTable = new SymbolTable();
            symbolTable.ResetIdCounter(); // Ensure consistent IDs for each test
            program = new ProgramNode();
            program.SymbolTable.ResetIdCounter(); // Reset the program's symbol table too
        }

        [Test]
        public void TestSymbolCreation() {
            // Test creating a new symbol
            var symbol = symbolTable.GetOrCreateSymbol("test_label", SymbolScope.Local, SymbolType.Label);
            
            Assert.IsNotNull(symbol);
            Assert.AreEqual("test_label", symbol.Name);
            Assert.AreEqual(SymbolScope.Local, symbol.Scope);
            Assert.AreEqual(SymbolType.Label, symbol.Type);
            Assert.IsFalse(symbol.IsDefined);
            Assert.AreEqual(1, symbol.Id); // First symbol should have ID 1
        }

        [Test]
        public void TestSymbolIdAutoIncrement() {
            var symbol1 = symbolTable.GetOrCreateSymbol("label1");
            var symbol2 = symbolTable.GetOrCreateSymbol("label2");
            var symbol3 = symbolTable.GetOrCreateSymbol("label3");
            
            Assert.AreEqual(1, symbol1.Id);
            Assert.AreEqual(2, symbol2.Id);
            Assert.AreEqual(3, symbol3.Id);
        }

        [Test]
        public void TestSymbolDefinition() {
            var symbol = symbolTable.GetOrCreateSymbol("loop", SymbolScope.Unknown, SymbolType.Label);
            Assert.IsFalse(symbol.IsDefined);
            Assert.AreEqual(SymbolScope.Unknown, symbol.Scope);
            
            // Define the symbol
            symbol.Define(42, ".text");
            
            Assert.IsTrue(symbol.IsDefined);
            Assert.AreEqual(42, symbol.LineNumber);
            Assert.AreEqual(".text", symbol.Section);
            Assert.AreEqual(SymbolScope.Local, symbol.Scope); // Should be promoted from Unknown to Local
        }

        [Test]
        public void TestSymbolReference() {
            var dummyNode = new LabelNode("dummy");
            var symbol = symbolTable.ReferenceSymbol("forward_ref", dummyNode);
            
            Assert.AreEqual("forward_ref", symbol.Name);
            Assert.AreEqual(SymbolScope.Unknown, symbol.Scope);
            Assert.IsFalse(symbol.IsDefined);
            Assert.AreEqual(1, symbol.References.Count);
            Assert.Contains(dummyNode, symbol.References);
        }

        [Test]
        public void TestSymbolScopePromotion() {
            // Create symbol as Unknown
            var symbol = symbolTable.GetOrCreateSymbol("test_symbol", SymbolScope.Unknown);
            Assert.AreEqual(SymbolScope.Unknown, symbol.Scope);
            
            // Try to get it again with Global scope - should promote
            var sameSymbol = symbolTable.GetOrCreateSymbol("test_symbol", SymbolScope.Global);
            Assert.AreSame(symbol, sameSymbol);
            Assert.AreEqual(SymbolScope.Global, symbol.Scope);
        }

        [Test]
        public void TestSymbolsByScope() {
            symbolTable.GetOrCreateSymbol("global1", SymbolScope.Global);
            symbolTable.GetOrCreateSymbol("global2", SymbolScope.Global);
            symbolTable.GetOrCreateSymbol("local1", SymbolScope.Local);
            symbolTable.GetOrCreateSymbol("unknown1", SymbolScope.Unknown);
            
            var globalSymbols = symbolTable.GetSymbolsByScope(SymbolScope.Global);
            var localSymbols = symbolTable.GetSymbolsByScope(SymbolScope.Local);
            var unknownSymbols = symbolTable.GetSymbolsByScope(SymbolScope.Unknown);
            
            Assert.AreEqual(2, globalSymbols.Count);
            Assert.AreEqual(1, localSymbols.Count);
            Assert.AreEqual(1, unknownSymbols.Count);
        }

        [Test]
        public void TestUndefinedSymbols() {
            var defined = symbolTable.DefineSymbol("defined_label", 10, ".text");
            var undefined = symbolTable.GetOrCreateSymbol("undefined_label");
            
            var undefinedSymbols = symbolTable.GetUndefinedSymbols().ToList();
            Assert.AreEqual(1, undefinedSymbols.Count);
            Assert.Contains(undefined, undefinedSymbols);
            Assert.IsFalse(undefinedSymbols.Contains(defined));
        }

        [Test]
        public void TestSymbolValidation() {
            Assert.IsTrue(symbolTable.AllSymbolsDefined()); // Empty table
            
            symbolTable.GetOrCreateSymbol("undefined");
            Assert.IsFalse(symbolTable.AllSymbolsDefined());
            
            symbolTable.DefineSymbol("undefined", 5);
            Assert.IsTrue(symbolTable.AllSymbolsDefined());
        }

        [Test]
        public void TestSymbolReferenceNode() {
            var symbol = symbolTable.GetOrCreateSymbol("test_ref");
            var symbolRef = new SymbolReferenceNode(symbol.Id, symbol.Name, symbolTable);
            
            Assert.AreEqual(symbol.Id, symbolRef.SymbolId);
            Assert.AreEqual(symbol.Name, symbolRef.SymbolName);
            Assert.AreSame(symbol, symbolRef.GetSymbol());
            Assert.IsFalse(symbolRef.IsSymbolDefined());
            Assert.IsNull(symbolRef.GetSymbolAddress());
            
            // Define the symbol
            symbol.Define(100);
            symbol.Address = 0x1000;
            
            Assert.IsTrue(symbolRef.IsSymbolDefined());
            Assert.AreEqual(0x1000, symbolRef.GetSymbolAddress());
        }

        [Test]
        public void TestProgramNodeSymbolTable() {
            Assert.IsNotNull(program.SymbolTable);
            Assert.AreEqual(0, program.SymbolTable.Count);
            
            // Test extension methods
            var labelSymbol = program.DefineLabel("main", 1, ".text");
            Assert.AreEqual(1, program.SymbolTable.Count);
            Assert.AreEqual("main", labelSymbol.Name);
            Assert.IsTrue(labelSymbol.IsDefined);
        }

        [Test]
        public void TestSymbolDirectives() {
            var globalDirective = program.ProcessGlobalDirective("global_func");
            var localDirective = program.ProcessLocalDirective("local_var");
            
            Assert.AreEqual(".global", globalDirective.Name);
            Assert.AreEqual("global_func", globalDirective.SymbolName);
            Assert.AreEqual(SymbolScope.Global, globalDirective.Symbol.Scope);
            
            Assert.AreEqual(".local", localDirective.Name);
            Assert.AreEqual("local_var", localDirective.SymbolName);
            Assert.AreEqual(SymbolScope.Local, localDirective.Symbol.Scope);
        }

        [Test]
        public void TestSymbolStatistics() {
            program.DefineLabel("label1", 1, ".text");
            program.ProcessGlobalDirective("global_symbol");
            program.CreateSymbolReference("undefined_ref", null);
            
            var stats = program.GetSymbolStatistics();
            
            Assert.AreEqual(3, stats["TotalSymbols"]);
            Assert.AreEqual(1, stats["GlobalSymbols"]);
            Assert.AreEqual(1, stats["LocalSymbols"]);
            Assert.AreEqual(1, stats["UnknownSymbols"]);
        }

        [Test]
        public void TestSymbolExport() {
            var symbol = symbolTable.DefineSymbol("exported", 42, ".data", SymbolScope.Global);
            symbol.Address = 0x2000;
            
            var exportInfo = symbolTable.ExportSymbolInfo();
            
            Assert.IsTrue(exportInfo.ContainsKey("exported"));
            var symbolInfo = exportInfo["exported"];
            
            // This would need to be adjusted based on the actual export format
            Assert.IsNotNull(symbolInfo);
        }

        [Test]
        public void TestComplexSymbolScenario() {
            // Simulate a real assembly program scenario
            
            // 1. Reference a forward label (unknown scope)
            var dummyInstruction = new LabelNode("dummy");
            var forwardRef = symbolTable.ReferenceSymbol("end_loop", dummyInstruction);
            Assert.AreEqual(SymbolScope.Unknown, forwardRef.Scope);
            Assert.IsFalse(forwardRef.IsDefined);
            
            // 2. Define some local labels
            var loopStart = symbolTable.DefineSymbol("loop_start", 10, ".text");
            var dataLabel = symbolTable.DefineSymbol("my_data", 50, ".data");
            
            // 3. Mark a symbol as global
            var globalFunc = symbolTable.GetOrCreateSymbol("main", SymbolScope.Global);
            symbolTable.DefineSymbol("main", 1, ".text", SymbolScope.Global);
            
            // 4. Finally define the forward reference
            symbolTable.DefineSymbol("end_loop", 20, ".text");
            
            // Verify the state
            Assert.AreEqual(4, symbolTable.Count);
            Assert.IsTrue(symbolTable.AllSymbolsDefined());
            
            var globalSymbols = symbolTable.GetSymbolsByScope(SymbolScope.Global);
            var localSymbols = symbolTable.GetSymbolsByScope(SymbolScope.Local);
            
            Assert.AreEqual(1, globalSymbols.Count);
            Assert.AreEqual(3, localSymbols.Count); // end_loop was promoted from Unknown to Local
            
            // Verify the forward reference was resolved
            Assert.IsTrue(forwardRef.IsDefined);
            Assert.AreEqual(20, forwardRef.LineNumber);
        }

        [Test]
        public void TestLocalDirectiveIntegration() {
            // Test the .local directive integration with the parser
            var program = new ProgramNode();
            
            // Simulate parsing .local directive
            var localDirective = program.ProcessLocalDirective("helper_function");
            
            // Verify the directive was created correctly
            Assert.AreEqual(".local", localDirective.Name);
            Assert.AreEqual("helper_function", localDirective.SymbolName);
            Assert.AreEqual(SymbolDirectiveNode.DirectiveType.Local, localDirective.Type);
            
            // Verify the symbol was added to the symbol table with correct scope
            var symbol = program.SymbolTable.GetSymbol("helper_function");
            Assert.IsNotNull(symbol);
            Assert.AreEqual("helper_function", symbol.Name);
            Assert.AreEqual(SymbolScope.Local, symbol.Scope);
            Assert.AreEqual(SymbolType.Label, symbol.Type);
            Assert.IsFalse(symbol.IsDefined); // Not defined yet, just declared as local
            
            // Now define the symbol (like when we encounter the actual label)
            program.DefineLabel("helper_function", 42, ".text");
            
            // Verify it's now defined but still local scope
            Assert.IsTrue(symbol.IsDefined);
            Assert.AreEqual(42, symbol.LineNumber);
            Assert.AreEqual(".text", symbol.Section);
            Assert.AreEqual(SymbolScope.Local, symbol.Scope); // Should remain local
        }

        [Test]
        public void TestGlobalVsLocalDirectiveComparison() {
            // Test that .global and .local directives work differently
            var program = new ProgramNode();
            
            // Create a global directive
            var globalDirective = program.ProcessGlobalDirective("public_function");
            var globalSymbol = program.SymbolTable.GetSymbol("public_function");
            
            // Create a local directive  
            var localDirective = program.ProcessLocalDirective("private_function");
            var localSymbol = program.SymbolTable.GetSymbol("private_function");
            
            // Verify scopes are different
            Assert.AreEqual(SymbolScope.Global, globalSymbol.Scope);
            Assert.AreEqual(SymbolScope.Local, localSymbol.Scope);
            
            // Verify directive types are different
            Assert.AreEqual(SymbolDirectiveNode.DirectiveType.Global, globalDirective.Type);
            Assert.AreEqual(SymbolDirectiveNode.DirectiveType.Local, localDirective.Type);
            
            // Both should be in the symbol table
            Assert.AreEqual(2, program.SymbolTable.Count);
            
            // Verify symbol table scope queries
            var globalSymbols = program.SymbolTable.GetSymbolsByScope(SymbolScope.Global);
            var localSymbols = program.SymbolTable.GetSymbolsByScope(SymbolScope.Local);
            
            Assert.AreEqual(1, globalSymbols.Count);
            Assert.AreEqual(1, localSymbols.Count);
            Assert.IsTrue(globalSymbols.Contains(globalSymbol));
            Assert.IsTrue(localSymbols.Contains(localSymbol));
        }

        [Test]
        public void TestSymbolAddressAssignment() {
            // Arrange
            var program = new ProgramNode();
            var symbol = program.SymbolTable.GetOrCreateSymbol("test_label", SymbolScope.Local);
            
            // Act - simulate what CodeGenerator does
            symbol.Address = 0x1000; // Assign address like CodeGenerator would
            symbol.IsDefined = true;
            
            // Assert
            Assert.AreEqual(0x1000, symbol.Address);
            Assert.IsTrue(symbol.IsDefined);
            Assert.AreEqual("test_label", symbol.Name);
            Assert.AreEqual(SymbolScope.Local, symbol.Scope);
        }

        [Test]
        public void TestSymbolAddressResolution() {
            // Arrange
            var program = new ProgramNode();
            var targetSymbol = program.SymbolTable.DefineSymbol("target", 0x2000, ".text");
            var referenceSymbol = program.SymbolTable.ReferenceSymbol("target", null);
            
            // Act - simulate address assignment
            targetSymbol.Address = 0x2000;
            
            // Assert
            Assert.AreEqual(0x2000, targetSymbol.Address);
            Assert.IsTrue(targetSymbol.IsDefined);
            Assert.AreSame(targetSymbol, referenceSymbol); // Should be the same object
        }
    }
} 