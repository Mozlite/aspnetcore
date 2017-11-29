using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Mozlite.Data;

namespace Mozlite.Extensions.Security.Activities
{
    /// <summary>
    /// 活动状态管理类。
    /// </summary>
    public class ActivityManager : IActivityManager
    {
        private readonly IRepository<UserActivity> _repository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        /// <summary>
        /// 初始化<see cref="ActivityManager"/>。
        /// </summary>
        /// <param name="repository">数据库操作接口。</param>
        /// <param name="httpContextAccessor">HTTP上下文访问器。</param>
        public ActivityManager(IRepository<UserActivity> repository, IHttpContextAccessor httpContextAccessor)
        {
            _repository = repository;
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
            activity.Activity = activity.Activity.Replace("${uname}", context.User.GetUserName())
                .Replace("${uid}", activity.UserId.ToString());
            activity.IPAdress = context.GetUserAddress();
            return await _repository.CreateAsync(activity);
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
            activity.Activity = activity.Activity.Replace("${uname}", context.User.GetUserName())
                .Replace("${uid}", activity.UserId.ToString());
            activity.IPAdress = context.GetUserAddress();
            return _repository.Create(activity);
        }

        /// <summary>
        /// 添加活动状态。
        /// </summary>
        /// <param name="activity">活动状态实例。</param>
        /// <returns>返回添加结果。</returns>
        public virtual Task<bool> CreateAsync(string activity)
        {
            return CreateAsync(new UserActivity { Activity = activity });
        }

        /// <summary>
        /// 添加活动状态。
        /// </summary>
        /// <param name="activity">活动状态实例。</param>
        /// <returns>返回添加结果。</returns>
        public virtual bool Create(string activity)
        {
            return Create(new UserActivity { Activity = activity });
        }
    }
}