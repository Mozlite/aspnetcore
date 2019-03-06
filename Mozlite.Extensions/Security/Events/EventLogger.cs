using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Mozlite.Extensions.Properties;
using System;
using System.Threading.Tasks;

namespace Mozlite.Extensions.Security.Events
{
    /// <summary>
    /// 事件日志实现类。
    /// </summary>
    public class EventLogger : IEventLogger
    {
        private readonly IEventTypeManager _eventTypeManager;
        private readonly IEventManager _eventManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<EventLogger> _logger;

        /// <summary>
        /// 初始化类<see cref="EventLogger"/>。
        /// </summary>
        /// <param name="eventTypeManager">事件类型管理接口。</param>
        /// <param name="eventManager">事件管理接口。</param>
        /// <param name="httpContextAccessor">HTTP上下文访问器。</param>
        /// <param name="logger">日志接口。</param>
        public EventLogger(IEventTypeManager eventTypeManager, IEventManager eventManager, IHttpContextAccessor httpContextAccessor, ILogger<EventLogger> logger)
        {
            _eventTypeManager = eventTypeManager;
            _eventManager = eventManager;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        /// <summary>
        /// 添加事件日志。
        /// </summary>
        /// <param name="eventType">事件类型名称。</param>
        /// <param name="message">事件消息实例。</param>
        public void Log(string eventType, EventMessage message)
        {
            Init(eventType, message);
            _eventManager.Save(message);
            _logger.LogInformation(message.EventId, message.Message);
        }

        private void Init(string eventType, EventMessage message)
        {
            if (string.IsNullOrEmpty(message.Message))
                throw new ArgumentNullException(nameof(message.Message), Resources.EventMessage_NullMessage);

            if (message.EventId == 0)
            {
                var type = _eventTypeManager.Find(x => x.Name.Equals(eventType, StringComparison.OrdinalIgnoreCase));
                if (type == null)
                {
                    type = new EventType { Name = eventType };
                    _eventTypeManager.Save(type);
                }

                message.EventId = type.Id;
            }

            var httpContext = _httpContextAccessor.HttpContext;
            if (string.IsNullOrEmpty(message.IPAdress))
                message.IPAdress = httpContext.GetUserAddress();
            if (message.UserId == 0)
            {
                var userId = httpContext.User.GetUserId();
                if (userId == 0)
                    throw new ArgumentNullException(nameof(message.UserId), Resources.EventMessage_NullUserId);
                message.UserId = userId;
            }
        }

        /// <summary>
        /// 添加事件日志。
        /// </summary>
        /// <param name="eventType">事件类型名称。</param>
        /// <param name="message">事件消息实例。</param>
        public async Task LogAsync(string eventType, EventMessage message)
        {
            Init(eventType, message);
            await _eventManager.SaveAsync(message);
            _logger.LogInformation(message.EventId, message.Message);
        }

        /// <summary>
        /// 添加事件日志。
        /// </summary>
        /// <param name="eventType">事件类型名称。</param>
        /// <param name="message">事件消息。</param>
        public Task LogAsync(string eventType, string message) => LogAsync(0, eventType, message);

        /// <summary>
        /// 添加事件日志。
        /// </summary>
        /// <param name="eventType">事件类型名称。</param>
        /// <param name="message">事件消息。</param>
        /// <param name="args">格式化参数。</param>
        public Task LogAsync(string eventType, string message, params object[] args) => LogAsync(0, eventType, message, args);

        /// <summary>
        /// 添加事件日志。
        /// </summary>
        /// <param name="userId">用户Id。</param>
        /// <param name="eventType">事件类型名称。</param>
        /// <param name="message">事件消息。</param>
        public Task LogAsync(int userId, string eventType, string message)
        {
            var eventMessage = new EventMessage();
            eventMessage.Message = message;
            eventMessage.UserId = userId;
            return LogAsync(eventType, eventMessage);
        }

        /// <summary>
        /// 添加事件日志。
        /// </summary>
        /// <param name="userId">用户Id。</param>
        /// <param name="eventType">事件类型名称。</param>
        /// <param name="message">事件消息。</param>
        /// <param name="args">格式化参数。</param>
        public Task LogAsync(int userId, string eventType, string message, params object[] args)
        {
            if (args != null && args.Length > 0)
                message = string.Format(message, args);
            return LogAsync(userId, eventType, message);
        }

        /// <summary>
        /// 添加事件日志。
        /// </summary>
        /// <param name="eventType">事件类型名称。</param>
        /// <param name="message">事件消息。</param>
        public void Log(string eventType, string message) => Log(0, eventType, message);

        /// <summary>
        /// 添加事件日志。
        /// </summary>
        /// <param name="eventType">事件类型名称。</param>
        /// <param name="message">事件消息。</param>
        /// <param name="args">格式化参数。</param>
        public void Log(string eventType, string message, params object[] args) => Log(0, eventType, message, args);

        /// <summary>
        /// 添加事件日志。
        /// </summary>
        /// <param name="userId">用户Id。</param>
        /// <param name="eventType">事件类型名称。</param>
        /// <param name="message">事件消息。</param>
        public void Log(int userId, string eventType, string message)
        {
            var eventMessage = new EventMessage();
            eventMessage.Message = message;
            eventMessage.UserId = userId;
            Log(eventType, eventMessage);
        }

        /// <summary>
        /// 添加事件日志。
        /// </summary>
        /// <param name="userId">用户Id。</param>
        /// <param name="eventType">事件类型名称。</param>
        /// <param name="message">事件消息。</param>
        /// <param name="args">格式化参数。</param>
        public void Log(int userId, string eventType, string message, params object[] args)
        {
            if (args != null && args.Length > 0)
                message = string.Format(message, args);
            Log(userId, eventType, message);
        }
        
        /// <summary>
        /// 添加用户事件日志。
        /// </summary>
        /// <param name="result">数据操作结果。</param>
        /// <param name="eventType">事件类型名称。</param>
        /// <param name="message">事件消息。</param>
        public void LogResult(DataResult result, string eventType, string message)
        {
            if (!result) return;
            var action = ((DataAction)result.Code).ToString();
            action = Resources.ResourceManager.GetString($"DataResult_{action}");
            Log(eventType, $"{action} {message}");
        }

        /// <summary>
        /// 添加用户事件日志。
        /// </summary>
        /// <param name="result">数据操作结果。</param>
        /// <param name="eventType">事件类型名称。</param>
        /// <param name="message">事件消息。</param>
        /// <param name="args">格式化参数。</param>
        public void LogResult(DataResult result, string eventType, string message, params object[] args)
        {
            if (args != null && args.Length > 0)
                message = string.Format(message, args);
            LogResult(result, eventType, message);
        }

        /// <summary>
        /// 添加用户事件日志。
        /// </summary>
        /// <param name="result">数据操作结果。</param>
        /// <param name="eventType">事件类型名称。</param>
        /// <param name="message">事件消息。</param>
        public Task LogResultAsync(DataResult result, string eventType, string message)
        {
            if (!result) return Task.CompletedTask;
            var action = ((DataAction)result.Code).ToString();
            action = Resources.ResourceManager.GetString($"DataResult_{action}");
            return LogAsync(eventType, $"{action} {message}");
        }

        /// <summary>
        /// 添加用户事件日志。
        /// </summary>
        /// <param name="result">数据操作结果。</param>
        /// <param name="eventType">事件类型名称。</param>
        /// <param name="message">事件消息。</param>
        /// <param name="args">格式化参数。</param>
        public Task LogResultAsync(DataResult result, string eventType, string message, params object[] args)
        {
            if (args != null && args.Length > 0)
                message = string.Format(message, args);
            return LogResultAsync(result, eventType, message);
        }
    }
}