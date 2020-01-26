using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.IO;

namespace UserProgram
{
    class Program
    {
        private static Random rand = new Random();
        static string[] STRINGS_string;static double[, ] INTMAP_number;

        static void Gosub(double lineNumber)
        {
        GosubStart:
            switch (lineNumber)
            {
                case -1:
                    return;
                case 1:STRINGS_string = new string[(int)((double)(5))];
goto case 2;case 2:INTMAP_number = new double[(int)((double)(10)), (int)((double)(20))];
goto case 2.5;case 2.5:STRINGS_string[(int)((double)(0))] = "Adam";
goto case 3;case 3:Console.WriteLine(GetLength(STRINGS_string));
goto case 5;case 5:Console.WriteLine(STRINGS_string[(int)((double)(0))]);
goto case 6;case 6:INTMAP_number[(int)((double)(0)), (int)((double)(1))] = (double)(2);
goto case 7;case 7:Console.WriteLine(INTMAP_number[(int)((double)(0)), (int)((double)(1))]);
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

        static double GetLength<T> (T thing)
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

            throw new Exception("[SuperBAS] Cannot get the length of that object.");
        }
    }
}
