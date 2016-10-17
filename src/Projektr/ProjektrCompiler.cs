using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Dynamic;

namespace Projektr
{
    public static class ProjektrCompiler
    {
        private static readonly ConcurrentDictionary<string, object> Cache = new ConcurrentDictionary<string, object>();

        public static Func<object, ExpandoObject> Compile(Type type, string filter)
        {
            return (Func<object, ExpandoObject>)Cache.GetOrAdd($"Compile_{GetFilterKey(type, filter)}", (s) =>
            {
                var ast = CreateAST(filter);
                return new CodeGenerator().Generate(type, ast);
            });
        }

        public static Func<T, ExpandoObject> Compile<T>(string filter)
        {
            return (Func<T, ExpandoObject>) Cache.GetOrAdd($"Compile_{GetFilterKey(typeof(T), filter)}", (s) =>
            {
                var ast = CreateAST(filter);
                return new CodeGenerator().Generate<T>(ast);
            });
        }

        public static Func<IEnumerable<T>, IEnumerable<ExpandoObject>> CompileEnumerable<T>(string filter)
        {
            return (Func<IEnumerable<T>, IEnumerable<ExpandoObject>>) Cache.GetOrAdd($"CompileEnumerable_{GetFilterKey(typeof(T), filter)}", (s) =>
            {
                var ast = CreateAST(filter);
                return new CodeGenerator().GenerateForEnumerable<T>(ast);
            });
        }

        private static string GetFilterKey(Type type, string filter) => $"Compile_{type.AssemblyQualifiedName}_{filter}";

        private static RootNode CreateAST(string filter)
        {
            var tokens = new Tokenizer().Tokenize(filter);
            var ast = new Parser(tokens).Parse().Root;
            return ast;
        }
    }
}
