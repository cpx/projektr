using FluentAssertions;
using Projektr.Entities;
using Xunit;

namespace Projektr.Tests.UnitTests
{
    public class ParserTests
    {
        [Fact]
        public void Should_Return_One_FieldNode()
        {
            var input = new[] { CreateTextToken("field1") };

            var parser = new Parser(input);
            var output = parser.Parse();

            output.Should().NotBeNull();
            output.Root.Should().NotBeNull();
            output.Root.Should().NotBeNull();
            var ast = output.Root;
            ast.Should().HaveCount(1);
            ast[0].FieldName.Should().Be("field1");
            ast[0].Should().NotBeNull();
            ast[0].Should().BeEmpty();
        }

        [Fact]
        public void Should_Return_One_Renamed_FieldNode()
        {
            var input = new[] { CreateTextToken("field1"), CreateRenameToken(), CreateTextToken("newfield1") };

            var parser = new Parser(input);
            var output = parser.Parse();

            output.Should().NotBeNull();
            output.Root.Should().NotBeNull();
            output.Root.Should().NotBeNull();
            var ast = output.Root;
            ast.Should().HaveCount(1);
            ast[0].FieldName.Should().Be("field1");
            ast[0].IsRenamed.Should().BeTrue();
            ast[0].NewName.Should().Be("newfield1");
            ast[0].Should().NotBeNull();
            ast[0].Should().BeEmpty();
        }

        [Fact]
        public void Should_Return_Two_FieldNodes()
        {
            var input = new[]
            {
                CreateTextToken("field1"),
                CreateSeparatorToken(),
                CreateTextToken("field2"),
            };

            var parser = new Parser(input);
            var output = parser.Parse();

            output.Should().NotBeNull();
            output.Root.Should().NotBeNull();
            output.Root.Should().NotBeNull();
            var ast = output.Root;
            ast.Should().HaveCount(2);
            ast[0].FieldName.Should().Be("field1");
            ast[0].Should().NotBeNull();
            ast[0].Should().BeEmpty();
            ast[1].FieldName.Should().Be("field2");
            ast[1].Should().NotBeNull();
            ast[1].Should().BeEmpty();
        }

        [Fact]
        public void Should_Return_One_FieldNode_With_One_Level_Of_Children()
        {
            var input = new[]
            {
                CreateTextToken("field1"),
                CreateOpenParensToken(),
                CreateTextToken("subfield1"),
                CreateCloseParensToken(),
            };

            var parser = new Parser(input);
            var output = parser.Parse();

            output.Should().NotBeNull();
            output.Root.Should().NotBeNull();
            output.Root.Should().NotBeNull();
            var ast = output.Root;
            ast.Should().HaveCount(1);
            ast[0].FieldName.Should().Be("field1");
            ast[0].Should().NotBeNull();
            ast[0].Should().HaveCount(1);
            ast[0][0].FieldName.Should().Be("subfield1");
        }

        [Fact]
        public void Should_Return_One_FieldNode_With_Two_Levels_Of_Children()
        {
            var input = new[]
            {
                CreateTextToken("field1"),
                CreateOpenParensToken(),
                CreateTextToken("subfield1"),
                CreateOpenParensToken(),
                CreateTextToken("subsubfield1"),
                CreateCloseParensToken(),
                CreateCloseParensToken(),
            };

            var parser = new Parser(input);
            var output = parser.Parse();

            output.Should().NotBeNull();
            output.Root.Should().NotBeNull();
            output.Root.Should().NotBeNull();
            var ast = output.Root;
            ast.Should().HaveCount(1);
            ast[0].FieldName.Should().Be("field1");
            ast[0].Should().NotBeNull();
            ast[0].Should().HaveCount(1);
            ast[0][0].FieldName.Should().Be("subfield1");
            ast[0][0].Should().NotBeNull();
            ast[0][0].Should().HaveCount(1);
            ast[0][0][0].FieldName.Should().Be("subsubfield1");
        }

        [Fact]
        public void Should_Return_Two_FieldNodes_With_One_Level_Of_Children()
        {
            var input = new[]
            {
                CreateTextToken("field1"),
                CreateOpenParensToken(),
                CreateTextToken("subfield1"),
                CreateCloseParensToken(),
                CreateSeparatorToken(),
                CreateTextToken("field2"),
                CreateOpenParensToken(),
                CreateTextToken("subfield2"),
                CreateCloseParensToken(),
            };

            var parser = new Parser(input);
            var output = parser.Parse();

            output.Should().NotBeNull();
            output.Root.Should().NotBeNull();
            output.Root.Should().NotBeNull();
            var ast = output.Root;
            ast.Should().HaveCount(2);
            ast[0].FieldName.Should().Be("field1");
            ast[0].Should().NotBeNull();
            ast[0].Should().HaveCount(1);
            ast[0][0].FieldName.Should().Be("subfield1");
            ast[0][0].Should().NotBeNull();
            ast[0][0].Should().BeEmpty();
            ast[1].FieldName.Should().Be("field2");
            ast[1].Should().NotBeNull();
            ast[1].Should().HaveCount(1);
            ast[1][0].FieldName.Should().Be("subfield2");
            ast[1][0].Should().NotBeNull();
            ast[1][0].Should().BeEmpty();
        }

        private Token CreateOpenParensToken()
        {
            return new Token(TokenType.OpenParens);
        }

        private Token CreateCloseParensToken()
        {
            return new Token(TokenType.CloseParens);
        }

        private Token CreateRenameToken()
        {
            return new Token(TokenType.RenameSeparator);
        }

        private Token CreateSeparatorToken()
        {
            return new Token(TokenType.FieldSeparator);
        }

        private static Token CreateTextToken(string text)
        {
            var token = new Token(TokenType.Text);
            foreach (var c in text)
            {
                token.Append(c);
            }
            return token;
        }
    }
}
