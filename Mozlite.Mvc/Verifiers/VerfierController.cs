using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Mozlite.Mvc.Verifiers
{
    /// <summary>
    /// 验证码。
    /// </summary>
    public class VerfierController : Microsoft.AspNetCore.Mvc.ControllerBase
    {
        /// <summary>
        /// 验证码。
        /// </summary>
        [AllowAnonymous]
        [Route("{key}-vcode.png")]
        public IActionResult Index(string key)
        {
            var ms = Verifiers.Create(out var code);
            Response.Cookies.Append(key, Verifiers.Hashed(code), new CookieOptions { Expires = DateTimeOffset.Now.AddMinutes(3) });
            Response.Body.Dispose();
            return File(ms.ToArray(), @"image/png");
        }
    }
}