using System;
using SuperBAS.Parser;

namespace SuperBAS.Transpiler.Javascript
{
    public class TemplateCode
    {
        private Action<string> DefineVar;
        public float LowestLine = float.MaxValue;

        public TemplateCode (Action<string> defVar)
        {
            // Basically puts a string at the top of the program
            DefineVar = defVar;
        }

        public string GetCodeForProgram (SyntaxTreeTopLevel[] AST)
        {
            string final = "";
            for (int i = 0; i < AST.Length; i++)
            {
                final += GetCodeForLine(AST[i]);
            }
            return final;
        }

        public string GetCodeForLine(SyntaxTreeTopLevel line)
        {
            if (line.LineNumber < LowestLine)
                LowestLine = line.LineNumber;
            string code = $"case {line.LineNumber}:\n";
            foreach (var command in line.Commands)
            {
                // TODO: Does not support multi-line if. This cool?
                code += GetCodeForCommand(command, line.LineNumber) + '\n';
            }
            return code;
        }

        public string GetCodeForCommand (IASTNode command, float LineNumber)
        {
            // As opposed to, say, a loop.
            if (command.Type == ASTNodeType.Command)
            {
                var cmd = (ASTCommand)command;
                switch (cmd.Command)
                {
                    case "PRINT":
                        return $"console.log({GetCodeForExpression(cmd.Operand)})";
                    case "CLS":
                        return "console.clear()";
                    case "GOTO":
                        return $"ln = {GetCodeForExpression(cmd.Operand)}\nbreak";
                    case "GOSUB":
                        return $"await goSub({GetCodeForExpression(cmd.Operand)})";
                    case "RETURN":
                        return "return";
                    case "SLEEP":
                        return $"await sleep({GetCodeForExpression(cmd.Operand)})";
                    case "LET":
                        return GetCodeForAssignment(cmd, true);
                    case "ASSIGN":
                        return GetCodeForAssignment(cmd, false);
                    case "INPUT":
                        return GetCodeForInput(cmd.Operand);
                    case "EXIT":
                    case "STOP":
                        return "stop = true\nbreak";
                    default:
                        Croak($"Unimplemented command {cmd.Command}");
                        return "";
                }
            }

            if (command.Type == ASTNodeType.If)
            {
                /*
    TODO: Fix this when we get the JS transpiler up to date

                var ifCmd = (ASTIf)command;
                var elseStr = "";
                if (ifCmd.Else != null)
                {
                    elseStr += $" else {{ {GetCodeForCommand(ifCmd.Else, LineNumber)} }}";
                }
                return $"if ({GetCodeForExpression(ifCmd.Condition, true)}) {{ {GetCodeForCommand(ifCmd.Then, LineNumber)} }} {elseStr}";
                */
            }

            Croak("Unimplemented control structure");
            return "";
        }

        public string GetCodeForInput(IASTNode operand)
        {
            ASTVariable target;
            if (operand.Type == ASTNodeType.CompoundExpression)
            {
                var args = ((ASTCompoundExpression)operand).Expressions;
                target = (ASTVariable)args[1];
                return $"{GetVarName(target)} = prompt({GetCodeForExpression(args[0])})";
            }
            target = (ASTVariable)operand;
            return $"{GetVarName(target)} = prompt()";
        }

        public string GetCodeForAssignment (ASTCommand cmd, bool definition)
        {
            var oper = (ASTBinary)cmd.Operand;
            if (definition)
            {
                if (oper.Left.Type != ASTNodeType.Variable)
                    Croak("You can only LET variables. Note: You don't need LET for array items.");
                var vr = (ASTVariable)oper.Left;
                DefineVar($"let {GetVarName(vr)}");
            }
            return $"{GetCodeForExpression(oper.Left)} = {GetCodeForExpression(oper.Right)}";
        }

        public void Croak (string msg)
        {
            throw new Exception($"Javascript transpiler croaked:\n{msg}");
        }

        public string GetVarName (ASTVariable vr)
        {
            // See comment in the C# transpiler
            return $"{vr.Name}{(vr.IsString ? "_string" : "_number")}";
        }

        public string GetCodeForOperator (string op, bool inIf)
        {
            // TODO: Check if we should be using == for BASIC behaviour
            if (inIf && op == "=") return "===";
            if (op == "MOD") return "%";
            if (op == "AND") return "&&";
            if (op == "OR") return "||";
            return op;
        }

        public string GetCodeForExpression (IASTNode expression, bool inIf = false)
        {
            switch (expression.Type)
            {
                case ASTNodeType.String:
                    return $"'{((ASTString)expression).Value}'";
                case ASTNodeType.Number:
                    return ((ASTNumber)expression).Value.ToString();
                case ASTNodeType.Variable:
                    return GetVarName((ASTVariable)expression);
                case ASTNodeType.Binary:
                    var bin = (ASTBinary)expression;
                    return $"{GetCodeForExpression(bin.Left)} {GetCodeForOperator(bin.Operator, inIf)} {GetCodeForExpression(bin.Right)}";
                // TODO: Calls
            }

            Croak("Unimplemented expression type, likely call.");
            return "";
        }
    }
}
