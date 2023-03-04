using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kore.AST {
    /// <summary>
    /// Represents a miscellaneous instruction in the RISC-V assembly language.
    /// </summary>
    public class InstructionNodeTypeMisc : InstructionNode<string> {
        public InstructionNodeTypeMisc(string name) : base(name) { }

        public override void CallProcessor(ASTProcessor processor) {
            processor.ProcessASTNode(this);
        }
    }
}
