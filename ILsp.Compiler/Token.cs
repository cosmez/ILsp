using System;
using System.Collections.Generic;
using System.Text;

namespace ILsp.Compiler
{
    public enum TokenType
    {
        ParsingError,
        OpenParen,
        CloseParent,
        Number
    }

    public struct Token
    {
        public TokenType Type;
        public string Value;
        public int Position;
        public readonly override string ToString()
        {
            return $"Token({Type.ToString()}{(!string.IsNullOrWhiteSpace(Value) ? " " + Value : "")})";
        }
    }
}
