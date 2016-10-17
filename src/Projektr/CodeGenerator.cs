using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Projektr.Helpers;

namespace Projektr
{
    public class CodeGenerator
    {
        private readonly List<ParameterExpression> _variableDeclarations = new List<ParameterExpression>();
        private readonly List<BinaryExpression> _variableAssignments = new List<BinaryExpression>();
        private readonly List<MethodCallExpression> _addPropertyCalls = new List<MethodCallExpression>();

        public Func<T, ExpandoObject> Generate<T>(ParserNode ast)
        {
            var type = typeof(T);
            var f = GenerateDelegate(type, ast);
            return (Func<T, ExpandoObject>)f;
        }

        public Func<object, ExpandoObject> Generate(Type type, ParserNode ast)
        {
            var f = GenerateDelegate(type, ast);
            return (Func<object, ExpandoObject>) f;
        }

        public Func<IEnumerable<T>, IEnumerable<ExpandoObject>> GenerateForEnumerable<T>(ParserNode ast)
        {
            var enumerableItemType = typeof(T);
            var inputParameter = Expression.Parameter(typeof(IEnumerable<T>), "x");
            
            var valueFunc = new CodeGenerator().Generate<T>(ast);
            var valueFuncConstant = Expression.Constant(valueFunc);
            var mapCollectionMethod = GetType().GetMethod("MapCollection", BindingFlags.Static | BindingFlags.NonPublic);
            var mapCollectionMethodGeneric = mapCollectionMethod.MakeGenericMethod(enumerableItemType);
            var valueExpression = Expression.Call(null, mapCollectionMethodGeneric, inputParameter, valueFuncConstant);
            
            // create return statement
            var returnTarget = Expression.Label(typeof(IEnumerable<ExpandoObject>));
            var returnStatement = Expression.Return(returnTarget, valueExpression, typeof(IEnumerable<ExpandoObject>));
            var returnLabel = Expression.Label(returnTarget, returnStatement);

            var expressions = new List<Expression>();
            expressions.AddRange(_variableAssignments);
            expressions.AddRange(_addPropertyCalls);
            expressions.Add(returnStatement);
            expressions.Add(returnLabel);
            var block = Expression.Block(
                _variableDeclarations,
                expressions);
            var lambda = Expression.Lambda(block, false, inputParameter);
            var f = lambda.Compile();
            return (Func<IEnumerable<T>, IEnumerable<ExpandoObject>>) f;
        }

        private Delegate GenerateDelegate(Type type, ParserNode ast)
        {
            var inputParameter = Expression.Parameter(typeof(object), "x");
            var inputVariable = Expression.Variable(type, "xcast");
            var inputParameterCast = Expression.Convert(inputParameter, type);
            var inputVariableAssignment = Expression.Assign(inputVariable, inputParameterCast);
            _variableAssignments.Add(inputVariableAssignment);
            _variableDeclarations.Add(inputVariable);

            var varDict = CreateExpression(ast, type, inputVariable);

            // create return statement
            var returnTarget = Expression.Label(typeof(ExpandoObject));
            var returnStatement = Expression.Return(returnTarget, varDict, typeof(ExpandoObject));
            var returnLabel = Expression.Label(returnTarget, returnStatement);

            var expressions = new List<Expression>();
            expressions.AddRange(_variableAssignments);
            expressions.AddRange(_addPropertyCalls);
            expressions.Add(returnStatement);
            expressions.Add(returnLabel);
            var block = Expression.Block(
                _variableDeclarations,
                expressions);
            var lambda = Expression.Lambda(block, false, inputParameter);
            var f = lambda.Compile();
            return f;
        }

        private ParameterExpression CreateExpression(ParserNode ast, Type type, Expression inputParameter)
        {
            // create and assign ExpandoObject
            var newExpando = Expression.New(typeof(ExpandoObject));
            var varExpando = Expression.Variable(typeof(ExpandoObject), "d");
            _variableDeclarations.Add(varExpando);
            var assignDict = Expression.Assign(varExpando, newExpando);
            _variableAssignments.Add(assignDict);

            // create ExpandoObject Add calls
            var properties = ast.Any()
                ? ast.Select(n => new { PropertyInfo = type.GetProperty(n.FieldName), Node = n })
                : type.GetProperties().Select(pi => new { PropertyInfo = pi, Node = (FieldNode)null });
            foreach (var property in properties)
            {
                var propertyExpression = Expression.Property(inputParameter, type.GetProperty(property.PropertyInfo.Name));
                Expression valueExpression;
                if (property.Node == null || !property.Node.Any())
                {
                    valueExpression = propertyExpression;
                }
                else
                {
                    Type enumerableGenericType;
                    if (property.PropertyInfo.PropertyType.IsEnumerable(out enumerableGenericType))
                    {
                        var valueFunc = new CodeGenerator().Generate(enumerableGenericType, property.Node);
                        var valueFuncConstant = Expression.Constant(valueFunc);
                        var mapCollectionMethod = GetType().GetMethod("MapCollection", BindingFlags.Static|BindingFlags.NonPublic);
                        var mapCollectionMethodGeneric = mapCollectionMethod.MakeGenericMethod(enumerableGenericType);
                        valueExpression = Expression.Call(null, mapCollectionMethodGeneric, propertyExpression, valueFuncConstant);
                    }
                    else
                    {
                        var valueFunc = new CodeGenerator().Generate(property.PropertyInfo.PropertyType, property.Node);
                        var valueFuncInvoke = valueFunc.GetType().GetMethod("Invoke");
                        var valueFuncConstant = Expression.Constant(valueFunc);
                        valueExpression = Expression.Call(valueFuncConstant, valueFuncInvoke, propertyExpression);
                    }
                }
                if (valueExpression.Type.IsValueType)
                {
                    var castExpression = Expression.Convert(valueExpression, typeof(object));
                    valueExpression = castExpression;
                }
                var addCall = Expression.Call(
                    varExpando,
                    typeof(IDictionary<string, object>).GetMethod("Add"),
                    new[]
                    {
                        Expression.Constant(property.PropertyInfo.Name),
                        valueExpression
                    });
                _addPropertyCalls.Add(addCall);
            }
            return varExpando;
        }

        // ReSharper disable once UnusedMember.Local
        private static IEnumerable<ExpandoObject> MapCollection<T>(IEnumerable<T> enumerable, Func<T, ExpandoObject> valueFunc)
        {
            return enumerable.Select(valueFunc).ToList();
        }
    }
}
