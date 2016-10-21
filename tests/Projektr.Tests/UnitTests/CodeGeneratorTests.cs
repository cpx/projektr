using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace Projektr.Tests.UnitTests
{
    public class CodeGeneratorTests
    {
        public class Class1
        {
            public string X { get; set; }
            public string Y { get; set; }
            public Class2 Z { get; set; }
        }

        public class Class2
        {
            public string A { get; set; }
            public Class3 B { get; set; }
        }

        public class Class3
        {
            public string S { get; set; }
            public string T { get; set; }
        }

        public class ClassWithCollection1
        {
            public IEnumerable<Class3> Collection { get; set; }
        }

        [Fact]
        public void Generated_Method_Should_Copy_One_Property()
        {
            var ast = new RootNode
            {
                new FieldNode("X"),
            };
            var test = new Class1 { X = "Foo" };

            IDictionary<string, object> result = new CodeGenerator().Generate<Class1>(ast)(test);

            result.Should().Contain("X", test.X);
            result.Should().NotContainKey("Y");
        }

        [Fact]
        public void Generated_Method_Should_Copy_One_Property_And_Rename_It()
        {
            var ast = new RootNode
            {
                new FieldNode("X") {IsRenamed = true, NewName = "X1"},
            };
            var test = new Class1 { X = "Foo" };

            IDictionary<string, object> result = new CodeGenerator().Generate<Class1>(ast)(test);

            result.Should().Contain("X1", test.X);
            result.Should().NotContainKey("X");
        }

        [Fact]
        public void Generated_Method_Should_Copy_Two_Properties()
        {
            var ast = new RootNode
            {
                new FieldNode("X"),
                new FieldNode("Y"),
            };
            var test = new Class1 { X = "Foo", Y = "Bar" };

            IDictionary<string, object> result = new CodeGenerator().Generate<Class1>(ast)(test);

            result.Should().Contain("X", test.X);
            result.Should().Contain("Y", test.Y);
        }

        [Fact]
        public void Generated_Method_Should_Copy_All_Properties()
        {
            var ast = new RootNode();
            var test = new Class1 { X = "Foo", Y = "Bar" };

            IDictionary<string, object> result = new CodeGenerator().Generate<Class1>(ast)(test);

            result.Should().Contain("X", test.X);
            result.Should().Contain("Y", test.Y);
            result.Should().Contain("Z", test.Z);
        }

        [Fact]
        public void Generated_Method_Should_Copy_Two_Levels_Of_Properties()
        {
            var ast = new RootNode
            {
                new FieldNode("X"),
                new FieldNode("Y"),
                new FieldNode("Z")
                {
                    new FieldNode("A"),
                },
            };
            var test = new Class1 { X = "Foo", Y = "Bar", Z = new Class2 { A = "Baz" } };

            IDictionary<string, object> result = new CodeGenerator().Generate<Class1>(ast)(test);

            result.Should().Contain("X", test.X);
            result.Should().Contain("Y", test.Y);
            result.Should().ContainKey("Z");
            ((IDictionary<string, object>)result["Z"]).Should().NotBeNull();
            ((IDictionary<string, object>)result["Z"]).Should().Contain("A", "Baz");
        }

        [Fact]
        public void Generated_Method_Should_Copy_Three_Levels_Of_Properties()
        {
            var ast = new RootNode
            {
                new FieldNode("X"),
                new FieldNode("Z")
                {
                    new FieldNode("A"),
                    new FieldNode("B")
                    {
                        new FieldNode("S")
                    }
                },
            };
            var test = new Class1 {X = "Foo", Y = "Bar", Z = new Class2 {A = "Baz", B = new Class3 {S = "Boo"}}};

            IDictionary<string, object> result = new CodeGenerator().Generate<Class1>(ast)(test);

            result.Should().Contain("X", test.X);
            result.Should().ContainKey("Z");
            var z = (IDictionary<string, object>)result["Z"];
            z.Should().NotBeNull();
            z.Should().Contain("A", test.Z.A);
            z.Should().ContainKey("B");
            var b = (IDictionary<string, object>) z["B"];
            b.Should().Contain("S", test.Z.B.S);
        }

        [Fact]
        public void Generated_Method_Should_Copy_Child_Collection_Node()
        {
            var ast = new RootNode
            {
                new FieldNode("Collection")
                {
                    new FieldNode("S")
                }
            };
            var test = new ClassWithCollection1 { Collection = new[] { new Class3 { S = "Foo", T = "Bar" } } };

            IDictionary<string, object> result = new CodeGenerator().Generate<ClassWithCollection1>(ast)(test);

            result.Should().ContainKey("Collection");
            result["Collection"].Should().BeAssignableTo<IEnumerable<ExpandoObject>>();
            var c = ((IEnumerable<IDictionary<string, object>>)result["Collection"]).ToList();
            c.Should().HaveCount(1);
            c[0].Should().Contain("S", "Foo");
        }

        [Fact]
        public void Generated_Method_Should_Copy_Child_Nodes_Of_Child_Collection_Node()
        {
            var ast = new RootNode
            {
                new FieldNode("S")
            };
            var test = new[] {new Class3 {S = "Foo"}};

            var result = new CodeGenerator().GenerateForEnumerable<Class3>(ast)(test).ToList();

            result.Should().HaveCount(1);
            result[0].Should().ContainKey("S", test[0].S);
        }
    }
}
