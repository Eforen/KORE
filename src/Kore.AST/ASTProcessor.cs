namespace Kore.AST {
    public interface ASTProcessor {
        void ProcessASTNode(ProgramNode programNode);
        void ProcessASTNode(SectionNode sectionNode);
        void ProcessASTNode(DirectiveNode directiveNode);
        void ProcessASTNode<T>(InstructionNode<T> instructionNode);
        void ProcessASTNode(InstructionNodeTypeR instructionNodeTypeR);
        void ProcessASTNode(InstructionNodeTypeI instructionNodeTypeI);
        void ProcessASTNode(InstructionNodeTypeU instructionNodeTypeU);
        void ProcessASTNode(InstructionNodeTypeB instructionNodeTypeB);
        void ProcessASTNode(InstructionNodeTypeJ instructionNodeTypeJ);
        void ProcessASTNode(InstructionNodeTypeMisc miscInstructionNode);
        void ProcessASTNode(LabelNode labelNode);
        void ProcessASTNode(CommentNode commentNode);
    }
}
