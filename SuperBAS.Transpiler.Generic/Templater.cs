﻿using System;
using System.Collections.Generic;
using System.Collections;
using SuperBAS.Parser;
using System.Linq;

namespace SuperBAS.Transpiler.Generic
{
    public struct Loop
    {
        public float DefinedOnLine;
        public string SkipVar;
        public IASTNode Step;
        public IASTNode Start;
        public ASTBinary Condition;
        public ASTVariable Counter;
    }

    public class Templater
    {
        public TargetLanguage Target;
        public float LowestLine = float.PositiveInfinity;

        private Dictionary<string, Loop> loops = new Dictionary<string, Loop>();
        private SyntaxTreeTopLevel[] lines;
        private string declarations = "";
        private List<string> seenVars = new List<string>();

        public Templater(TargetLanguage target, Parser.Parser parser)
        {
            Target = target;
            lines = parser.GenerateAbstractSyntaxTree();
        }
    
        public string Croak (string message)
        {
            throw new Exception($@"
Transpiler croaked!
Target: {Target.Config["meta"]["name"]}
    {message}");
        }

        public string GetFullProgramCode ()
        {
            // This is executed fist 'cause it generates declarations
            var body = GetCodeForBody();
            return Target.CodeGen(declarations, body, LowestLine.ToString()) ;
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
                    case "CLS":
                        return Target.GetSnippet("commands", "cls");
                    case "GOTO":
                        return Target.GetSnippet("commands", "goto", "lineNumber", GetCodeForExpression(cmd.Operand));
                    case "GOSUB":
                        return Target.GetSnippet("commands", "gosub", "lineNumber", GetCodeForExpression(cmd.Operand));
                    case "RETURN":
                        return Target.GetSnippet("commands", "return");
                    case "SLEEP":
                        return Target.GetSnippet("commands", "sleep", "time", GetCodeForExpression(cmd.Operand));
                    case "LET":
                        // Operand of LET is a binary expression with =
                        return GetCodeForExpression(cmd.Operand);
                    case "NEXT":
                        return GetCodeForNext((ASTVariable)cmd.Operand);
                    // TOPOF
                    // DIM
                    // LISTADD
                    // LISTRM
                    // LIST
                    // PRINTAT
                    case "INK":
                        return Target.GetSnippet("commands", "ink", "colour", GetCodeForExpression(cmd.Operand));
                    case "PAPER":
                        return Target.GetSnippet("commands", "paper", "colour", GetCodeForExpression(cmd.Operand));
                    case "EXIT":
                        return Target.GetSnippet("commands", "exit");
                    case "STOP":
                        return Target.GetSnippet("commands", "stop");
                    case "WAITKEY":
                        return Target.GetSnippet("commands", "waitkey");
                    case "INPUT":
                        return GetCodeForInput(cmd.Operand);
                    case "WRITEFILE":
                        return GetCodeForFileWrite(cmd);
                    case "APPENDFILE":
                        return GetCodeForFileWrite(cmd);
                    default:
                        return Croak($@"
Unsupported command ""{cmd.Command}""
It's a valid command, but not yet implemented in the new transpiler.
");
                }
            }

            if (command.Type == ASTNodeType.If)
            {
                var ifCmd = (ASTIf)command;
                var thenCode = "";
                foreach (var thn in ifCmd.Then)
                {
                    thenCode += GetCodeForCommand(thn, lineNumber);
                }

                var elseCode = "";
                if (ifCmd.Else != null)
                {
                    foreach (var els in ifCmd.Else)
                    {
                        elseCode += GetCodeForCommand(els, lineNumber);
                    }
                    elseCode = Target.GetSnippet("structures", "else", "body", elseCode);
                }

                return Target.GetComplexSnippet("structures", "if", new Dictionary<string, string>()
                {
                    { "condition", GetCodeForExpression(ifCmd.Condition) },
                    { "body", thenCode }
                }) + elseCode;
            }

            if (command.Type == ASTNodeType.For)
            {
                var forLoop = (ASTFor)command;
                var loopVar = (ASTVariable)forLoop.Assignment.Left;
                var counter = GetCodeForVar(loopVar);

                // Bool for skip reassign, not exposed to user code
                var skp = $"{counter}_skip";
                declarations += Target.GetSnippet("vars", "boolDeclaration", "name", skp);

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
                    SkipVar = skp,
                    Start = startCount,
                    Step = forLoop.Step,
                    Counter = loopVar
                };
                loops.Add(counter, loop);

                return Target.GetComplexSnippet("loops", "check", new Dictionary<string, string>()
                {
                    { "skip", skp },
                    { "loopVar", counter },
                    { "start", GetCodeForExpression(startCount) }
                });
            }

            return Croak("Unsupported AST Node in new transpiler.");
        }

        public string GetCodeForTopof (ASTVariable loopVar)
        {
            var counter = GetCodeForVar(loopVar);
            if (!loops.Keys.Contains(counter))
                Croak($"Called TOPOF before defining a loop for variable \"{counter}\"");

            var loop = loops[counter];
            var gotoCode = Target.GetSnippet("commands", "goto", "lineNumber", loop.DefinedOnLine.ToString());
            return gotoCode;
        }

