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
        static List<string> NAMES_string;static double I_number = 0.0;
static bool I_number_skip = false;


        static void Gosub(double lineNumber)
        {
        GosubStart:
            switch (lineNumber)
            {
                case -1:
                    return;
                case 1:NAMES_string = new List<string>();
goto case 2;case 2:NAMES_string.Add("Adam");
goto case 3;case 3:NAMES_string.Add("Rob");
goto case 4;case 4:if (I_number_skip) I_number_skip = false; else I_number = (double)(0);
goto case 5;case 5:Console.WriteLine(NAMES_string[(int)(I_number)]);
goto case 6;case 6:I_number += (double)(1);if ((I_number <= (GetLength(NAMES_string) - (double)(1)))) { I_number_skip = true; lineNumber = 4; goto GosubStart; }
goto case 7;case 7:Console.WriteLine(GetLength(("Hello" + "World")));
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

            throw new Exception("SuperBAS cannot get the length of that object.");
        }
    }
}
