using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Mozlite.Extensions.Tasks
{
    /// <summary>
    /// 后台服务。
    /// </summary>
    public class TaskHostedService : HostedService
    {
        private readonly Dictionary<string, ITaskService> _services;
        private readonly ITaskManager _taskManager;
        private readonly IMemoryCache _cache;
        private readonly ILogger<TaskHostedService> _logger;

        /// <summary>
        /// 初始化<see cref="TaskHostedService"/>。
        /// </summary>
        /// <param name="services">后台服务列表。</param>
        /// <param name="taskManager">后台服务管理。</param>
        /// <param name="cache">缓存接口。</param>
        /// <param name="logger">日志接口。</param>
        public TaskHostedService(IEnumerable<ITaskService> services, ITaskManager taskManager, IMemoryCache cache, ILogger<TaskHostedService> logger)
        {
            _services = services.ToDictionary(x => x.GetType().DisplayName(), StringComparer.OrdinalIgnoreCase);
            _taskManager = taskManager;
            _cache = cache;
            _logger = logger;
        }

        /// <summary>
        /// 当应用程序开启时候触发得方法。
        /// </summary>
        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("开启后台任务执行...");
            return base.StartAsync(cancellationToken);
        }

        /// <summary>
        /// 当应用程序关闭时候触发得方法。
        /// </summary>
        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("关闭后台任务执行...");
            return base.StopAsync(cancellationToken);
        }

        private async Task<IEnumerable<TaskContext>> LoadContextsAsync()
        {
            return await _cache.GetOrCreateAsync(typeof(TaskHostedService), async ctx =>
            {
                ctx.SetAbsoluteExpiration(TimeSpan.FromMinutes(5));//五分钟重新获取一次数据库配置
                var contexts = new List<TaskContext>();
                var tasks = await _taskManager.LoadTasksAsync();
                if (!tasks.Any())
                    return contexts;

                foreach (var task in tasks)
                {
                    if (!_services.TryGetValue(task.Type, out var service))
                        continue;
                    var context = new TaskContext();
                    context.Argument = task.TaskArgument;
                    context.Interval = !string.IsNullOrWhiteSpace(context.Argument.Interval) ? context.Argument.Interval : task.Interval;
                    context.Id = task.Id;
                    context.ExecuteAsync = service.ExecuteAsync;
                    context.Name = task.Name;
                    context.LastExecuted = task.LastExecuted;
                    context.NextExecuting = task.NextExecuting;
                    context.Argument.TaskContext = context;
                    contexts.Add(context);
                }
                return contexts;
            });
        }

        /// <summary>
        /// 执行的后台任务方法。
        /// </summary>
        /// <returns>返回任务实例。</returns>
        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            //等待数据迁移
            await cancellationToken.WaitInstalledAsync();
            //将后台服务添加到数据库中
            await _taskManager.EnsuredTaskServicesAsync(_services.Values);
            //开启后台服务线程，执行后台服务
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var contexts = await LoadContextsAsync();
                    foreach (var context in contexts)
                    {
                        if (context.NextExecuting <= DateTime.Now && !context.IsRunning)
                        {
#pragma warning disable CS4014 // 由于此调用不会等待，因此在调用完成前将继续执行当前方法
                            Task.Run(async () =>
                            {
                                context.IsRunning = true;
                                try
                                {
                                    context.Argument.Error = null;
                                    //在服务运行后可以更改当前参数值
                                    await context.ExecuteAsync(context.Argument);
                                }
                                catch (Exception ex)
                                {
                                    if (context.Argument.IsStack)
                                    {
                                        context.Argument.Error = $"{ex.Message}\r\n{ex.StackTrace}";
                                        _taskManager.LogError(context.Name, ex);
                                    }
                                    else
                                    {
                                        context.Argument.Error = ex.Message;
                                    }
                                }
                                context.LastExecuted = DateTime.Now;
                                context.NextExecuting = context.Interval.Next();
                                await _taskManager.SetCompletedAsync(context);
                                context.IsRunning = false;
                            }, cancellationToken);
#pragma warning restore CS4014 // 由于此调用不会等待，因此在调用完成前将继续执行当前方法
                        }
                        await Task.Delay(100, cancellationToken);
                    }
                }
                catch
                {
                }
                await Task.Delay(500, cancellationToken);
            }
        }
    }
}