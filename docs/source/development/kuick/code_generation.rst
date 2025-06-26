Code Generation
===============

The KUICK code generator transforms the Abstract Syntax Tree (AST) into RISC-V machine code through a multi-pass visitor pattern approach. It handles instruction encoding, symbol resolution, and address assignment while supporting forward references and multiple output formats.

Overview
--------

The code generator performs the final translation step in the assembly process:

- **Instruction Encoding**: Converts AST nodes to RISC-V binary instructions
- **Symbol Resolution**: Resolves symbol references to memory addresses
- **Address Calculation**: Assigns addresses to instructions and data
- **Binary Output**: Generates executable binary files
- **Relocation Handling**: Manages relocatable symbols for linking

Architecture
------------

The code generation system consists of several key components:

**Code Generator Core:**
- ``CodeGenerator`` - Main orchestrator class
- ``InstructionEncoder`` - RISC-V instruction encoding
- ``SymbolResolver`` - Symbol address resolution
- ``BinaryWriter`` - Output file generation

**Encoding Modules:**
- ``RTypeEncoder`` - R-type instruction encoding
- ``ITypeEncoder`` - I-type instruction encoding  
- ``STypeEncoder`` - S-type instruction encoding
- ``BTypeEncoder`` - B-type instruction encoding
- ``UTypeEncoder`` - U-type instruction encoding
- ``JTypeEncoder`` - J-type instruction encoding

**Output Formats:**
- ``ELFWriter`` - ELF object file generation
- ``BinaryWriter`` - Raw binary output
- ``HexWriter`` - Intel HEX format output

Code Generation Process
-----------------------

The code generation follows a multi-phase approach:

**Phase 1: Address Assignment**
1. Traverse AST to calculate section sizes
2. Assign base addresses to sections (.text, .data, .bss)
3. Calculate instruction and data addresses
4. Update symbol table with resolved addresses

**Phase 2: Symbol Resolution**
1. Resolve all symbol references
2. Calculate relative addresses for branches/jumps
3. Handle forward and backward references
4. Validate address ranges and alignment

**Phase 3: Instruction Encoding**
1. Visit each instruction node in the AST
2. Encode instructions based on type (R/I/S/B/U/J)
3. Apply immediate values and register encodings
4. Handle pseudo-instruction expansion

**Phase 4: Binary Generation**
1. Generate section headers and metadata
2. Write encoded instructions to output buffer
3. Apply relocations and symbol patches
4. Generate final binary output file

Instruction Encoding
--------------------

**R-Type Instructions:**
```
31    25 24  20 19  15 14  12 11   7 6    0
+-------+-----+-----+-----+-----+-------+
| funct7| rs2 | rs1 |funct3| rd  |opcode |
+-------+-----+-----+-----+-----+-------+
```

**I-Type Instructions:**
```
31        20 19  15 14  12 11   7 6    0
+-----------+-----+-----+-----+-------+
|    imm    | rs1 |funct3| rd  |opcode |
+-----------+-----+-----+-----+-------+
```

**S-Type Instructions:**
```
31    25 24  20 19  15 14  12 11   7 6    0
+-------+-----+-----+-----+-----+-------+
|imm[11:5]| rs2 | rs1 |funct3|imm[4:0]|opcode |
+-------+-----+-----+-----+-----+-------+
```

**B-Type Instructions:**
```
31    25 24  20 19  15 14  12 11   7 6    0
+-------+-----+-----+-----+-----+-------+
|imm[12,10:5]| rs2| rs1|funct3|imm[4:1,11]|opcode|
+-------+-----+-----+-----+-----+-------+
```

**U-Type Instructions:**
```
31        12 11   7 6    0
+-----------+-----+-------+
|    imm    | rd  |opcode |
+-----------+-----+-------+
```

**J-Type Instructions:**
```
31        12 11   7 6    0
+-----------+-----+-------+
|    imm    | rd  |opcode |
+-----------+-----+-------+
```

Symbol Resolution
-----------------

**Address Calculation:**
```cpp
class SymbolResolver {
    Address resolve_symbol(const Symbol& symbol) {
        switch (symbol.scope) {
            case SymbolScope::Local:
                return base_address + symbol.offset;
            case SymbolScope::Global:
                return global_symbol_table[symbol.name];
            case SymbolScope::External:
                return create_relocation(symbol);
        }
    }
}
```

