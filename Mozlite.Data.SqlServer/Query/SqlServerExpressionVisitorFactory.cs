using System;
using Mozlite.Data.Migrations;
using Mozlite.Data.Query;
using Mozlite.Data.Query.Translators;

namespace Mozlite.Data.SqlServer.Query
{
    /// <summary>
    /// SQLServer条件表达式访问工厂实现类。
    /// </summary>
    public class SqlServerExpressionVisitorFactory : IExpressionVisitorFactory
    {
        private readonly ISqlHelper _sqlHelper;
        private readonly ITypeMapper _typeMapper;
        private readonly IMemberTranslator _memberTranslator;
        private readonly IMethodCallTranslator _methodCallTranslator;
        private readonly IExpressionFragmentTranslator _fragmentTranslator;

        /// <summary>
        /// 初始化类<see cref="SqlServerExpressionVisitorFactory"/>。
        /// </summary>
        /// <param name="sqlHelper">SQL操作特殊标识接口。</param>
        /// <param name="typeMapper">类型匹配接口。</param>
        /// <param name="memberTranslator">字段或属性转换接口。</param>
        /// <param name="methodCallTranslator">方法调用转换接口。</param>
        /// <param name="fragmentTranslator">代码段转换接口。</param>
        public SqlServerExpressionVisitorFactory(ISqlHelper sqlHelper, ITypeMapper typeMapper, IMemberTranslator memberTranslator, IMethodCallTranslator methodCallTranslator, IExpressionFragmentTranslator fragmentTranslator)
        {
            _sqlHelper = sqlHelper;
            _typeMapper = typeMapper;
            _memberTranslator = memberTranslator;
            _methodCallTranslator = methodCallTranslator;
            _fragmentTranslator = fragmentTranslator;
        }

        /// <summary>
        /// 新建表达式访问器接口实例对象。
        /// </summary>
        /// <returns>返回表达式访问接口。</returns>
        public IExpressionVisitor Create()
        {
            return new SqlServerExpressionVisitor(_sqlHelper, _typeMapper, _memberTranslator, _methodCallTranslator, _fragmentTranslator);
        }

        /// <summary>
        /// 新建表达式访问器接口实例对象。
        /// </summary>
        /// <param name="delimiter">获取前缀的代理方法。</param>
        /// <returns>返回表达式访问接口。</returns>
        public IExpressionVisitor Create(Func<Type, string> delimiter)
        {
            return new SqlServerExpressionVisitor(_sqlHelper, _typeMapper, _memberTranslator, _methodCallTranslator, _fragmentTranslator, delimiter);
        }
    }
}