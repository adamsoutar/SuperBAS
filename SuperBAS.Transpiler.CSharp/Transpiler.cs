﻿using System;
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
        private string skeleton = "";
        private Parser.Parser parser;
        private List<string> strings = new List<string>();
        private List<string> numbers = new List<string>();
        private List<string> bools = new List<string>();
        private TemplateCode templater;


        private void DefineVar (VarType type, string name)
        {
            if (type == VarType.Number) numbers.Add(name);
            if (type == VarType.String) strings.Add(name);
        }

        public Transpiler (string file)
        {
            parser = Parser.Parser.FromFile(file);
            var sR = new StreamReader("./Template.cs.txt");
            skeleton = sR.ReadToEnd();
            sR.Close();

            templater = new TemplateCode(DefineVar);

            string finalProgram = skeleton;
            finalProgram = finalProgram.Replace("/*CASES*/",
                templater.GetCodeForProgram(
                    parser.GenerateAbstractSyntaxTree()
                    )
                );
            finalProgram = finalProgram.Replace("/*DECLARATIONS*/",
                GetDeclarations()
                );
            finalProgram = finalProgram.Replace("/*LOWESTLINE*/", templater.LineNumbers[0].ToString());

            var sW = new StreamWriter("./Program.cs");
            sW.Write(finalProgram);
            sW.Close();
            Console.WriteLine(finalProgram);
        }

        private string GetDeclarations ()
        {
            string final = "";
            foreach (var str in strings)
                final += $"static string {str}_string = \"\";\n";
            foreach (var num in numbers)
                final += $"static double {num}_number = 0f;\n";
            foreach (var bl in bools)
                final += $"static bool {bl}_bool = false;\n";
            return final;
        }
    }
}
