using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Mozlite.Data;
using Mozlite.Extensions.Security.Activities;

namespace Demo.Extensions.Security.Activities
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

        /// <summary>
        /// 查询活动状态。
        /// </summary>
        /// <param name="query">当前查询实例对象。</param>
        /// <returns>查询实例对象。</returns>
        public virtual UserActivityQuery Load(UserActivityQuery query)
        {
            return Load<UserActivityQuery, User>(query);
        }

        /// <summary>
        /// 查询活动状态。
        /// </summary>
        /// <param name="query">当前查询实例对象。</param>
        /// <returns>查询实例对象。</returns>
        public virtual Task<UserActivityQuery> LoadAsync(UserActivityQuery query)
        {
            return LoadAsync<UserActivityQuery, User>(query);
        }
    }
}