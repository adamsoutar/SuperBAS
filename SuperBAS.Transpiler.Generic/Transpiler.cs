using System;
using System.IO;
using SuperBAS.Parser;

namespace SuperBAS.Transpiler.Generic
{
    public class Transpiler
    {
        public TargetLanguage Target;
        public string OutputCode;
        private Parser.Parser parser;
        private Templater templater;

        public Transpiler(string file, TargetLanguage transpilerTarget)
        {
            Target = transpilerTarget;
            parser = Parser.Parser.FromFile(file);
            // TODO: Populate OutputCode
        }

        public void SaveTo (string file) {
            var sW = new StreamWriter(file);
            sW.Write(OutputCode);
            sW.Close();
        }
    }
}
