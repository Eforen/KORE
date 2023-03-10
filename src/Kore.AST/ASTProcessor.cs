namespace Kore.AST {
    public interface ASTProcessor {
        void ProcessASTNode(ProgramNode node);
        void ProcessASTNode(SectionNode node);
        void ProcessASTNode(DirectiveNode node);
        void ProcessASTNode<T>(InstructionNode<T> node);
        void ProcessASTNode(InstructionNodeTypeR node);
        void ProcessASTNode(InstructionNodeTypeI node);
        void ProcessASTNode(InstructionNodeTypeU node);
        void ProcessASTNode(InstructionNodeTypeBImmidiate node);
        void ProcessASTNode(InstructionNodeTypeBLabel node);
        void ProcessASTNode(InstructionNodeTypeJImmidiate node);
        void ProcessASTNode(InstructionNodeTypeJLabel node);
        void ProcessASTNode(InstructionNodeTypeMisc node);
        void ProcessASTNode(LabelNode node);
        void ProcessASTNode(CommentNode node);
    }
}
