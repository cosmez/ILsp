using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Collections;

namespace ILsp.Compiler
{
    public class Tokenizer : IEnumerator<Token>
    {
        private readonly StreamReader _reader;
        private int _position = 0;

        private int _tokenPosition = 0;
        private TokenType _tokenOnProgress;
        private StringBuilder _tokenString;

        private Token _current;

        private Char Chr;
        private Char Peek;

        readonly char[] buffer;
        int _bi;
        int _bmax;


        public Tokenizer(StreamReader reader)
        {
            _bi = 0;
            _bmax = 0;
            buffer = new char[500];
            this._reader = reader;
            Next();
        }


        /****
            STREAMREADER CONTROL LOGIC
        ****/

        /// <summary>
        /// Reads a character from the stream or rewind cache
        /// </summary>
        /// <returns></returns>
        public char Next()
        {
            _position++;
            if (_bi < _bmax)
            {

            }
            else
            {
                buffer[_bi] = (char)_reader.Read();
                _bmax++;
            }

            Chr = buffer[_bi++];

            if ((_bi + 1) < _bmax)
            {
                Peek = buffer[_bi + 1];
            }
            else
            {
                Peek = (char)_reader.Peek();
            }

            return Chr;
        }

        /// <summary>
        /// Resets the rewind for parsing errors
        /// </summary>
        private void ResetRewind()
        {
            _bi = 0;
            _bmax = 0;
        }


        /// <summary>
        /// Resets all chars from previous parsing attempt to the initial version
        /// </summary>
        public void Rewind()
        {
            _position -= _bi;
            _bi = 0;
        }

        public void Reset()
        {

        }





        public Token Current => _current;

        object IEnumerator.Current => _current;

        public void Dispose()
        {

        }





        /// <summary>
        /// Throws a parsing error, recover and move on
        /// </sumary>
        private void Error(string message)
        {
            _current = new Token()
            {
                Type = TokenType.ParsingError,
                Position = _tokenPosition,
                Value = $"{message} at {_tokenPosition}"
            };

            while (!IsSeparator())
            {
                Next();
            }
        }

        private void Recover()
        {
            while (!IsSeparator())
            {
                Next();
            }
        }


        /****
            PARSING LOGIC
        ****/

        /// <summary>
        /// Sets the marker to begin parsing a token
        /// </summary>
        private void TokenBegin(TokenType tokenType)
        {
            _tokenPosition = _position;
            _tokenOnProgress = tokenType;
            _tokenString = new StringBuilder();
        }

        /// <summary>
        /// Finalize the Current Token Parsing
        /// </summary>
        private void TokenEnd()
        {
            _current = new Token()
            {
                Type = _tokenOnProgress,
                Position = _tokenPosition,
                Value = _tokenString.ToString()
            };
        }

        private void TokenAppend()
        {
            _tokenString.Append(Chr);
            Next();
        }

        /// <summary>
        /// Main Tokenizing Logic
        /// </summary>
        /// <returns></returns>
        public bool MoveNext()
        {
            ConsumeSpaces();
            switch (Chr)
            {
                case '(':
                    _current = new Token() { Type = TokenType.OpenParen, Position = _tokenPosition };
                    Next();
                    break;
                case ')':
                    _current = new Token() { Type = TokenType.CloseParent, Position = _tokenPosition };
                    Next();
                    break;
                case Char dec when (IsMaybeNumber(dec, Peek)):
                    ParseNumber();
                    break;
                case Char ident when (Char.IsLetter(ident) || ident == '_'):
                    ParseSymbol();
                    break;
                default:
                    Recover();
                    break;
            }

            return !_reader.EndOfStream;
        }

        public void ParseSymbol()
        {
            throw new NotImplementedException();
        }


        private bool IsHexadecimal(char c)
        {
            return (Char.IsDigit(c) ||
                c == 'A' || c == 'B' || c == 'C' || c == 'D' || c == 'E' || c == 'F' ||
                c == 'a' || c == 'b' || c == 'c' || c == 'd' || c == 'e' || c == 'f');
        }

        private bool IsMaybeNumber(char c, char peek)
        {
            if (c == '#' && (peek == 'b' || peek == 'o' || peek == 'd' || peek == 'x')) return true;
            if (Char.IsDigit(Chr) || IsHexadecimal(c) || c == '.') return true;
            return false;
        }


        /// <summary>
        /// Parses a Number 
        /// <radix r> <number r> .* <number  r> / <number r> .* <number  r>
        /// </summary>
        public void ParseNumber()
        {
            TokenBegin(TokenType.Number);

            //parse consecutive values between [0-9]
            void ParseDecimal()
            {
                while (Char.IsDigit(Chr) || Chr == '_')
                    TokenAppend();

            }

            //parse consecutive values between [0-1]
            void ParseBinary()
            {
                while (Chr == '0' || Chr == '1' || Chr == '_')
                    TokenAppend();
            }

            //parse consecutive values between [0-9][A-F]
            void ParseHexadecimal()
            {
                while (IsHexadecimal(Chr) || Chr == '_')
                    TokenAppend();
            }

            //parse consecutive values between [0-7]
            void ParseOctal()
            {
                while (Chr == '_' ||
                    Chr == '0' || Chr == '1' || Chr == '2' || Chr == '3' ||
                    Chr == '4' || Chr == '5' || Chr == '6' || Chr == '7')
                    TokenAppend();

            }


            Action parsingFunc = ParseDecimal;

            //<radix r> = does the number have any kind of notation?
            if (Chr == '#')
            {
                //notations: #b binary, #o octal, #d decimal, #x hexadecimal
                if (Peek == 'b') parsingFunc = ParseBinary;
                else if (Peek == 'o') parsingFunc = ParseOctal;
                else if (Peek == 'd') parsingFunc = ParseDecimal;
                else if (Peek == 'x') parsingFunc = ParseHexadecimal;
                else
                {
                    Error("Expected a number radix");
                    return;
                }
                TokenAppend(); //consume #
                TokenAppend(); //consume radix
            }

            //scheme can parse unreal numbers
            int parsedNumbers = 0;
            while (true)
            {
                bool parsedPoint = false;
                if (Chr == '.')
                {
                    TokenAppend();
                    parsedPoint = true;
                }

                if (!IsHexadecimal(Chr))
                {
                    Error("Expected a number");
                    return;
                }

                parsingFunc();

                if (Chr == '.')
                {
                    if (!parsedPoint)
                    {
                        if (Char.IsDigit(Peek))
                        {
                            TokenAppend();
                            parsingFunc();
                        }
                        else
                        {
                            Error("Expected a number after a .");
                            return;
                        }
                    }
                    else
                    {
                        Error("Already parsed a point");
                        return;
                    }
                }


                if (parsedNumbers == 1) break;
                else if (Chr == '/')
                {
                    TokenAppend();
                    parsedNumbers++;
                }
                else break;

            }


            //if were trying to parse a number, and it doesnt work,then it should fail here
            if (!IsSeparator())
            {
                Error("Not a valid number");
                return;
            }

            TokenEnd();
        }



        public bool IsSeparator()
        {
            return (Char.IsWhiteSpace(Chr) || IsParentType());
        }

        public bool IsParentType()
        {
            return Chr == '[' || Chr == '(' || Chr == '{' ||
                Chr == ']' || Chr == ')' || Chr == '}';
        }

        public void ConsumeSpaces()
        {
            while (Char.IsWhiteSpace(Chr))
            {
                Next();
            }
            ResetRewind();
        }


    }
}
