using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using SuperBAS.Parser;

namespace SuperBAS.Transpiler.CSharp
{
    public struct Loop
    {
        public float DefinedOnLine;
        public IASTNode Step;
        public IASTNode Start;
        public ASTBinary Condition;
        public ASTVariable Counter;
    }

    public class TemplateCode
    {
        private Action<VarType, string> DefineVar;
        public List<float> LineNumbers = new List<float>();
        public Dictionary<string, Loop> loops = new Dictionary<string, Loop>();

        public TemplateCode(Action<VarType, string> Define)
        {
            DefineVar = Define;
        }

        public string GetCodeForProgram(SyntaxTreeTopLevel[] lines)
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

        public string GetCodeForLine(SyntaxTreeTopLevel line, float nextLine)
        {
            LineNumbers.Add(line.LineNumber);
            string code = $"case {line.LineNumber}:\n";
            foreach (var command in line.Commands)
            {
                // TODO: Handle multi-statement ifs?
                code += GetCodeForCommand(command, line.LineNumber) + "\n";
            }
            code += $"goto case {nextLine};\n";
            return code;
        }

        private void Croak(string msg)
        {
            throw new Exception($"C# Transpiler templater croaked: {msg}");
        }

        private Loop GetLoop (IASTNode operand)
        {
            if (operand.Type != ASTNodeType.Variable)
                Croak("Must NEXT to a variable");

            var loopVar = (ASTVariable)operand;
            if (loopVar.IsString)
                Croak("Cannot use a string as a loop counter");

            var counter = loopVar.Name;
            if (!loops.Keys.Contains(counter))
                Croak($"That loop counter hasn't been defined (NEXT {counter})");

            return loops[counter];
        }

        private string GetCodeForNext (IASTNode operand)
        {
            var lp = GetLoop(operand);
            
            var increment = $"{GetVarName(lp.Counter)} += {GetCodeForExpression(lp.Step)};\n";
            var check = $"if ({GetCodeForExpression(lp.Condition, true)}) {{ skip{lp.Counter.Name}_bool = true; {GetCodeForGoto(lp.DefinedOnLine.ToString())} }}";
            return $"{increment}{check}";
        }

        private string GetCodeForGoto (string lnNumber)
        {
            return $"lineNumber = {lnNumber};\n goto GosubStart;";
        }

        private string GetCodeForTopof (IASTNode operand)
        {
            var lp = GetLoop(operand);
            return GetCodeForGoto(lp.DefinedOnLine.ToString());
        }
    
        private string GetCodeForCommand(IASTNode command, float lineNumber)
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
                        return GetCodeForGoto(GetCodeForExpression(cmd.Operand));
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
                    case "NEXT":
                        return GetCodeForNext(cmd.Operand);
                    case "TOPOF":
                        return GetCodeForTopof(cmd.Operand);
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
                    elseStr += $" else {{ {GetCodeForCommand(ifCmd.Else, lineNumber)} }}";
                }
                return $"if ({GetCodeForExpression(ifCmd.Condition, true)}) {{ {GetCodeForCommand(ifCmd.Then, lineNumber)} }} {elseStr}";
            }
            if (command.Type == ASTNodeType.For)
            {
                var forLoop = (ASTFor)command;
                var loopVar = (ASTVariable)forLoop.Assignment.Left;
                // It's definitely a number so doesn't need the _number postfix
                var counter = loopVar.Name;

                DefineVar(VarType.Number, counter);
                var skp = $"skip{counter}";
                DefineVar(VarType.Bool, skp);
                skp = $"{skp}_bool";

                var startCount = forLoop.Assignment.Right;
                var condition = new ASTBinary()
                {
                    Operator = "<=",
                    Left = loopVar,
                    Right = forLoop.ToMax
                };
                var loop = new Loop()
                {
                    Condition = condition,
                    DefinedOnLine = lineNumber,
                    Start = startCount,
                    Step = forLoop.Step,
                    Counter = loopVar
                };
                loops.Add(counter, loop);

                // This is a starting loop line,
                // if we transpile a NEXT call, it'll look up the loop and do the condition check
                return $"if ({skp}) {skp} = false; else {GetVarName(loopVar)} = {GetCodeForExpression(startCount)};";
            }

            Croak("Unimplemented control structure.");
            return "";
        }

        private string GetCodeForVarAssignment (IASTNode left)
        {
            // TODO
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

        public string GetCodeForNumber(string num)
        {
            string extra = "";
            // Add a decimal so that 10 / 3 is 3.333... not 3
            // (This makes number literals a 'double' type)
            if (!num.Contains(".")) extra = ".0";
            // extra = "m"; for accurate but 20x slower decimal type
            return $"{num}{extra}";
        }

        // Returns true if the call is to a standard library function
        private bool IsStdLib (ASTVariable funcName)
        {
            foreach (var std in LangUtils.StdLib)
            {
                if (
                    funcName.Name == std.Name &&
                    funcName.IsString == std.IsString)
                    return true;
            }
            return false;
        }

        private string GetCodeForStdLib (ASTCall call)
        {
            var args = call.Arguments.Expressions;
            var argCount = args.Length;
            var fName = call.FunctionName.Name;

            switch (fName)
            {
                case "STR":
                    if (argCount != 1)
                        Croak($"StdLib STR takes 1 argument, given {argCount}");
                    return $"({GetCodeForExpression(args[0])}).ToString()";
                case "VAL":
                    if (argCount != 1)
                        Croak($"StdLib VAL takes 1 argument, given {argCount}");
                    return $"double.Parse({GetCodeForExpression(args[0])})";
                case "LEN":
                    // TODO: Use count if the arg is a list
                    if (argCount != 1)
                        Croak($"StdLib LEN takes 1 argument, given {argCount}");
                    return $"({GetCodeForExpression(args[0])}).Length";
                default:
                    return $"The C# transpiler does not yet support StdLib function {fName}";
            }
        }

        private string GetCodeForCall (ASTCall call)
        {
            if (IsStdLib(call.FunctionName))
            {
                return GetCodeForStdLib(call);
            }
            else
            {
                Croak("User functions and arrays are not implemented.");
                return "";
            }
        }

        private string GetCodeForExpression (IASTNode expression, bool inIf = false)
        {
            switch (expression.Type)
            {
                case ASTNodeType.String:
                    return $"\"{((ASTString)expression).Value}\"";
                case ASTNodeType.Number:
                    var num = ((ASTNumber)expression).Value.ToString();
                    return GetCodeForNumber(num);
                case ASTNodeType.Variable:
                    var vr = (ASTVariable)expression;
                    return GetVarName(vr);
                case ASTNodeType.Binary:
                    var bin = ((ASTBinary)expression);
                    return $"({GetCodeForExpression(bin.Left)} {GetCodeForOperator(bin.Operator, inIf)} {GetCodeForExpression(bin.Right)})";
                case ASTNodeType.Call:
                    // These are complicated. They can mean array adressing,
                    // stdLib calls or user function calls
                    return GetCodeForCall((ASTCall)expression);
            }

            Croak("Unimplemented ASTNodeType in expression (likely an illegal expression type).");
            return "";
        }

        // The meaning of the = operator changes from = to == in an if
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
