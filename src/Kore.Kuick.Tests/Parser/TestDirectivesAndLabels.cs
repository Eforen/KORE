using Kore.AST;
using Kore.RiscMeta;
using NUnit.Framework;
using System.Linq;
using System.Collections.Generic;
using System;

namespace Kore.Kuick.Tests.Parser {
    
    [TestFixture]
    public class TestDirectivesAndLabels {

        [Test]
        [TestCase(".local helper_function", SymbolDirectiveNode.DirectiveType.Local, "helper_function")]
        [TestCase(".global main", SymbolDirectiveNode.DirectiveType.Global, "main")]
        [TestCase(".local _private_func", SymbolDirectiveNode.DirectiveType.Local, "_private_func")]
        [TestCase(".global _start", SymbolDirectiveNode.DirectiveType.Global, "_start")]
        public void TestSymbolDirectiveParsing(string directiveText, SymbolDirectiveNode.DirectiveType expectedType, string expectedSymbol) {
            // Test parsing of .local and .global directives
            var input = $".text\n{directiveText}\n";
            
            var lexer = new Lexer();
            lexer.Load(input);
            
            var ast = Kore.Kuick.Parser.Parse(lexer);
            
            Assert.IsInstanceOf<ProgramNode>(ast);
            var programNode = (ProgramNode)ast;
            Assert.AreEqual(1, programNode.Sections.Count);
            
            var textSection = programNode.Sections[0];
            Assert.AreEqual(".text", textSection.Name);
            Assert.AreEqual(1, textSection.Contents.Count);
            
            // Check the directive node
            var directiveNode = textSection.Contents[0];
            Assert.IsInstanceOf<SymbolDirectiveNode>(directiveNode);
            var symbolDirective = (SymbolDirectiveNode)directiveNode;
            
            Assert.AreEqual(expectedType, symbolDirective.Type);
            Assert.AreEqual(expectedSymbol, symbolDirective.SymbolName);
            Assert.IsNotNull(symbolDirective.Symbol);
            Assert.AreEqual(expectedSymbol, symbolDirective.Symbol.Name);
            
            // Verify symbol scope
            var expectedScope = expectedType == SymbolDirectiveNode.DirectiveType.Global ? SymbolScope.Global : SymbolScope.Local;
            Assert.AreEqual(expectedScope, symbolDirective.Symbol.Scope);
            
            // Verify symbol is in program's symbol table
            var symbolFromTable = programNode.SymbolTable.GetSymbol(expectedSymbol);
            Assert.IsNotNull(symbolFromTable);
            Assert.AreSame(symbolDirective.Symbol, symbolFromTable);
        }

        [Test]
        public void TestLabelParsing() {
            // Test parsing of labels
            var input = ".text\nmain:\n    addi x1, x0, 10\n    ret\n";
            
            var lexer = new Lexer();
            lexer.Load(input);
            
            var ast = Kore.Kuick.Parser.Parse(lexer);
            
            Assert.IsInstanceOf<ProgramNode>(ast);
            var programNode = (ProgramNode)ast;
            Assert.AreEqual(1, programNode.Sections.Count);
            
            var textSection = programNode.Sections[0];
            Assert.AreEqual(".text", textSection.Name);
            Assert.AreEqual(3, textSection.Contents.Count); // label + 2 instructions
            
            // Check the label node
            var labelNode = textSection.Contents[0];
            Assert.IsInstanceOf<LabelNode>(labelNode);
            var label = (LabelNode)labelNode;
            Assert.AreEqual("main", label.Name);
            
            // Check that the label creates a symbol in the symbol table
            var symbolFromTable = programNode.SymbolTable.GetSymbol("main");
            Assert.IsNotNull(symbolFromTable);
            Assert.AreEqual("main", symbolFromTable.Name);
            Assert.AreEqual(SymbolType.Label, symbolFromTable.Type);
        }

