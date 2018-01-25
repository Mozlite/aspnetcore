using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Mozlite.Data;
using Mozlite.Extensions.Security.Stores;

namespace Mozlite.Extensions.Security.Activities
{
    /// <summary>
    /// 活动状态管理类。
    /// </summary>
    public class ActivityManager : IActivityManager
    {
        private readonly IDbContext<UserActivity> _db;
        private readonly IHttpContextAccessor _httpContextAccessor;
        /// <summary>
        /// 初始化<see cref="ActivityManager"/>。
        /// </summary>
        /// <param name="db">数据库操作接口。</param>
        /// <param name="httpContextAccessor">HTTP上下文访问器。</param>
        public ActivityManager(IDbContext<UserActivity> db, IHttpContextAccessor httpContextAccessor)
        {
            _db = db;
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// 添加活动状态。
        /// </summary>
        /// <param name="activity">活动状态实例。</param>
        /// <returns>返回添加结果。</returns>
        public virtual async Task<bool> CreateAsync(UserActivity activity)
        {
            var context = _httpContextAccessor.HttpContext;
            if (context?.User?.Identity.IsAuthenticated != true)
                return false;
            activity.UserId = context.User.GetUserId();
            activity.IPAdress = context.GetUserAddress();
            return await _db.CreateAsync(activity);
        }

        /// <summary>
        /// 添加活动状态。
        /// </summary>
        /// <param name="activity">活动状态实例。</param>
        /// <returns>返回添加结果。</returns>
        public virtual bool Create(UserActivity activity)
        {
            var context = _httpContextAccessor.HttpContext;
            if (context?.User?.Identity.IsAuthenticated != true)
                return false;
            activity.UserId = context.User.GetUserId();
            activity.IPAdress = context.GetUserAddress();
            return _db.Create(activity);
        }

        /// <summary>
        /// 添加活动状态。
        /// </summary>
        /// <param name="activity">活动状态实例。</param>
        /// <param name="userId">当前用户。</param>
        /// <returns>返回添加结果。</returns>
        public virtual Task<bool> CreateAsync(string activity, int userId)
        {
            var current = new UserActivity();
            current.UserId = userId;
            current.Activity = activity;
            current.IPAdress = _httpContextAccessor.HttpContext.GetUserAddress();
            return _db.CreateAsync(current);
        }

        /// <summary>
        /// 添加活动状态。
        /// </summary>
        /// <param name="activity">活动状态实例。</param>
        /// <param name="userId">当前用户。</param>
        /// <returns>返回添加结果。</returns>
        public virtual bool Create(string activity, int userId)
        {
            var current = new UserActivity();
            current.UserId = userId;
            current.Activity = activity;
            current.IPAdress = _httpContextAccessor.HttpContext.GetUserAddress();
            return _db.Create(current);
        }

        /// <summary>
        /// 查询活动状态。
        /// </summary>
        /// <typeparam name="TQuery">查询实例类型。</typeparam>
        /// <typeparam name="TUser">用户类型。</typeparam>
        /// <param name="query">当前查询实例对象。</param>
        /// <returns>查询实例对象。</returns>
        public virtual TQuery Load<TQuery, TUser>(TQuery query) where TQuery : UserActivityQuery<TUser> where TUser : UserBase
        {
            return _db.Load(query, x => x.Id);
        }

        /// <summary>
        /// 查询活动状态。
        /// </summary>
        /// <typeparam name="TQuery">查询实例类型。</typeparam>
        /// <typeparam name="TUser">用户类型。</typeparam>
        /// <param name="query">当前查询实例对象。</param>
        /// <returns>查询实例对象。</returns>
        public virtual Task<TQuery> LoadAsync<TQuery, TUser>(TQuery query) where TQuery : UserActivityQuery<TUser> where TUser : UserBase
        {
            return _db.LoadAsync(query, x => x.Id);
        }
    }
}