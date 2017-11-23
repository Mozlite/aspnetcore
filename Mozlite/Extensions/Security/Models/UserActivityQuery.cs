using Mozlite.Data;

namespace Mozlite.Extensions.Security.Models
{
    /// <summary>
    /// 用户活动查询实例。
    /// </summary>
    public class UserActivityQuery : QueryBase<UserActivity>
    {
        /// <summary>
        /// 用户Id。
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 初始化查询上下文。
        /// </summary>
        /// <param name="context">查询上下文。</param>
        protected override void Init(IQueryContext<UserActivity> context)
        {
            context.InnerJoin<User>((a, u) => a.UserId == u.UserId)
                .Select()
                .Select<User>(x => new { x.UserName, x.NickName });
            if (UserId > 0)
                context.Where(x => x.UserId == UserId);
            context.OrderByDescending(x => x.Id);
        }
    }
}