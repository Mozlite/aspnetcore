using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Mozlite.Data;
using Mozlite.Extensions.Properties;

namespace Mozlite.Extensions.Tasks
{
    /// <summary>
    /// 后台服务管理类型。
    /// </summary>
    public class TaskManager : ITaskManager
    {
        private readonly IRepository<TaskDescriptor> _repository;
        private readonly ILogger<TaskManager> _logger;

        /// <summary>
        /// 初始化类<see cref="TaskManager"/>。
        /// </summary>
        /// <param name="repository">数据库操作接口。</param>
        /// <param name="logger">日志接口。</param>
        public TaskManager(IRepository<TaskDescriptor> repository, ILogger<TaskManager> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        /// <summary>
        /// 确保服务列表。
        /// </summary>
        /// <param name="services">当前程序包含的后台服务列表。</param>
        public async Task EnsuredTaskServicesAsync(IEnumerable<ITaskService> services)
        {
            var descriptors = (await _repository.FetchAsync())?.ToDictionary(x => x.Type, StringComparer.OrdinalIgnoreCase) ??
                new Dictionary<string, TaskDescriptor>(StringComparer.OrdinalIgnoreCase);
            foreach (var service in services)
            {
                if (descriptors.TryGetValue(service.GetType().DisplayName(), out var descriptor))
                {
                    if (service.Disabled)
                        descriptor.Enabled = false;
                    if (descriptor.Name != service.Name ||
                        descriptor.Description != service.Description)
                        await _repository.UpdateAsync(x => x.Id == descriptor.Id,
                            new { service.Name, service.Description, descriptor.Enabled });
                }
                else
                {
                    descriptor = new TaskDescriptor();
                    descriptor.Name = service.Name;
                    descriptor.Description = service.Description;
                    descriptor.Enabled = !service.Disabled;
                    descriptor.Type = service.GetType().DisplayName();
                    descriptor.ExtensionName = service.ExtensionName;
                    descriptor.Interval = service.Interval.ToString();
                    await _repository.CreateAsync(descriptor);
                }
            }
        }

        /// <summary>
        /// 加载所有后台服务。
        /// </summary>
        /// <returns>返回所有后台服务列表。</returns>
        public async Task<IEnumerable<TaskDescriptor>> LoadTasksAsync()
        {
            return await _repository.FetchAsync();
        }

        /// <summary>
        /// 通过类型获取后台服务。
        /// </summary>
        /// <param name="type">任务<seealso cref="ITaskService"/>类型。</param>
        /// <returns>返回当前类型的服务对象。</returns>
        public TaskDescriptor GeTask(Type type)
        {
            var fullName = type.DisplayName();
            return _repository.Find(t => t.Type == fullName);
        }

        /// <summary>
        /// 设置时间间隔。
        /// </summary>
        /// <param name="id">服务Id。</param>
        /// <param name="interval">时间间隔。</param>
        /// <returns>返回设置结果。</returns>
        public bool SetInterval(int id, TaskInterval interval)
        {
            return _repository.Update(t => t.Id == id, new { Interval = interval.ToString() });
        }

        /// <summary>
        /// 设置参数。
        /// </summary>
        /// <param name="id">当前服务Id。</param>
        /// <param name="argument">参数。</param>
        /// <returns>返回设置结果。</returns>
        public async Task<bool> SetArgumentAsync(int id, string argument)
        {
            return await _repository.UpdateAsync(t => t.Id == id, new { Argument = argument });
        }

        /// <summary>
        /// 设置执行时间。
        /// </summary>
        /// <param name="id">当前服务Id。</param>
        /// <param name="next">下一次执行时间。</param>
        /// <param name="last">上一次执行时间。</param>
        /// <returns>返回设置结果。</returns>
        public async Task<bool> SetExecuteDateAsync(int id, DateTime next, DateTime last)
        {
            return await _repository.UpdateAsync(t => t.Id == id, new { NextExecuting = next, LastExecuted = last });
        }

        /// <summary>
        /// 保存错误日志。
        /// </summary>
        /// <param name="name">服务名称。</param>
        /// <param name="exception">错误实例。</param>
        public void LogError(string name, Exception exception)
        {
            _logger.LogError(1, exception, Resources.TaskExecuteError, name, exception.Message);
        }

        /// <summary>
        /// 保存错误日志。
        /// </summary>
        /// <param name="id">服务ID。</param>
        /// <param name="name">名称。</param>
        /// <param name="exception">错误实例。</param>
        public async Task LogErrorAsync(int id, string name, Exception exception)
        {
            _logger.LogError(1, exception, Resources.TaskExecuteError, name, exception.Message);
            var error = new StringBuilder();
            error.AppendFormat(Resources.TaskExecuteError, name, exception.Message);
            error.AppendLine().AppendLine("===========================================================");
            error.AppendLine(exception.StackTrace);
            await _repository.UpdateAsync(x => x.Id == id, new {Error = error.ToString()});
        }
    }
}