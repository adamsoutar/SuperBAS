using System;
using System.IO;

namespace SuperBAS.Parser
{
    public class FileCharStream : ICharStream
    {
        private StreamReader streamReader;
        public string SourcePath { get; }
        public bool EndOfStream { get => streamReader.EndOfStream; }
        private uint col;
        private uint line;
        private string lineSoFar;

        public FileCharStream(StreamReader stream, string sourcePath)
        {
            streamReader = stream;
            SourcePath = sourcePath;
        }
        public static FileCharStream FromFile (string path)
        {
            return new FileCharStream(new StreamReader(path), path);
        }

        public char Peek ()
        {
            return (char)streamReader.Peek();
        }

        public char Read ()
        {
            col++;
            char outChar = (char)streamReader.Read();
            lineSoFar += outChar;
            if (outChar == '\n')
            {
                col = 0;
                lineSoFar = "";
                line++;
            }
            return outChar;
        }

        public void GetFullLine ()
        {
            while (true)
            {
                if (streamReader.Peek() == -1) break;
                char c = (char)streamReader.Read();
                if (c == '\n') break;
                lineSoFar += c;
            }
        }

        public void Croak (string msg, string source = "FileStream")
        {
            GetFullLine();
            string details = $"{lineSoFar}\n";
            for (int i = 0; i < col; i++) details += ' ';
            details += '^';
            /* This creates something like:
             * 10 PRINT !HELLO"
             *          ^
             */
            throw new Exception($"{source} croaked at line {line}: {msg}\n\n{details}");
        }
    }
}
