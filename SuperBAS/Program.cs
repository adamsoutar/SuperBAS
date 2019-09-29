using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace SuperBAS
{
    class Program
    {
        static void Main(string[] args)
        {
            string file = "";
            string output = "./Output.cs";

            if (args.Length == 0)
            {
                // TODO: Use last program
                Console.WriteLine("SuperBAS source file:");
                file = Console.ReadLine();
            }
            else file = args[0];

            if (args.Length > 1)
                output = args[1];
            else Console.WriteLine($"[warn] No output filename provided, defaulting");

            var Transpiler = new Transpiler.CSharp.Transpiler(file);
            Transpiler.SaveTo(output);

            Console.WriteLine($"Compiled {file} and saved to {output}");
        }
    }
}
