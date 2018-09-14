using System;
using System.Linq.Expressions;
using Mozlite.Data;

namespace MS.Extensions.Security
{
    /// <summary>
    /// 级联用户选择数据扩展类。
    /// </summary>
    public static class DataExtensions
    {
        /// <summary>
        /// 选择用户基础列，包含用户名，昵称，头像地址和积分。
        /// </summary>
        /// <typeparam name="TModel">当前模型类型。</typeparam>
        /// <param name="context">查询上下文。</param>
        /// <param name="onExpression">关联表达式。</param>
        /// <returns>返回查询上下文实例。</returns>
        public static IQueryContext<TModel> IncludeUsers<TModel>(this IQueryContext<TModel> context, Expression<Func<TModel, User, bool>> onExpression)
        {
            return context
                .InnerJoin<User>(onExpression)
                .Select<User>(u => new { u.NormalizedUserName, u.Avatar, u.UserName });
        }

        /// <summary>
        /// 选择用户基础列，包含用户名，昵称，头像地址和积分。
        /// </summary>
        /// <typeparam name="TModel">当前模型类型。</typeparam>
        /// <param name="context">查询上下文。</param>
        /// <param name="onExpression">关联表达式。</param>
        /// <returns>返回查询上下文实例。</returns>
        public static IQueryable<TModel> IncludeUsers<TModel>(this IQueryable<TModel> context, Expression<Func<TModel, User, bool>> onExpression)
        {
            return context
                .InnerJoin<User>(onExpression)
                .Select<User>(u => new { u.NormalizedUserName, u.Avatar, u.UserName });
        }
    }
}