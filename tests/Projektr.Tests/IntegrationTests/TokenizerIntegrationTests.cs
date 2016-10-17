using FluentAssertions;
using Xunit;

namespace Projektr.Tests.IntegrationTests
{
    public class TokenizerIntegrationTests
    {
        [Fact]
        public void Multiple_Fields_Multiple_Levels()
        {
            var input = "q,w(e,r(t,y),u),i,o(p)";
            var tokens = new Tokenizer().Tokenize(input);
            var ast = new Parser(tokens).Parse().Root;

            // level 1
            ast.Should().HaveCount(4);
            // level 2
            ast[0].Should().HaveCount(0);
            ast[0].FieldName.Should().Be("q");
            ast[1].Should().HaveCount(3);
            ast[1].FieldName.Should().Be("w");
            ast[2].Should().HaveCount(0);
            ast[2].FieldName.Should().Be("i");
            ast[3].Should().HaveCount(1);
            ast[3].FieldName.Should().Be("o");
            // level 3
            ast[1][0].FieldName.Should().Be("e");
            ast[1][1].Should().HaveCount(2);
            ast[1][1].FieldName.Should().Be("r");
            ast[1][2].FieldName.Should().Be("u");
            ast[3][0].FieldName.Should().Be("p");
            // level 4
            ast[1][1][0].FieldName.Should().Be("t");
            ast[1][1][1].FieldName.Should().Be("y");
        }
    }
}
