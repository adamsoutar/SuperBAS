using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using SuperBAS.Transpiler;

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
            string lang;

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

            if (args.Length < 3)
            {
                Console.WriteLine("Target language:");
                lang = Console.ReadLine();
            }
            else lang = args[2];

            var configFolder = "/Users/adam/Documents/Mac Projects/SuperBAS/SuperBAS.Transpiler.Configs/" + lang;
            var target = TargetLanguage.FromDirectory(configFolder);

            Console.WriteLine($"[info] Target language: {target.Config["meta"]["name"]}");
            Console.WriteLine($"[info] {target.Config["meta"]["printInfo"]}");

            var transpiler = new Transpiler.Transpiler(file, target);
            transpiler.SaveTo(output);

            Console.WriteLine("[info] Transpiled! Saving.");
        }
    }
}
