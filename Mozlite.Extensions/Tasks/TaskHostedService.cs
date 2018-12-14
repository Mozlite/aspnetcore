using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Mozlite.Extensions.Tasks
{
    /// <summary>
    /// 后台服务。
    /// </summary>
    public class TaskHostedService : HostedService
    {
        private readonly IEnumerable<ITaskService> _services;
        private readonly ITaskManager _taskManager;
        private readonly ConcurrentDictionary<string, TaskContext> _contexts = new ConcurrentDictionary<string, TaskContext>(StringComparer.OrdinalIgnoreCase);
        private DateTime _updatedDate = DateTime.MinValue;
        private readonly ILogger _logger;

        /// <summary>
        /// 初始化<see cref="TaskHostedService"/>。
        /// </summary>
        /// <param name="services">后台服务列表。</param>
        /// <param name="taskManager">后台服务管理。</param>
        /// <param name="logger">日志接口。</param>
        public TaskHostedService(IEnumerable<ITaskService> services, ITaskManager taskManager, ILogger<TaskHostedService> logger)
        {
            _services = services;
            _taskManager = taskManager;
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

        private async Task EnsuredTaskServicesAsync()
        {
            foreach (var service in _services)
            {
                var context = new TaskContext();
                context.ExecuteAsync = service.ExecuteAsync;
                context.Interval = service.Interval;
                _contexts.TryAdd(service.GetType().DisplayName(), context);
            }
            await _taskManager.EnsuredTaskServicesAsync(_services);
        }

        private async Task LoadContextsAsync()
        {
            if (_updatedDate.AddMinutes(1) >= DateTime.Now)
                return;
            var tasks = await _taskManager.LoadTasksAsync();
            foreach (var task in tasks)
            {
                if (!_contexts.TryGetValue(task.Type, out var context))
                    continue;
                context.Argument = task.TaskArgument;
                context.Id = task.Id;
                context.Name = task.Name;
                if (!context.IsRunning)
                {
                    context.LastExecuted = task.LastExecuted;
                    context.NextExecuting = task.NextExecuting;
                }
            }
            _updatedDate = DateTime.Now;
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
            await EnsuredTaskServicesAsync();
            //开启后台服务线程，执行后台服务
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    await LoadContextsAsync();
                    foreach (var context in _contexts.Values)
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