        [Test]
        public void TestUndefinedReferences() {
            // Test forward references that are initially undefined
            var input = ".text\n" +
                       "main:\n" +
                       "    jal x1, helper_function\n" +
                       "    ret\n" +
                       "helper_function:\n" +
                       "    addi x1, x1, 1\n" +
                       "    ret\n";
            
            var lexer = new Lexer();
            lexer.Load(input);
            
            var ast = Kore.Kuick.Parser.Parse(lexer);
            
            Assert.IsInstanceOf<ProgramNode>(ast);
            var programNode = (ProgramNode)ast;
            
            // Check that both symbols exist in the symbol table
            var mainSymbol = programNode.SymbolTable.GetSymbol("main");
            var helperSymbol = programNode.SymbolTable.GetSymbol("helper_function");
            
            Assert.IsNotNull(mainSymbol);
            Assert.IsNotNull(helperSymbol);
            
            // Both should be defined since we have their labels
            Assert.IsTrue(mainSymbol.IsDefined);
            Assert.IsTrue(helperSymbol.IsDefined);
            
            // Check that the JAL instruction references the helper function
            var textSection = programNode.Sections[0];
            var jalInstruction = textSection.Contents[1]; // Should be the JAL instruction
            Assert.IsInstanceOf<InstructionNodeTypeJLabel>(jalInstruction);
            var jal = (InstructionNodeTypeJLabel)jalInstruction;
            Assert.AreEqual("helper_function", jal.label);
        }

        [Test]
        public void TestTrulyUndefinedReference() {
            // Test a reference to a symbol that never gets defined
            var input = ".text\n" +
                       "main:\n" +
                       "    jal x1, undefined_function\n" +
                       "    ret\n";
            
            var lexer = new Lexer();
            lexer.Load(input);
            
            var ast = Kore.Kuick.Parser.Parse(lexer);
            
            Assert.IsInstanceOf<ProgramNode>(ast);
            var programNode = (ProgramNode)ast;
            
            // The main symbol should be defined
            var mainSymbol = programNode.SymbolTable.GetSymbol("main");
            Assert.IsNotNull(mainSymbol);
            Assert.IsTrue(mainSymbol.IsDefined);
            
            // Check that we can detect undefined symbols
            var undefinedSymbols = programNode.SymbolTable.GetUndefinedSymbols().ToList();
            Assert.AreEqual(1, undefinedSymbols.Count);
            Assert.AreEqual("undefined_function", undefinedSymbols[0].Name);
            Assert.IsFalse(undefinedSymbols[0].IsDefined);
            
            // Verify the symbol table knows not all symbols are defined
            Assert.IsFalse(programNode.SymbolTable.AllSymbolsDefined());
        }

        [Test]
        public void TestMixedDirectivesAndLabels() {
            // Test a more complex scenario with directives and labels
            var input = ".text\n" +
                       ".global main\n" +
                       ".local helper\n" +
                       "main:\n" +
                       "    jal x1, helper\n" +
                       "    ret\n" +
                       "helper:\n" +
                       "    addi x1, x1, 1\n" +
                       "    ret\n";
            
            var lexer = new Lexer();
            lexer.Load(input);
            
            var ast = Kore.Kuick.Parser.Parse(lexer);
            
            Assert.IsInstanceOf<ProgramNode>(ast);
            var programNode = (ProgramNode)ast;
            
            // Check that symbols have correct scopes
            var mainSymbol = programNode.SymbolTable.GetSymbol("main");
            var helperSymbol = programNode.SymbolTable.GetSymbol("helper");
            
            Assert.IsNotNull(mainSymbol);
            Assert.IsNotNull(helperSymbol);
            
            Assert.AreEqual(SymbolScope.Global, mainSymbol.Scope);
            Assert.AreEqual(SymbolScope.Local, helperSymbol.Scope);
            
            // Both should be defined
            Assert.IsTrue(mainSymbol.IsDefined);
            Assert.IsTrue(helperSymbol.IsDefined);
            
            // Verify symbol table statistics
            var globalSymbols = programNode.SymbolTable.GetSymbolsByScope(SymbolScope.Global);
            var localSymbols = programNode.SymbolTable.GetSymbolsByScope(SymbolScope.Local);
            
            Assert.AreEqual(1, globalSymbols.Count);
            Assert.AreEqual(1, localSymbols.Count);
            Assert.IsTrue(globalSymbols.Contains(mainSymbol));
            Assert.IsTrue(localSymbols.Contains(helperSymbol));
        }

