﻿using System;
using System.Linq.Expressions;
using System.Reflection;
using Mozlite.Data.Query.Expressions;
using Mozlite.Data.Query.Translators;

namespace Mozlite.Data.MySql.Query.Translators
{
    /// <summary>
    /// string.TrimStart。
    /// </summary>
    public class StringTrimStartTranslator : IMethodCallTranslator
    {
        // Method defined in netcoreapp2.0 only
        private static readonly MethodInfo _methodInfoWithoutArgs
            = typeof(string).GetRuntimeMethod(nameof(string.TrimStart), new Type[] { });

        // Method defined in netstandard2.0
        private static readonly MethodInfo _methodInfoWithCharArrayArg
            = typeof(string).GetRuntimeMethod(nameof(string.TrimStart), new[] { typeof(char[]) });

        public virtual Expression Translate(MethodCallExpression methodCallExpression)
        {
            if (_methodInfoWithoutArgs?.Equals(methodCallExpression.Method) == true
                || _methodInfoWithCharArrayArg.Equals(methodCallExpression.Method)
                // MySql LTRIM does not take arguments
                && ((methodCallExpression.Arguments[0] as ConstantExpression)?.Value as Array)?.Length == 0)
            {
                var sqlArguments = new[] { methodCallExpression.Object };

                return new SqlFunctionExpression("LTRIM", methodCallExpression.Type, sqlArguments);
            }

            return null;
        }
    }
}