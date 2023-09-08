using Kore.RiscMeta;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kore.AST {
    /// <summary>
    /// Represents a U-Type RISC-V instruction.
    /// </summary>
    public class InstructionNodeTypeU : InstructionNode<Kore.RiscMeta.Instructions.TypeU> {
        /// <summary>
        /// The destination register for the instruction.
        /// </summary>
        public Register rd { get; set; }

        /// <summary>
        /// The immediate value for the instruction.
        /// </summary>
        public int imm { get; set; }

        public InstructionNodeTypeU(Kore.RiscMeta.Instructions.TypeU op, Register rd, int immediate): base(op) {
            this.rd = rd;
            this.imm = immediate;
        }

        public override AstNode CallProcessor(ASTProcessor processor) {
            return processor.ProcessASTNode(this);
        }

        public override bool Equals(object obj) {
            if (obj == null || GetType() != obj.GetType())
                return false;

            InstructionNodeTypeU other = (InstructionNodeTypeU)obj;
            return base.Equals(other) && rd == other.rd && imm == other.imm;
        }

        public override int GetHashCode() {
            unchecked {
                int hash = base.GetHashCode();
                hash = (hash * 397) ^ rd.GetHashCode();
                hash = (hash * 397) ^ imm.GetHashCode();
                return hash;
            }
        }

        public override StringBuilder getDebugText(int indentLevel, StringBuilder builder) {
            return addDebugTextHeader(indentLevel, builder).AppendLine($"TypeB {op} RD:{rd} IMM:{imm}");
        }
    }
}
