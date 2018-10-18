using Mozlite.Data;
using Mozlite.Extensions.Security.Stores;
using System;

namespace Mozlite.Extensions.Security.Activities
{
    /// <summary>
    /// 用户活动查询实例。
    /// </summary>
    /// <typeparam name="TUser">用户类型。</typeparam>
    /// <typeparam name="TUserActivity">用户活动状态类型。</typeparam>
    public abstract class ActivityQueryBase<TUser, TUserActivity> : QueryBase<TUserActivity>
        where TUser : UserBase
        where TUserActivity : UserActivity
    {
        /// <summary>
        /// 分类Id。
        /// </summary>
        public int? Cid { get; set; }

        /// <summary>
        /// 用户Id。
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 用户名称。
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// IP地址。
        /// </summary>
        public string IP { get; set; }

        /// <summary>
        /// 起始时间。
        /// </summary>
        public DateTimeOffset? Start { get; set; }

        /// <summary>
        /// 结束时间。
        /// </summary>
        public DateTimeOffset? End { get; set; }

        /// <summary>
        /// 初始化查询上下文。
        /// </summary>
        /// <param name="context">查询上下文。</param>
        protected override void Init(IQueryContext<TUserActivity> context)
        {
            context.InnerJoin<TUser>((a, u) => a.UserId == u.UserId)
                .Select()
                .Select<TUser>(x => new { x.UserName, x.NormalizedUserName });
            if (Cid != null)
                context.Where(x => x.CategoryId == Cid);
            if (UserId > 0)
                context.Where(x => x.UserId == UserId);
            if (Start != null)
                context.Where(x => x.CreatedDate >= Start);
            if (End != null)
                context.Where(x => x.CreatedDate <= End);
            if (!string.IsNullOrEmpty(Name))
                context.Where<TUser>(x => x.UserName.Contains(Name) || x.NormalizedUserName.Contains(Name));
            if (!string.IsNullOrEmpty(IP))
                context.Where(x => x.IPAdress == IP);
            context.OrderByDescending(x => x.Id);
        }

    }

    /// <summary>
    /// 用户活动查询实例。
    /// </summary>
    /// <typeparam name="TUser">用户类型。</typeparam>
    /// <typeparam name="TUserActivity">用户活动状态类型。</typeparam>
    /// <typeparam name="TRole">角色类型。</typeparam>
    public abstract class ActivityQueryBase<TUser, TRole, TUserActivity> : ActivityQueryBase<TUser, TUserActivity>
        where TUser : UserBase
        where TRole : RoleBase
        where TUserActivity : UserActivity
    {
        /// <summary>
        /// 当前用户角色等级。
        /// </summary>
        public int MaxRoleLevel { get; set; }

        /// <summary>
        /// 初始化查询上下文。
        /// </summary>
        /// <param name="context">查询上下文。</param>
        protected override void Init(IQueryContext<TUserActivity> context)
        {
            base.Init(context);
            if (MaxRoleLevel > 0)
            {
                context.Select()
                    .LeftJoin<TUser, TRole>((u, r) => u.RoleId == r.RoleId)
                    .Where<TRole>(x => x.RoleLevel < MaxRoleLevel);
            }
        }
    }
}