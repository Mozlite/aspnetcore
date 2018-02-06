using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Mozlite.Extensions.Security.Activities
{
    /// <summary>
    /// 用户活动状态日志提供者。
    /// </summary>
    internal class ActivityLoggerProvider : ILoggerProvider
    {
        private readonly IActivityManagerBase _activityManager;
        /// <summary>
        /// 初始化类<see cref="ActivityLoggerProvider"/>。
        /// </summary>
        /// <param name="serviceProvider">服务提供者接口。</param>
        public ActivityLoggerProvider(IServiceProvider serviceProvider)
        {
            _activityManager = serviceProvider.GetRequiredService<IActivityManagerBase>();
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