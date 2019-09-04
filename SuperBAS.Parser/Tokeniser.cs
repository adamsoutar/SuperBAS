using System;
using System.IO;

namespace SuperBAS.Parser
{
    public enum TokenType
    {
        String,
        Variable,
        Number,
        Operator,
        Keyword,
        Punctuation
    }

    public struct Token
    {
        public TokenType Type;
        public string Value;
    }

    public class Tokeniser
    {
        private ICharStream codeStream;
        private Token? current;
        public bool EndOfStream { get => current == null; }

        public Tokeniser(ICharStream codeStream)
        {
            this.codeStream = codeStream;
            current = ReadNext();
        }

        public Token? Peek ()
        {
            return current;
        }
        public Token? Read ()
        {
            var t = current;
            current = ReadNext();
            return t;
        }

        private string ReadWhile(Func<char, bool> ShouldConsume)
        {
            string final = "";
            while (!codeStream.EndOfStream)
            {
                char peek = codeStream.Peek();
                if (!ShouldConsume(peek)) break;
                final += codeStream.Read();
            }
            return final;
        }

        private Token ReadString ()
        {
            string outStr = "";
            codeStream.Read();
            while (!codeStream.EndOfStream)
            {
                char next = codeStream.Read();
                if (next == '"')
                {
                    if (
                        !codeStream.EndOfStream &&
                        codeStream.Peek() == '"'
                       )
                    {
                        // Double escaped string, everything is fine
                        codeStream.Read();
                    } else
                    {
                        // That's all folks
                        break;
                    }
                }
                outStr += next;
            }
            return new Token()
            {
                Type = TokenType.String,
                Value = outStr
            };
        }
        private Token ReadNumber ()
        {
            return new Token() {
                Type = TokenType.Number,
                Value = ReadWhile(TokeniserUtils.IsNumber)
            };
        }
        private Token ReadIdentifier ()
        {
            string identifier = ReadWhile(TokeniserUtils.IsIdentifierChar);
            TokenType type = TokeniserUtils.IsKeyword(identifier) ? TokenType.Keyword : TokenType.Variable;
            // This catches things like AND/OR/MOD from being seen as variables
            if (TokeniserUtils.IsOperator(identifier))
                type = TokenType.Operator;
            
            return new Token()
            {
                Type = type,
                Value = identifier
            };
        }
        private Token TokenFromNextChar (TokenType type)
        {
            return new Token()
            {
                Type = type,
                Value = codeStream.Read().ToString()
            };
        }

        public void Croak (string msg, string source = "Tokeniser")
        {
            codeStream.Croak(msg, source);
        }

        private Token? ReadNext ()
        {
            ReadWhile(TokeniserUtils.IsWhitespace);
            if (codeStream.EndOfStream) return null;
            var ch = codeStream.Peek();

            if (ch == '"') return ReadString();
            if (TokeniserUtils.IsNumber(ch)) return ReadNumber();
            if (TokeniserUtils.IsIdentifierChar(ch)) return ReadIdentifier();
            if (TokeniserUtils.IsPunctuation(ch)) return TokenFromNextChar(TokenType.Punctuation);
            if (TokeniserUtils.IsOperator(ch)) return TokenFromNextChar(TokenType.Operator);

            Croak($"Unexpected character");
            return null;
        }
    }
}