        public string GetCodeForNext (ASTVariable loopVar)
        {
            var counter = GetCodeForVar(loopVar);
            if (!loops.Keys.Contains(counter))
                Croak($"Called NEXT before defining a loop for variable \"{counter}\"");

            var loop = loops[counter];
            var increment = Target.GetComplexSnippet("loops", "increment", new Dictionary<string, string>() {
                { "counter", counter },
                { "step", GetCodeForExpression(loop.Step) }
            });

            var gotoCode = Target.GetSnippet("commands", "goto", "lineNumber", loop.DefinedOnLine.ToString());
            var jump = Target.GetComplexSnippet("loops", "jump", new Dictionary<string, string>()
            {
                { "condition", GetCodeForExpression(loop.Condition) },
                { "skip", loop.SkipVar },
                { "goto", gotoCode }
            });

            return increment + jump;
        }

        public string GetCodeForFileWrite (ASTCommand cmd)
        {
            var type = cmd.Command.ToLower();
            var ops = ((ASTCompoundExpression)cmd.Operand).Expressions;
            var fileName = GetCodeForExpression(ops[0]);
            var contents = GetCodeForExpression(ops[1]);

            return Target.GetComplexSnippet("commands", type, new Dictionary<string, string>()
            {
                { "fileName", fileName },
                { "contents", contents }
            });
        }

        public string GetCodeForInput (IASTNode op)
        {
            var outCode = "";
            ASTVariable target;

            if (op.Type == ASTNodeType.CompoundExpression)
            {
                var args = ((ASTCompoundExpression)op).Expressions;
                target = (ASTVariable)args[1];

                outCode += Target.GetSnippet("commands", "print", "text", GetCodeForExpression(args[0]));
            }
            else target = (ASTVariable)op;

            var type = target.IsString ? "inputString" : "inputNumber";
            return outCode + Target.GetSnippet("commands", type, "var", GetCodeForVar(target));
        }

        public string GetCodeForCall (ASTCall call)
        {
            if (IsStdLib(call))
            {
                return GetCodeForStdLib(call);
            }

            return Croak("Sorry! Functions and arrays aren't supported yet.");
        }

        public string GetCodeForStdLib (ASTCall call)
        {
            var args = call.Arguments.Expressions;
            var argCount = args.Length;
            var fName = call.FunctionName.Name;

            var cd = "";
            // Some calls like RANDOM don't use args
            if (argCount > 0)
                cd = GetCodeForExpression(args[0]);

            switch (fName)
            {
                case "STR":
                    return Target.GetSnippet("stdLib", "str", "val", cd);
                case "VAL":
                    return Target.GetSnippet("stdLib", "val", "val", cd);
                case "SIN":
                    return Target.GetSnippet("stdLib", "sin", "val", cd);
                case "COS":
                    return Target.GetSnippet("stdLib", "cos", "val", cd);
                case "TAN":
                    return Target.GetSnippet("stdLib", "tan", "val", cd);
                case "FLOOR":
                    return Target.GetSnippet("stdLib", "floor", "val", cd);
                case "CEIL":
                    return Target.GetSnippet("stdLib", "ceil", "val", cd);
                case "RANDOM":
                    return Target.GetSnippet("stdLib", "random", "val", cd);
                case "ROUND":
                    var dps = GetCodeForExpression(new ASTNumber()
                    {
                        Value = 0
                    });
                    if (args.Length > 1)
                        dps = GetCodeForExpression(args[1]);
                    return Target.GetComplexSnippet("stdLib", "round", new Dictionary<string, string>() {
                        { "val", cd },
                        { "dps", dps }
                    });
                case "READFILE":
                    return Target.GetSnippet("stdLib", "readfile", "val", cd);
                case "SPLIT":
                    var sep = GetCodeForExpression(args[1]);
                    return Target.GetComplexSnippet("stdLib", "split", new Dictionary<string, string>()
                    {
                        { "val", cd },
                        { "sep", sep }
                    });
                case "JOIN":
                    var sep2 = GetCodeForExpression(args[1]);
                    return Target.GetComplexSnippet("stdLib", "join", new Dictionary<string, string>()
                    {
                        { "val", cd },
                        { "sep", sep2 }
                    });
                case "LEN":
                    return Target.GetSnippet("stdLib", "len", "val", cd);
                default:
                    return Croak("This standard lib function is not yet implemented.");
            }
        }

        public bool IsStdLib (ASTCall call)
        {
            foreach (var fn in LangUtils.StdLib)
            {
                if (
                    fn.Name == call.FunctionName.Name &&
                    fn.IsString == call.FunctionName.IsString)
                    return true;
            }
            return false;
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

            if (expression.Type == ASTNodeType.Binary)
            {
                var bin = (ASTBinary)expression;
                return Target.GetComplexSnippet("operators", bin.Operator,
                    new Dictionary<string, string>()
                    {
                        { "a", GetCodeForExpression(bin.Left) },
                        { "b", GetCodeForExpression(bin.Right) }
                    });
            }

            if (expression.Type == ASTNodeType.Variable)
            {
                var vr = (ASTVariable)expression;
                return GetCodeForVar(vr);
            }

            if (expression.Type == ASTNodeType.Call)
            {
                return GetCodeForCall((ASTCall)expression);
            }

            return Croak("Unsupported expression type in AST.");
        }

        public string GetCodeForVar (ASTVariable vr, bool autoDefine = true)
        {
            var name = vr.IsString ? vr.Name + "_string" : vr.Name + "_number";

            if (autoDefine && !seenVars.Contains(name))
            {
                Console.WriteLine($"[debug] Auto-defining {name}");
                seenVars.Add(name);
                var type = vr.IsString ? "stringDeclaration" : "numberDeclaration";
                declarations += Target.GetSnippet("vars", type, "name", name);
            }

            return name;
        }
    }
}
