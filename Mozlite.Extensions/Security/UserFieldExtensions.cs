using System;
using System.Linq.Expressions;
using Mozlite.Data;

namespace Mozlite.Extensions.Security
{
    /// <summary>
    /// 用户列扩展类。
    /// </summary>
    public static class UserFieldExtensions
    {
        /// <summary>
        /// 选择用户相关联字段。
        /// </summary>
        /// <typeparam name="TModel">当前实例模型。</typeparam>
        /// <typeparam name="TUser">用户类型。</typeparam>
        /// <param name="queryable">查询实例。</param>
        /// <param name="expression">关联表达式。</param>
        /// <returns>返回当前查询实例。</returns>
        public static IQueryable<TModel> JoinSelect<TModel, TUser>(this IQueryable<TModel> queryable,
            Expression<Func<TModel, TUser, bool>> expression)
            where TModel : UserFieldBase
            where TUser : UserBase
            => queryable.InnerJoin<TUser>(expression)
                .Select<TUser>(x => new { x.NormalizedUserName, x.UserName, x.RoleId, x.RoleName, x.Avatar });
    }
}