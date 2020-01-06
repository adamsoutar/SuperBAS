using System;
using System.Collections.Generic;
using System.IO;
using SuperBAS.Parser;

namespace SuperBAS.Transpiler.CSharp
{
    /* TOOD: Number & string arrays/lists */
    public enum VarType
    {
        String, Number, Bool
    }

    public class Transpiler
    {
        private Parser.Parser parser;
        private List<string> strings = new List<string>();
        private List<string> numbers = new List<string>();
        private List<string> bools = new List<string>();
        private TemplateCode templater;
        private string varDecs = "";
        public string FinalProgram;

        private void DefineVar (VarType type, string name)
        {
            if (Array.IndexOf(LangUtils.StdLib, name) != -1)
            {
                Croak($"Program attempted to define name \"{name}\" which already exists in the standard library.");
            }
            switch (type)
            {
                case VarType.Number:
                    numbers.Add(name);
                    break;
                case VarType.String:
                    strings.Add(name);
                    break;
                case VarType.Bool:
                    bools.Add(name);
                    break;
                default:
                    Croak("Invalid VarType passed to DefineVar");
                    break;
            }
        }
        private void DefineRaw (string cSharpCode)
        {
            varDecs += cSharpCode + '\n';
        }

        public static void Croak (string msg)
        {
            throw new Exception($"C# Transpiler core croaked: {msg}");
        }

        public Transpiler (string file)
        {
            parser = Parser.Parser.FromFile(file);
            FinalProgram = Skeleton.Code;

            templater = new TemplateCode(file, DefineVar, DefineRaw);

            FinalProgram = FinalProgram.Replace("/*CASES*/",
                templater.GetCodeForProgram(
                    parser.GenerateAbstractSyntaxTree()
                    )
                );
            FinalProgram = FinalProgram.Replace("/*DECLARATIONS*/",
                GetDeclarations() + templater.nativeIncludes
                );
            FinalProgram = FinalProgram.Replace("/*LOWESTLINE*/", templater.LineNumbers[0].ToString());
        }

        public void SaveTo(string path)
        {
            var sW = new StreamWriter(path);
            sW.Write(FinalProgram);
            sW.Close();
        }

        private string GetDeclarations ()
        {
            string final = varDecs;
            foreach (var str in strings)
                final += $"private static string {str}_string = \"\";\n";
            foreach (var num in numbers)
                final += $"private static double {num}_number = 0.0;\n";
            foreach (var bl in bools)
                final += $"private static bool {bl}_bool = false;\n";
            return final;
        }
    }
}
