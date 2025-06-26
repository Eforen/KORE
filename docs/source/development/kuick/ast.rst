Abstract Syntax Tree (AST)
============================

The KUICK AST provides a structured, hierarchical representation of RISC-V assembly programs. Built during the parsing phase, it serves as the primary data structure for symbol resolution and code generation.

AST Design Philosophy
---------------------

**Immutable Structure:**
AST nodes are designed to be immutable after construction, enabling safe multi-threaded processing and preventing accidental modification.

**Type Safety:**
Strong typing throughout the node hierarchy catches errors at compile time and provides clear interfaces for processing.

**Visitor Pattern Support:**
The AST implements the visitor pattern through the `ASTProcessor` interface, enabling clean separation of traversal logic from node structure.

**Efficient Traversal:**
Node references and parent-child relationships are optimized for fast traversal during code generation.

Node Hierarchy
---------------

The KUICK AST follows a hierarchical structure rooted at the program level:

```
ProgramNode (root)
├── SymbolTable (integrated)
├── SectionNode[]
│   ├── InstructionNode[]
│   │   ├── InstructionNodeTypeR
│   │   ├── InstructionNodeTypeI  
│   │   ├── InstructionNodeTypeS
│   │   ├── InstructionNodeTypeB
│   │   ├── InstructionNodeTypeU
│   │   └── InstructionNodeTypeJ
│   ├── LabelNode[]
│   └── SymbolDirectiveNode[]
└── ErrorNode[] (for error recovery)
```

Core AST Nodes
---------------

**AstNode (Base Class)**

The foundational class for all AST nodes, providing common functionality:

.. code-block:: csharp

    public abstract class AstNode {
        public string SourceFile { get; set; }
        public int LineNumber { get; set; }
        public int Column { get; set; }
        public long Address { get; set; }
        
        // Type identification for visitor pattern
        public abstract AstNodeType NodeType { get; }
        
        // Error information
        public bool HasErrors { get; set; }
        public List<string> Errors { get; set; }
    }

**Common Properties:**
- **Source Location**: File, line, and column information for error reporting
- **Address Information**: Memory address for code generation
- **Error Handling**: Built-in error tracking and reporting
- **Type Identification**: Runtime type identification for processing

**ProgramNode (Root)**

The top-level container for the entire assembly program:

.. code-block:: csharp

    public class ProgramNode : AstNode {
        public SymbolTable SymbolTable { get; set; }
        public List<SectionNode> Sections { get; set; }
        public string DefaultSection { get; set; } = ".text";
        
        public override AstNodeType NodeType => AstNodeType.Program;
    }

**Key Features:**
- **Integrated Symbol Table**: Centralized symbol management
- **Section Management**: Organization of code and data sections
- **Default Section Handling**: Automatic section assignment for unlabeled content

**SectionNode**

Represents a program section (`.text`, `.data`, etc.):

.. code-block:: csharp

    public class SectionNode : AstNode {
        public string SectionName { get; set; }
        public List<AstNode> Contents { get; set; }
        public long StartAddress { get; set; }
        public long CurrentAddress { get; set; }
        
        public override AstNodeType NodeType => AstNodeType.Section;
    }

**Section Management:**
- **Flexible Content**: Contains instructions, labels, and directives
- **Address Tracking**: Maintains current and start addresses
- **Name-Based Organization**: Sections identified by string names

Instruction Nodes
-----------------

**InstructionNode (Base)**

Abstract base class for all instruction types:

.. code-block:: csharp

    public abstract class InstructionNode : AstNode {
        public string Mnemonic { get; set; }
        public List<string> Operands { get; set; }
        public uint MachineCode { get; set; }
        
        // Instruction format identification
        public abstract InstructionFormat Format { get; }
    }

**Common Instruction Properties:**
- **Mnemonic**: Instruction name (e.g., "add", "lw", "beq")
- **Operands**: List of operand strings as parsed
- **Machine Code**: Generated binary instruction
- **Format Identification**: RISC-V instruction format type

**R-Type Instructions**

Register-to-register operations:

.. code-block:: csharp

    public class InstructionNodeTypeR : InstructionNode {
        public string Rd { get; set; }    // Destination register
        public string Rs1 { get; set; }   // Source register 1  
        public string Rs2 { get; set; }   // Source register 2
        
        public override InstructionFormat Format => InstructionFormat.R;
        public override AstNodeType NodeType => AstNodeType.InstructionR;
    }

**Examples**: `add`, `sub`, `xor`, `slt`

**I-Type Instructions**

Immediate value operations:

.. code-block:: csharp

    public class InstructionNodeTypeI : InstructionNode {
        public string Rd { get; set; }        // Destination register
        public string Rs1 { get; set; }       // Source register
        public string Immediate { get; set; } // Immediate value
        
        public override InstructionFormat Format => InstructionFormat.I;
        public override AstNodeType NodeType => AstNodeType.InstructionI;
    }

