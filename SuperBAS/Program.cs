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
            Autoupdater.thisVersion = "0.4";
            Autoupdater.UpdateIfAvailable();

            string file;
            string output;

            if (args.Length == 0)
            {
                Console.WriteLine("SuperBAS source file:");
                file = Console.ReadLine();
            }
            else file = args[0];

            if (args.Length < 2)
            {
                Console.WriteLine("Output file name:");
                output = Console.ReadLine();
            }
            else output = args[1];

            if (args.Length > 2 && args[2] == "web")
            {
                Console.WriteLine("[info] Transpiling to JS (web)");
                var Transpiler = new Transpiler.Javascript.Transpiler(file);
                Transpiler.SaveTo(output);
            } else
            {
                Console.WriteLine("[info] Transpiling to C# (native)");
                var Transpiler = new Transpiler.CSharp.Transpiler(file);
                Transpiler.SaveTo(output);
            }

            Console.WriteLine($"Compiled {file} and saved to {output}");
        }
    }
}
