using System;
using Mozlite.Data;
using Mozlite.Extensions.Security.Stores;

namespace Mozlite.Extensions.Security
{
    /// <summary>
    /// 分页查询基类。
    /// </summary>
    /// <typeparam name="TUser">用户类型。</typeparam>
    public abstract class QueryBase<TUser> : Mozlite.Data.QueryBase<TUser>
        where TUser : UserBase
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 注册开始时间。
        /// </summary>
        public DateTime? Start { get; set; }

        /// <summary>
        /// 注册结束时间。
        /// </summary>
        public DateTime? End { get; set; }

        /// <summary>
        /// 初始化查询上下文。
        /// </summary>
        /// <param name="context">查询上下文。</param>
        protected override void Init(IQueryContext<TUser> context)
        {
            if (!string.IsNullOrWhiteSpace(Name))
                context.Where(x => x.NormalizedUserName.Contains(Name) || x.UserName.Contains(Name));
            if (Start != null)
                context.Where(x => x.CreatedDate >= Start);
            if (End != null)
                context.Where(x => x.CreatedDate <= End);
        }
    }
    
    /// <summary>
    /// 分页查询基类。
    /// </summary>
    /// <typeparam name="TUser">用户类型。</typeparam>
    /// <typeparam name="TRole">用户组类型。</typeparam>
    public abstract class QueryBase<TUser, TRole> : QueryBase<TUser>
        where TUser : UserBase
        where TRole : RoleBase
    {
        /// <summary>
        /// 当前用户用户组等级。
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