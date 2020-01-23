using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace SuperBAS.Transpiler.Generic
{
    public class TargetLanguage
    {
        public dynamic Config;
        public string Skeleton;

        public static TargetLanguage FromDirectory (string path)
        {
            var sR = new StreamReader(Path.Combine(path, "language.json"));
            var cfgText = sR.ReadToEnd();
            sR.Close();

            sR = new StreamReader(Path.Combine(path, "skeleton.cs"));
            var code = sR.ReadToEnd();
            sR.Close();

            var p = new TargetLanguage();

            p.Config = JsonConvert.DeserializeObject(cfgText);
            p.Skeleton = code;

            return p;
        }

        public string CodeGen (string declarations, string body, string lowestLine)
        {
            var s = Skeleton;
            var iD = GetSnippet("injectionPoints", "declarations");
            var iB = GetSnippet("injectionPoints", "body");
            var iL = GetSnippet("injectionPoints", "lowestLine");
            s = s.Replace(iD, declarations);    
            s = s.Replace(iB, body);
            s = s.Replace(iL, lowestLine);
            return s;
        }

        public string GetComplexSnippet (string type, string name, Dictionary<string, string> replacements)
        {
            var snip = (string)Config["snippets"][type][name];

            foreach (var r in replacements)
            {
                snip = snip.Replace($"%{r.Key}%", r.Value);
            }

            return snip;
        }

        // Gets a snippet with only one replacement
        public string GetSnippet (string type, string name, string toReplace = "", string value = "")
        {
            var reps = new Dictionary<string, string>();
            reps.Add(toReplace, value);
            return GetComplexSnippet(type, name, reps);
        }
    }
}
