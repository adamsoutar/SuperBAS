using System;
namespace SuperBAS.Parser
{
    public interface ICharStream
    {
        bool EndOfStream { get; }
        string SourcePath { get; }
        void Croak(string msg, string source);
        char Read();
        char Peek();
    }
}
