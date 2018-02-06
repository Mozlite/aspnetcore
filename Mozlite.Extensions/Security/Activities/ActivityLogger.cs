using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions.Internal;

namespace Mozlite.Extensions.Security.Activities
{
    internal class ActivityLogger : ILogger
    {
        private readonly IActivityManagerBase _activityManager;

        public ActivityLogger(IActivityManagerBase activityManager)
        {
            _activityManager = activityManager;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (eventId.Name != CategoryHelper.EventName)
                return;
            if (formatter == null)
                throw new ArgumentNullException(nameof(formatter));
            var message = formatter(state, null);
            if (string.IsNullOrWhiteSpace(message))
                return;
            _activityManager.Create(eventId.Id, message);
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return NullScope.Instance;
        }
    }
}