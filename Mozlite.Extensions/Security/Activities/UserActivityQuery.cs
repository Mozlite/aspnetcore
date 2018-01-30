using Mozlite.Data;
using Mozlite.Extensions.Security.Stores;

namespace Mozlite.Extensions.Security.Activities
{
    /// <summary>
    /// 用户活动查询实例。
    /// </summary>
    public abstract class UserActivityQuery<TUser> : QueryBase<UserActivity>
        where TUser : UserBase
    {
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
        /// 初始化查询上下文。
        /// </summary>
        /// <param name="context">查询上下文。</param>
        protected override void Init(IQueryContext<UserActivity> context)
        {
            context.InnerJoin<TUser>((a, u) => a.UserId == u.UserId)
                .Select()
                .Select<TUser>(x => new { x.UserName, x.NormalizedUserName });
            if (UserId > 0)
                context.Where(x => x.UserId == UserId);
            if (!string.IsNullOrEmpty(Name))
                context.Where<TUser>(x => x.UserName.Contains(Name) || x.NormalizedUserName.Contains(Name));
            if (!string.IsNullOrEmpty(IP))
                context.Where(x => x.IPAdress == IP);
            context.OrderByDescending(x => x.Id);
        }
    }
}