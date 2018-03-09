using Mozlite.Data;
using Mozlite.Extensions.Security.Stores;

namespace Mozlite.Extensions.Security.Activities
{
    /// <summary>
    /// 用户活动查询实例。
    /// </summary>
    /// <typeparam name="TUser">用户类型。</typeparam>
    /// <typeparam name="TUserActivity">用户活动状态类型。</typeparam>
    public abstract class UserActivityExQueryBase<TUser, TUserActivity> : UserActivityQueryBase<TUser, TUserActivity>, ISitable
        where TUser : UserBase
        where TUserActivity : UserActivityEx
    {
        /// <summary>
        /// 初始化查询上下文。
        /// </summary>
        /// <param name="context">查询上下文。</param>
        protected override void Init(IQueryContext<TUserActivity> context)
        {
            base.Init(context);
            if (SiteId > 0)
                context.Where(x => x.SiteId == SiteId);
        }

        /// <summary>
        /// 获取当前站Id。
        /// </summary>
        public int SiteId { get; set; }
    }
}