**Forward Reference Resolution:**
1. First pass: Collect all symbol definitions
2. Second pass: Resolve all symbol references
3. Third pass: Apply relocations and patches

**Branch Target Calculation:**
```cpp
int32_t calculate_branch_offset(Address current, Address target) {
    int32_t offset = target - current;
    
    // Validate offset range for branch instructions
    if (offset < -4096 || offset > 4095) {
        throw BranchOffsetError("Branch target out of range");
    }
    
    return offset;
}
```

Pseudo-Instruction Expansion
----------------------------

The code generator automatically expands pseudo-instructions:

**MOVE Pseudo-Instruction:**
```
move rd, rs  →  addi rd, rs, 0
```

**LOAD IMMEDIATE (LI):**
```
li rd, imm  →  lui rd, %hi(imm)
               ori rd, rd, %lo(imm)
```

**LOAD ADDRESS (LA):**
```
la rd, symbol  →  auipc rd, %pcrel_hi(symbol)
                  addi rd, rd, %pcrel_lo(symbol)
```

**CALL Pseudo-Instruction:**
```
call symbol  →  auipc x1, %pcrel_hi(symbol)
                jalr x1, x1, %pcrel_lo(symbol)
```

Binary Output Formats
----------------------

**ELF Object Files:**
- Standard ELF header generation
- Section header creation
- Symbol table generation
- Relocation table creation
- Debug information support

**Raw Binary:**
- Pure machine code output
- No headers or metadata
- Suitable for embedded systems
- Configurable base address

**Intel HEX:**
- Hexadecimal text format
- Checksum verification
- Address information included
- Compatible with programming tools

Optimization Passes
-------------------

**Instruction Optimization:**
- Redundant instruction elimination
- Constant folding for immediates
- Register usage optimization
- Branch optimization

**Size Optimization:**
- Instruction selection for size
- Alignment optimization
- Dead code elimination
- Unused symbol removal

**Performance Optimization:**
- Branch prediction hints
- Instruction scheduling
- Register allocation guidance
- Cache-friendly code layout

Error Handling
--------------

**Encoding Errors:**
- Invalid instruction formats
- Out-of-range immediate values
- Invalid register specifications
- Unsupported instruction combinations

**Symbol Errors:**
- Undefined symbol references
- Circular symbol dependencies
- Symbol scope violations
- Address alignment errors

**Output Errors:**
- File system errors
- Insufficient disk space
- Permission errors
- Format specification errors

Testing and Validation
----------------------

**Encoding Tests:**
- Verify correct instruction encoding
- Test all instruction formats
- Validate immediate value handling
- Check register encoding

**Symbol Resolution Tests:**
- Forward reference resolution
- Backward reference resolution
- Cross-section references
- External symbol handling

**Binary Output Tests:**
- ELF format validation
- Raw binary verification
- Intel HEX format testing
- Round-trip testing (assemble/disassemble)

**Performance Tests:**
- Large program assembly
- Memory usage optimization
- Assembly speed benchmarks
- Output file size analysis

**Test Coverage:**
- 60+ instruction encoding tests
- 40+ symbol resolution tests
- 25+ binary output format tests
- 30+ optimization tests
- 20+ error handling tests

Integration Points
------------------

**AST Integration:**
- Direct AST node processing
- Visitor pattern implementation
- Node type-specific handling
- Metadata preservation

**Symbol Table Integration:**
- Symbol address assignment
- Scope resolution
- Reference validation
- Cross-reference tracking

**Output File Integration:**
- Multiple format support
- Configurable output options
- Metadata preservation
- Debug information generation 

Code Generation Architecture
-----------------------------

**Multi-Pass Strategy:**
The code generator employs a four-pass strategy to handle forward references and complex symbol resolution:

1. **Pseudo-Code Pass**: Expands pseudo-instructions and prepares the AST structure
2. **Line Number Pass**: Assigns addresses and creates symbol table entries
3. **Line Number Cleanup Pass**: Resolves cache misses and handles forward references
4. **Code Generation Pass**: Generates final machine code with resolved symbols

