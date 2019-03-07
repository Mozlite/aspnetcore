using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;

namespace Mozlite.Extensions.Security.Events
{
    /// <summary>
    /// 事件日志接口。
    /// </summary>
    public interface IEventLogger : ISingletonService
    {
        /// <summary>
        /// 添加事件日志。
        /// </summary>
        /// <param name="eventType">事件类型名称。</param>
        /// <param name="message">事件消息实例。</param>
        void Log(string eventType, EventMessage message);

        /// <summary>
        /// 添加事件日志。
        /// </summary>
        /// <param name="eventType">事件类型名称。</param>
        /// <param name="message">事件消息。</param>
        void Log(string eventType, string message);

        /// <summary>
        /// 添加事件日志。
        /// </summary>
        /// <param name="eventType">事件类型名称。</param>
        /// <param name="message">事件消息。</param>
        /// <param name="args">格式化参数。</param>
        void Log(string eventType, string message, params object[] args);

        /// <summary>
        /// 添加事件日志。
        /// </summary>
        /// <param name="userId">用户Id。</param>
        /// <param name="eventType">事件类型名称。</param>
        /// <param name="message">事件消息。</param>
        void Log(int userId, string eventType, string message);

        /// <summary>
        /// 添加事件日志。
        /// </summary>
        /// <param name="userId">用户Id。</param>
        /// <param name="eventType">事件类型名称。</param>
        /// <param name="message">事件消息。</param>
        /// <param name="args">格式化参数。</param>
        void Log(int userId, string eventType, string message, params object[] args);

        /// <summary>
        /// 添加事件日志。
        /// </summary>
        /// <param name="eventType">事件类型名称。</param>
        /// <param name="message">事件消息实例。</param>
        Task LogAsync(string eventType, EventMessage message);

        /// <summary>
        /// 添加事件日志。
        /// </summary>
        /// <param name="eventType">事件类型名称。</param>
        /// <param name="message">事件消息。</param>
        Task LogAsync(string eventType, string message);

        /// <summary>
        /// 添加事件日志。
        /// </summary>
        /// <param name="eventType">事件类型名称。</param>
        /// <param name="message">事件消息。</param>
        /// <param name="args">格式化参数。</param>
        Task LogAsync(string eventType, string message, params object[] args);

        /// <summary>
        /// 添加事件日志。
        /// </summary>
        /// <param name="userId">用户Id。</param>
        /// <param name="eventType">事件类型名称。</param>
        /// <param name="message">事件消息。</param>
        Task LogAsync(int userId, string eventType, string message);

        /// <summary>
        /// 添加事件日志。
        /// </summary>
        /// <param name="userId">用户Id。</param>
        /// <param name="eventType">事件类型名称。</param>
        /// <param name="message">事件消息。</param>
        /// <param name="args">格式化参数。</param>
        Task LogAsync(int userId, string eventType, string message, params object[] args);
        
        /// <summary>
        /// 添加用户事件日志。
        /// </summary>
        /// <param name="result">数据操作结果。</param>
        /// <param name="eventType">事件类型名称。</param>
        /// <param name="message">事件消息。</param>
        void LogResult(DataResult result, string eventType, string message);

        /// <summary>
        /// 添加用户事件日志。
        /// </summary>
        /// <param name="result">数据操作结果。</param>
        /// <param name="eventType">事件类型名称。</param>
        /// <param name="message">事件消息。</param>
        /// <param name="args">格式化参数。</param>
        void LogResult(DataResult result, string eventType, string message, params object[] args);

        /// <summary>
        /// 添加用户事件日志。
        /// </summary>
        /// <param name="result">数据操作结果。</param>
        /// <param name="eventType">事件类型名称。</param>
        /// <param name="message">事件消息。</param>
        Task LogResultAsync(DataResult result, string eventType, string message);

        /// <summary>
        /// 添加用户事件日志。
        /// </summary>
        /// <param name="result">数据操作结果。</param>
        /// <param name="eventType">事件类型名称。</param>
        /// <param name="message">事件消息。</param>
        /// <param name="args">格式化参数。</param>
        Task LogResultAsync(DataResult result, string eventType, string message, params object[] args);
    }
}