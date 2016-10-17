using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace Projektr.Tests.IntegrationTests.CodeGeneratorIntegrationTests
{
    public class Object_Containing_Collection
    {
        public class Class1
        {
            public string X { get; set; }
            public IEnumerable<Class2> Collection { get; set; }
        }

        public class Class2
        {
            public string A { get; set; }
            public string B { get; set; }
        }

        private RootNode _ast;
        private Class1 _obj;

        public Object_Containing_Collection()
        {
            _obj = new Class1
            {
                X = "Foo",
                Collection = new[]
                {
                    new Class2
                    {
                        A="Aaah",
                        B="Bhhh"
                    }
                }
            };
            var input = "X,Collection(A)";
            var tokens = new Tokenizer().Tokenize(input).ToArray();
            _ast = new Parser(tokens).Parse().Root;
        }

        [Fact]
        public void GenericCompilationTest()
        {
            var f = new CodeGenerator().Generate<Class1>(_ast);

            dynamic result = f(_obj);

            ((object)result).Should().NotBeNull();
            ((object)result.X).Should().Be(_obj.X);
            ((object)result.Collection).Should().NotBeNull();
            ((IEnumerable<object>)result.Collection).Should().HaveCount(1);
        }

        [Fact]
        public void NonGenericCompilationTest()
        {
            var f = new CodeGenerator().Generate(typeof(Class1), _ast);

            dynamic result = f(_obj);

            ((object)result).Should().NotBeNull();
            ((object)result.X).Should().Be(_obj.X);
            ((object)result.Collection).Should().NotBeNull();
            ((IEnumerable<object>)result.Collection).Should().HaveCount(1);
        }
    }
}