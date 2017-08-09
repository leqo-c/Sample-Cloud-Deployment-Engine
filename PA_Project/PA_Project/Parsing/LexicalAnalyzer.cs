using System.Collections.Generic;
using System.Text;

namespace PA_Project.Parsing
{
    public class LexicalAnalyzer
    {
        public enum Token { Indent, Dedent, Colon, String, SquareOpen, SquareClose, Comma, Dash, Eof }
        private readonly CharacterStream _stream;
        private Token _currentToken;
        private string _tokenVal;
        private readonly List<char?> _specialChars = new List<char?> {'>','<',':','[',']',',','-'};

        public string TokenStringVal
        {
            get { return _currentToken == Token.String ? _tokenVal : null; }
            set { _tokenVal = value; }
        }

        public LexicalAnalyzer(CharacterStream strm)
        {
            _stream = strm;
        }

        public void DiscardWhiteSpaces()
        {
            while (_stream.Peek() != null && _stream.Peek() == ' ') _stream.Consume();
        }

        public Token GetNextToken()
        {
            Token result;
            DiscardWhiteSpaces();
            switch (_stream.Peek())
            {
                case '>': result = Token.Indent; break;
                case '<': result = Token.Dedent; break;
                case ':': result = Token.Colon; break;
                case '[': result = Token.SquareOpen; break;
                case ']': result = Token.SquareClose; break;
                case ',': result = Token.Comma; break;
                case '-': result = Token.Dash; break;
                case null: result = Token.Eof; break;
                default:
                    ParseString();
                    result = Token.String; 
                    return _currentToken = result;
                    
            }
            _stream.Consume();
            return _currentToken = result;
        }

        private void ParseString()
        {
            DiscardWhiteSpaces();
            var temp = new StringBuilder();
            var currentChar = _stream.Peek();
            while (currentChar != null && !_specialChars.Contains(currentChar))
            {
                temp.Append(currentChar);
                _stream.Consume();
                currentChar = _stream.Peek();
            }
            TokenStringVal = temp.ToString();
        }
    }
}