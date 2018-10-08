using Microsoft.AspNetCore.Mvc.Rendering;
using Mozlite;

namespace MS.Extensions.Security.Actions
{
    /// <summary>
    /// 操作提供者。
    /// </summary>
    public interface IActionProvider : IServices
    {
        /// <summary>
        /// 操作唯一码。
        /// </summary>
        int Action { get; }

        /// <summary>
        /// 名称。
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 执行当前操作。
        /// </summary>
        /// <param name="context">HTTP上下文。</param>
        void Invoke(ViewContext context);
    }
}