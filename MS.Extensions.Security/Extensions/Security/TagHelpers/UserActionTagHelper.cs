using Microsoft.AspNetCore.Razor.TagHelpers;
using Mozlite.Extensions.Security;
using Mozlite.Mvc.TagHelpers;
using MS.Extensions.Security.Actions;

namespace MS.Extensions.Security.TagHelpers
{
    /// <summary>
    /// 用户登陆后操作标签。
    /// </summary>
    [HtmlTargetElement("moz:user-action-panel")]
    public class UserActionTagHelper : ViewContextableTagHelperBase
    {
        private readonly IActionFactory _factory;
        /// <summary>
        /// 初始化类<see cref="UserActionTagHelper"/>。
        /// </summary>
        /// <param name="factory">当前提供者接口。</param>
        public UserActionTagHelper(IActionFactory factory)
        {
            _factory = factory;
        }

        /// <summary>
        /// 访问并呈现当前标签实例。
        /// </summary>
        /// <param name="context">当前HTML标签上下文，包含当前HTML相关信息。</param>
        /// <param name="output">当前标签输出实例，用于呈现标签相关信息。</param>
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (HttpContext.Request.Method == "GET" && HttpContext.User.Identity.IsAuthenticated)
            {
                var user = HttpContext.GetUser<User>();
                if (user != null && _factory.TryGetProvider(user.Action, out var provider))
                {
                    provider.Invoke(ViewContext);
                }
            }
            output.SuppressOutput();
        }
    }
}