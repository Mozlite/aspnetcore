using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

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
                await Task.Delay(TimeSpan.FromSeconds(1));

                using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
                {
                    try
                    {
                        var starters = serviceScope.ServiceProvider.GetRequiredService<IEnumerable<ITaskStarter>>();
                        foreach (var starter in starters)
                        {
#pragma warning disable 4014
                           Task.Run(starter.RunAsync);
#pragma warning restore 4014
                        }
                    }
                    catch (Exception exception)
                    {
                        serviceScope.ServiceProvider.GetRequiredService<ILogger<ITaskStarter>>().LogError(exception, exception.Message);
                    }
                }
            });
        }
    }
}