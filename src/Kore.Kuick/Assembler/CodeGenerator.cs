using Kore.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kore.Kuick.Assembler {
    public class CodeGenerator : ASTProcessor {
        private Exception ThrowAssemblerPanic(string msg = "Assembler Panic") {
            throw new Exception($"Assembler Panic: {msg}");
        }
        private int assignLineNumber(int advance = 1) {
            int current = nextLineNumber;
            nextLineNumber += advance;
            return current;
        }
        private int assignLineNumber(AstNode node) {
            node.lineNumber = assignLineNumber();
            return node.lineNumber;
        }
        private int assignLineNumber(LabelNode label)
        {
            if (labels.ContainsKey(label.Name)) throw ThrowAssemblerPanic($"Label `{label.Name}` Assigned Twice");
            label.lineNumber = assignLineNumber(0);
            labels.Add(label.Name, label);
            return label.lineNumber;
        }
        private int getLineNumber(string label) {
            if(labels.ContainsKey(label)) return labels[label].lineNumber;
            labelCacheMiss = true;
            return -1;
        }

        // Symbol address assignment methods
        private void assignSymbolAddress(string symbolName, int address) {
            if (currentSymbolTable != null) {
                var symbol = currentSymbolTable.GetSymbol(symbolName);
                if (symbol != null) {
                    symbol.Address = address;
                    symbol.IsDefined = true;
                }
            }
        }

        private int getSymbolAddress(string symbolName) {
            if (currentSymbolTable != null) {
                var symbol = currentSymbolTable.GetSymbol(symbolName);
                if (symbol != null && symbol.IsDefined) {
                    return (int)symbol.Address;
                }
            }
            symbolCacheMiss = true;
            return -1;
        }

        // Address assignment methods - separate from line numbers
        private int assignAddress(int advance = 1) {
            int current = nextAddress;
            nextAddress += advance * 4; // RISC-V instructions are 4 bytes each
            return current;
        }

        private int assignAddress(LabelNode label) {
            // Assign address and also update symbol table
            int address = assignAddress(0);
            label.byteAddress = address; // Set the byte address on the node
            if (currentSymbolTable != null) {
                assignSymbolAddress(label.Name, address);
            }
            // Resolve any pending symbol misses for this label
            resolveSymbolMisses(label.Name);
            return address;
        }

        private int assignAddress(AstNode node) {
            // Assign address to any instruction node
            int address = assignAddress();
            node.byteAddress = address;
            return address;
        }

        private int getLabelAddress(string label) {
            if(labels.ContainsKey(label)) {
                // Get the address directly from the label node
                var labelNode = labels[label];
                if (labelNode.byteAddress >= 0) {
                    return labelNode.byteAddress;
                }
                // Fallback to line number conversion if byteAddress not set
                return labelNode.lineNumber * 4;
            }
            return -1; // Not found
        }

        public enum GeneratorPass : byte {
            /// <summary> Convert PsudoCode and Directives to AST Nodes </summary>
            PsudoCode = 0,
            /// <summary> Convert PsudoCode to Code </summary>
            LineNumber = 1,
            /// <summary> Convert PsudoCode to Code </summary>
            LineNumberCleanup = 2,
            /// <summary> Convert PsudoCode to Code </summary>
            GenerateCode = 3,
        }
        public GeneratorPass phase = GeneratorPass.PsudoCode;

        public List<byte> machineCode = new List<byte>();
        public Dictionary<string, LabelNode> labels = new Dictionary<string, LabelNode>();
        public bool labelCacheMiss = false;
        public bool symbolCacheMiss = false;
        public int nextLineNumber = 0;  // Tracks line numbers for debugging (0, 1, 2, 3...)
        private SymbolTable currentSymbolTable = null;
        public int nextAddress = 0; // Tracks byte addresses (0, 4, 8, 12...)

        // New symbol miss tracking system
        private Dictionary<string, List<AstNode>> symbolMisses = new Dictionary<string, List<AstNode>>();
        private Dictionary<AstNode, AstNode> nodeReplacements = new Dictionary<AstNode, AstNode>();

        // Helper methods for symbol miss tracking
        private void addSymbolMiss(string symbolName, AstNode node) {
            if (!symbolMisses.ContainsKey(symbolName)) {
                symbolMisses[symbolName] = new List<AstNode>();
            }
            symbolMisses[symbolName].Add(node);
        }

        private void resolveSymbolMisses(string symbolName) {
            if (symbolMisses.ContainsKey(symbolName)) {
                var originalPhase = phase;
                phase = GeneratorPass.LineNumberCleanup; // Temporarily set cleanup phase
                
                foreach (var missedNode in symbolMisses[symbolName]) {
                    var replacementNode = missedNode.CallProcessor(this); // Process the missed node to resolve its symbol
                    if (replacementNode != null) {
                        nodeReplacements[missedNode] = replacementNode;
                    }
                }
                
                phase = originalPhase; // Restore original phase
                symbolMisses.Remove(symbolName); // Remove resolved misses
            }
        }

        public byte[] Generate(ProgramNode node) {
            machineCode.Clear();
            labels.Clear();
            labelCacheMiss = false;
            symbolCacheMiss = false;
            nextLineNumber = 0;
            nextAddress = 0;
            symbolMisses.Clear();
            nodeReplacements.Clear();
            currentSymbolTable = node.SymbolTable;

            // TODO: Implement code generation based on the given AST node.
            phase = GeneratorPass.PsudoCode;
            node.CallProcessor(this);
            phase = GeneratorPass.LineNumber;
            node.CallProcessor(this);
            // Check that all symbol misses have been resolved
            foreach(var symbol in symbolMisses) {
                if(symbol.Value.Count > 0) {
                    throw new Exception($"Symbol {symbol.Key} has unresolved misses");
                }
            }
            phase = GeneratorPass.GenerateCode;
            node.CallProcessor(this);

            return machineCode.ToArray();
        }

        public AstNode ProcessASTNode(ProgramNode node) {
            // Set the current symbol table for this program
            currentSymbolTable = node.SymbolTable;
            
            foreach(SectionNode n in node.Sections) {
                n.CallProcessor(this);
            }
            return null;
        }

        public AstNode ProcessASTNode(SectionNode node) {
            AstNode cache = null;
            for(int i = 0; i < node.Contents.Count; i++) {
                cache = node.Contents[i].CallProcessor(this);
                if(cache == null) continue;
                node.Contents[i] = cache;
            }
            
            // Apply any node replacements from symbol miss resolution
            if (nodeReplacements.Count > 0) {
                for(int i = 0; i < node.Contents.Count; i++) {
                    if (nodeReplacements.ContainsKey(node.Contents[i])) {
                        node.Contents[i] = nodeReplacements[node.Contents[i]];
                    }
                }
            }
            
            return null;
        }

        public AstNode ProcessASTNode(DirectiveNode node) {
            // Handle IntDirectiveNode (like .org) separately
            if (node is IntDirectiveNode intDirective) {
                return ProcessASTNode(intDirective);
            }
            
            if(phase != GeneratorPass.PsudoCode) throw ThrowAssemblerPanic("Directive Found in AST after first pass");
            
            //TODO: Decide if the following directives should be handled here or later
            // Handle .byte
            // Handle .word
            // Handle .string
            throw new NotImplementedException();
        }

        public AstNode ProcessASTNode(IntDirectiveNode node) {
            switch(phase) {
                case GeneratorPass.PsudoCode:
                    // Validate directive in this pass
                    if (node.Name == ".org") {
                        // Validate .org address
                        if (node.Value < 0) {
                            throw new Exception($".org directive cannot have negative address on line {node.lineNumber}: {node.Value}");
                        }
                        // .org addresses should be aligned to 4-byte boundaries for RISC-V
                        if (node.Value % 4 != 0) {
                            throw new Exception($".org directive address must be 4-byte aligned on line {node.lineNumber}: {node.Value}");
                        }
                    }
                    return null;
                    
                case GeneratorPass.LineNumber:
                    // Actually set the address counter
                    if (node.Name == ".org") {
                        // Confirm that the address is greater than the current address
                        if (node.Value < nextAddress) {
                            throw new Exception($"KUICK only supports forward .org directives, thus cannot move address counter backwards on line {node.lineNumber}: {node.Value}");
                        }
                        nextAddress = node.Value;
                        // Note: we don't increment line number for directives
                    }
                    return null;
                    
                case GeneratorPass.LineNumberCleanup:
                case GeneratorPass.GenerateCode:
                    // Nothing to do in these phases for .org
                    return null;
                    
                default:
                    return null;
            }
        }

        public AstNode ProcessASTNode<T>(InstructionNode<T> node) {
            throw new NotImplementedException();
        }

        public AstNode ProcessASTNode(InstructionNodeTypeR node) {
            switch(phase) {
                case GeneratorPass.PsudoCode:
                    return null;
                case GeneratorPass.LineNumber:
                    assignLineNumber(node);
                    assignAddress(node); // Also assign address for this instruction
                    return null;
                case GeneratorPass.LineNumberCleanup:
                    return null;
                case GeneratorPass.GenerateCode:
                    // Create an empty byte array to hold the machine code
                    byte[] machineCode = new byte[4];

                    // Set the opcode bits in the first 7 bits of the machine code
                    machineCode[0] = (byte)(0b0110011);

                    // Set the destination register in bits 7-11
                    machineCode[1] = (byte)(((byte)node.rd) << 7);

                    // Set the source register 1 in bits 15-19
                    machineCode[1] |= (byte)(((byte)node.rs1) << 3);

                    // Set the source register 2 in bits 20-24
                    machineCode[2] = (byte)(((byte)node.rs2) << 7);

                    // Set the funct3 bits in bits 12-14
                    //machineCode[2] |= (byte)(((byte)node.funct3) << 4);

                    // Set the funct7 bits in bits 25-31
                    //machineCode[3] = (byte)(((byte)node.funct7) << 1);

                    //TODO: Replace node with binary AST Node

                    return null;
                default:
                    return null;
            }
            throw new NotImplementedException();
        }

        /// <summary> Generates the machine code for an I-Type instruction </summary>
        public AstNode ProcessASTNode(InstructionNodeTypeI node) {
            switch(phase) {
                case GeneratorPass.PsudoCode:
                    return null;
                case GeneratorPass.LineNumber:
                    assignLineNumber(node);
                    assignAddress(node); // Also assign address for this instruction
                    return null;
                case GeneratorPass.LineNumberCleanup:
                    return null;
                case GeneratorPass.GenerateCode:
                    return null;
                default:
                    return null;
            }
        }

        public AstNode ProcessASTNode(InstructionNodeTypeU node) {
            switch(phase) {
                case GeneratorPass.PsudoCode:
                    return null;
                case GeneratorPass.LineNumber:
                    assignLineNumber(node);
                    assignAddress(node); // Also assign address for this instruction
                    return null;
                case GeneratorPass.LineNumberCleanup:
                    return null;
                case GeneratorPass.GenerateCode:
                    return null;
                default:
                    return null;
            }
        }

        public AstNode ProcessASTNode(InstructionNodeTypeBImmediate node) {
            switch(phase) {
                case GeneratorPass.PsudoCode:
                    return null;
                case GeneratorPass.LineNumber:
                    assignLineNumber(node);
                    assignAddress(node); // Also assign address for this instruction
                    return null;
                case GeneratorPass.LineNumberCleanup:
                    return null;
                case GeneratorPass.GenerateCode:
                    return null;
                default:
                    return null;
            }
        }

        public AstNode ProcessASTNode(InstructionNodeTypeBLabel node) {
            switch(phase) {
                case GeneratorPass.PsudoCode:
                    return null;
                case GeneratorPass.LineNumber:
                    // Get this instructions Line number
                    assignLineNumber(node);
                    assignAddress(node); // Also assign address for this instruction
                    // Fall Through
                    break;
                case GeneratorPass.LineNumberCleanup:
                    // This phase is for resolving symbol misses
                    break;
                case GeneratorPass.GenerateCode:
                    return null;
                default:
                    return null;
            }

            // Try to get line number of label (B-type uses line numbers, not addresses)
            int labelLineNum = getLineNumber(node.label);
            if(labelLineNum < 0) {
                // Label not found, add to miss list if we're in LineNumber phase
                if (phase == GeneratorPass.LineNumber) {
                    addSymbolMiss(node.label, node);
                }
                return null; // Can't resolve yet
            }
            
            // Label found, create immediate instruction
            var inst = new InstructionNodeTypeBImmediate(node.op, node.rs1, node.rs2, labelLineNum);
            inst.lineNumber = node.lineNumber;
            inst.byteAddress = node.byteAddress;
            return inst;
        }

        public AstNode ProcessASTNode(InstructionNodeTypeJImmediate node) {
            switch(phase) {
                case GeneratorPass.PsudoCode:
                    return null;
                case GeneratorPass.LineNumber:
                    assignLineNumber(node);
                    assignAddress(node); // Also assign address for this instruction
                    return null;
                case GeneratorPass.LineNumberCleanup:
                    return null;
                case GeneratorPass.GenerateCode:
                    return null;
                default:
                    return null;
            }
        }

        public AstNode ProcessASTNode(InstructionNodeTypeJLabel node) {
            switch(phase) {
                case GeneratorPass.PsudoCode:
                    return null;
                case GeneratorPass.LineNumber:
                    // Get this instructions Line number
                    assignLineNumber(node);
                    assignAddress(node); // Also assign address for this instruction
                    // Fall Through
                    break;
                case GeneratorPass.LineNumberCleanup:
                    // This phase is for resolving symbol misses
                    break;
                case GeneratorPass.GenerateCode:
                    return null;
                default:
                    return null;
            }

            // Try to get address of label
            int labelAddr = getLabelAddress(node.label);
            if(labelAddr < 0) {
                // Label not found, add to miss list if we're in LineNumber phase
                if (phase == GeneratorPass.LineNumber) {
                    addSymbolMiss(node.label, node);
                }
                return null; // Can't resolve yet
            }
            
            // Label found, create immediate instruction
            var inst = new InstructionNodeTypeJImmediate(node.op, node.rd, labelAddr);
            inst.lineNumber = node.lineNumber;
            inst.byteAddress = node.byteAddress;
            return inst;
        }

        public AstNode ProcessASTNode(InstructionNodeTypeMisc node) {
            throw new NotImplementedException();
        }

        public AstNode ProcessASTNode(InlineDirectiveNode node) {
            throw new NotImplementedException();
        }

        public AstNode ProcessASTNode(LabelNode node) {
            if(phase != GeneratorPass.LineNumber) return null;
            assignLineNumber(node);
            assignAddress(node); // Also assign the address for this label
            if(node.lineNumber < 0) throw ThrowAssemblerPanic("Error in Label Linenumber Assignment");
            return null;
        }

        public AstNode ProcessASTNode(CommentNode node) {
            return null;
        }

        // New symbol-based node processing methods
        public AstNode ProcessASTNode(SymbolDirectiveNode node) {
            // For now, just pass through - symbol directives are handled at parse time
            // In the future, this could be used for additional processing or validation
            return null;
        }

        public AstNode ProcessASTNode(InstructionNodeTypeBSymbol node) {
            // Process symbol-based B-type instruction similar to InstructionNodeTypeBLabel
            switch(phase) {
                case GeneratorPass.PsudoCode:
                    return null;
                case GeneratorPass.LineNumber:
                    // Get this instructions Line number
                    assignLineNumber(node);
                    assignAddress(node); // Also assign address for this instruction
                    // Fall Through
                    break;
                case GeneratorPass.LineNumberCleanup:
                    // This phase is for resolving symbol misses
                    break;
                case GeneratorPass.GenerateCode:
                    return null;
                default:
                    return null;
            }

            // Try to get address of symbol
            int symbolAddr = getSymbolAddress(node.SymbolReference.SymbolName);
            if(symbolAddr < 0) {
                // Symbol not found, add to miss list if we're in LineNumber phase
                if (phase == GeneratorPass.LineNumber) {
                    addSymbolMiss(node.SymbolReference.SymbolName, node);
                }
                return null; // Can't resolve yet
            }
            
            // Symbol found, create immediate instruction
            var inst = new InstructionNodeTypeBImmediate(node.op, node.rs1, node.rs2, symbolAddr);
            inst.lineNumber = node.lineNumber;
            inst.byteAddress = node.byteAddress;
            return inst;
        }

        public AstNode ProcessASTNode(InstructionNodeTypeJSymbol node) {
            // Process symbol-based J-type instruction similar to InstructionNodeTypeJLabel
            switch(phase) {
                case GeneratorPass.PsudoCode:
                    return null;
                case GeneratorPass.LineNumber:
                    // Get this instructions Line number
                    assignLineNumber(node);
                    assignAddress(node); // Also assign address for this instruction
                    // Fall Through
                    break;
                case GeneratorPass.LineNumberCleanup:
                    // This phase is for resolving symbol misses
                    break;
                case GeneratorPass.GenerateCode:
                    return null;
                default:
                    return null;
            }

            // Try to get address of symbol
            int symbolAddr = getSymbolAddress(node.SymbolReference.SymbolName);
            if(symbolAddr < 0) {
                // Symbol not found, add to miss list if we're in LineNumber phase
                if (phase == GeneratorPass.LineNumber) {
                    addSymbolMiss(node.SymbolReference.SymbolName, node);
                }
                return null; // Can't resolve yet
            }
            
            // Symbol found, create immediate instruction
            var inst = new InstructionNodeTypeJImmediate(node.op, node.rd, symbolAddr);
            inst.lineNumber = node.lineNumber;
            inst.byteAddress = node.byteAddress;
            return inst;
        }

        public AstNode ProcessASTNode(SymbolReferenceNode node) {
            // When we encounter a symbol reference that defines a symbol (like a label),
            // Assign its line number and address
            if (phase == GeneratorPass.LineNumber)
            {
                assignLineNumber(node);
                assignSymbolAddress(node.SymbolName, nextAddress);  // Use nextAddress for byte addresses
            }
            return null;
        }
    }

}
