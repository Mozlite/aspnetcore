using Microsoft.Extensions.Logging;
using Mozlite.Data;
using Mozlite.Extensions.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mozlite.Extensions.Tasks
{
    /// <summary>
    /// 后台服务管理类型。
    /// </summary>
    public class TaskManager : ITaskManager
    {
        private readonly IDbContext<TaskDescriptor> _db;
        private readonly ILogger<TaskManager> _logger;

        /// <summary>
        /// 初始化类<see cref="TaskManager"/>。
        /// </summary>
        /// <param name="db">数据库操作接口。</param>
        /// <param name="logger">日志接口。</param>
        public TaskManager(IDbContext<TaskDescriptor> db, ILogger<TaskManager> logger)
        {
            _db = db;
            _logger = logger;
        }

        /// <summary>
        /// 确保服务列表。
        /// </summary>
        /// <param name="services">当前程序包含的后台服务列表。</param>
        public async Task EnsuredTaskServicesAsync(IEnumerable<ITaskService> services)
        {
            var descriptors = await _db.FetchAsync();
            foreach (var service in services)
            {
                var type = service.GetType().DisplayName();
                var descriptor = descriptors.SingleOrDefault(x => x.Type == type);
                if (descriptor != null)
                {
                    await _db.UpdateAsync(x => x.Id == descriptor.Id, new { service.Name, Interval = service.Interval.ToString(), service.Description, Enabled = !service.Disabled });
                    descriptor.ShouldBeDeleting = false;
                }
                else
                {
                    descriptor = new TaskDescriptor();
                    descriptor.Name = service.Name;
                    descriptor.Description = service.Description;
                    descriptor.Enabled = !service.Disabled;
                    descriptor.Type = type;
                    descriptor.ExtensionName = service.ExtensionName;
                    descriptor.Interval = service.Interval.ToString();
                    await _db.CreateAsync(descriptor);
                }
            }
            //删除程序移除的后台服务
            foreach (var descriptor in descriptors)
            {
                if (descriptor.ShouldBeDeleting)
                    await _db.DeleteAsync(descriptor.Id);
            }
        }

        /// <summary>
        /// 加载所有后台服务。
        /// </summary>
        /// <returns>返回所有后台服务列表。</returns>
        public async Task<IEnumerable<TaskDescriptor>> LoadTasksAsync()
        {
            return await _db.FetchAsync();
        }

        /// <summary>
        /// 通过类型获取后台服务。
        /// </summary>
        /// <param name="type">任务<seealso cref="ITaskService"/>类型。</param>
        /// <returns>返回当前类型的服务对象。</returns>
        public TaskDescriptor GeTask(Type type)
        {
            var fullName = type.DisplayName();
            return _db.Find(t => t.Type == fullName);
        }

        /// <summary>
        /// 设置时间间隔。
        /// </summary>
        /// <param name="id">服务Id。</param>
        /// <param name="interval">时间间隔。</param>
        /// <returns>返回设置结果。</returns>
        public bool SetInterval(int id, string interval)
        {
            return _db.Update(id, new { Interval = interval });
        }

        /// <summary>
        /// 设置参数。
        /// </summary>
        /// <param name="id">当前服务Id。</param>
        /// <param name="interval">时间间隔。</param>
        /// <returns>返回设置结果。</returns>
        public async Task<bool> SaveArgumentIntervalAsync(int id, string interval)
        {
            var value = await _db.FindAsync(id);
            if (value == null)
                return false;
            if (interval != value.TaskArgument.Interval)
            {
                value.TaskArgument.Interval = interval;
                value.NextExecuting = ((TaskInterval)interval).Next();
            }
            return await _db.UpdateAsync(id, new { value.NextExecuting, Argument = value.TaskArgument.ToString() });
        }

        /// <summary>
        /// 设置参数。
        /// </summary>
        /// <param name="id">当前服务Id。</param>
        /// <param name="argument">参数。</param>
        /// <returns>返回设置结果。</returns>
        public async Task<bool> SaveArgumentAsync(int id, Argument argument)
        {
            var value = await _db.FindAsync(id);
            if (value == null)
                return false;
            if (argument.Interval != value.TaskArgument.Interval)
            {
                TaskInterval interval = argument.Interval ?? value.Interval;
                return await _db.UpdateAsync(id, new { Argument = argument.ToString(), NextExecuting = interval.Next() });
            }
            return await _db.UpdateAsync(id, new { Argument = argument.ToString() });
        }

        /// <summary>
        /// 设置完成状态。
        /// </summary>
        /// <param name="context">当前服务上下文。</param>
        /// <returns>返回设置结果。</returns>
        public async Task<bool> SetCompletedAsync(TaskContext context)
        {
            return await _db.UpdateAsync(context.Id, new { context.NextExecuting, context.LastExecuted, Argument = context.Argument.ToString() });
        }

        /// <summary>
        /// 保存错误日志。
        /// </summary>
        /// <param name="name">服务名称。</param>
        /// <param name="exception">错误实例。</param>
        public void LogError(string name, Exception exception)
        {
            _logger.LogError(exception, Resources.TaskExecuteError, name, exception.Message);
        }
    }
}