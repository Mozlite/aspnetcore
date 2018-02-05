using Mozlite.Data;
using Mozlite.Extensions.Security.Stores;

namespace Mozlite.Extensions.Security.Activities
{
    /// <summary>
    /// 用户活动查询实例。
    /// </summary>
    public abstract class UserActivityExQuery<TUser> : UserActivityQuery<TUser>, ISitable
        where TUser : UserBase
    {
        /// <summary>
        /// 初始化查询上下文。
        /// </summary>
        /// <param name="context">查询上下文。</param>
        protected override void Init(IQueryContext<UserActivity> context)
        {
            base.Init(context);
            if (SiteId > 0)
                context.Where("SiteId = " + SiteId);
        }

        /// <summary>
        /// 获取当前站Id。
        /// </summary>
        public int SiteId { get; set; }
    }
}