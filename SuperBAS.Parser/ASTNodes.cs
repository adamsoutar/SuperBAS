using System;
namespace SuperBAS.Parser
{
    public class ASTCompoundExpression : IASTNode
    {
        public ASTNodeType Type { get => ASTNodeType.CompoundExpression; }
        public IASTNode[] Expressions;
    }

    public class ASTNumber : IASTNode
    {
        public ASTNodeType Type { get => ASTNodeType.Number; }
        public float Value;
    }

    public class ASTString : IASTNode
    {
        public ASTNodeType Type { get => ASTNodeType.String; }
        public string Value;
    }

    public class ASTVariable : IASTNode
    {
        public ASTNodeType Type { get => ASTNodeType.Variable; }
        public string Name;
        public bool IsString;
    }

    public class ASTCall : IASTNode
    {
        public ASTNodeType Type { get => ASTNodeType.Call; }
        public ASTVariable FunctionName;
        public ASTCompoundExpression Arguments;
    }

    public class ASTIf : IASTNode
    {
        public ASTNodeType Type { get => ASTNodeType.If; }
        public IASTNode Condition;
        public ASTCommand Then;
        public ASTCommand Else;
    }

    public class ASTKeyword : IASTNode
    {
        public ASTNodeType Type { get => ASTNodeType.Keyword; }
        public string Keyword;
    }

    public class ASTBinary : IASTNode
    {
        public ASTNodeType Type { get => ASTNodeType.Binary; }
        public string Operator;
        public IASTNode Left;
        public IASTNode Right;
    }

    public class ASTInvalidNode : IASTNode
    {
        public ASTNodeType Type { get => ASTNodeType.InvalidNode; }
    }

    public class ASTCommand : IASTNode
    {
        public ASTNodeType Type { get => ASTNodeType.Command; }
        public string Command;
        public IASTNode Operand;
    }

    public class ASTFor : IASTNode
    {
        public ASTNodeType Type { get => ASTNodeType.For; }
        public ASTBinary Assignment;
        public IASTNode ToMax;
        public IASTNode Step;
    }
}
