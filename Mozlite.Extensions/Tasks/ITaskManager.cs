using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mozlite.Extensions.Tasks
{
    /// <summary>
    /// 后台服务管理接口。
    /// </summary>
    public interface ITaskManager : ISingletonService
    {
        /// <summary>
        /// 加载所有后台服务。
        /// </summary>
        /// <returns>返回所有后台服务列表。</returns>
        Task<IEnumerable<TaskDescriptor>> LoadTasksAsync();

        /// <summary>
        /// 加载服务列表。
        /// </summary>
        /// <param name="extensionName">扩展名称。</param>
        /// <returns>返回服务描述列表。</returns>
        IEnumerable<TaskDescriptor> LoadArgumentTasks(string extensionName);

        /// <summary>
        /// 通过类型获取后台服务。
        /// </summary>
        /// <param name="type">任务<seealso cref="ITaskService"/>类型。</param>
        /// <returns>返回当前类型的服务对象。</returns>
        TaskDescriptor GeTask(Type type);

        /// <summary>
        /// 保存后台服务实例对象。
        /// </summary>
        /// <param name="task">后台服务实例对象。</param>
        Task SaveAsync(TaskDescriptor task);

        /// <summary>
        /// 设置时间间隔。
        /// </summary>
        /// <param name="id">服务Id。</param>
        /// <param name="interval">时间间隔。</param>
        /// <returns>返回设置结果。</returns>
        bool SetInterval(int id, TaskInterval interval);

        /// <summary>
        /// 设置执行时间。
        /// </summary>
        /// <param name="id">当前服务Id。</param>
        /// <param name="next">下一次执行时间。</param>
        /// <param name="last">上一次执行时间。</param>
        /// <returns>返回设置结果。</returns>
        Task<bool> SetExecuteDateAsync(int id, DateTime next, DateTime last);

        /// <summary>
        /// 保存错误日志。
        /// </summary>
        /// <param name="name">服务名称。</param>
        /// <param name="exception">错误实例。</param>
        void LogError(string name, Exception exception);

        /// <summary>
        /// 获取当前任务可执行的参数。
        /// </summary>
        /// <param name="taskId">任务Id。</param>
        /// <returns>返回任务参数。</returns>
        Task<TaskArgument> GetArgumentAsync(int taskId);

        /// <summary>
        /// 分页加载参数。
        /// </summary>
        /// <param name="query">参数查询实例。</param>
        /// <returns>返回查询结果。</returns>
        Task<ArgumentQuery> LoadArgumentsAsync(ArgumentQuery query);

        /// <summary>
        /// 保存参数。
        /// </summary>
        /// <param name="argument">参数实例对象。</param>
        /// <returns>返回保存结果。</returns>
        Task<DataResult> SaveArgumentAsync(TaskArgument argument);

        /// <summary>
        /// 删除参数。
        /// </summary>
        /// <param name="ids">参数Id集合。</param>
        /// <returns>返回删除结果。</returns>
        Task<DataResult> DeleteArgumentAsync(string ids);

        /// <summary>
        /// 获取任务名称。
        /// </summary>
        /// <param name="taskId">任务Id。</param>
        /// <returns>返回任务名称。</returns>
        string GetTaskName(int taskId);

        /// <summary>
        /// 执行参数后设置参数状态。
        /// </summary>
        /// <param name="id">参数ID。</param>
        /// <param name="times">执行次数。</param>
        /// <param name="message">错误消息。</param>
        /// <returns>返回执行任务。</returns>
        Task SetArgumentAsync(long id, int times = 0, string message = null);

        /// <summary>
        /// 设置参数。
        /// </summary>
        /// <param name="id">参数ID。</param>
        /// <param name="argument">参数实例对象。</param>
        /// <returns>返回执行任务。</returns>
        Task SetArgumentAsync(long id, Argument argument);
    }
}