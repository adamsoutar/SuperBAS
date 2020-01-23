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
        /*DECLARATIONS*/

        static void Gosub(double lineNumber)
        {
        GosubStart:
            switch (lineNumber)
            {
                case -1:
                    return;
                /*BODY*/
                default:
                    throw new Exception($"Invalid GOTO { lineNumber } - Not a line");
            }
        }

        static void Main(string[] args)
        {
            Gosub(/*LOWESTLINE*/);
        }

        static string ReadAllFile (string flName) {
            // Read an entire file to a string
            // Put into a function for use in WRITEFILE command
            var sr = new StreamReader(flName);
            var s = sr.ReadToEnd();
            sr.Close();
            return s;
        }
    }
}
