
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
        private static int startX;
        private static int startY;
        private static List<string> a_string;


        static void Gosub(double lineNumber)
        {
        GosubStart:
            switch (lineNumber)
            {
                case -1:
                    return;
                case 1:
a_string = new List<string>();
goto case 2;
case 2:
a_string.Add("Hello");
goto case 3;
case 3:
a_string.Add("World");
goto case 4;
case 4:

            var flStream = new FileStream("hello.txt", FileMode.Create);
            flStream.Write((String.Join(",", a_string) + "!"));
            flStream.Close();
            
goto case 5;
case 5:
Console.WriteLine(("It says: " + ReadAllFile("hello.txt")));
goto case -1;

                default:
                 throw new Exception($"Invalid GOTO { lineNumber } - Not a line");
            }
}

static void Main(string[] args)
{
    Console.Clear();
    startX = Console.CursorLeft;
    startY = Console.CursorTop;
    Gosub(1);
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
        