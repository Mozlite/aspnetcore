using Mozlite.Data;
using Mozlite.Extensions.Security.Stores;
using Mozlite.Extensions.Extensions.Security.Stores;

namespace Mozlite.Extensions.Extensions.Security
{
    /// <summary>
    /// 分页查询基类。
    /// </summary>
    /// <typeparam name="TUser">用户类型。</typeparam>
    public abstract class QueryBase<TUser> : Mozlite.Extensions.Security.QueryBase<TUser>, ISitable
        where TUser : UserExBase, ISitable
    {
        /// <summary>
        /// 获取当前站Id。
        /// </summary>
        public int SiteId { get; set; }

        /// <summary>
        /// 初始化查询上下文。
        /// </summary>
        /// <param name="context">查询上下文。</param>
        protected override void Init(IQueryContext<TUser> context)
        {
            base.Init(context);
            if (SiteId > 0)
                context.Where(x => x.SiteId == SiteId);
        }
    }

    /// <summary>
    /// 分页查询基类。
    /// </summary>
    /// <typeparam name="TUser">用户类型。</typeparam>
    /// <typeparam name="TRole">角色类型。</typeparam>
    public abstract class QueryBase<TUser, TRole> : QueryBase<TUser>
        where TUser : UserExBase, ISitable
        where TRole : RoleBase, ISitable
    {
        /// <summary>
        /// 当前用户角色等级。
        /// </summary>
        public int MaxRoleLevel { get; set; }

        /// <summary>
        /// 初始化查询上下文。
        /// </summary>
        /// <param name="context">查询上下文。</param>
        protected override void Init(IQueryContext<TUser> context)
        {
            base.Init(context);
            if (MaxRoleLevel > 0)
            {
                context.Select()
                    .LeftJoin<TRole>((u, r) => u.RoleId == r.RoleId)
                    .Where<TRole>(x => x.RoleLevel < MaxRoleLevel);
            }
        }
    }
}