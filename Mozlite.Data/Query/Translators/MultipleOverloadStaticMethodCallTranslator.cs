using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Mozlite.Data.Query.Expressions;

namespace Mozlite.Data.Query.Translators
{
    /// <summary>
    /// 静态方法调用表达式基类，静态的方法只要重写此类即可实现。
    /// </summary>
    public abstract class MultipleOverloadStaticMethodCallTranslator : IMethodCallTranslator
    {
        private readonly Type _declaringType;
        private readonly string _clrMethodName;
        private readonly string _sqlFunctionName;
        /// <summary>
        /// 初始化类<see cref="MultipleOverloadStaticMethodCallTranslator"/>。
        /// </summary>
        /// <param name="declaringType">声明类型。</param>
        /// <param name="clrMethodName">CLR方法名称。</param>
        /// <param name="sqlFunctionName">SQL函数名称。</param>
        protected MultipleOverloadStaticMethodCallTranslator( Type declaringType,  string clrMethodName,  string sqlFunctionName)
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
            var methodInfos = _declaringType.GetTypeInfo().GetDeclaredMethods(_clrMethodName);
            if (methodInfos.Contains(methodCallExpression.Method))
            {
                return new SqlFunctionExpression(_sqlFunctionName, methodCallExpression.Type, methodCallExpression.Arguments);
            }

            return null;
        }
    }
}