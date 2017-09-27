using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Mozlite.Data.Query.Expressions;

namespace Mozlite.Data.Query.Translators
{
    /// <summary>
    /// 无参数的方法调用转换器基类。
    /// </summary>
    public abstract class ParameterlessInstanceMethodCallTranslator : IMethodCallTranslator
    {
        private readonly Type _declaringType;
        private readonly string _clrMethodName;
        private readonly string _sqlFunctionName;
        /// <summary>
        /// 初始化类<see cref="ParameterlessInstanceMethodCallTranslator"/>。
        /// </summary>
        /// <param name="declaringType">声明类型。</param>
        /// <param name="clrMethodName">CLR方法名称。</param>
        /// <param name="sqlFunctionName">SQL函数名称。</param>
        protected ParameterlessInstanceMethodCallTranslator( Type declaringType,  string clrMethodName,  string sqlFunctionName)
        {
            _declaringType = declaringType;
            _clrMethodName = clrMethodName;
            _sqlFunctionName = sqlFunctionName;
        }

        /// <summary>
        /// 转换表达式。
        /// </summary>
        /// <param name="methodCallExpression">方法调用表达式。</param>
        /// <returns>返回转换后的表达式。</returns>
        public virtual Expression Translate( MethodCallExpression methodCallExpression)
        {
            var methodInfo = _declaringType.GetTypeInfo()
                .GetDeclaredMethods(_clrMethodName).SingleOrDefault(m => !m.GetParameters().Any());

            if (methodInfo == methodCallExpression.Method)
            {
                var sqlArguments = new[] { methodCallExpression.Object }.Concat(methodCallExpression.Arguments);
                return new SqlFunctionExpression(_sqlFunctionName, methodCallExpression.Type, sqlArguments);
            }

            return null;
        }
    }
}