using System;
namespace SuperBAS.Parser
{
    public interface ICharStream
    {
        bool EndOfStream { get; }
        void Croak(string msg, string source);
        char Read();
        char Peek();
    }
}
