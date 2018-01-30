using Microsoft.Extensions.Logging;

namespace Mozlite.Extensions.Security.Activities
{
    /// <summary>
    /// 用户活动状态日志提供者。
    /// </summary>
    public class ActivityLoggerProvider : ILoggerProvider
    {
        private readonly IActivityManager _activityManager;
        /// <summary>
        /// 初始化类<see cref="ActivityLoggerProvider"/>。
        /// </summary>
        /// <param name="activityManager">用户活动状态管理接口。</param>
        public ActivityLoggerProvider(IActivityManager activityManager)
        {
            _activityManager = activityManager;
        }

        /// <summary>
        /// 释放资源。
        /// </summary>
        public void Dispose()
        {

        }

        /// <summary>
        /// 实例化<see cref="ILogger"/>。
        /// </summary>
        /// <param name="categoryName">分类名称。</param>
        /// <returns>返回<see cref="ILogger"/>实例。</returns>
        public ILogger CreateLogger(string categoryName)
        {
            return new ActivityLogger(_activityManager);
        }
    }
}