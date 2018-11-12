using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Mozlite.Mvc.Messages;
using System;

namespace MS.Extensions.Security.Actions
{
    /// <summary>
    /// 强制要求用户修改密码。
    /// </summary>
    public class ChangePasswordActionProvider : ActionProvider
    {
        /// <summary>
        /// 修改密码提供者。
        /// </summary>
        public const int Provider = 1;

        /// <summary>
        /// 操作唯一码。
        /// </summary>
        public override int Action => Provider;

        /// <summary>
        /// 名称。
        /// </summary>
        public override string Name => "修改密码";

        /// <summary>
        /// 执行当前上下文。
        /// </summary>
        /// <param name="context">HTTP上下文。</param>
        protected override void Invoke(HttpContext context)
        {
            var url = Url.Page("/Account/ChangePassword", new { area = SecuritySettings.ExtensionName });
            if (url.Equals(context.Request.Path, StringComparison.OrdinalIgnoreCase))
                StatusMessage(StatusType.Warning, "请修改密码后才能够进行其他操作！");
            else
                context.Response.Redirect(url);
        }
    }
}