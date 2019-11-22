using System;
using System.Collections.Generic;
using System.Linq;

namespace SuperBAS.Parser
{
    public struct VariableAlias {
        public ASTVariable Alias;
        public string IsAliasFor;
    }

    public static class LangUtils
    {
        // In the form key = operator, value = precedence 
        public static Dictionary<string, int> BinaryOperators = new Dictionary<string, int>
        {
            ["="] = 1,
            ["<"] = 7,
            [">"] = 7,
            ["<="] = 7,
            [">="] = 7,
            ["!="] = 7,
            ["AND"] = 7,
            ["OR"] = 7,
            ["NOT"] = 7,
            ["+"] = 15,
            ["-"] = 10,
            ["*"] = 20,
            ["/"] = 25,
            ["MOD"] = 20
        };
        public static Dictionary<string, string> BinaryOperatorAliases = new Dictionary<string, string>
        {
            ["<>"] = "!=",
            ["||"] = "OR",
            ["&&"] = "AND",
            ["%"] = "MOD",
            ["!"] = "NOT"
        };
        public static string[] Commands =
        {
            "PRINT", "IF", "LET", "GOTO", "GOSUB", "RETURN", "CLS", "FOR", "NEXT",
            "TOPOF", "DIM", "LIST", "LISTADD", "LISTRM", "PRINTAT", "INK", "PAPER",
            "SLEEP", "WAITKEY", "EXIT", "STOP", "INPUT", "WRITEFILE", "APPENDFILE",
            "DEF"
        };
        public static ASTVariable[] StdLib =
        {
            new ASTVariable() {
                Name = "STR",
                IsString = true
            },
            new ASTVariable() {
                Name = "VAL",
                IsString = false
            },
            new ASTVariable()
            {
                Name = "LEN",
                IsString = false
            },
            new ASTVariable()
            {
                Name = "SIN",
                IsString = false
            },
            new ASTVariable()
            {
                Name = "COS",
                IsString = false
            },
            new ASTVariable()
            {
                Name = "TAN",
                IsString = false
            },
            new ASTVariable()
            {
                Name = "FLOOR",
                IsString = false
            },
            new ASTVariable()
            {
                Name = "CEIL",
                IsString = false
            },
            new ASTVariable()
            {
                Name = "ROUND",
                IsString = false
            },
            new ASTVariable()
            {
                Name = "RANDOM",
                IsString = false
            },
            new ASTVariable()
            {
                Name = "READFILE",
                IsString = true
            },
            new ASTVariable()
            {
                Name = "SPLIT",
                IsString = true
            },
            new ASTVariable()
            {
                Name = "JOIN",
                IsString = true
            }
        };
        public static VariableAlias[] StdLibAliases = {
            new VariableAlias() {
                Alias = new ASTVariable() {
                    Name = "INT",
                    IsString = false
                },
                IsAliasFor = "FLOOR"
            }
        };
        public static string[] CompilerKeywords =
        {
            "INCLUDE"
        };
        // Reserved words, some of which are not commands
        public static string[] Keywords = CompilerKeywords.Concat(Commands.Concat(new string[]
        {
            "THEN", "ELSE", "TO", "STEP"
        })).ToArray();
    }
}
