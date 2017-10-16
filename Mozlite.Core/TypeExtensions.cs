using System;
using System.Text;
using System.Linq;
using System.Reflection;
using System.Globalization;
using System.Collections.Generic;

namespace Mozlite
{
    /// <summary>
    /// 类型扩展类。
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        /// 获取类型中名称为<paramref name="name"/>的属性信息。
        /// </summary>
        /// <param name="type">当前类型。</param>
        /// <param name="name">类型名称。</param>
        /// <returns>返回属性信息列表。</returns>
        public static IEnumerable<PropertyInfo> GetPropertiesInHierarchy(this Type type, string name)
        {
            do
            {
                var typeInfo = type.GetTypeInfo();
                var propertyInfo = typeInfo.GetDeclaredProperty(name);
                if (propertyInfo != null
                    && !(propertyInfo.GetMethod ?? propertyInfo.SetMethod).IsStatic)
                {
                    yield return propertyInfo;
                }
                type = typeInfo.BaseType;
            }
            while (type != null);
        }

        /// <summary>
        /// 获取基础类型。
        /// </summary>
        /// <param name="type">当前类型。</param>
        /// <returns>返回基础类型。</returns>
        public static Type UnwrapNullableType(this Type type) => Nullable.GetUnderlyingType(type) ?? type;

        /// <summary>
        /// 脱掉枚举类型并返回。
        /// </summary>
        /// <param name="type">当前类型。</param>
        /// <returns>返回类型实例。</returns>
        public static Type UnwrapEnumType(this Type type)
        {
            var isNullable = type.IsNullableType();
            var underlyingNonNullableType = isNullable ? type.UnwrapNullableType() : type;
            if (!underlyingNonNullableType.GetTypeInfo().IsEnum)
            {
                return type;
            }

            var underlyingEnumType = Enum.GetUnderlyingType(underlyingNonNullableType);
            return isNullable ? MakeNullable(underlyingEnumType) : underlyingEnumType;
        }

        /// <summary>
        /// 将基础类型转换为可空类型。
        /// </summary>
        /// <param name="type">类型实例。</param>
        /// <returns>返回转换后的类型。</returns>
        public static Type MakeNullable(this Type type)
            => type.IsNullableType()
                ? type
                : typeof(Nullable<>).MakeGenericType(type);

        /// <summary>
        /// 判断当前类型是否可以承载null值。
        /// </summary>
        /// <param name="type">当前类型实例。</param>
        /// <returns>返回判断结果。</returns>
        public static bool IsNullableType(this Type type)
        {
            var typeInfo = type.GetTypeInfo();

            return !typeInfo.IsValueType
                   || (typeInfo.IsGenericType
                       && (typeInfo.GetGenericTypeDefinition() == typeof(Nullable<>)));
        }

        /// <summary>
        /// 获取泛型类型中元素类型。
        /// </summary>
        /// <param name="type">当前类型。</param>
        /// <param name="interfaceOrBaseType">泛型接口或基类。</param>
        /// <returns>返回元素类型。</returns>
        public static Type TryGetElementType(this Type type, Type interfaceOrBaseType)
        {
            if (!type.GetTypeInfo().IsGenericTypeDefinition)
            {
                var types = GetGenericTypeImplementations(type, interfaceOrBaseType).ToList();

                return types.Count == 1 ? types[0].GetTypeInfo().GenericTypeArguments.FirstOrDefault() : null;
            }

            return null;
        }

        /// <summary>
        /// 获取当前泛型的实现类列表。
        /// </summary>
        /// <param name="type">当前类型。</param>
        /// <param name="interfaceOrBaseType">接口或者基类。</param>
        /// <returns>返回实现类列表。</returns>
        public static IEnumerable<Type> GetGenericTypeImplementations(this Type type, Type interfaceOrBaseType)
        {
            var typeInfo = type.GetTypeInfo();
            if (!typeInfo.IsGenericTypeDefinition)
            {
                return (interfaceOrBaseType.GetTypeInfo().IsInterface ? typeInfo.ImplementedInterfaces : type.GetBaseTypes())
                    .Union(new[] { type })
                    .Where(
                        t => t.GetTypeInfo().IsGenericType
                             && (t.GetGenericTypeDefinition() == interfaceOrBaseType));
            }

            return Enumerable.Empty<Type>();
        }

