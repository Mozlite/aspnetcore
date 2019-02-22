using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Mozlite.Extensions.Messages;
using Mozlite.Extensions.Messages.Notifications;
using Mozlite.Extensions.Security.Activities;
using Mozlite.Extensions.Security.Stores;
using Mozlite.Extensions.Storages.Apis;

namespace Mozlite.Mvc.Apis
{
    /// <summary>
    /// API基类。
    /// </summary>
    [Route("api/[controller]")]
    public abstract class ControllerBase : Controller
    {
        private Version _version;
        /// <summary>
        /// 当前程序的版本。
        /// </summary>
        public Version Version => _version ?? (_version = Cores.Version);

        private ILocalizer _localizer;
        /// <summary>
        /// 本地化接口。
        /// </summary>
        public ILocalizer Localizer => _localizer ?? (_localizer = GetRequiredService<ILocalizer>());

        private ILogger _logger;
        /// <summary>
        /// 日志接口。
        /// </summary>
        protected virtual ILogger Logger
        {
            get
            {
                if (_logger == null)
                {
                    _logger = HttpContext.RequestServices.GetRequiredService<ILoggerFactory>()
                        .CreateLogger(GetType());
                }
                return _logger;
            }
        }

        /// <summary>
        /// 添加操作日志。
        /// </summary>
        /// <param name="message">日志消息。</param>
        /// <param name="args">格式化参数。</param>
        protected void Log(string message, params object[] args) => Logger.Info(EventId, message, args);

        /// <summary>
        /// 日志分类Id。
        /// </summary>
        protected virtual int EventId => 1;

        /// <summary>
        /// 获取注册的服务对象。
        /// </summary>
        /// <typeparam name="TService">服务类型或者接口。</typeparam>
        /// <returns>返回当前服务的实例对象。</returns>
        protected TService GetService<TService>()
        {
            return HttpContext.RequestServices.GetService<TService>();
        }

        /// <summary>
        /// 获取已经注册的服务对象。
        /// </summary>
        /// <typeparam name="TService">服务类型或者接口。</typeparam>
        /// <returns>返回当前服务的实例对象。</returns>
        protected TService GetRequiredService<TService>()
        {
            return HttpContext.RequestServices.GetRequiredService<TService>();
        }
        
        /// <summary>
        /// 判断验证码。
        /// </summary>
        /// <param name="key">当前唯一键。</param>
        /// <param name="code">验证码。</param>
        /// <returns>返回判断结果。</returns>
        protected virtual bool IsValidateCode(string key, string code)
        {
            if (string.IsNullOrEmpty(code) || !Request.Cookies.TryGetValue(key, out var value))
                return false;
            code = Verifiers.Hashed(code);
            return string.Equals(value, code, StringComparison.OrdinalIgnoreCase);
        }

        private INotifier _notifier;
        /// <summary>
        /// 通知接口实例。
        /// </summary>
        protected INotifier Notifier => _notifier ?? (_notifier = GetRequiredService<INotifier>());
        
        /// <summary>
        /// 发送电子邮件。
        /// </summary>
        /// <param name="user">用户实例。</param>
        /// <param name="resourceKey">资源键：<paramref name="resourceKey"/>_{Title}，<paramref name="resourceKey"/>_{Content}。</param>
        /// <param name="replacement">替换对象，使用匿名类型实例。</param>
        /// <param name="action">实例化方法。</param>
        /// <returns>返回发送结果。</returns>
        protected bool SendEmail(UserBase user, string resourceKey, object replacement = null, Action<Email> action = null) =>
            GetRequiredService<IMessageManager>().SendEmail(user, resourceKey, replacement, GetType(), action);

        /// <summary>
        /// 发送电子邮件。
        /// </summary>
        /// <param name="user">用户实例。</param>
        /// <param name="resourceKey">资源键：<paramref name="resourceKey"/>_{Title}，<paramref name="resourceKey"/>_{Content}。</param>
        /// <param name="replacement">替换对象，使用匿名类型实例。</param>
        /// <param name="action">实例化方法。</param>
        /// <returns>返回发送结果。</returns>
        protected Task<bool> SendEmailAsync(UserBase user, string resourceKey, object replacement = null, Action<Email> action = null) =>
            GetRequiredService<IMessageManager>().SendEmailAsync(user, resourceKey, replacement, GetType(), action);
    }
}