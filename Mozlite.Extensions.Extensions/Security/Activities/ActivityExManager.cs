using Microsoft.AspNetCore.Http;
using Mozlite.Data;
using Mozlite.Extensions.Sites;

namespace Mozlite.Extensions.Security.Activities
{
    /// <summary>
    /// 活动状态管理类。
    /// </summary>
    /// <typeparam name="TUserActivity">当前活动状态类型。</typeparam>
    public abstract class ActivityExManager<TUserActivity> : ActivityManager<TUserActivity>
        where TUserActivity : UserActivityEx, new()
    {
        private readonly ISiteContextAccessorBase _siteContextAccessor;
        /// <summary>
        /// 网站上下文。
        /// </summary>
        protected SiteContextBase Site => _siteContextAccessor.SiteContext;

        /// <summary>
        /// 初始化<see cref="ActivityManager{TActivity}"/>。
        /// </summary>
        /// <param name="db">数据库操作接口。</param>
        /// <param name="httpContextAccessor">HTTP上下文访问器。</param>
        /// <param name="siteContextAccessor">网站上下文访问接口。</param>
        protected ActivityExManager(IDbContext<TUserActivity> db, IHttpContextAccessor httpContextAccessor, ISiteContextAccessorBase siteContextAccessor)
            : base(db, httpContextAccessor)
        {
            _siteContextAccessor = siteContextAccessor;
        }

        /// <summary>
        /// 实例化类<see cref="UserActivityEx"/>。
        /// </summary>
        /// <param name="activity">用户活动实例。</param>
        /// <returns>返回是否实例化成功。</returns>
        protected override bool Init(TUserActivity activity)
        {
            if (activity.SiteId == 0)
                activity.SiteId = Site.SiteId;
            return base.Init(activity);
        }
    }
}