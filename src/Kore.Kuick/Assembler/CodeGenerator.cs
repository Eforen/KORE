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
        private int assignLineNumber(LabelNode label) {
            if(labels.ContainsKey(label.Name)) throw ThrowAssemblerPanic($"Label `{label.Name}` Assigned Twice");
            label.lineNumber = assignLineNumber(0);
            labels.Add(label.Name, label);
            return label.lineNumber;
        }
        private int getLineNumber(string label) {
            if(labels.ContainsKey(label)) return labels[label].lineNumber;
            labelCacheMiss = true;
            return -1;
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
        public int nextLineNumber = 0;

        public byte[] Generate(ProgramNode node) {
            machineCode.Clear();
            labels.Clear();
            labelCacheMiss = false;
            nextLineNumber = 0;

            // TODO: Implement code generation based on the given AST node.
            phase = GeneratorPass.PsudoCode;
            node.CallProcessor(this);
            phase = GeneratorPass.LineNumber;
            node.CallProcessor(this);
            if(labelCacheMiss) {
                phase = GeneratorPass.LineNumberCleanup;
                node.CallProcessor(this);
            }
            phase = GeneratorPass.GenerateCode;
            node.CallProcessor(this);

            return machineCode.ToArray();
        }

        public AstNode ProcessASTNode(ProgramNode node) {
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
            return null;
        }

        public AstNode ProcessASTNode(DirectiveNode node) {
            if(phase != GeneratorPass.PsudoCode) throw ThrowAssemblerPanic("Directive Found in AST after first pass");
            
            //TODO: Decide if the following directives should be handled here or later
            // Handle .byte
            // Handle .word
            // Handle .string
            throw new NotImplementedException();
        }

        public AstNode ProcessASTNode<T>(InstructionNode<T> node) {
            throw new NotImplementedException();
        }

        public AstNode ProcessASTNode(InstructionNodeTypeR node) {
            switch(phase) {
                case GeneratorPass.PsudoCode:
                    return null;
                case GeneratorPass.LineNumber:
                    node.lineNumber = assignLineNumber();
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
                    node.lineNumber = assignLineNumber();
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
                    node.lineNumber = assignLineNumber();
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
                    node.lineNumber = assignLineNumber();
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
                    node.lineNumber = assignLineNumber();
                    // Fall Through
                    break;
                case GeneratorPass.LineNumberCleanup:
                    // Fall Through
                    break;
                case GeneratorPass.GenerateCode:
                    return null;
                default:
                    return null;
            }

            // Get Line number of label if its been assigned
            int labelAddr = getLineNumber(node.label);
            // Drop out if < 0 because this means its a cache miss
            if(labelAddr < 0) return null;
            // Must have label make new instruction
            var inst = new InstructionNodeTypeBImmediate(node.op, node.rs1, node.rs2, labelAddr);
            inst.lineNumber = node.lineNumber;
            return inst;
        }

        public AstNode ProcessASTNode(InstructionNodeTypeJImmediate node) {
            throw new NotImplementedException();
        }

        public AstNode ProcessASTNode(InstructionNodeTypeJLabel node) {
            switch(phase) {
                case GeneratorPass.PsudoCode:
                    return null;
                case GeneratorPass.LineNumber:
                    // Get this instructions Line number
                    node.lineNumber = assignLineNumber();
                    // Fall Through
                    break;
                case GeneratorPass.LineNumberCleanup:
                    // Fall Through
                    break;
                case GeneratorPass.GenerateCode:
                    return null;
                default:
                    return null;
            }

            // Get Line number of label if its been assigned
            int labelAddr = getLineNumber(node.label);
            // Drop out if < 0 because this means its a cache miss
            if(labelAddr < 0) return null;
            // Must have label make new instruction
            var inst = new InstructionNodeTypeJImmediate(node.op, node.rd, labelAddr);
            inst.lineNumber = node.lineNumber;
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
            if(node.lineNumber < 0) throw ThrowAssemblerPanic("Error in Label Linenumber Assignment");
            return null;
        }

        public AstNode ProcessASTNode(CommentNode node) {
            return null;
        }
    }

}
