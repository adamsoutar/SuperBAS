using System;
using System.Linq;

namespace SuperBAS.Parser
{
    public static class TokeniserUtils
    {
        private static char[] whitespace = "\r\t ".ToCharArray();
        // New lines are significant, they're punctuation not whitespace
        private static char[] punctuation = ":$#,()\n".ToCharArray();
        // Includes . for 3.14, there's no
        // properties in BASIC, so any time we see
        // a . it's safe to assume a number, ie .5, .2
        private static char[] numbers = ".0123456789".ToCharArray();
        private static char[] letters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
        // Strings valid for the body of an identifier such as myString2
        private static char[] identifierChars = numbers.Concat(letters).ToArray();
        private static char[] operatorChars = "=<>!&%|+-*/".ToCharArray();

        private static bool Contains<T> (T[] a, T c)
        {
            return Array.IndexOf(a, c) != -1;
        }

        public static bool IsWhitespace(char c)
        {
            return Contains(whitespace, c);
        }
        public static bool IsPunctuation(char c)
        {
            return Contains(punctuation, c);
        }
        public static bool IsNumber(char c)
        {
            return Contains(numbers, c);
        }
        public static bool IsIdentifierStart (char c)
        {
            return Contains(letters, c);
        }
        public static bool IsIdentifierChar (char c)
        {
            return Contains(identifierChars, c);
        }

        public static bool IsOperator (string s)
        {
            return Contains(LangUtils.BinaryOperators.Keys.ToArray(), s);
        }
        public static bool IsOperator(char c)
        {
            return IsOperator(c.ToString());
        }
        public static bool IsOperatorChar(char c)
        {
            // Chars that could be contained in a longer operator,
            // but aren't necessarily their own operator, like !
            return Contains(operatorChars, c);
        }
        public static bool IsKeyword(string s)
        {
            return Contains(LangUtils.Keywords, s);
        }

        public static string ReplaceOperatorAliases (string o)
        {
            var bo = LangUtils.BinaryOperatorAliases;
            if (Contains(bo.Keys.ToArray(), o))
            {
                return bo[o];
            }
            return o;
        }
    }
}
