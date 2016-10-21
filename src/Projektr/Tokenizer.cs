using System.Collections.Generic;
using Projektr.Entities;

namespace Projektr
{
    public class Tokenizer
    {
        public ICollection<Token> Tokenize(string input)
        {
            var tokens = new List<Token>();
            Token token = null;
            foreach (var c in input)
            {
                var tokenType = Classify(c);
                if (tokenType == TokenType.Text)
                {
                    if (token?.Type == TokenType.Text)
                    {
                        token.Append(c);
                    }
                    else
                    {
                        token = new Token(tokenType, c);
                        tokens.Add(token);
                    }
                }
                else
                {
                    token = new Token(tokenType);
                    tokens.Add(token);
                }
            }
            return tokens;
        }

        private TokenType Classify(char c)
        {
            switch (c)
            {
                case '(':
                    return TokenType.OpenParens;
                case ')':
                    return TokenType.CloseParens;
                case ',':
                    return TokenType.FieldSeparator;
                case ':':
                    return TokenType.RenameSeparator;
                default:
                    return TokenType.Text;
            }
        }
    }
}