**Visitor Pattern Implementation:**
The code generator extends the `ASTProcessor` base class, implementing the visitor pattern for type-safe AST traversal.

**Symbol Integration:**
Tight integration with the symbol table enables multi-pass resolution and efficient address assignment.

Implementation Structure
------------------------

**CodeGenerator Class:**

.. code-block:: csharp

    public class CodeGenerator : ASTProcessor {
        public enum GeneratorPass {
            PsudoCode,        // Convert pseudo-instructions to real instructions
            LineNumber,       // Assign line numbers and addresses
            LineNumberCleanup,// Handle cache misses from forward references
            GenerateCode      // Generate final binary machine code
        }
        
        private GeneratorPass _currentPass;
        private MemoryStream _outputStream;
        private BinaryWriter _writer;
        private Dictionary<string, long> _sectionAddresses;
        private List<Symbol> _unresolvedSymbols;
        
        public byte[] Generate(ProgramNode node);
    }

**Core Generation Process:**

.. code-block:: csharp

    public byte[] Generate(ProgramNode node) {
        // Pass 1: Expand pseudo-instructions
        _currentPass = GeneratorPass.PsudoCode;
        ProcessASTNode(node);
        
        // Pass 2: Assign addresses and line numbers
        _currentPass = GeneratorPass.LineNumber;
        ProcessASTNode(node);
        
        // Pass 3: Handle forward reference cache misses
        _currentPass = GeneratorPass.LineNumberCleanup;
        ProcessASTNode(node);
        
        // Pass 4: Generate final machine code
        _currentPass = GeneratorPass.GenerateCode;
        _outputStream = new MemoryStream();
        _writer = new BinaryWriter(_outputStream);
        ProcessASTNode(node);
        
        return _outputStream.ToArray();
    }

Multi-Pass Strategy
-------------------

**Pass 1: Pseudo-Code Expansion**

Converts pseudo-instructions to real RISC-V instructions:

.. code-block:: csharp

    public AstNode ProcessASTNode(InstructionNodeTypeI node) {
        if (_currentPass == GeneratorPass.PsudoCode) {
            // Handle pseudo-instructions like "li" (load immediate)
            if (node.Mnemonic == "li") {
                // Convert to lui/addi sequence if immediate > 12 bits
                return ExpandLoadImmediate(node);
            }
        }
        return node;
    }

**Key Pseudo-Instructions Handled:**
- `li` (load immediate) → `lui` + `addi` sequence
- `la` (load address) → `auipc` + `addi` sequence  
- `nop` → `addi x0, x0, 0`
- `ret` → `jalr x0, x1, 0`

**Pass 2: Address Assignment**

Assigns memory addresses to all instructions and symbols:

.. code-block:: csharp

    public AstNode ProcessASTNode(LabelNode node) {
        if (_currentPass == GeneratorPass.LineNumber) {
            var symbol = node.SymbolReference;
            symbol.Address = _currentAddress;
            symbol.IsDefined = true;
            
            // Update all forward references
            foreach (var reference in symbol.References) {
                UpdateReferenceAddress(reference, symbol.Address);
            }
        }
        return node;
    }

**Address Calculation Features:**
- **Sequential Assignment**: Instructions receive consecutive addresses
- **Section-Aware**: Different sections maintain separate address spaces
- **Alignment Handling**: Automatic alignment for data and instructions
- **Forward Reference Tracking**: Deferred resolution for undefined symbols

**Pass 3: Cache Miss Resolution**

Handles symbols that were referenced before definition:

.. code-block:: csharp

    public AstNode ProcessASTNode(InstructionNodeTypeB node) {
        if (_currentPass == GeneratorPass.LineNumberCleanup) {
            // Check if branch target was resolved
            var symbol = GetSymbolForTarget(node.Target);
            if (symbol != null && !symbol.IsDefined) {
                // Add to unresolved list for error reporting
                _unresolvedSymbols.Add(symbol);
            }
        }
        return node;
    }

**Cache Miss Handling:**
- **Detection**: Identifies unresolved forward references
- **Error Reporting**: Collects undefined symbols for user feedback
- **Recovery**: Continues processing to find all errors

