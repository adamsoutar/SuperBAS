using System;
namespace SuperBAS.Transpiler.CSharp
{
    public static class Skeleton
    {
        public static string Code = @"
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.IO;
using SFML.Audio;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace UserProgram
{
    class Program
    {
        private static Random rand;
        private static int startX;
        private static int startY;
        /*DECLARATIONS*/

        static void Gosub(double lineNumber)
        {
        GosubStart:
            switch (lineNumber)
            {
                case -1:
                    return;
                /*CASES*/
                default:
                 throw new Exception($""Invalid GOTO { lineNumber } - Not a line"");
            }
}

static void Main(string[] args)
{
    Console.Clear();
    startX = Console.CursorLeft;
    startY = Console.CursorTop;
    Gosub(/*LOWESTLINE*/);
}

static void PrintAt(double x, double y, string text)
{
    /* I'm not sure this works. It certainly doesn't on macOS .NET Native */
    int oldx = Console.CursorLeft;
    int oldy = Console.CursorTop;
    Console.SetCursorPosition(startX + (int)x, startY + (int)y);
    Console.Write(text);
    Console.SetCursorPosition(oldx, oldy);
}

static string ReadAllFile (string flName) {
    // Read an entire file to a string
    // Put into a function for use in expressions
    var sr = new StreamReader(flName);
    var s = sr.ReadToEnd();
    sr.Close();
    return s;
}
    }
}
        ";
    }
}
