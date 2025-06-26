using Kore.RiscMeta;
using System.Text;

namespace Kore.AST {
    /// <summary>
    /// Represents a B-Type RISC-V instruction that references a symbol.
    /// This is the modern replacement for InstructionNodeTypeBLabel that uses the symbol table system.
    /// </summary>
    public class InstructionNodeTypeBSymbol : InstructionNode<Kore.RiscMeta.Instructions.TypeB> {
        /// <summary>
        /// The first source register for the instruction.
        /// </summary>
        public Register rs1 { get; set; }

        /// <summary>
        /// The second source register for the instruction.
        /// </summary>
        public Register rs2 { get; set; }

        /// <summary>
        /// The symbol reference for the branch target.
        /// </summary>
        public SymbolReferenceNode SymbolReference { get; set; }

        public InstructionNodeTypeBSymbol(Kore.RiscMeta.Instructions.TypeB op, Register rs1, Register rs2, SymbolReferenceNode symbolReference)
            : base(op) {
            this.rs1 = rs1;
            this.rs2 = rs2;
            this.SymbolReference = symbolReference;
        }

        public override AstNode CallProcessor(ASTProcessor processor) {
            return processor.ProcessASTNode(this);
        }

        public override bool Equals(object obj) {
            if (obj == null || GetType() != obj.GetType())
                return false;

            InstructionNodeTypeBSymbol other = (InstructionNodeTypeBSymbol)obj;
            return base.Equals(other) && rs1 == other.rs1 && rs2 == other.rs2 && 
                   SymbolReference?.SymbolId == other.SymbolReference?.SymbolId;
        }

        public override int GetHashCode() {
            unchecked {
                int hash = base.GetHashCode();
                hash = (hash * 397) ^ rs1.GetHashCode();
                hash = (hash * 397) ^ rs2.GetHashCode();
                hash = (hash * 397) ^ (SymbolReference?.SymbolId.GetHashCode() ?? 0);
                return hash;
            }
        }

        public override StringBuilder getDebugText(int indentLevel, StringBuilder builder) {
            addDebugTextHeader(indentLevel, builder).AppendLine($"TypeB {op} RS1:{rs1} RS2:{rs2} SYMBOL:{{");
            if (SymbolReference != null) {
                SymbolReference.getDebugText(indentLevel + 1, builder);
            }
            return addDebugTextHeader(indentLevel, builder).AppendLine("}");
        }
    }

    /// <summary>
    /// Represents a J-Type RISC-V instruction that references a symbol.
    /// This is the modern replacement for InstructionNodeTypeJLabel that uses the symbol table system.
    /// </summary>
    public class InstructionNodeTypeJSymbol : InstructionNode<Kore.RiscMeta.Instructions.TypeJ> {
        /// <summary>
        /// The destination register for the result of the instruction.
        /// </summary>
        public Register rd { get; set; }

        /// <summary>
        /// The symbol reference for the jump target.
        /// </summary>
        public SymbolReferenceNode SymbolReference { get; set; }

        public InstructionNodeTypeJSymbol(Kore.RiscMeta.Instructions.TypeJ op, Register rd, SymbolReferenceNode symbolReference)
            : base(op) {
            this.rd = rd;
            this.SymbolReference = symbolReference;
        }

        public override AstNode CallProcessor(ASTProcessor processor) {
            return processor.ProcessASTNode(this);
        }

        public override bool Equals(object obj) {
            if (obj == null || GetType() != obj.GetType())
                return false;

            InstructionNodeTypeJSymbol other = (InstructionNodeTypeJSymbol)obj;
            return base.Equals(other) && rd == other.rd && 
                   SymbolReference?.SymbolId == other.SymbolReference?.SymbolId;
        }

        public override int GetHashCode() {
            unchecked {
                int hash = base.GetHashCode();
                hash = (hash * 397) ^ rd.GetHashCode();
                hash = (hash * 397) ^ (SymbolReference?.SymbolId.GetHashCode() ?? 0);
                return hash;
            }
        }

        public override StringBuilder getDebugText(int indentLevel, StringBuilder builder) {
            addDebugTextHeader(indentLevel, builder).AppendLine($"TypeJ {op} RD:{rd} SYMBOL:{{");
            if (SymbolReference != null) {
                SymbolReference.getDebugText(indentLevel + 1, builder);
            }
            return addDebugTextHeader(indentLevel, builder).AppendLine("}");
        }
    }
} 