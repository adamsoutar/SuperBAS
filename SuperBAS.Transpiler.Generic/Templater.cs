using System;
using System.Collections.Generic;
using SuperBAS.Parser;

namespace SuperBAS.Transpiler.Generic
{
    public class Templater
    {
        public TargetLanguage Target;
        public float LowestLine = float.PositiveInfinity;
        private SyntaxTreeTopLevel[] lines;

        public Templater(TargetLanguage target, Parser.Parser parser)
        {
            Target = target;
            lines = parser.GenerateAbstractSyntaxTree();
        }
    
        public void Croak (string message)
        {
            throw new Exception($@"
Transpiler croaked!
Target: {Target.Config["meta"]["name"]}
    {message}");
        }

        public string GetFullProgramCode ()
        {
            return Target.CodeGen("", GetCodeForBody(), LowestLine.ToString()) ;
        }

        public string GetCodeForBody ()
        {
            string final = "";
            for (int i = 0; i < lines.Length; i++)
            {
                float nextLine = -1;
                if (i != lines.Length - 1)
                {
                    nextLine = lines[i + 1].LineNumber;
                }
                if (lines[i].LineNumber < LowestLine)
                {
                    LowestLine = lines[i].LineNumber;
                }
                final += GetCodeForLine(lines[i], nextLine);
            }
            return final;
        }

        public string GetCodeForLine (SyntaxTreeTopLevel line, float nextLine)
        {
            string code = Target.GetSnippet("cases", "cases", "lineNumber", line.LineNumber.ToString());
            foreach (var command in line.Commands)
            {
                code += GetCodeForCommand(command, line.LineNumber) + '\n';
            }
            code += Target.GetSnippet("cases", "endCase", "nextLine", nextLine.ToString());
            return code;
        }

        public string GetCodeForCommand(IASTNode command, float lineNumber)
        {
            if (command.Type == ASTNodeType.Command)
            {
                var cmd = (ASTCommand)command;
                switch (cmd.Command)
                {
                    case "PRINT":
                        return Target.GetSnippet("commands", "print", "text", GetCodeForExpression(cmd.Operand));
                }
            }

            Croak("Unsupported AST Node in new transpiler.");
            return "";
        }

        public string GetCodeForExpression (IASTNode expression)
        {
            if (expression.Type == ASTNodeType.String)
            {
                var str = (ASTString)expression;
                return Target.GetSnippet("expressions", "string", "value", str.Value);
            }

            if (expression.Type == ASTNodeType.Number)
            {
                var num = (ASTNumber)expression;
                return Target.GetSnippet("expressions", "number", "value", num.Value.ToString());
            }

            Croak("Unsupported expression type in AST.");
            return "";
        }
    }
}
