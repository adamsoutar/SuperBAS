﻿using System;
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
        // For complex, operand dependent declarations like maps
        private Action<string> DefineRaw;
        public List<float> LineNumbers = new List<float>();
        public Dictionary<string, Loop> loops = new Dictionary<string, Loop>();
        public List<string> DefinedLists = new List<string>();

        public TemplateCode(Action<VarType, string> Define, Action<string> Raw)
        {
            DefineVar = Define;
            DefineRaw = Raw;
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
            string linesCode = "";
            foreach (var command in line.Commands)
            {
                // TODO: Handle multi-statement ifs?
                linesCode += GetCodeForCommand(command, line.LineNumber) + "\n";
            }
            // Some lines might not generate code
            if (linesCode == "") return linesCode;
            code += $"{linesCode}goto case {nextLine};\n";
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

        private string GetCodeForDim (IASTNode operand)
        {
            if (operand.Type != ASTNodeType.Call)
                Croak("Dim must be followed by a call, eg. DIM words(3)");

            var cl = (ASTCall)operand;
            if (IsStdLib(cl.FunctionName))
                Croak("Attempt to DIM a name from the standard library");

            var varName = GetVarName(cl.FunctionName);
            var args = cl.Arguments.Expressions;
            var dimensions = args.Length;
            var commas = "";
            for (int i = 0; i < dimensions - 1; i++) commas += ",";

            var arType = cl.FunctionName.IsString ? "string" : "double";
            var arCode = $"{arType}[{commas}]";

            DefineRaw($"private static {arCode} {varName};");

            var arDef = $"{arType}[";
            for (int i = 0; i < args.Length; i++)
                // Cast the dimensions
                // The cast is silent for something like DIM X(5.5, 3.2) - TODO Warn
                arDef += $"(int)({GetCodeForExpression(args[i])})" + (i == args.Length - 1 ? "" : ",");
            arDef += ']';

            return $"{varName} = new {arDef};";
        }

        private string GetCodeForList (IASTNode operand)
        {
            if (operand.Type != ASTNodeType.Variable)
                Croak("You should pass LIST a variable name to define");

            var vr = (ASTVariable)operand;
            var nm = GetVarName(vr);
            DefinedLists.Add(nm);
            var listType = vr.IsString ? "string" : "double";
            DefineRaw($"private static List<{listType}> {nm};");

            return $"{nm} = new List<{listType}>();";
        }

        private string GetCodeForListAdd (IASTNode operand)
        {
            if (operand.Type != ASTNodeType.CompoundExpression)
                Croak("LISTADD is a tupple command (see spec)");

            var args = ((ASTCompoundExpression)operand).Expressions;
            if (args[0].Type != ASTNodeType.Variable)
                Croak("LISTADD's first argument should be a list variable");

            var nm = GetVarName((ASTVariable)args[0]);
            if (!DefinedLists.Contains(nm))
                Croak($"Attempt to add to undefined list {nm}");

            return $"{nm}.Add({GetCodeForExpression(args[1])});";
        }

        private string GetCodeForListRm (IASTNode operand)
        {
            if (operand.Type != ASTNodeType.CompoundExpression)
                Croak("LISTRM is a tupple command (see spec)");

            var args = ((ASTCompoundExpression)operand).Expressions;
            // TODO: This is exteremely similar to ListAdd, I WANT US TO MERGE
            if (args[0].Type != ASTNodeType.Variable)
                Croak("LISTRM's first argument should be a list variable");

            var nm = GetVarName((ASTVariable)args[0]);
            if (!DefinedLists.Contains(nm))
                Croak($"Attempt to remove from undefined list {nm}");

            return $"{nm}.RemoveAt((int)({GetCodeForExpression(args[1])}));";
        }

        private string GetCodeForPrintAt (IASTNode op)
        {
            if (op.Type != ASTNodeType.CompoundExpression)
                Croak("PRINTAT is a tupple command (see spec)");

            var args = ((ASTCompoundExpression)op).Expressions;
            // PrintAt depends on runtime variables
            // And so is implemented in Templace.cs.txt
            return $"PrintAt({GetCodeForExpression(args[0])}, {GetCodeForExpression(args[1])}, {GetCodeForExpression(args[2])});";
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
                    case "DIM":
                        return GetCodeForDim(cmd.Operand);
                    case "LISTADD":
                        return GetCodeForListAdd(cmd.Operand);
                    case "LISTRM":
                        return GetCodeForListRm(cmd.Operand);
                    case "LIST":
                        return GetCodeForList(cmd.Operand);
                    case "PRINTAT":
                        return GetCodeForPrintAt(cmd.Operand);
                    case "INK":
                        return $"Console.ForegroundColor = (ConsoleColor)({GetCodeForExpression(cmd.Operand)});";
                    case "PAPER":
                        return $"Console.BackgroundColor = (ConsoleColor)({GetCodeForExpression(cmd.Operand)});";
                    case "EXIT":
                        // TODO: Give the option to supply an exit code?
                        return "Environment.Exit(0);";
                    case "STOP":
                        return "Console.ReadKey(); Environment.Exit(0);";
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

        // Allows assigning to, say, myArray[3, 4]
        // as well as x
        private string GetCodeForVarAssignment (IASTNode left)
        {
            if (left.Type == ASTNodeType.Variable)
            {
                var vr = (ASTVariable)left;
                return GetVarName(vr);
            }
            else
            {
                if (left.Type != ASTNodeType.Call)
                    Croak("What? You can't assign to that.\nWhat did you even do? 3 = 4??");
                var cl = (ASTCall)left;
                if (IsStdLib(cl.FunctionName))
                    Croak("Can't assign to the standard library.");
                //TODO: Check we aren't assigning to a user function
                return GetCodeForCall(cl);
            }
        }

        private string GetCodeForAssignment(ASTCommand cmd, bool define)
        {
            var oper = (ASTBinary)cmd.Operand;
            if (define)
            {
                // LET
                if (oper.Left.Type != ASTNodeType.Variable)
                    Croak("You can only LET variables. Note: You don't need LET for array items.");
                var vr = (ASTVariable)oper.Left;
                DefineVar(vr.IsString ? VarType.String : VarType.Number, vr.Name);
            }
            return $"{GetCodeForVarAssignment(oper.Left)} = {GetCodeForExpression(oper.Right)};";
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

            var cd = GetCodeForExpression(args[0]);
            switch (fName)
            {
                case "STR":
                    return $"({cd}).ToString()";
                case "VAL":
                    return $"double.Parse({cd})";
                case "SIN":
                    return $"Math.Sin({cd})";
                case "COS":
                    return $"Math.Cos({cd})";
                case "TAN":
                    return $"Math.Tan({cd})";
                case "FLOOR":
                    return $"Math.Floor({cd})";
                case "CEIL":
                    return $"Math.Ceiling({cd})";
                case "LEN":
                    var end = "Length";

                    // Switch to Count for lists, they don't have a Length
                    if (args[0].Type == ASTNodeType.Variable)
                    {
                        var vr = (ASTVariable)args[0];
                        if (DefinedLists.Contains(GetVarName(vr)))
                            end = "Count";
                    }

                    return $"({cd}).{end}";
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
                // TODO: User functions
                var arName = GetVarName(call.FunctionName);
                var assignCode = $"{arName}[";

                var args = call.Arguments.Expressions;
                for (int i = 0; i < args.Length; i++)
                {
                    assignCode += $"(int)({GetCodeForExpression(args[i])})" +
                        (i == args.Length - 1 ? "" : ",");
                }

                return assignCode + ']';
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
