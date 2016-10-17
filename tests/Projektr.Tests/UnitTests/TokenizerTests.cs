using System.Linq;
using FluentAssertions;
using Projektr.Entities;
using Xunit;

namespace Projektr.Tests.UnitTests
{
    public class TokenizerTests
    {
        [Fact()]
        public void Test_Single_Field()
        {
            var input = "field1";

            var tokens = new Tokenizer().Tokenize(input).ToList();

            tokens.Should().HaveCount(1);
            tokens[0].Type.Should().Be(TokenType.Text);
            tokens[0].Value.Should().Be("field1");
        }

        [Fact]
        public void Test_Two_Fields()
        {
            var input = "field1,field2";

            var tokens = new Tokenizer().Tokenize(input).ToList();

            tokens.Should().HaveCount(3);
            tokens[0].Type.Should().Be(TokenType.Text);
            tokens[0].Value.Should().Be("field1");
            tokens[1].Type.Should().Be(TokenType.Separator);
            tokens[2].Type.Should().Be(TokenType.Text);
            tokens[2].Value.Should().Be("field2");
        }

        [Fact]
        public void Test_Field_With_Subfield()
        {
            var input = "field1(subfield1)";

            var tokens = new Tokenizer().Tokenize(input).ToList();

            tokens.Should().HaveCount(4);
            tokens[0].Type.Should().Be(TokenType.Text);
            tokens[0].Value.Should().Be("field1");
            tokens[1].Type.Should().Be(TokenType.OpenParens);
            tokens[2].Type.Should().Be(TokenType.Text);
            tokens[2].Value.Should().Be("subfield1");
            tokens[3].Type.Should().Be(TokenType.CloseParens);
        }

        [Fact]
        public void Test_Two_Fields_Each_With_Two_Subfields()
        {
            var input = "field1(subfield1),field2(subfield2)";

            var tokens = new Tokenizer().Tokenize(input).ToList();

            tokens.Should().HaveCount(9);
            tokens[0].Type.Should().Be(TokenType.Text);
            tokens[0].Value.Should().Be("field1");
            tokens[1].Type.Should().Be(TokenType.OpenParens);
            tokens[2].Type.Should().Be(TokenType.Text);
            tokens[2].Value.Should().Be("subfield1");
            tokens[3].Type.Should().Be(TokenType.CloseParens);
            tokens[4].Type.Should().Be(TokenType.Separator);
            tokens[5].Type.Should().Be(TokenType.Text);
            tokens[5].Value.Should().Be("field2");
            tokens[6].Type.Should().Be(TokenType.OpenParens);
            tokens[7].Type.Should().Be(TokenType.Text);
            tokens[7].Value.Should().Be("subfield2");
            tokens[8].Type.Should().Be(TokenType.CloseParens);
        }
    }
}
