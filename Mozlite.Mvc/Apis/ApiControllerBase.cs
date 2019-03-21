using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Mozlite.Extensions.Messages;
using Mozlite.Extensions.Messages.Notifications;
using Mozlite.Extensions.Security.Events;
using Mozlite.Extensions.Security.Stores;
using Mozlite.Extensions.Storages.Apis;
using System;
using System.Threading.Tasks;
using Mozlite.Extensions.Settings;

namespace Mozlite.Mvc.Apis
{
    /// <summary>
    /// API基类。
    /// </summary>
    [Route("api/[controller]")]
    public abstract class ApiControllerBase : Controller
    {
        private CacheApplication _application;
        /// <summary>
        /// 当前应用程序实例，只有在使用<seealso cref="ApplicationAuthorizeAttribute"/>修饰才可使用当前属性。
        /// </summary>
        protected CacheApplication Application => _application ?? (_application = HttpContext.Items[typeof(CacheApplication)] as CacheApplication);

        /// <summary>
        /// 成功。
        /// </summary>
        protected ApiResult Succeeded() => ApiResult.Succeeded;

        /// <summary>
        /// 失败。
        /// </summary>
        protected ApiResult Failured() => ApiResult.Failured;

        /// <summary>
        /// 未知错误。
        /// </summary>
        protected ApiResult UnknownError() => ApiResult.UnknownError;

        /// <summary>
        /// 返回错误代码。
        /// </summary>
        /// <param name="code">错误码。</param>
        /// <returns>返回API结果。</returns>
        protected ApiResult Error(ErrorCode code) => new ApiResult(code);

        /// <summary>
        /// 返回错误代码。
        /// </summary>
        /// <param name="code">错误码。</param>
        /// <param name="msg">错误信息。</param>
        /// <returns>返回API结果。</returns>
        protected ApiResult Error(ErrorCode code, string msg) => new ApiResult(code, msg);

        /// <summary>
        /// 空参数。
        /// </summary>
        /// <param name="name">参数名称。</param>
        /// <returns>返回API结果。</returns>
        protected ApiResult NullParameter(string name) => new ApiResult(ErrorCode.NullParameter).Format(name);

        /// <summary>
        /// 参数错误。
        /// </summary>
        /// <param name="name">参数名称。</param>
        /// <returns>返回API结果。</returns>
        protected ApiResult InvalidParameter(string name) => new ApiResult(ErrorCode.InvalidParameter).Format(name);

        /// <summary>
        /// 数据实例对象。
        /// </summary>
        /// <param name="data">数据实例对象。</param>
        /// <returns>返回数据实例对象。</returns>
        protected ApiDataResult Data(object data) => new ApiDataResult(data);

        /// <summary>
        /// 未找到相关信息。
        /// </summary>
        /// <param name="name">信息名称。</param>
        /// <returns>返回API结果。</returns>
        protected ApiResult NotFound(string name) => new ApiResult(ErrorCode.NotFound, name);

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

        private IEventLogger _eventLogger;
        /// <summary>
        /// 日志接口。
        /// </summary>
        protected virtual IEventLogger EventLogger => _eventLogger ?? (_eventLogger = GetRequiredService<IEventLogger>());

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

        private IObjectDiffer _differ;
        /// <summary>
        /// 对象属性变更实例。
        /// </summary>
        protected IObjectDiffer Differ => _differ ?? (_differ = GetRequiredService<IObjectDiffer>());

        private ISettingDictionaryManager _settingDictionaryManager;
        /// <summary>
        /// 获取字典值。
        /// </summary>
        /// <param name="path">标识。</param>
        /// <returns>返回字典值。</returns>
        public string GetDictionaryValue(string path) =>
            (_settingDictionaryManager ?? (_settingDictionaryManager = GetRequiredService<ISettingDictionaryManager>()))
            .GetOrAddSettings(path);
    }
}