using System;
using Microsoft.AspNetCore.Mvc;

namespace Mozlite.Mvc.Controllers
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
            var client = new Version(version);
            return Success(client != Version);
        }
    }
}