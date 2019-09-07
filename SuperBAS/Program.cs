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
            string output = "./Program.cs";

            if (args.Length == 0)
            {
                // Use last program
            }
            else file = args[0];

            if (args.Length > 1)
                output = args[1];
            else Console.WriteLine("[warn] No output filename provided, using Program.cs");

            var Transpiler = new Transpiler.CSharp.Transpiler(file);
            Transpiler.SaveTo(output);
        }
    }
}
