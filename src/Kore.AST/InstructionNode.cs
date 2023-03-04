using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kore.AST {
    /// <summary>
    /// Represents a RISC-V instruction node in the abstract syntax tree.
    /// This is an abstract class that provides the base structure for all RISC-V instruction types.
    /// It provides the opcode field which is a string containing the mnemonic for the instruction.
    /// Concrete classes derived from this class implement the details of each instruction.
    /// Depending on the instruction type, they may include fields such as the destination register, 
    /// source registers, immediate values, and the format of the instruction.
    /// </summary>
    public abstract class InstructionNode<T> : AstNode {
        /// <summary>
        /// The type of the instruction.
        /// </summary>
        public T op { get; set; }

        public InstructionNode(T op) {
            this.op = op;
        }
    }
}
