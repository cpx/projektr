using System.Text;

namespace Projektr.Entities
{
    public class Token
    {
        private readonly StringBuilder _valueBuilder;

        public TokenType Type { get; set; }
        public string Value { get { return _valueBuilder.ToString(); } }

        public Token(TokenType tokenType)
        {
            Type = tokenType;
            _valueBuilder = new StringBuilder();
        }

        public Token(TokenType tokenType, char c)
            : this(tokenType)
        {
            Append(c);
        }

        public void Append(char c)
        {
            _valueBuilder.Append(c);
        }
    }
}