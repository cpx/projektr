using System;
using System.Collections.Generic;
using System.Linq;

namespace Projektr.Helpers
{
    internal static class TypeExtensions
    {
        internal static bool IsEnumerable(this Type type, out Type enumerableItemType)
        {
            var enumerableInterface = GetEnumerableInterface(type);
            if (enumerableInterface == null)
            {
                enumerableItemType = null;
                return false;
            }
            enumerableItemType = enumerableInterface.GetGenericArguments()[0];
            return true;
        }

        private static Type GetEnumerableInterface(this Type propertyType)
        {
            Func<Type, bool> predicate = t =>
                t != typeof(string)
                && t.IsGenericType
                && t.GetGenericTypeDefinition() == typeof(IEnumerable<>);
            Type enumerableInterface = null;
            if (predicate(propertyType))
            {
                enumerableInterface = propertyType;
            }
            else
            {
                var interfaces = propertyType.GetInterfaces();
                enumerableInterface = interfaces
                    .FirstOrDefault(i =>
                        i != typeof(string)
                        && i.IsGenericType
                        && i.GetGenericTypeDefinition() == typeof(IEnumerable<>));
            }
            return enumerableInterface;
        }
    }
}