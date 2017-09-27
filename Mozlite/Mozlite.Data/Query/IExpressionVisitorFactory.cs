using System;

namespace Mozlite.Data.Query
{
    /// <summary>
    /// 条件表达式访问工厂接口。
    /// </summary>
    public interface IExpressionVisitorFactory
    {
        /// <summary>
        /// 新建表达式访问器接口实例对象。
        /// </summary>
        /// <returns>返回表达式访问接口。</returns>
        IExpressionVisitor Create();

        /// <summary>
        /// 新建表达式访问器接口实例对象。
        /// </summary>
        /// <param name="delimiter">获取前缀的代理方法。</param>
        /// <returns>返回表达式访问接口。</returns>
        IExpressionVisitor Create(Func<Type, string> delimiter);
    }
}