using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions.Internal;

namespace Mozlite.Extensions.Security.Activities
{
    internal class ActivityLogger : ILogger
    {
        public static readonly EventId EventId = new EventId(0, "{:user<->activity:}");

        private readonly IActivityManagerBase _activityManager;

        public ActivityLogger(IActivityManagerBase activityManager)
        {
            _activityManager = activityManager;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel) || eventId.Name != EventId.Name)
                return;
            if (formatter == null)
                throw new ArgumentNullException(nameof(formatter));
            var message = formatter(state, null);
            if (string.IsNullOrWhiteSpace(message))
                return;
            _activityManager.Create(message);
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel == LogLevel.Information;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return NullScope.Instance;
        }
    }
}