**Pass 4: Machine Code Generation**

Produces final binary output with resolved addresses:

.. code-block:: csharp

    public AstNode ProcessASTNode(InstructionNodeTypeR node) {
        if (_currentPass == GeneratorPass.GenerateCode) {
            uint machineCode = EncodeRTypeInstruction(
                node.Mnemonic,
                node.Rd,
                node.Rs1,
                node.Rs2
            );
            _writer.Write(machineCode);
            node.MachineCode = machineCode;
        }
        return node;
    }

Instruction Encoding
--------------------

**R-Type Instruction Encoding:**

.. code-block:: csharp

    private uint EncodeRTypeInstruction(string mnemonic, string rd, string rs1, string rs2) {
        uint opcode = GetOpcode(mnemonic);
        uint funct3 = GetFunct3(mnemonic);
        uint funct7 = GetFunct7(mnemonic);
        
        uint rdNum = RegisterToNumber(rd);
        uint rs1Num = RegisterToNumber(rs1);
        uint rs2Num = RegisterToNumber(rs2);
        
        return (funct7 << 25) | (rs2Num << 20) | (rs1Num << 15) | 
               (funct3 << 12) | (rdNum << 7) | opcode;
    }

**I-Type Instruction Encoding:**

.. code-block:: csharp

    private uint EncodeITypeInstruction(string mnemonic, string rd, string rs1, string immediate) {
        uint opcode = GetOpcode(mnemonic);
        uint funct3 = GetFunct3(mnemonic);
        
        uint rdNum = RegisterToNumber(rd);
        uint rs1Num = RegisterToNumber(rs1);
        uint immValue = ParseImmediate(immediate, 12); // 12-bit immediate
        
        return (immValue << 20) | (rs1Num << 15) | (funct3 << 12) | (rdNum << 7) | opcode;
    }

**Branch Instruction Encoding:**

.. code-block:: csharp

    private uint EncodeBTypeInstruction(string mnemonic, string rs1, string rs2, string target) {
        uint opcode = GetOpcode(mnemonic);
        uint funct3 = GetFunct3(mnemonic);
        
        uint rs1Num = RegisterToNumber(rs1);
        uint rs2Num = RegisterToNumber(rs2);
        
        // Calculate branch offset
        long targetAddress = GetTargetAddress(target);
        long offset = targetAddress - _currentAddress;
        uint immValue = EncodeBranchImmediate(offset);
        
        return EncodeImmediateBType(immValue) | (rs2Num << 20) | (rs1Num << 15) | 
               (funct3 << 12) | opcode;
    }

Symbol Resolution
-----------------

**Symbol Address Resolution:**

.. code-block:: csharp

    private long GetTargetAddress(string target) {
        // Check if target is a label/symbol
        if (IsSymbolReference(target)) {
            var symbol = _symbolTable.GetSymbol(target);
            if (symbol?.IsDefined == true) {
                return symbol.Address;
            } else {
                // Forward reference - will be resolved in later pass
                return 0;
            }
        } else {
            // Direct immediate value
            return ParseImmediate(target, 32);
        }
    }

**Forward Reference Handling:**

.. code-block:: csharp

    private void UpdateReferenceAddress(AstNode reference, long address) {
        switch (reference) {
            case InstructionNodeTypeB branch:
                // Update branch target with resolved address
                RecalculateBranchOffset(branch, address);
                break;
            case InstructionNodeTypeJ jump:
                // Update jump target with resolved address
                RecalculateJumpOffset(jump, address);
                break;
        }
    }

**Symbol Scope Resolution:**

.. code-block:: csharp

    public AstNode ProcessASTNode(SymbolDirectiveNode node) {
        if (_currentPass == GeneratorPass.LineNumber) {
            foreach (var symbolName in node.SymbolNames) {
                var symbol = _symbolTable.GetOrCreateSymbol(symbolName, SymbolScope.Unknown);
                
                // Promote symbol scope based on directive
                switch (node.DirectiveName) {
                    case ".global":
                        symbol.Scope = SymbolScope.Global;
                        break;
                    case ".local":
                        symbol.Scope = SymbolScope.Local;
                        break;
                }
            }
        }
        return node;
    }

