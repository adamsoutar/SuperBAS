using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.IO;

namespace UserProgram
{
    class Program
    {
        private static Random rand;
        private static double NUM_number = 0.0;


        static void Gosub(double lineNumber)
        {
        GosubStart:
            switch (lineNumber)
            {
                case -1:
                    return;
                case 1:Console.WriteLine("Enter a number:");NUM_number = double.Parse(Console.ReadLine());
goto case 2;case 2:if ((NUM_number > (double)(10))) { Console.WriteLine("Big number");Console.WriteLine("Impressed"); } else { Console.WriteLine("Small number");Console.WriteLine("Not impressed"); }
goto case 3;case 3:lineNumber = (double)(1); goto GosubStart;
goto case -1;
                default:
                    throw new Exception($"Invalid GOTO { lineNumber } - Not a line");
            }
        }

        static void Main(string[] args)
        {
            Gosub(1);
        }

        static string ReadAllFile (string flName) {
            // Read an entire file to a string
            // Put into a function for use in WRITEFILE command
            var sr = new StreamReader(flName);
            var s = sr.ReadToEnd();
            sr.Close();
            return s;
        }

        static void WriteAllFile (string flName, string contents, bool append) {
            var sw = new StreamWriter(flName, append);
            sw.Write(contents);
            sw.Close();
        }
    }
}