**Examples**: `addi`, `lw`, `jalr`

**S-Type Instructions**

Store operations:

.. code-block:: csharp

    public class InstructionNodeTypeS : InstructionNode {
        public string Rs1 { get; set; }       // Base register
        public string Rs2 { get; set; }       // Source register
        public string Immediate { get; set; } // Offset value
        
        public override InstructionFormat Format => InstructionFormat.S;
        public override AstNodeType NodeType => AstNodeType.InstructionS;
    }

**Examples**: `sw`, `sh`, `sb`

**B-Type Instructions**

Branch operations:

.. code-block:: csharp

    public class InstructionNodeTypeB : InstructionNode {
        public string Rs1 { get; set; }       // Source register 1
        public string Rs2 { get; set; }       // Source register 2
        public string Target { get; set; }    // Branch target (label or immediate)
        
        public override InstructionFormat Format => InstructionFormat.B;
        public override AstNodeType NodeType => AstNodeType.InstructionB;
    }

**Examples**: `beq`, `bne`, `blt`, `bge`

**U-Type Instructions**

Upper immediate operations:

.. code-block:: csharp

    public class InstructionNodeTypeU : InstructionNode {
        public string Rd { get; set; }        // Destination register
        public string Immediate { get; set; } // 20-bit immediate value
        
        public override InstructionFormat Format => InstructionFormat.U;
        public override AstNodeType NodeType => AstNodeType.InstructionU;
    }

**Examples**: `lui`, `auipc`

**J-Type Instructions**

Jump operations:

.. code-block:: csharp

    public class InstructionNodeTypeJ : InstructionNode {
        public string Rd { get; set; }     // Destination register (for return address)
        public string Target { get; set; } // Jump target (label or immediate)
        
        public override InstructionFormat Format => InstructionFormat.J;
        public override AstNodeType NodeType => AstNodeType.InstructionJ;
    }

**Examples**: `jal`

Symbol and Label Nodes
----------------------

**LabelNode**

Represents label definitions in the assembly code:

.. code-block:: csharp

    public class LabelNode : AstNode {
        public string LabelName { get; set; }
        public string Section { get; set; }
        public Symbol SymbolReference { get; set; }
        
        public override AstNodeType NodeType => AstNodeType.Label;
    }

**Label Features:**
- **Name Management**: String-based label identification
- **Section Association**: Labels tied to specific sections
- **Symbol Integration**: Direct reference to symbol table entries

**SymbolDirectiveNode**

Handles `.global`, `.local`, and similar directives:

.. code-block:: csharp

    public class SymbolDirectiveNode : AstNode {
        public string DirectiveName { get; set; }  // e.g., ".global", ".local"
        public List<string> SymbolNames { get; set; }
        public SymbolScope TargetScope { get; set; }
        
        public override AstNodeType NodeType => AstNodeType.SymbolDirective;
    }

**Directive Processing:**
- **Multi-Symbol Support**: Single directive can affect multiple symbols
- **Scope Management**: Integration with symbol table scoping
- **Forward Declaration**: Support for symbols defined later

AST Processing
--------------

**Visitor Pattern Implementation**

The AST uses the visitor pattern for traversal and processing:

.. code-block:: csharp

    public abstract class ASTProcessor {
        // Node-specific processing methods
        public abstract AstNode ProcessASTNode(InstructionNodeTypeR node);
        public abstract AstNode ProcessASTNode(InstructionNodeTypeI node);
        public abstract AstNode ProcessASTNode(InstructionNodeTypeS node);
        public abstract AstNode ProcessASTNode(InstructionNodeTypeB node);
        public abstract AstNode ProcessASTNode(InstructionNodeTypeU node);
        public abstract AstNode ProcessASTNode(InstructionNodeTypeJ node);
        public abstract AstNode ProcessASTNode(LabelNode node);
        public abstract AstNode ProcessASTNode(SymbolDirectiveNode node);
        
        // Generic processing entry point
        public AstNode ProcessASTNode(AstNode node) {
            return node.NodeType switch {
                AstNodeType.InstructionR => ProcessASTNode((InstructionNodeTypeR)node),
                AstNodeType.InstructionI => ProcessASTNode((InstructionNodeTypeI)node),
                AstNodeType.InstructionS => ProcessASTNode((InstructionNodeTypeS)node),
                AstNodeType.InstructionB => ProcessASTNode((InstructionNodeTypeB)node),
                AstNodeType.InstructionU => ProcessASTNode((InstructionNodeTypeU)node),
                AstNodeType.InstructionJ => ProcessASTNode((InstructionNodeTypeJ)node),
                AstNodeType.Label => ProcessASTNode((LabelNode)node),
                AstNodeType.SymbolDirective => ProcessASTNode((SymbolDirectiveNode)node),
                _ => throw new NotImplementedException($"Processing not implemented for {node.NodeType}")
            };
        }
    }