Register Handling
-----------------

**Register Name Resolution:**

.. code-block:: csharp

    private uint RegisterToNumber(string registerName) {
        // Handle numeric registers (x0-x31)
        if (registerName.StartsWith("x")) {
            if (uint.TryParse(registerName.Substring(1), out uint regNum)) {
                return regNum & 0x1F; // Ensure 5-bit range
            }
        }
        
        // Handle ABI register names
        return registerName switch {
            "zero" => 0, "ra" => 1, "sp" => 2, "gp" => 3,
            "tp" => 4, "t0" => 5, "t1" => 6, "t2" => 7,
            "s0" or "fp" => 8, "s1" => 9,
            "a0" => 10, "a1" => 11, "a2" => 12, "a3" => 13,
            "a4" => 14, "a5" => 15, "a6" => 16, "a7" => 17,
            "s2" => 18, "s3" => 19, "s4" => 20, "s5" => 21,
            "s6" => 22, "s7" => 23, "s8" => 24, "s9" => 25,
            "s10" => 26, "s11" => 27, "t3" => 28, "t4" => 29,
            "t5" => 30, "t6" => 31,
            _ => throw new ArgumentException($"Invalid register: {registerName}")
        };
    }

**Floating-Point Register Support:**

.. code-block:: csharp

    private uint FloatRegisterToNumber(string registerName) {
        // Handle numeric FP registers (f0-f31)
        if (registerName.StartsWith("f")) {
            if (uint.TryParse(registerName.Substring(1), out uint regNum)) {
                return regNum & 0x1F;
            }
        }
        
        // Handle ABI FP register names
        return registerName switch {
            "ft0" => 0, "ft1" => 1, "ft2" => 2, "ft3" => 3,
            "ft4" => 4, "ft5" => 5, "ft6" => 6, "ft7" => 7,
            "fs0" => 8, "fs1" => 9, "fa0" => 10, "fa1" => 11,
            "fa2" => 12, "fa3" => 13, "fa4" => 14, "fa5" => 15,
            "fa6" => 16, "fa7" => 17, "fs2" => 18, "fs3" => 19,
            "fs4" => 20, "fs5" => 21, "fs6" => 22, "fs7" => 23,
            "fs8" => 24, "fs9" => 25, "fs10" => 26, "fs11" => 27,
            "ft8" => 28, "ft9" => 29, "ft10" => 30, "ft11" => 31,
            _ => throw new ArgumentException($"Invalid FP register: {registerName}")
        };
    }

Immediate Value Handling
------------------------

**Immediate Parsing and Validation:**

.. code-block:: csharp

    private uint ParseImmediate(string immediate, int bitWidth) {
        long value;
        
        // Handle different number formats
        if (immediate.StartsWith("0x")) {
            value = Convert.ToInt64(immediate, 16);
        } else if (immediate.StartsWith("0b")) {
            value = Convert.ToInt64(immediate.Substring(2), 2);
        } else {
            value = Convert.ToInt64(immediate);
        }
        
        // Validate range for signed immediate
        long maxValue = (1L << (bitWidth - 1)) - 1;
        long minValue = -(1L << (bitWidth - 1));
        
        if (value > maxValue || value < minValue) {
            throw new ArgumentException($"Immediate value {value} out of range for {bitWidth}-bit field");
        }
        
        return (uint)(value & ((1L << bitWidth) - 1));
    }

**Branch Offset Calculation:**

.. code-block:: csharp

    private uint EncodeBranchImmediate(long offset) {
        // Branch offsets must be even (2-byte aligned)
        if (offset % 2 != 0) {
            throw new ArgumentException("Branch offset must be even");
        }
        
        // 12-bit signed immediate, but LSB is implicit (always 0)
        long scaledOffset = offset >> 1;
        
        if (scaledOffset > 2047 || scaledOffset < -2048) {
            throw new ArgumentException($"Branch offset {offset} out of range");
        }
        
        return (uint)(scaledOffset & 0xFFF);
    }

Error Handling
--------------

**Compilation Error Detection:**

.. code-block:: csharp

    public class CodeGenerationError {
        public string Message { get; set; }
        public int LineNumber { get; set; }
        public string SourceFile { get; set; }
        public ErrorSeverity Severity { get; set; }
        public string Context { get; set; }
    }

