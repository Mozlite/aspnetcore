using Microsoft.AspNetCore.Http;
using Mozlite.Data;
using Mozlite.Extensions.Security.Activities;

namespace MS.Extensions.Security.Activities
{
    /// <summary>
    /// 活动状态管理类型。
    /// </summary>
    public class ActivityManager : ActivityManager<UserActivity>, IActivityManager
    {
        /// <summary>
        /// 初始化<see cref="ActivityManager"/>。
        /// </summary>
        /// <param name="db">数据库操作接口。</param>
        /// <param name="httpContextAccessor">HTTP上下文访问器。</param>
        public ActivityManager(IDbContext<UserActivity> db, IHttpContextAccessor httpContextAccessor) 
            : base(db, httpContextAccessor)
        {
        }
    }
}