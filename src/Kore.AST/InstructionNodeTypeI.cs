using Kore.RiscMeta;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kore.AST {
    /// <summary>
    /// Represents an I-Type RISC-V instruction.
    /// </summary>
    public class InstructionNodeTypeI : InstructionNode<Kore.RiscMeta.Instructions.TypeI> {
        /// <summary>
        /// The destination register for the instruction.
        /// </summary>
        public Register rd { get; set; }

        /// <summary>
        /// The source register for the instruction.
        /// </summary>
        public Register rs { get; set; }

        /// <summary>
        /// The immediate value for the instruction.
        /// </summary>
        public int immediate { get; set; }

        public InstructionNodeTypeI(Kore.RiscMeta.Instructions.TypeI op, Register rd, Register rs, int immediate)
            : base(op) {
            this.rd = rd;
            this.rs = rs;
            this.immediate = immediate;
        }

        public override AstNode CallProcessor(ASTProcessor processor) {
            return processor.ProcessASTNode(this);
        }

        public override bool Equals(object obj) {
            if (obj == null || GetType() != obj.GetType())
                return false;

            InstructionNodeTypeI other = (InstructionNodeTypeI)obj;
            return base.Equals(other) && rd == other.rd && rs == other.rs && immediate == other.immediate;
        }

        public override int GetHashCode() {
            unchecked {
                int hash = base.GetHashCode();
                hash = (hash * 397) ^ rd.GetHashCode();
                hash = (hash * 397) ^ rs.GetHashCode();
                hash = (hash * 397) ^ immediate.GetHashCode();
                return hash;
            }
        }

        public override StringBuilder getDebugText(int indentLevel, StringBuilder builder) {
            return addDebugTextHeader(indentLevel, builder).AppendLine($"TypeI {op} RD:{rd} RS:{rs} IMM:{immediate}");
        }
    }
}
