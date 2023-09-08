using Kore.RiscMeta;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kore.AST {
    /// <summary>
    /// Represents an S-type instruction in the RISC-V assembly language.
    /// </summary>
    public class InstructionNodeTypeS : InstructionNode<Kore.RiscMeta.Instructions.TypeS> {
        /// <summary>
        /// The destination register for the result of the instruction.
        /// </summary>
        public Register rs1 { get; set; }

        /// <summary>
        /// The source register for the instruction.
        /// </summary>
        public Register rs2 { get; set; }

        /// <summary>
        /// The offset used in the instruction.
        /// </summary>
        public int imm { get; set; }

        public InstructionNodeTypeS(Kore.RiscMeta.Instructions.TypeS op, Register rs1, Register rs2, int immediate) : base(op) {
            this.rs1 = rs1;
            this.rs2 = rs2;
            this.imm = immediate;
        }

        public override AstNode CallProcessor(ASTProcessor processor) {
            return processor.ProcessASTNode(this);
        }

        public override bool Equals(object obj) {
            if (obj == null || GetType() != obj.GetType())
                return false;

            InstructionNodeTypeS other = (InstructionNodeTypeS)obj;
            return base.Equals(other) && rs1 == other.rs1 && rs2 == other.rs2 && imm == other.imm;
        }

        public override int GetHashCode() {
            unchecked {
                int hash = base.GetHashCode();
                hash = (hash * 397) ^ rs1.GetHashCode();
                hash = (hash * 397) ^ rs2.GetHashCode();
                hash = (hash * 397) ^ imm.GetHashCode();
                return hash;
            }
        }

        public override StringBuilder getDebugText(int indentLevel, StringBuilder builder) {
            return addDebugTextHeader(indentLevel, builder).AppendLine($"TypeS {op} RS1:{rs1} RS2:{rs2} IMM:{imm}");
        }
    }
}
