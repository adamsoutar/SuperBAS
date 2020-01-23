using System;
namespace SuperBAS.Transpiler.Generic
{
    public struct LanguageConfig
    {
        public LangMeta Meta;
    }

    public struct LangMeta
    {
        public string Name;
        public string FileType;
    }
}