**Processing Benefits:**
- **Type Safety**: Compile-time checking of node types
- **Extensibility**: Easy addition of new processing logic
- **Separation of Concerns**: Processing logic separate from node structure
- **Multiple Passes**: Same AST can be processed multiple times

**Tree Traversal Patterns**

Common traversal patterns for AST processing:

.. code-block:: csharp

    // Depth-first processing
    public void TraverseDepthFirst(AstNode node, ASTProcessor processor) {
        // Process children first
        if (node is ProgramNode program) {
            foreach (var section in program.Sections) {
                TraverseDepthFirst(section, processor);
            }
        } else if (node is SectionNode section) {
            foreach (var content in section.Contents) {
                TraverseDepthFirst(content, processor);
            }
        }
        
        // Process current node
        processor.ProcessASTNode(node);
    }
    
    // Section-aware processing
    public void ProcessBySection(ProgramNode program, ASTProcessor processor) {
        foreach (var section in program.Sections) {
            // Process all nodes in section sequentially
            foreach (var node in section.Contents) {
                processor.ProcessASTNode(node);
            }
        }
    }

Symbol Integration
------------------

**Symbol-AST Relationships**

The AST maintains tight integration with the symbol table:

.. code-block:: csharp

    // Labels create symbol table entries
    public void ProcessLabel(LabelNode label, SymbolTable symbols) {
        var symbol = symbols.DefineSymbol(
            label.LabelName, 
            label.LineNumber, 
            label.Section
        );
        symbol.Address = label.Address;
        label.SymbolReference = symbol;
    }
    
    // Instructions reference symbols
    public void ProcessBranch(InstructionNodeTypeB branch, SymbolTable symbols) {
        if (branch.Target.StartsWith("label_")) {
            var symbol = symbols.GetOrCreateSymbol(branch.Target, SymbolScope.Unknown);
            symbol.References.Add(branch);
        }
    }

**Forward Reference Handling:**
- **Deferred Resolution**: Symbols can be referenced before definition
- **Reference Tracking**: All references recorded for later patching
- **Multi-Pass Support**: AST structure supports multiple resolution passes

Error Handling in AST
---------------------

**Error Node Integration**

The AST includes specialized nodes for error recovery:

.. code-block:: csharp

    public class ErrorNode : AstNode {
        public string ErrorMessage { get; set; }
        public TokenData[] ErrorTokens { get; set; }
        public AstNodeType ExpectedType { get; set; }
        
        public override AstNodeType NodeType => AstNodeType.Error;
    }

**Error Recovery Strategy:**
- **Partial Parsing**: Continue parsing after recoverable errors
- **Error Containment**: Isolate errors to specific nodes
- **Context Preservation**: Maintain structural integrity around errors
- **Multiple Error Support**: Multiple errors in single parse

**Error Reporting Integration:**

.. code-block:: csharp

    public class AstNodeError {
        public string Message { get; set; }
        public int LineNumber { get; set; }
        public int Column { get; set; }
        public ErrorSeverity Severity { get; set; }
        public string SuggestedFix { get; set; }
    }

AST Optimization
----------------

**Memory Optimization:**
- **String Interning**: Repeated strings (mnemonics, register names) are interned
- **Lazy Evaluation**: Expensive computations deferred until needed
- **Reference Sharing**: Common structures shared between nodes where safe

**Performance Optimization:**
- **Type-Based Dispatch**: Fast node type identification for processing
- **Cache-Friendly Layout**: Node structure optimized for memory access patterns
- **Minimal Allocations**: Reduced object creation during traversal

**Space Optimization:**
- **Compact Representation**: Node data packed efficiently
- **Optional Fields**: Uncommon data stored separately to reduce base node size
- **Reference Counting**: Automatic cleanup of unused nodes

Testing Integration
-------------------

**AST Testing Framework:**

The AST includes comprehensive testing support:

.. code-block:: csharp

    public class AstTestHelper {
        // AST comparison for testing
        public static bool AreEqual(AstNode expected, AstNode actual);
        
        // AST construction helpers
        public static InstructionNodeTypeR CreateRInstruction(
            string mnemonic, string rd, string rs1, string rs2);
        
        // Symbol table validation
        public static bool ValidateSymbolIntegrity(ProgramNode program);
        
        // Error validation
        public static List<AstNodeError> CollectAllErrors(AstNode root);
    }

**Test Coverage Areas:**
- **Node Construction**: Verify correct node creation from parser
- **Symbol Integration**: Validate symbol table relationships
- **Traversal Logic**: Confirm visitor pattern implementation
- **Error Handling**: Test error node creation and recovery
- **Memory Usage**: Monitor memory allocation patterns

The KUICK AST provides a robust, type-safe foundation for assembly program representation, enabling efficient processing while maintaining clear structure and extensibility. 