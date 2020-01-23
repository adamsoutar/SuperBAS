using System;
using System.IO;
using System.Text.Json;

namespace SuperBAS.Transpiler.Generic
{
    public class ConfigParser
    {
        public LanguageConfig config;

        public ConfigParser FromFile (string path)
        {
            var sR = new StreamReader(path);
            var cfgText = sR.ReadToEnd();
            sR.Close();

            var p = new ConfigParser();
            p.config = JsonSerializer.Deserialize<LanguageConfig>(cfgText);

            return p;
        }
    }
}
