using FluentAssertions;
using Xunit;

namespace Projektr.Tests.IntegrationTests.CodeGeneratorIntegrationTests
{
    public class Multiple_Fields_Multiple_Levels
    {
        public class Class1
        {
            public string Q { get; set; }
            public Class2 W { get; set; }
            public string I { get; set; }
            public Class4 O { get; set; }
        }

        public class Class2
        {
            public string E { get; set; }
            public Class3 R { get; set; }
            public string U { get; set; }
        }

        public class Class3
        {
            public string T { get; set; }
            public string Y { get; set; }
        }

        public class Class4
        {
            public string P { get; set; }
        }

        [Fact]
        public void Test()
        {
            var obj = new Class1
            {
                Q = "Baz",
                W = new Class2
                {
                    E = "Foobar",
                    R = new Class3
                    {
                        T = "Barfoo",
                        Y = "Fazbaz"
                    },
                    U = "Bazfaz"
                },
                I = "Foo",
                O = new Class4
                {
                    P = "Bar"
                },
            };
            var input = "Q,W(E,R(T,Y),U),I,O(P)";
            var tokens = new Tokenizer().Tokenize(input);
            var ast = new Parser(tokens).Parse().Root;
            var f = new CodeGenerator().Generate<Class1>(ast);

            dynamic result = f(obj);

            ((object) result).Should().NotBeNull();
            ((object) result.Q).Should().Be(obj.Q);
            ((object) result.W).Should().NotBeNull();
            ((object) result.W.E).Should().Be(obj.W.E);
            ((object) result.W.R).Should().NotBeNull();
            ((object) result.W.R.T).Should().Be(obj.W.R.T);
            ((object) result.W.R.Y).Should().Be(obj.W.R.Y);
            ((object) result.W.U).Should().Be(obj.W.U);
            ((object) result.I).Should().Be(obj.I);
            ((object) result.O).Should().NotBeNull();
            ((object) result.O.P).Should().Be(obj.O.P);
        }
    }
}