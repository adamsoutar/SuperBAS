using System;
namespace SuperBAS.Parser
{
    public enum ASTNodeType
    {
        CompoundExpression,
        Number,
        String,
        Variable,
        Call,
        If,
        Binary,
        Keyword,
        Command,
        InvalidNode,
        For
    }

    // Interface for a generic Abstract Syntax Tree node
    public interface IASTNode
    {
        ASTNodeType Type { get; }
    }
}
