using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Mozlite.Extensions.Tasks
{
    /// <summary>
    /// 后台服务扩展类。
    /// </summary>
    public static class TaskExtensions
    {
        /// <summary>
        /// 启动服务。
        /// </summary>
        /// <param name="app">APP构建实例。</param>
        /// <param name="delays">延迟启动时间（秒）。</param>
        public static void UseTasks(this IApplicationBuilder app, int delays = 30)
        {
            Task.Run(async () =>
            {
                await Task.Delay(TimeSpan.FromSeconds(delays));
                using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
                {
                    foreach (var task in serviceScope.ServiceProvider.GetRequiredService<IEnumerable<ITaskStarter>>())
                    {
                        Task.Run(task.RunAsync);
                    }
                }
            });
        }
    }
}