using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Mozlite.Mvc.Apis.Server
{
    /// <summary>
    /// 自动刷新功能。
    /// </summary>
    public class RefresherController : ControllerBase
    {
        /// <summary>
        /// 验证码。
        /// </summary>
        [HttpPost]
        [Route("js-refresher")]
        public IActionResult Index(string version)
        {
            try
            {
                var client = new Version(version);
                return Success(client != Version);
            }
            catch (Exception exception)
            {
                Logger.LogError(exception, $"自动刷新出现错误：{exception.Message}");
                return Success();
            }
        }
    }
}