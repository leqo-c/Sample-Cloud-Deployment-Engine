using System;
using System.Collections.Generic;
using System.Linq;
using PA_Project.Constructs;
using static PA_Project.Parsing.LexicalAnalyzer;

namespace PA_Project.Parsing {
    public class Parser {
        private LexicalAnalyzer _lexer;
        private Token _token;
        private Specification _specification = new Specification();

        public Specification Parse(CharacterStream characterStream) {
            _lexer = new LexicalAnalyzer(characterStream);
            _token = _lexer.GetNextToken();
            Spec();
            return _specification;
        }

        // Spec ::= Srv Agnt Rel
        private void Spec() {
            _specification.Services = Srv();
            _specification.Agents = Agnt();
            _specification.Relations = Rel();
        }

        //Rel ::= epsilon | relations: > - Elem (>< - Elem)* <
        private List<Relation> Rel() {
            var res = new List<Relation>();
            if (_lexer.TokenStringVal != "relations") return res;
            MatchExactStringValue("relations");
            MatchToken(Token.Colon, Token.Indent, Token.Dash);
            res.Add(Elem());
            while (_token == Token.Indent) {
                MatchToken(Token.Indent, Token.Dedent, Token.Dash);
                res.Add(Elem());
            }
            MatchToken(Token.Dedent);
            return res;
        }

        private Relation Elem() {
            var res = new Relation();
            MatchToken(Token.SquareOpen);
            res.ServiceRequesting = ExtractStringVal("Expected a string");
            MatchToken(Token.String, Token.Colon);
            res.Condition = res.ServiceRequesting + "_" + ExtractStringVal("Expected a string");
            MatchToken(Token.String, Token.Comma);
            res.ServiceProviding = ExtractStringVal("Expected a string");
            MatchToken(Token.String, Token.Colon);
            res.Event = res.ServiceProviding + "_" + ExtractStringVal("Expected a string");
            MatchToken(Token.String, Token.SquareClose);
            return res;
        }

        private List<Agent> Agnt() {
            var res = new List<Agent>();
            MatchExactStringValue("agents");
            MatchToken(Token.Colon, Token.Indent);
            while (_token == Token.String) {
                var agentName = _lexer.TokenStringVal;
                MatchToken(Token.String, Token.Colon, Token.Indent);
                MatchExactStringValue("class");
                MatchToken(Token.Colon);
                var implClass = ExtractStringVal("Expected class name");
                var a = new Agent(implClass, agentName);
                MatchToken(Token.String, Token.Indent, Token.Dedent);
                MatchExactStringValue("events");
                MatchToken(Token.Colon, Token.Indent);
                // (Evnt)*
                while (_token == Token.String) {
                    var eventName = _lexer.TokenStringVal;
                    MatchToken(Token.String, Token.Colon, Token.Indent);
                    var eventDetails = ExtractStringVal("Expected \"handler\" or \"after\"");
                    var evnt = new MyEvent {Name = eventName};
                    if (eventDetails == "after") {
                        MatchToken(Token.String, Token.Colon, Token.SquareOpen);
                        evnt.After.AddRange(from x in StrList() select a.Name + "_" + x);
                        MatchToken(Token.SquareClose, Token.Indent, Token.Dedent);
                        eventDetails = ExtractStringVal("Expected \"handler\"");
                    }
                    if (eventDetails == "handler") {
                        MatchToken(Token.String, Token.Colon);
                        evnt.Handler = ExtractStringVal("Expected handler name");
                        MatchToken(Token.String, Token.Dedent);
                    }
                    else throw new Exception("Event handler is missing");
                    a.Events.Add(evnt);
                } //Fuori da (Evnt)*
                // Agnt ::= agents: > ( string: > class: string >< events: > (Evnt)* < Requirements Provisions )* <
                MatchToken(Token.Dedent);
                Requirements(a);
                Provisions(a);
                MatchToken(Token.Dedent);
                res.Add(a);
            }
            MatchToken(Token.Dedent);
            return res;
        }

        //Provisions ::= epsilon | provides: [StrList] | >< provides: [StrList] 
        private void Provisions(Agent agent) {
            if (_token != Token.Indent && _token != Token.String) return;
            if (_token == Token.Indent)
                MatchToken(Token.Indent, Token.Dedent);

            MatchExactStringValue("provides");
            MatchToken(Token.Colon, Token.SquareOpen);
            agent.Provides.AddRange(StrList());
            _specification.ProvidedServices.AddRange(agent.Provides);
            MatchToken(Token.SquareClose);
        }

        private void Requirements(Agent agent) {
            if (_lexer.TokenStringVal != "requires") return;
            MatchExactStringValue("requires");
            MatchToken(Token.Colon, Token.SquareOpen);
            agent.Requires.AddRange(StrList());
            MatchToken(Token.SquareClose);
        }

        private List<string> StrList() {
            var res = new List<string> {ExtractStringVal("Expected a list of strings")};
            MatchToken(Token.String);
            while (_token == Token.Comma) {
                MatchToken(Token.Comma);
                res.Add(ExtractStringVal("Expected a list of strings"));
                MatchToken(Token.String);
            }
            return res;
        }

        public string ExtractStringVal(string errorMsg)
        {
            if (_token == Token.String) return _lexer.TokenStringVal;
            else throw new Exception(errorMsg);
        }

        // Srv ::= services: > (string: > units: int <)* <
        private Services Srv() {
            var res = new Services();
            MatchExactStringValue("services");
            MatchToken(Token.Colon, Token.Indent);
            while (_token == Token.String) {
                var unitName = _lexer.TokenStringVal;
                MatchToken(Token.String, Token.Colon, Token.Indent);
                MatchExactStringValue("units");
                MatchToken(Token.Colon);
                var numberOfUnits = MatchIntValue();
                MatchToken(Token.Dedent);
                res.Units.Add(unitName, numberOfUnits);
            }
            MatchToken(Token.Dedent);
            return res;
        }

        private int MatchIntValue() {
            int res;
            var ok = int.TryParse(_lexer.TokenStringVal, out res);
            if (!ok) throw new Exception("Expected integer value");
            _token = _lexer.GetNextToken();
            return res;
        }

        private void MatchExactStringValue(string s) {
            if (_token != Token.String || _lexer.TokenStringVal != s)
                throw new Exception("Expected string token of value: " + s);
            _token = _lexer.GetNextToken();
        }

        private void MatchToken(params Token[] t) {
            foreach (var tkn in t) {
                if (_token != tkn) throw new Exception("Expected " + t);
                _token = _lexer.GetNextToken();
            }
        }
    }
}