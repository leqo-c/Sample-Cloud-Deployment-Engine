using System;

namespace PA_Project.Parsing {
    public class CharacterStream {
        private readonly string _input;
        private int _position = 0;
        public CharacterStream(string stream) { _input = stream; }
        
        public char? Peek() {
            return _position < _input.Length ? (char?) _input[_position] : null;
        }
        
        public void Consume() { _position++; }
    }
}