using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.DependencyInjection;
using Mozlite.Mvc.Messages;

namespace MS.Extensions.Security.Actions
{
    /// <summary>
    /// 操作基类。
    /// </summary>
    public abstract class ActionProvider : IActionProvider
    {
        /// <summary>
        /// 操作唯一码。
        /// </summary>
        public abstract int Action { get; }

        /// <summary>
        /// 名称。
        /// </summary>
        public abstract string Name { get; }

        private StatusMessage _statusMessage;
        /// <summary>
        /// 设置状态消息。
        /// </summary>
        /// <param name="message">消息内容。</param>
        protected void StatusMessage(string message) => StatusMessage(StatusType.Success, message);

        /// <summary>
        /// 设置状态消息。
        /// </summary>
        /// <param name="type">状态类型。</param>
        /// <param name="message">消息内容。</param>
        protected void StatusMessage(StatusType type, string message)
        {
            if (_statusMessage == null)
                throw new Exception("必须先执行Invoke才能够设置状态消息。");
            _statusMessage.Message = message;
            _statusMessage.Type = type;
        }

        /// <summary>
        /// 执行当前操作。
        /// </summary>
        /// <param name="context">HTTP上下文。</param>
        public void Invoke(ViewContext context)
        {
            _statusMessage = new StatusMessage(context.TempData);
            Url = context.HttpContext.RequestServices.GetRequiredService<IUrlHelperFactory>().GetUrlHelper(context);
            Invoke(context.HttpContext);
        }

        /// <summary>
        /// URL辅助接口。
        /// </summary>
        protected IUrlHelper Url { get; private set; }

        /// <summary>
        /// 执行当前上下文。
        /// </summary>
        /// <param name="context">HTTP上下文。</param>
        protected abstract void Invoke(HttpContext context);
    }
}