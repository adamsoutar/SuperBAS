﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SuperBAS.Parser
{
    // Represents a line
    // eg. 10 PRINT "Hello" : PRINT "World"
    public struct SyntaxTreeTopLevel
    {
        public float LineNumber;
        public List<IASTNode> Commands;
    }

    public class Parser
    {
        private Tokeniser tokenStream;
        public string SourcePath { get => tokenStream.SourcePath; }

        public Parser (Tokeniser tokeniser)
        {
            tokenStream = tokeniser;
        }

        public void Croak (string msg, string source = "Parser")
        {
            tokenStream.Croak(msg, source);
        }

        private Token NextToken ()
        {
            Token? token = tokenStream.Read();
            if (token == null)
                Croak("Did not expect a null token without reaching EOF.");
            return (Token)token;
        }

        private bool IsCommand (IASTNode node)
        {
            if (node.Type != ASTNodeType.Keyword) return false;
            var word = ((ASTKeyword)node).Keyword;
            return Array.IndexOf(LangUtils.Commands, word) != -1;
        }
        private bool IsAssignment (IASTNode node)
        {
            if (node.Type != ASTNodeType.Binary) return false;
            var op = ((ASTBinary)node).Operator;
            return op == "=";
        }

        public SyntaxTreeTopLevel[] GenerateAbstractSyntaxTree ()
        {
            var nodes = new List<SyntaxTreeTopLevel>();

            while (!tokenStream.EndOfStream)
            {
                if (IsNextPunctuation("#"))
                {
                    tokenStream.Read();
                    ExpectKeyword("INCLUDE");

                    var path = (Token)tokenStream.Read();
                    if (path.Type != TokenType.String)
                        Croak("Include must be a compile-time constant string.");

                    Console.WriteLine($"[info] Including {path.Value}");

                    // Include paths are relative to the original source
                    var includePath = Path.Combine(Path.GetDirectoryName(SourcePath), path.Value);
                    var newFileParser = Parser.FromFile(includePath);
                    foreach (var ast in newFileParser.GenerateAbstractSyntaxTree())
                        nodes.Add(ast);

                    continue;
                }

                var node = new SyntaxTreeTopLevel();
                node.Commands = new List<IASTNode>();

                // Ignore blank newlines
                if (IsNextPunctuation("\n"))
                {
                    tokenStream.Read();
                    continue;
                }

                var lineNumber = ParseExpression();
                if (lineNumber.Type != ASTNodeType.Number)
                {
                    Croak("Lines must start with a line number.");
                }

                node.LineNumber = ((ASTNumber)lineNumber).Value;

                while (true)
                {
                    node.Commands.Add(ParseCommand());
                    // If there's a :, we can put multiple commands on one line
                    if (!IsNextPunctuation(":"))
                    {
                        break;
                    }
                    // Consume the :
                    tokenStream.Read();
                }

                nodes.Add(node);

                // Enforce newlines after commands
                if (!tokenStream.EndOfStream) ExpectPunctuation("\n");
            }

            // Sort program
            var nodeArr = nodes.OrderBy(n => n.LineNumber).ToArray();

            if (
                 nodeArr.Select(x => x.LineNumber).Distinct().Count() != nodeArr.Count()
               )
            {
                Croak("Duplicate line number defined");
            }

            return nodeArr;
        }

        private bool IsControlStructure (IASTNode node)
        {
            return node.Type == ASTNodeType.If || node.Type == ASTNodeType.For;
        }

        private IASTNode ParseCommand ()
        {
            var command = ParseExpression();
            if (IsControlStructure(command)) return command;
            if (
                !IsCommand(command) && !IsAssignment(command)
               )
            {
                Croak("The token following a line number or : must be a command, assignment or control structure");
            }

            // Operand
            if (IsAssignment(command))
            {
                return new ASTCommand()
                {
                    Command = "ASSIGN",
                    Operand = command
                };
            }
            else
            {
                IASTNode operand;
                if (IsNextPunctuation(":") || IsNextPunctuation("\n"))
                {
                    // Operands are optional for commands like CLS
                    operand = new ASTInvalidNode();
                } else
                {
                    operand = ParseExpression();
                    if (IsNextPunctuation(","))
                    {
                        // A tupple command like
                        // LISTADD myList$, "Hello"
                        tokenStream.Read();

                        var lst = new List<IASTNode>();
                        lst.Add(operand);

                        while (true)
                        {
                            lst.Add(ParseExpression());
                            if (IsNextPunctuation(","))
                            {
                                tokenStream.Read();
                            } else break;
                        }

                        operand = new ASTCompoundExpression()
                        {
                            Expressions = lst.ToArray()
                        };
                    }
                }
                return new ASTCommand()
                {
                    Command = ((ASTKeyword)command).Keyword,
                    Operand = operand
                };
            }
        }

        private bool IsNextKeyword(string keyword)
        {
            var token = ((Token)tokenStream.Peek());
            return token.Type == TokenType.Keyword && token.Value == keyword;
        }

        private IASTNode ParseAtom ()
        {
            // Bracketed expressions like in (2 + 3) * 4
            if (IsNextPunctuation("("))
            {
                tokenStream.Read();
                var exp = ParseExpression();
                ExpectPunctuation(")");
                return exp;
            }

            var token = (Token)tokenStream.Read();

            // Structures, eg IF, FOR
            if (token.Type == TokenType.Keyword && token.Value == "IF")
            {
                var condition = ParseExpression();
                ExpectKeyword("THEN");
                var then = ParseCommand();

                // Optional else
                IASTNode elseStatement = null;
                if (IsNextKeyword("ELSE"))
                {
                    tokenStream.Read();
                    var elseRaw = ParseCommand();
                    elseStatement = elseRaw;
                }
                return new ASTIf()
                {
                    Condition = condition,
                    Then = then,
                    Else = elseStatement
                };
            }

            if (token.Type == TokenType.Keyword && token.Value == "FOR")
            {
                // Loooops
                var loop = new ASTFor();
                var assign = ParseExpression();

                // Checks
                if (assign.Type != ASTNodeType.Binary)
                    Croak("Expected a binary expression after FOR");
                var assignBin = (ASTBinary)assign;
                if (assignBin.Operator != "=")
                    Croak($"Expected assignment, got operator {assignBin.Operator}");
                if (assignBin.Left.Type != ASTNodeType.Variable)
                    Croak("Loop counter must be a variable.");
                var assignLeft = (ASTVariable)assignBin.Left;
                if (assignLeft.IsString)
                    Croak("Loop counter cannot be a string.");

                loop.Assignment = assignBin;

                ExpectKeyword("TO");
                loop.ToMax = ParseExpression();

                loop.Step = new ASTNumber()
                {
                    Value = 1
                };
                if (IsNextKeyword("STEP"))
                {
                    tokenStream.Read();
                    loop.Step = ParseExpression();
                }
                return loop;
            }


            if (token.Type == TokenType.Variable)
            {
                bool isString = false;
                if (IsNextPunctuation("$"))
                {
                    tokenStream.Read();
                    isString = true;
                }
                return new ASTVariable()
                {
                    IsString = isString,
                    Name = token.Value
                };
            }

            // Literals
            if (token.Type == TokenType.Number)
            {
                return new ASTNumber()
                {
                    Value = float.Parse(token.Value)
                };
            }
            if (token.Type == TokenType.String)
            {
                return new ASTString()
                {
                    Value = token.Value
                };
            }
            if (token.Type == TokenType.Keyword)
            {
                return new ASTKeyword()
                {
                    Keyword = token.Value
                };
            }

            if (token.Value == "\n")
                Croak("Unexpected newline");
            else
                Croak($"Unexpected token \"{token.Value}\"");
            return new ASTInvalidNode();
        }

        private IASTNode ParseExpression ()
        {
            // These used to be reversed
            // but caused a bug with array indexing
            return MightBeBinary(MightBeCall(ParseAtom()), 0);
        }

        private IASTNode MightBeCall (IASTNode node)
        {
            if (node.Type == ASTNodeType.Variable && IsNextPunctuation("("))
            {
                return new ASTCall()
                {
                    FunctionName = (ASTVariable)node,
                    Arguments = Delimited("(", ")", ",", ParseExpression)
                };
            }
            return node;
        }

        private IASTNode MightBeBinary (IASTNode left, int myPrecedence)
        {
            if (IsNextOperator())
            {
                var op = (Token)tokenStream.Peek();
                int theirPrecedence = LangUtils.BinaryOperators[op.Value];
                if (theirPrecedence > myPrecedence)
                {
                    tokenStream.Read();

                    if (op.Value == "=" &&
                        !(left.Type == ASTNodeType.Variable ||
                          left.Type == ASTNodeType.Call))
                        Croak("Can only assign to variable or array.");

                    return MightBeBinary(new ASTBinary()
                    {
                        Operator = op.Value,
                        Left = left,
                        Right = MightBeCall(MightBeBinary(ParseAtom(), theirPrecedence))
                    }, myPrecedence);
                }
            }
            return left;
        }

        private ASTCompoundExpression Delimited (string start, string end, string separator, Func<IASTNode> parser)
        {
            var args = new List<IASTNode>();
            bool first = true;

            ExpectPunctuation(start);
            while (!tokenStream.EndOfStream)
            {
                if (IsNextPunctuation(end)) break;
                if (first) first = false;
                else ExpectPunctuation(separator);
                args.Add(parser());
            }
            ExpectPunctuation(end);

            return new ASTCompoundExpression()
            {
                Expressions = args.ToArray()
            };
        }

        private bool IsNextOperator ()
        {
            var token = tokenStream.Peek();
            if (token == null) return false;
            var defToken = (Token)token;
            return defToken.Type == TokenType.Operator;
        }

        private bool IsNextPunctuation (string punct)
        {
            var token = tokenStream.Peek();
            if (token == null) return false;
            var defToken = (Token)token;
            return defToken.Type == TokenType.Punctuation && defToken.Value == punct;
        }

        private void ExpectPunctuation(string punct)
        {
            // Silently advances file if the next token is this punctuation
            var node = NextToken();
            if (!(node.Type == TokenType.Punctuation && node.Value == punct))
            {
                // Errors don't format correctly if the punctuation is a newline
                if (punct == "\n")
                {
                    Croak($"Expected newline but got {node.Value}");
                    return;
                }
                Croak($"Expected punctuation {punct} but got {node.Value}");
            }
        }
        private void ExpectKeyword(string keyword)
        {
            var node = NextToken();
            if (!(node.Type == TokenType.Keyword && node.Value == keyword))
            {
                Croak($"Expected keyword {keyword} but got {node.Value}");
            }
        }

        public static Parser FromFile (string path)
        {
            return new Parser(new Tokeniser(FileCharStream.FromFile(path)));
        }
    }
}