**Error Collection and Reporting:**

.. code-block:: csharp

    private List<CodeGenerationError> _errors = new List<CodeGenerationError>();
    
    private void ReportError(string message, AstNode node, ErrorSeverity severity = ErrorSeverity.Error) {
        _errors.Add(new CodeGenerationError {
            Message = message,
            LineNumber = node.LineNumber,
            SourceFile = node.SourceFile,
            Severity = severity,
            Context = GetNodeContext(node)
        });
    }

**Common Error Scenarios:**
- **Undefined Symbols**: References to non-existent labels or symbols
- **Invalid Registers**: Use of non-existent or malformed register names
- **Immediate Overflow**: Immediate values exceeding instruction field limits
- **Alignment Errors**: Misaligned branch targets or data access
- **Instruction Format Errors**: Invalid operand combinations for instruction types

Output Format Support
---------------------

**Binary Output Generation:**

.. code-block:: csharp

    public byte[] GenerateBinary(ProgramNode program) {
        var output = Generate(program);
        return output; // Raw binary machine code
    }

**Intel HEX Format Support:**

.. code-block:: csharp

    public string GenerateIntelHex(ProgramNode program, uint baseAddress = 0) {
        var binary = Generate(program);
        return ConvertToIntelHex(binary, baseAddress);
    }

**ELF Object File Support (Planned):**

.. code-block:: csharp

    public byte[] GenerateELF(ProgramNode program) {
        var binary = Generate(program);
        return CreateELFObject(binary, program.SymbolTable);
    }

Performance Optimization
------------------------

**Instruction Encoding Cache:**

.. code-block:: csharp

    private static readonly Dictionary<string, (uint opcode, uint funct3, uint funct7)> _instructionCache 
        = new Dictionary<string, (uint, uint, uint)> {
            ["add"] = (0x33, 0x0, 0x00),
            ["sub"] = (0x33, 0x0, 0x20),
            ["addi"] = (0x13, 0x0, 0x00),
            // ... other instructions
        };

**Register Name Cache:**

.. code-block:: csharp

    private static readonly Dictionary<string, uint> _registerCache = 
        new Dictionary<string, uint> {
            ["x0"] = 0, ["x1"] = 1, ["x2"] = 2, // ... numeric registers
            ["zero"] = 0, ["ra"] = 1, ["sp"] = 2, // ... ABI names
        };

**Symbol Lookup Optimization:**

.. code-block:: csharp

    private readonly Dictionary<string, Symbol> _symbolCache = new Dictionary<string, Symbol>();
    
    private Symbol GetCachedSymbol(string name) {
        if (!_symbolCache.TryGetValue(name, out var symbol)) {
            symbol = _symbolTable.GetSymbol(name);
            if (symbol != null) {
                _symbolCache[name] = symbol;
            }
        }
        return symbol;
    }

Testing and Validation
-----------------------

**Machine Code Validation:**

The code generator includes comprehensive testing for instruction encoding:

.. code-block:: csharp

    [Test]
    public void MachineCode() {
        var asm = """
            add x1, x2, x3
            sub x4, x5, x6
            addi x7, x8, 100
            """;
        
        var lexer = new Lexer(asm);
        var ast = Parser.Parse(lexer);
        var generator = new CodeGenerator();
        var binary = generator.Generate(ast);
        
        // Verify generated machine code
        Assert.AreEqual(0x003100B3u, BitConverter.ToUInt32(binary, 0)); // add x1, x2, x3
        Assert.AreEqual(0x40628233u, BitConverter.ToUInt32(binary, 4)); // sub x4, x5, x6
        Assert.AreEqual(0x06440393u, BitConverter.ToUInt32(binary, 8)); // addi x7, x8, 100
    }

**Error Condition Testing:**
- **Invalid instruction combinations**
- **Out-of-range immediate values**
- **Undefined symbol references**
- **Register allocation conflicts**
- **Address alignment violations**

The KUICK code generator provides a robust, multi-pass approach to machine code generation with comprehensive error handling and optimization features, enabling efficient transformation of assembly programs into executable RISC-V machine code. 