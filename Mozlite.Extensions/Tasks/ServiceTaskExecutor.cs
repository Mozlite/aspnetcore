using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Mozlite.Data;
using Mozlite.Tasks;

namespace Mozlite.Extensions.Tasks
{
    /// <summary>
    /// 后台服务。
    /// </summary>
    public class ServiceTaskExecutor : ITaskExecutor
    {
        private readonly Dictionary<string, ITaskService> _services;
        private readonly ITaskManager _taskManager;
        private readonly IMemoryCache _cache;

        /// <summary>
        /// 初始化<see cref="ServiceTaskExecutor"/>。
        /// </summary>
        /// <param name="services">后台服务列表。</param>
        /// <param name="taskManager">后台服务管理。</param>
        /// <param name="cache">缓存接口。</param>
        public ServiceTaskExecutor(IEnumerable<ITaskService> services, ITaskManager taskManager, IMemoryCache cache)
        {
            _services = services.ToDictionary(x => x.GetType().DisplayName(), StringComparer.OrdinalIgnoreCase);
            _taskManager = taskManager;
            _cache = cache;
        }

        private async Task<IEnumerable<TaskContext>> LoadContextsAsync()
        {
            return await _cache.GetOrCreateAsync(typeof(ServiceTaskExecutor), async ctx =>
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
                    var context = new TaskContext
                    {
                        Id = task.Id,
                        Interval = task.Interval,
                        ExecuteAsync = service.ExecuteAsync,
                        Argument = task.Argument ?? string.Empty,
                        Name = task.Name,
                        LastExecuted = task.LastExecuted,
                        NextExecuting = task.NextExecuting
                    };
                    context.Argument.TaskContext = context;
                    context.Argument.TaskManager = _taskManager;
                    contexts.Add(context);
                }
                return contexts;
            });
        }

        /// <summary>
        /// 执行的后台任务方法。
        /// </summary>
        /// <returns>返回任务实例。</returns>
        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            //等待数据迁移
            await cancellationToken.WaitInstalledAsync(ExecuteAsync);
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
                        try
                        {
                            if (context.NextExecuting <= DateTime.Now && !context.IsRunning)
                            {
#pragma warning disable 4014
                                Task.Run(async () =>
#pragma warning restore 4014
                                {
                                    context.IsRunning = true;
                                    //在服务运行后可以更改当前参数值
                                    await context.ExecuteAsync(context.Argument);
                                    context.LastExecuted = DateTime.Now;
                                    context.NextExecuting = context.Interval.Next();
                                    await _taskManager.SetExecuteDateAsync(context.Id, context.NextExecuting,
                                        // ReSharper disable once PossibleInvalidOperationException
                                        context.LastExecuted.Value);
                                    context.IsRunning = false;
                                }, cancellationToken);
                            }
                        }
                        catch (Exception ex)
                        {
                            _taskManager.LogError(context.Name, ex);
                        }
                        await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
                    }
                }
                catch (Exception ex)
                {
                    _taskManager.LogError(null, ex);
                }
                finally
                {
                    await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
                }
            }
        }
    }
}