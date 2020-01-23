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

        public double GetLength<T> (T thing)
        {
            // Get the length of anything.
            // This allows one LEN() method for lists, arrays and strings
            if (thing is List<string> lS)
            {
                return lS.Count;
            }
            if (thing is List<double> lD)
            {
                return lD.Count;
            }
            if (thing is string s)
            {
                return s.Length;
            }
            if (thing is string[] sA)
            {
                return sA.Length;
            }
            if (thing is double[] dA)
            {
                return dA.Length;
            }

            throw new Exception("SuperBAS cannot get the length of that object.");
        }
    }
}
