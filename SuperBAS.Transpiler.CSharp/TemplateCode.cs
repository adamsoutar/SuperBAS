using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using SuperBAS.Parser;

namespace SuperBAS.Transpiler.CSharp
{
    public class TemplateCode
    {
        private Action<VarType, string> DefineVar;
        public List<float> LineNumbers = new List<float>();

        public TemplateCode (Action<VarType, string> Define)
        {
            DefineVar = Define;
        }

        public string GetCodeForProgram (SyntaxTreeTopLevel[] lines)
        {
            string final = "";
            for (int i = 0; i < lines.Length; i++)
            {
                float nextLine = -1;
                if (i != lines.Length - 1)
                    nextLine = lines[i + 1].LineNumber;
                final += GetCodeForLine(lines[i], nextLine);
            }
            return final;
        }

        public string GetCodeForLine (SyntaxTreeTopLevel line, float nextLine)
        {
            LineNumbers.Add(line.LineNumber);
            string code = $"case {line.LineNumber}:\n";
            foreach (var command in line.Commands)
            {
                // TODO: Handle multi-statement ifs
                code += GetCodeForCommand(command) + "\n";
            }
            code += $"goto case {nextLine};\n";
            return code;
        }

        private void Croak (string msg)
        {
            throw new Exception($"C# Transpiler croaked: {msg}");
        }

        private string GetCodeForCommand (IASTNode command)
        {
            /* This is either a command or a control structure */
            if (command.Type == ASTNodeType.Command)
            {
                var cmd = ((ASTCommand)command);
                switch (cmd.Command)
                {
                    case "PRINT":
                        return $"Console.WriteLine({GetCodeForExpression(cmd.Operand)});";
                    case "CLS":
                        return "Console.Clear();";
                    case "GOTO":
                        return $"lineNumber = {GetCodeForExpression(cmd.Operand)};\n goto GosubStart;";
                    case "GOSUB":
                        return $"Gosub({GetCodeForExpression(cmd.Operand)});";
                    case "RETURN":
                        return "return;";
                    case "SLEEP":
                        return $"Thread.Sleep({GetCodeForExpression(cmd.Operand)});";
                    case "LET":
                        return GetCodeForAssignment(cmd, true);
                    case "ASSIGN":
                        return GetCodeForAssignment(cmd, false);
                    default:
                        Croak($"As-yet unsupported command \"{cmd.Command}\"");
                        break;
                }
            }
            if (command.Type == ASTNodeType.If)
            {
                var ifCmd = (ASTIf)command;
                var elseStr = "";
                if (ifCmd.Else != null)
                {
                    elseStr += $" else {{ {GetCodeForCommand(ifCmd.Else)} }}";
                }
                return $"if ({GetCodeForExpression(ifCmd.Condition, true)}) {{ {GetCodeForCommand(ifCmd.Then)} }} {elseStr}";
            }
            if (command.Type == ASTNodeType.For)
            {
                var forLoop = (ASTFor)command;

            }

            Croak("Unimplemented control structure.");
            return "";
        }

        private string GetCodeForAssignment(ASTCommand cmd, bool define)
        {
            var oper = (ASTBinary)cmd.Operand;
            // TODO: Let arrays
            if (oper.Left.Type != ASTNodeType.Variable)
                Croak("Right now, you can only assign to variable names, no arrays yet.");
            var vr = ((ASTVariable)oper.Left);
            if (define)
                DefineVar(vr.IsString ? VarType.String : VarType.Number, vr.Name);
            return $"{GetVarName(vr)} = {GetCodeForExpression(oper.Right)};";
        }

        public string GetVarName(ASTVariable vr)
        {
            // So we can have vars of different types with the same name
            // myStr$ -> myStr_string
            // myFloat -> myFloat_float
            return $"{vr.Name}{(vr.IsString ? "_string" : "_number")}";
        }

        // The meaning of the = operator changes from = to == in an if
        private string GetCodeForExpression (IASTNode expression, bool inIf = false)
        {
            switch (expression.Type)
            {
                case ASTNodeType.String:
                    return $"\"{((ASTString)expression).Value}\"";
                case ASTNodeType.Number:
                    var num = ((ASTNumber)expression).Value.ToString();
                    string extra = "";
                    // Add a decimal so that 10 / 3 is 3.333... not 3
                    // (This makes number literals a 'double' type)
                    if (!num.Contains(".")) extra = ".0";
                    // extra = "m"; for accurate but 20x slower decimal type
                    return $"{num}{extra}";
                case ASTNodeType.Variable:
                    var vr = (ASTVariable)expression;
                    return GetVarName(vr);
                case ASTNodeType.Binary:
                    var bin = ((ASTBinary)expression);
                    return $"({GetCodeForExpression(bin.Left)} {GetCodeForOperator(bin.Operator, inIf)} {GetCodeForExpression(bin.Right)})";
            }

            Croak("Unimplemented ASTNodeType in expression (likely ASTCall).");
            return "";
        }

        private string GetCodeForOperator (string op, bool inIf)
        {
            if (inIf && op == "=") return "==";
            if (op == "MOD") return "%";
            if (op == "AND") return "&&";
            if (op == "OR") return "||";
            return op;
        }
    }
}
