Testing Framework
==================

The KUICK assembler includes a comprehensive testing framework built on NUnit, ensuring reliability and correctness of all components from lexical analysis to code generation.

Test Organization
-----------------

The test suite is organized into several categories across 19 test files:

**Lexer Tests:**
- **Token Recognition**: Validates all token types and patterns
- **Register Aliases**: Tests all RISC-V register name mappings
- **Edge Cases**: Handles malformed input and boundary conditions
- **Position Tracking**: Verifies accurate line/column reporting

**Parser Tests:**
- **Instruction Types**: Complete coverage of R, I, S, B, U, and J-type instructions
- **Pseudo Instructions**: Tests pseudo-instruction expansion
- **Directives**: Symbol visibility and section directives
- **Labels**: Forward and backward reference handling
- **Inline Directives**: %hi, %lo, and other preprocessor constructs

**AST Tests:**
- **Node Construction**: Validates AST node creation and relationships
- **Symbol Table Integration**: Tests symbol resolution and scoping
- **Tree Traversal**: Verifies visitor pattern implementation

**Code Generator Tests:**
- **Machine Code Generation**: End-to-end assembly to binary conversion
- **Symbol Resolution**: Multi-pass assembly with forward references
- **Address Assignment**: Correct symbol address calculation

**Symbol Table Tests:**
- **Multi-scope Management**: Global, local, and unknown symbol handling
- **Forward References**: Two-pass resolution verification
- **Symbol Metadata**: Type classification and reference tracking

Test Structure
--------------

**Unit Tests:**
Each component is tested in isolation with mock dependencies where necessary.

**Integration Tests:**
End-to-end tests verify the complete pipeline from source code to machine code.

**Parameterized Tests:**
Instruction parsing tests use NUnit's ``[TestCase]`` attribute for comprehensive coverage:

.. code-block:: csharp

    [TestCase("add x1, x2, x3", TypeR.add, Register.x1, Register.x2, Register.x3)]
    [TestCase("sub x4, x5, x6", TypeR.sub, Register.x4, Register.x5, Register.x6)]
    public void TestParseRInstruction(string input, TypeR expectedOp, 
        Register expectedRd, Register expectedRs1, Register expectedRs2)

Key Test Categories
-------------------

**Instruction Parsing Tests:**

- **R-Type Instructions**: ADD, SUB, AND, OR, XOR, SLL, SRL, SRA, SLT, SLTU
- **I-Type Instructions**: ADDI, SLTI, XORI, ORI, ANDI, SLLI, SRLI, SRAI, LB, LH, LW, LBU, LHU
- **S-Type Instructions**: SB, SH, SW
- **B-Type Instructions**: BEQ, BNE, BLT, BGE, BLTU, BGEU
- **U-Type Instructions**: LUI, AUIPC
- **J-Type Instructions**: JAL, JALR

**Pseudo-Instruction Tests:**
- **Arithmetic**: NEG, SEQZ, SNEZ, SLTZ, SGTZ
- **Branch**: BEQZ, BNEZ, BLEZ, BGEZ, BLTZ, BGTZ
- **Move/Load**: MV, LI, LA

**Symbol and Label Tests:**

.. code-block:: csharp

    [Test]
    public void TestForwardReferenceThenDefinition() {
        var input = ".text\n" +
                   "main:\n" +
                   "    jal x1, helper_function\n" +
                   "helper_function:\n" +
                   "    ret\n";
        // Test that forward references are correctly resolved
    }

**Directive Tests:**

.. code-block:: csharp

    [TestCase(".local helper_function", SymbolDirectiveNode.DirectiveType.Local, "helper_function")]
    [TestCase(".global main", SymbolDirectiveNode.DirectiveType.Global, "main")]
    public void TestSymbolDirectiveParsing(string directiveText, 
        SymbolDirectiveNode.DirectiveType expectedType, string expectedSymbol)

**Machine Code Generation Tests:**

.. code-block:: csharp

    [TestCase(@".text
        li      a5,0      # should result in 0x00000793u
        lui     a0,10     # should result in 0x00010537u
        ret               # should result in 0x00008067u", 
        new uint[] {0x00000793u, 0x00010537u, 0x00008067u})]
    public void MachineCode(string asm, uint[] expectedData)

Quality Assurance
-----------------

**Test Coverage:**
The test suite provides comprehensive coverage of:

- All RISC-V instruction formats
- Symbol table operations and edge cases
- Error conditions and recovery
- Multi-pass assembly scenarios
- Register alias resolution

**Continuous Integration:**
- Tests run automatically on code changes
- Multiple target platforms and configurations
- Performance regression detection

**Test Utilities:**
Helper classes provide:

- **Instruction encoding verification**: ``TestUtils.getDataMismatchString()``
- **AST comparison utilities**: Deep structural comparison
- **Random test data generation**: Fuzz testing support

**Error Testing:**
- **Malformed input handling**: Invalid syntax recovery
- **Undefined symbol detection**: Forward reference validation
- **Type safety verification**: Strong typing enforcement

Test Execution
--------------

**Running Tests:**
Tests can be executed using standard .NET testing tools:

.. code-block:: bash

    # Run all tests
    dotnet test src/Kore.Kuick.Tests/
    
    # Run specific test categories
    dotnet test --filter "Category=Parser"
    dotnet test --filter "Category=CodeGen"

**Test Data:**
- **Instruction patterns**: Based on RISC-V specification examples
- **Real-world assembly**: Common programming patterns
- **Edge cases**: Boundary conditions and error scenarios

**Performance Testing:**
- **Parsing speed**: Large file processing benchmarks
- **Memory usage**: Symbol table scaling tests
- **Code generation**: Optimization effectiveness measurement

Benchmarking
------------

**Performance Metrics:**
The test suite includes performance benchmarks for:

- **Lexing speed**: Tokens per second for various input sizes
- **Parsing throughput**: Instructions processed per millisecond
- **Symbol resolution**: Forward reference handling performance
- **Code generation**: Binary output generation speed

**Regression Detection:**
- Baseline performance metrics stored in test results
- Automatic alerts for significant performance degradation
- Comparative analysis across different architectures

Test Philosophy
---------------

**Reliability First:**
Every feature must have corresponding tests before implementation completion.

**Comprehensive Coverage:**
Tests cover not just happy paths but also edge cases and error conditions.

**Maintainability:**
Test code follows the same quality standards as production code.

**Documentation:**
Tests serve as executable documentation of expected behavior. 