namespace Kore.AST {
    public interface ASTProcessor {
        AstNode ProcessASTNode(ProgramNode node);
        AstNode ProcessASTNode(SectionNode node);
        AstNode ProcessASTNode(DirectiveNode node);
        AstNode ProcessASTNode<T>(InstructionNode<T> node);
        AstNode ProcessASTNode(InstructionNodeTypeR node);
        AstNode ProcessASTNode(InstructionNodeTypeI node);
        AstNode ProcessASTNode(InstructionNodeTypeU node);
        AstNode ProcessASTNode(InstructionNodeTypeBImmediate node);
        AstNode ProcessASTNode(InstructionNodeTypeBLabel node);
        AstNode ProcessASTNode(InstructionNodeTypeJImmediate node);
        AstNode ProcessASTNode(InstructionNodeTypeJLabel node);
        AstNode ProcessASTNode(InstructionNodeTypeMisc node);
        AstNode ProcessASTNode(LabelNode node);
        AstNode ProcessASTNode(CommentNode node);
    }
}
