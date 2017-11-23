using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Mozlite.Data;
using Mozlite.Extensions.Security.Models;

namespace Mozlite.Extensions.Security.Services
{
    /// <summary>
    /// 活动状态管理类。
    /// </summary>
    public class ActivityManager : IActivityManager
    {
        private readonly IRepository<UserActivity> _repository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<UserActivity> _logger;

        public ActivityManager(IRepository<UserActivity> repository, IHttpContextAccessor httpContextAccessor, ILogger<UserActivity> logger)
        {
            _repository = repository;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        /// <summary>
        /// 添加活动状态。
        /// </summary>
        /// <param name="activity">活动状态实例。</param>
        /// <returns>返回添加结果。</returns>
        public virtual async Task<bool> CreateAsync(UserActivity activity)
        {
            var context = _httpContextAccessor.HttpContext;
            activity.UserId = context.User.GetUserId();
            activity.Activity = activity.Activity.Replace("${uname}", context.User.GetUserName())
                .Replace("${uid}", activity.UserId.ToString());
            activity.IPAdress = context.GetUserAddress();
            if (await _repository.CreateAsync(activity))
            {
                _logger.LogInformation("[{0}] {1}{2}.", activity.CreatedDate.ToString("HH:mm"), context.User.GetUserName(), activity.Activity);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 添加活动状态。
        /// </summary>
        /// <param name="activity">活动状态实例。</param>
        /// <returns>返回添加结果。</returns>
        public virtual bool Create(UserActivity activity)
        {
            var context = _httpContextAccessor.HttpContext;
            activity.UserId = context.User.GetUserId();
            activity.Activity = activity.Activity.Replace("${uname}", context.User.GetUserName())
                .Replace("${uid}", activity.UserId.ToString());
            activity.IPAdress = context.GetUserAddress();
            if (_repository.Create(activity))
            {
                _logger.LogInformation("[{0}] {1}{2}.", activity.CreatedDate.ToString("HH:mm"), context.User.GetUserName(), activity.Activity);
                return true;
            }
            return false;
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