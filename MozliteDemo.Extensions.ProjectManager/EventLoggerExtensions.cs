﻿using System.Threading.Tasks;
using Mozlite.Extensions;
using Mozlite.Extensions.Security.Events;
using MozliteDemo.Extensions.ProjectManager.Properties;

namespace MozliteDemo.Extensions.ProjectManager
{
    /// <summary>
    /// 日志接口扩展。
    /// </summary>
    public static class EventLoggerExtensions
    {
        /// <summary>
        /// 添加用户事件日志。
        /// </summary>
        /// <param name="logger">日志接口。</param>
        /// <param name="message">事件消息。</param>
        public static void LogPM(this IEventLogger logger, string message) => logger.Log(Resources.EventType, message);

        /// <summary>
        /// 添加用户事件日志。
        /// </summary>
        /// <param name="logger">日志接口。</param>
        /// <param name="message">事件消息。</param>
        /// <param name="args">格式化参数。</param>
        public static void LogPM(this IEventLogger logger, string message, params object[] args) => logger.Log(Resources.EventType, message, args);

        /// <summary>
        /// 添加用户事件日志。
        /// </summary>
        /// <param name="logger">日志接口。</param>
        /// <param name="result">数据操作结果。</param>
        /// <param name="message">事件消息。</param>
        public static void LogPMResult(this IEventLogger logger, DataResult result, string message) => logger.LogResult(result, Resources.EventType, message);

        /// <summary>
        /// 添加用户事件日志。
        /// </summary>
        /// <param name="logger">日志接口。</param>
        /// <param name="result">数据操作结果。</param>
        /// <param name="message">事件消息。</param>
        /// <param name="args">格式化参数。</param>
        public static void LogPMResult(this IEventLogger logger, DataResult result, string message, params object[] args) => logger.LogResult(result, Resources.EventType, message, args);

        /// <summary>
        /// 添加用户事件日志。
        /// </summary>
        /// <param name="logger">日志接口。</param>
        /// <param name="message">事件消息。</param>
        public static Task LogPMAsync(this IEventLogger logger, string message) => logger.LogAsync(Resources.EventType, message);

        /// <summary>
        /// 添加用户事件日志。
        /// </summary>
        /// <param name="logger">日志接口。</param>
        /// <param name="message">事件消息。</param>
        /// <param name="args">格式化参数。</param>
        public static Task LogPMAsync(this IEventLogger logger, string message, params object[] args) => logger.LogAsync(Resources.EventType, message, args);

        /// <summary>
        /// 添加用户事件日志。
        /// </summary>
        /// <param name="logger">日志接口。</param>
        /// <param name="result">数据操作结果。</param>
        /// <param name="message">事件消息。</param>
        public static Task LogPMResultAsync(this IEventLogger logger, DataResult result, string message) => logger.LogResultAsync(result, Resources.EventType, message);

        /// <summary>
        /// 添加用户事件日志。
        /// </summary>
        /// <param name="logger">日志接口。</param>
        /// <param name="result">数据操作结果。</param>
        /// <param name="message">事件消息。</param>
        /// <param name="args">格式化参数。</param>
        public static Task LogPMResultAsync(this IEventLogger logger, DataResult result, string message, params object[] args) => logger.LogResultAsync(result, Resources.EventType, message, args);
    }
}