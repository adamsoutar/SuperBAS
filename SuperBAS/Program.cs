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
            Console.WriteLine("Name of a file in the BasicCode dir:");
            string file = $"../../../../BasicCode/{Console.ReadLine()}.bas";

            var Transpiler = new Transpiler.CSharp.Transpiler(file);
        }
    }
}