        [Test]
        public void TestBranchWithLabels() {
            // Test branch instructions with label references
            var input = ".text\n" +
                       "main:\n" +
                       "    addi x1, x0, 5\n" +
                       "    beq x1, x0, end\n" +
                       "    addi x1, x1, 1\n" +
                       "end:\n" +
                       "    ret\n";
            
            var lexer = new Lexer();
            lexer.Load(input);
            
            var ast = Kore.Kuick.Parser.Parse(lexer);
            
            Assert.IsInstanceOf<ProgramNode>(ast);
            var programNode = (ProgramNode)ast;
            
            // Check that both labels are in the symbol table
            var mainSymbol = programNode.SymbolTable.GetSymbol("main");
            var endSymbol = programNode.SymbolTable.GetSymbol("end");
            
            Assert.IsNotNull(mainSymbol);
            Assert.IsNotNull(endSymbol);
            Assert.IsTrue(mainSymbol.IsDefined);
            Assert.IsTrue(endSymbol.IsDefined);
            
            // Check the BEQ instruction references the end label
            var textSection = programNode.Sections[0];
            var beqInstruction = textSection.Contents[2]; // Should be the BEQ instruction
            Assert.IsInstanceOf<InstructionNodeTypeBLabel>(beqInstruction);
            var beq = (InstructionNodeTypeBLabel)beqInstruction;
            Assert.AreEqual("end", beq.label);
        }

        [Test]
        public void TestMultipleSections() {
            // Test directives and labels across multiple sections
            var input = ".text\n" +
                       ".global main\n" +
                       "main:\n" +
                       "    ret\n" +
                       ".data\n" +
                       ".local data_var\n" +
                       "data_var:\n" +
                       "    .word 42\n";
            
            var lexer = new Lexer();
            lexer.Load(input);
            
            var ast = Kore.Kuick.Parser.Parse(lexer);
            
            Assert.IsInstanceOf<ProgramNode>(ast);
            var programNode = (ProgramNode)ast;
            
            // Should have 2 sections
            Assert.AreEqual(2, programNode.Sections.Count);
            
            var textSection = programNode.Sections[0];
            var dataSection = programNode.Sections[1];
            
            Assert.AreEqual(".text", textSection.Name);
            Assert.AreEqual(".data", dataSection.Name);
            
            // Check symbols
            var mainSymbol = programNode.SymbolTable.GetSymbol("main");
            var dataSymbol = programNode.SymbolTable.GetSymbol("data_var");
            
            Assert.IsNotNull(mainSymbol);
            Assert.IsNotNull(dataSymbol);
            
            Assert.AreEqual(SymbolScope.Global, mainSymbol.Scope);
            Assert.AreEqual(SymbolScope.Local, dataSymbol.Scope);
        }

        [Test]
        public void TestForwardReferenceThenDefinition() {
            // Test the specific case of forward reference resolution
            var input = ".text\n" +
                       "main:\n" +
                       "    jal x1, forward_func\n" +
                       "    ret\n" +
                       ".local forward_func\n" +
                       "forward_func:\n" +
                       "    ret\n";
            
            var lexer = new Lexer();
            lexer.Load(input);
            
            var ast = Kore.Kuick.Parser.Parse(lexer);
            
            Assert.IsInstanceOf<ProgramNode>(ast);
            var programNode = (ProgramNode)ast;
            
            var forwardSymbol = programNode.SymbolTable.GetSymbol("forward_func");
            Assert.IsNotNull(forwardSymbol);
            
            // Should be defined and have local scope (from the directive)
            Assert.IsTrue(forwardSymbol.IsDefined);
            Assert.AreEqual(SymbolScope.Local, forwardSymbol.Scope);
            
            // All symbols should be defined
            Assert.IsTrue(programNode.SymbolTable.AllSymbolsDefined());
        }

        [Test]
        public void TestForwardReferenceNoDefinition() {
            // Test the specific case of forward reference resolution
            var input = ".text\n" +
                       "main:\n" +
                       "    jal x1, forward_func\n" +
                       "    ret\n";
            
            var lexer = new Lexer();
            lexer.Load(input);
            
            var ast = Kore.Kuick.Parser.Parse(lexer);
            
            Assert.IsInstanceOf<ProgramNode>(ast);
            var programNode = (ProgramNode)ast;
            
            var forwardSymbol = programNode.SymbolTable.GetSymbol("forward_func");
            Assert.IsNotNull(forwardSymbol);
            
            // Should be defined and have local scope (from the directive)
            Assert.IsFalse(forwardSymbol.IsDefined);
            Assert.AreEqual(SymbolScope.Unknown, forwardSymbol.Scope);
            
            // All symbols should be defined
            Assert.IsFalse(programNode.SymbolTable.AllSymbolsDefined());
        }

        [Test]
        public void DebugDataTokenization() {
            // Debug test to understand .data tokenization
            var input = ".data\n";
            
            var lexer = new Lexer();
            lexer.Load(input);
            
            var token = lexer.ReadToken();
            
            // Check what token type .data gets
            Assert.AreEqual(Lexer.Token.DIRECTIVE, token.token);
            Assert.AreEqual(".data", token.value);
        }
    }
} 