using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
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
        private readonly IRepository<TaskArgument> _args;
        private readonly IMemoryCache _cache;

        /// <summary>
        /// 初始化类<see cref="TaskManager"/>。
        /// </summary>
        /// <param name="repository">数据库操作接口。</param>
        /// <param name="logger">日志接口。</param>
        /// <param name="args">参数数据库操作接口。</param>
        /// <param name="cache">缓存接口。</param>
        public TaskManager(IRepository<TaskDescriptor> repository, ILogger<TaskManager> logger, IRepository<TaskArgument> args, IMemoryCache cache)
        {
            _repository = repository;
            _logger = logger;
            _args = args;
            _cache = cache;
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
        /// 加载服务列表。
        /// </summary>
        /// <param name="extensionName">扩展名称。</param>
        /// <returns>返回服务描述列表。</returns>
        public IEnumerable<TaskDescriptor> LoadArgumentTasks(string extensionName)
        {
            return _cache.GetOrCreate($"tasks[{extensionName}]", ctx =>
            {
                ctx.SetAbsoluteExpiration(TimeSpan.FromMinutes(3));
                var query = _repository.AsQueryable();
                if (!string.IsNullOrWhiteSpace(extensionName))
                    query.Where(x => x.ExtensionName == extensionName);
                query.Where(x => x.DependenceArgument == true);
                return query.AsEnumerable();
            });
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
        /// 保存后台服务实例对象。
        /// </summary>
        /// <param name="task">后台服务实例对象。</param>
        public async Task SaveAsync(TaskDescriptor task)
        {
            await _repository.CreateAsync(task);
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
        /// 获取当前任务可执行的参数。
        /// </summary>
        /// <param name="taskId">任务Id。</param>
        /// <returns>返回任务参数。</returns>
        public async Task<TaskArgument> GetArgumentAsync(int taskId)
        {
            return await _args.AsQueryable()
                 .Where(x => x.TaskId == taskId && x.Status == ArgumentStatus.Normal)
                 .SingleOrDefaultAsync();
        }

        /// <summary>
        /// 分页加载参数。
        /// </summary>
        /// <param name="query">参数查询实例。</param>
        /// <returns>返回查询结果。</returns>
        public Task<ArgumentQuery> LoadArgumentsAsync(ArgumentQuery query)
        {
            return _args.LoadAsync(query);
        }

        /// <summary>
        /// 保存参数。
        /// </summary>
        /// <param name="argument">参数实例对象。</param>
        /// <returns>返回保存结果。</returns>
        public async Task<DataResult> SaveArgumentAsync(TaskArgument argument)
        {
            if (await _args.AnyAsync(x => x.TaskId == argument.TaskId && x.Argument == argument.Argument))
                return DataAction.Duplicate;
            if (argument.Id > 0)
                return DataResult.FromResult(await _args.UpdateAsync(argument), DataAction.Updated);
            return DataResult.FromResult(await _args.CreateAsync(argument), DataAction.Created);
        }

        /// <summary>
        /// 删除参数。
        /// </summary>
        /// <param name="ids">参数Id集合。</param>
        /// <returns>返回删除结果。</returns>
        public async Task<DataResult> DeleteArgumentAsync(string ids)
        {
            var ints = ids.SplitToInt32();
            return DataResult.FromResult(await _args.DeleteAsync(x => x.Id.Included(ints)), DataAction.Deleted);
        }

        private readonly ConcurrentDictionary<int, string> _names = new ConcurrentDictionary<int, string>();
        /// <summary>
        /// 获取任务名称。
        /// </summary>
        /// <param name="taskId">任务Id。</param>
        /// <returns>返回任务名称。</returns>
        public string GetTaskName(int taskId)
        {
            return _names.GetOrAdd(taskId, id =>
            {
                return _repository.Find(x => x.Id == id).Name;
            });
        }
        
        /// <summary>
        /// 执行参数后设置参数状态。
        /// </summary>
        /// <param name="id">参数ID。</param>
        /// <param name="times">执行次数。</param>
        /// <param name="message">错误消息。</param>
        /// <returns>返回执行任务。</returns>
        public Task SetArgumentAsync(long id, int times = 0, string message = null)
        {
            if (times <= 0)
                return _args.UpdateAsync(s => s.Id == id, new { TryTimes = 0, Status = ArgumentStatus.Completed });
            if (times < (int)ArgumentStatus.Failured)
                return _args.UpdateAsync(s => s.Id == id, new { TryTimes = times, Error = message });
            return _args.UpdateAsync(s => s.Id == id, new { TryTimes = times, Error = message, Status = ArgumentStatus.Failured });
        }

        /// <summary>
        /// 设置参数。
        /// </summary>
        /// <param name="id">参数ID。</param>
        /// <param name="argument">参数实例对象。</param>
        /// <returns>返回执行任务。</returns>
        public Task SetArgumentAsync(long id, Argument argument)
        {
           return _args.UpdateAsync(x => x.Id == id, new {Argument = argument.ToString()});
        }
    }
}