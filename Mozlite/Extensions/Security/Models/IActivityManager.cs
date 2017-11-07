using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Mozlite.Data;

namespace Mozlite.Extensions.Security.Models
{
    /// <summary>
    /// 活动状态。
    /// </summary>
    public interface IActivityManager : ISingletonService
    {
        /// <summary>
        /// 添加活动状态。
        /// </summary>
        /// <param name="activity">活动状态实例。</param>
        /// <returns>返回添加结果。</returns>
        Task<bool> CreateAsync(UserActivity activity);

        /// <summary>
        /// 添加活动状态。
        /// </summary>
        /// <param name="activity">活动状态实例。</param>
        /// <returns>返回添加结果。</returns>
        bool Create(UserActivity activity);
    }

    /// <summary>
    /// 活动状态管理类。
    /// </summary>
    public class ActivityManager : IActivityManager
    {
        private readonly IRepository<UserActivity> _repository;
        private readonly IHttpContextAccessor _httpContextAccessor;

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
        public Task<bool> CreateAsync(UserActivity activity)
        {
            var user = _httpContextAccessor.HttpContext.User;
            activity.UserId = user.GetUserId();
            activity.Activity = activity.Activity.Replace("${current}", user.GetUserName());
            return _repository.CreateAsync(activity);
        }

        /// <summary>
        /// 添加活动状态。
        /// </summary>
        /// <param name="activity">活动状态实例。</param>
        /// <returns>返回添加结果。</returns>
        public bool Create(UserActivity activity)
        {
            var user = _httpContextAccessor.HttpContext.User;
            activity.UserId = user.GetUserId();
            activity.Activity = activity.Activity.Replace("${current}", user.GetUserName());
            return _repository.Create(activity);
        }
    }
}