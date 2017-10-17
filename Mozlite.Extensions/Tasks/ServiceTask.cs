using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mozlite.Extensions.Tasks
{
    /// <summary>
    /// 后台服务。
    /// </summary>
    public class ServiceTask : ITask
    {
        private readonly IEnumerable<ITaskService> _services;
        private readonly ITaskManager _taskManager;

        /// <summary>
        /// 初始化<see cref="ServiceTask"/>。
        /// </summary>
        /// <param name="services">后台服务列表。</param>
        /// <param name="taskManager">后台服务管理。</param>
        public ServiceTask(IEnumerable<ITaskService> services, ITaskManager taskManager)
        {
            _services = services;
            _taskManager = taskManager;
        }

        /// <summary>
        /// 执行的后台任务方法。
        /// </summary>
        /// <returns>返回任务实例。</returns>
        public async Task RunAsync()
        {
            var tasks = (await _taskManager.LoadTasksAsync()).ToList();
            if (!tasks.Any())
                tasks = new List<TaskDescriptor>();
            foreach (var service in _services)
            {
                var task = tasks.SingleOrDefault(t => t.Type.Equals(service.GetType().DisplayName()));
                if (task == null)
                {
                    task = new TaskDescriptor();
                    task.Type = service.GetType().DisplayName();
                    task.Name = service.Name;
                    task.Description = service.Description;
                    task.DependenceArgument = service.DependenceArgument;
                    task.TaskInterval = service.Interval;
                    task.NextExecuting = DateTime.Now.AddDays(-1);
                    task.ExtensionName = service.ExtensionName;
                    await _taskManager.SaveAsync(task);
                    tasks.Add(task);
                }
                task.Service = service.ExecuteAsync;
            }
            await ExecuteAsync(tasks);
        }

        private async Task ExecuteAsync(IEnumerable<TaskDescriptor> tasks)
        {
            while (true)
            {
                try
                {
                    foreach (var task in tasks)
                    {
                        try
                        {
                            if (task.NextExecuting <= DateTime.Now && !task.IsRunning)
                            {
#pragma warning disable 4014
                                Task.Run(async () =>
#pragma warning restore 4014
                                {
                                    task.IsRunning = true;
                                    if (task.DependenceArgument)
                                        await ExecuteArgumentAsync(task);
                                    else await task.Service(null);
                                    task.LastExecuted = DateTime.Now;
                                    task.NextExecuting = task.TaskInterval.Next();
                                    await _taskManager.SetExecuteDateAsync(task.Id, task.NextExecuting, task.LastExecuted.Value);
                                    task.IsRunning = false;
                                });
                            }
                        }
                        catch (Exception ex)
                        {
                            _taskManager.LogError(task.Name, ex);
                        }
                        await Task.Delay(TimeSpan.FromSeconds(1));
                    }
                }
                catch (Exception ex)
                {
                    _taskManager.LogError(null, ex);
                }
                await Task.Delay(TimeSpan.FromSeconds(1));
            }
        }

        private async Task ExecuteArgumentAsync(TaskDescriptor task)
        {
            var argument = await _taskManager.GetArgumentAsync(task.Id);
            if (argument != null)
            {
                try
                {
                    //在服务运行后可以更改当前参数值
                    if (argument.Args != null)
                        argument.Args.SetArgumentAsync = args => _taskManager.SetArgumentAsync(argument.Id, args);
                    await task.Service(argument.Args);
                    await _taskManager.SetArgumentAsync(argument.Id);
                }
                catch (Exception ex)
                {
                    await
                        _taskManager.SetArgumentAsync(argument.Id, argument.TryTimes + 1,
                            ex.Message);
                    _taskManager.LogError(task.Name, ex);
                }
            }
            await Task.Delay(TimeSpan.FromSeconds(1));
        }
    }
}