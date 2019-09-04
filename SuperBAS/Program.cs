using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using SuperBAS.Parser;

namespace SuperBAS
{
    class Program
    {
        static void TokeniserPrint (Tokeniser tokeniser)
        {
            while (tokeniser.Peek() != null)
            {
                var token = (Token)tokeniser.Read();
                Console.WriteLine($"{token.Type} -> {token.Value}");
            }
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Name of a file in the BasicCode dir:");
            string file = $"../../../../BasicCode/{Console.ReadLine()}.bas";

            var parser = Parser.Parser.FromFile(file);
            var timer = new Stopwatch();
            timer.Start();
            var syntaxTree = parser.GenerateAbstractSyntaxTree();
            timer.Stop();

            Console.WriteLine($"Generated in {timer.ElapsedMilliseconds}ms");
            Console.ReadKey();
        }
    }
}
