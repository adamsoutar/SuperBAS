using System;
using System.IO;

namespace SuperBAS.Transpiler.Javascript
{
    public class Transpiler
    {
        private Parser.Parser parser;
        private TemplateCode templater;
        public string FinalProgram;
        private string varDecs;

        public Transpiler (string file)
        {
            FinalProgram = Skeleton.Code;
            parser = Parser.Parser.FromFile(file);

            templater = new TemplateCode((string s) => varDecs += s);

            FinalProgram = FinalProgram.Replace("/*CASES*/",
                templater.GetCodeForProgram(parser.GenerateAbstractSyntaxTree())
                );

            FinalProgram = FinalProgram.Replace("/*DEFINITIONS*/", varDecs);

            FinalProgram = FinalProgram.Replace("/*LOWESTLINE*/", templater.LowestLine.ToString());
        }

        public void SaveTo (string path)
        {
            var sW = new StreamWriter(path);
            sW.Write(FinalProgram);
            sW.Close();
        }
    }
}
