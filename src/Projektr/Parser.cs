using System.Collections;
using System.Collections.Generic;
using Projektr.Entities;

namespace Projektr
{
    public class Parser
    {
        private readonly IEnumerable<Token> _input;

        public Parser(IEnumerable<Token> input)
        {
            _input = input;
        }

        public ParserResult Parse()
        {
            var result = new ParserResult();
            var input = _input;
            ParserNode parentNode = result.Root;
            Parse(input.GetEnumerator(), parentNode);
            return result;
        }

        private static void Parse(IEnumerator<Token> enumerator, ParserNode parentNode)
        {
            ParserNode prevNode = null;
            while(enumerator.MoveNext())
            {
                var token = enumerator.Current;
                if (token.Type == TokenType.Text)
                {
                    var fieldNode = new FieldNode(token.Value);
                    prevNode = fieldNode;
                    parentNode.Children.Add(fieldNode);
                }
                if (token.Type == TokenType.OpenParens)
                {
                    Parse(enumerator, prevNode);
                }
                if (token.Type == TokenType.CloseParens)
                {
                    break;
                }
            }
        }
    }

    public class ParserResult
    {
        public RootNode Root { get; } = new RootNode();
    }

    public abstract class ParserNode : IEnumerable<FieldNode>
    {
        public IList<FieldNode> Children { get; } = new List<FieldNode>();
        public FieldNode this[int index] => Children[index];
        public void Add(FieldNode node) => Children.Add(node);
        public IEnumerator<FieldNode> GetEnumerator() => Children.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public class RootNode : ParserNode
    {
    }

    public class FieldNode : ParserNode
    {
        public string FieldName { get; }

        public FieldNode(string fieldName)
        {
            FieldName = fieldName;
        }
    }
}