        /// <summary>
        /// 获取类型的基类列表。
        /// </summary>
        /// <param name="type">当前类型。</param>
        /// <returns>基类列表。</returns>
        public static IEnumerable<Type> GetBaseTypes(this Type type)
        {
            type = type.GetTypeInfo().BaseType;

            while (type != null)
            {
                yield return type;

                type = type.GetTypeInfo().BaseType;
            }
        }
        
        /// <summary>
        /// 显示类型名称。
        /// </summary>
        /// <param name="type">当前类型。</param>
        /// <param name="fullName">是否显示全名。</param>
        /// <returns>返回当前类型的名称。</returns>
        public static string DisplayName(this Type type, bool fullName = true)
        {
            var sb = new StringBuilder();
            ProcessTypeName(type, sb, fullName);
            return sb.ToString();
        }

        private static void AppendGenericArguments(Type[] args, int startIndex, int numberOfArgsToAppend, StringBuilder sb, bool fullName)
        {
            var totalArgs = args.Length;
            if (totalArgs >= startIndex + numberOfArgsToAppend)
            {
                sb.Append("<");
                for (var i = startIndex; i < startIndex + numberOfArgsToAppend; i++)
                {
                    ProcessTypeName(args[i], sb, fullName);
                    if (i + 1 < startIndex + numberOfArgsToAppend)
                    {
                        sb.Append(", ");
                    }
                }
                sb.Append(">");
            }
        }
        
        private static readonly Dictionary<Type, string> _builtInTypeNames = new Dictionary<Type, string>
        {
            { typeof(bool), "bool" },
            { typeof(byte), "byte" },
            { typeof(char), "char" },
            { typeof(decimal), "decimal" },
            { typeof(double), "double" },
            { typeof(float), "float" },
            { typeof(int), "int" },
            { typeof(long), "long" },
            { typeof(object), "object" },
            { typeof(sbyte), "sbyte" },
            { typeof(short), "short" },
            { typeof(string), "string" },
            { typeof(uint), "uint" },
            { typeof(ulong), "ulong" },
            { typeof(ushort), "ushort" }
        };

        private static void ProcessTypeName(Type t, StringBuilder sb, bool fullName)
        {
            if (t.GetTypeInfo().IsGenericType)
            {
                ProcessNestedGenericTypes(t, sb, fullName);
                return;
            }
            if (_builtInTypeNames.ContainsKey(t))
            {
                sb.Append(_builtInTypeNames[t]);
            }
            else
            {
                sb.Append(fullName ? t.FullName : t.Name);
            }
        }

        private static void ProcessNestedGenericTypes(Type t, StringBuilder sb, bool fullName)
        {
            var genericFullName = t.GetGenericTypeDefinition().FullName;
            var genericSimpleName = t.GetGenericTypeDefinition().Name;
            var parts = genericFullName.Split('+');
            var genericArguments = t.GetTypeInfo().GenericTypeArguments;
            var index = 0;
            var totalParts = parts.Length;
            if (totalParts == 1)
            {
                var part = parts[0];
                var num = part.IndexOf('`');
                if (num == -1)
                {
                    return;
                }

                var name = part.Substring(0, num);
                var numberOfGenericTypeArgs = int.Parse(part.Substring(num + 1), CultureInfo.InvariantCulture);
                sb.Append(fullName ? name : genericSimpleName.Substring(0, genericSimpleName.IndexOf('`')));
                AppendGenericArguments(genericArguments, index, numberOfGenericTypeArgs, sb, fullName);
                return;
            }
            for (var i = 0; i < totalParts; i++)
            {
                var part = parts[i];
                var num = part.IndexOf('`');
                if (num != -1)
                {
                    var name = part.Substring(0, num);
                    var numberOfGenericTypeArgs = int.Parse(part.Substring(num + 1), CultureInfo.InvariantCulture);
                    if (fullName || (i == totalParts - 1))
                    {
                        sb.Append(name);
                        AppendGenericArguments(genericArguments, index, numberOfGenericTypeArgs, sb, fullName);
                    }
                    if (fullName && (i != totalParts - 1))
                    {
                        sb.Append("+");
                    }
                    index += numberOfGenericTypeArgs;
                }
                else
                {
                    if (fullName || (i == totalParts - 1))
                    {
                        sb.Append(part);
                    }
                    if (fullName && (i != totalParts - 1))
                    {
                        sb.Append("+");
                    }
                }
            }
        }
    }
}