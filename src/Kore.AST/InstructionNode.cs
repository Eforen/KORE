using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kore.AST {
    
    /// <summary> Intermediate type used only for type targeting of instructions </summary>
    public abstract class InstructionNode : AstNode {
        public override bool Equals(object obj) {
            if (obj == null || GetType() != obj.GetType())
                return false;

            return base.Equals(obj);
        }

        public override int GetHashCode() {
            return base.GetHashCode();
        }
    }

    /// <summary>
    /// Represents a RISC-V instruction node in the abstract syntax tree.
    /// This is an abstract class that provides the base structure for all RISC-V instruction types.
    /// It provides the opcode field which is a string containing the mnemonic for the instruction.
    /// Concrete classes derived from this class implement the details of each instruction.
    /// Depending on the instruction type, they may include fields such as the destination register, 
    /// source registers, immediate values, and the format of the instruction.
    /// </summary>
    // where T is Kore.RiscMeta.Instructions.TypeB or Kore.RiscMeta.Instructions.TypeI or Kore.RiscMeta.Instructions.TypeR or Kore.RiscMeta.Instructions.TypeS or Kore.RiscMeta.Instructions.TypeU
    public abstract class InstructionNode<T> : InstructionNode {
        /// <summary>
        /// The type of the instruction.
        /// </summary>
        public T op { get; set; }

        public InstructionNode(T op) {
            this.op = op;
        }

        public override bool Equals(object obj) {
            if (obj == null || GetType() != obj.GetType())
                return false;

            InstructionNode<T> other = (InstructionNode<T>)obj;
            return op.Equals(other.op);
        }

        public override int GetHashCode() {
            unchecked {
                int hash = base.GetHashCode();
                hash = (hash * 397) ^ op.GetHashCode();
                return hash;
            }
        }
    }
}
