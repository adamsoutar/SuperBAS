using System;
using System.Collections.Generic;
using System.Linq;

namespace SuperBAS.Parser
{
    public static class LangUtils
    {
        // In the form key = operator, value = precedence 
        public static Dictionary<string, int> BinaryOperators = new Dictionary<string, int>
        {
            ["="] = 1,
            ["||"] = 2,
            ["&&"] = 3,
            ["<"] = 7,
            [">"] = 7,
            ["<="] = 7,
            [">="] = 7,
            ["NOT"] = 7,
            ["+"] = 15,
            ["-"] = 10,
            ["*"] = 20,
            ["/"] = 25,
            ["MOD"] = 20
        };
        public static string[] Commands =
        {
            "PRINT", "IF", "LET", "GOTO", "GOSUB", "RETURN"
        };
        // Reserved words, some of which are not commands
        public static string[] Keywords = Commands.Concat(new string[]
        {
            "THEN"
        }).ToArray();
    }
}
