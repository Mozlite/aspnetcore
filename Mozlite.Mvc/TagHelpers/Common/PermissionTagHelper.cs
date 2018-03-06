using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Mozlite.Extensions.Security.Permissions;

namespace Mozlite.Mvc.TagHelpers.Common
{
    /// <summary>
    /// 权限标签。
    /// </summary>
    [HtmlTargetElement("*", Attributes = AttributeName)]
    public class PermissionTagHelper : TagHelperBase
    {
        private readonly IPermissionManager _permissionManager;
        /// <summary>
        /// 初始化类<see cref="PermissionTagHelper"/>。
        /// </summary>
        /// <param name="permissionManager">权限管理接口。</param>
        public PermissionTagHelper(IPermissionManager permissionManager)
        {
            _permissionManager = permissionManager;
        }

        /// <summary>
        /// 排序。
        /// </summary>
        public override int Order => int.MaxValue;
        private const string AttributeName = ".permission";
        /// <summary>
        /// 权限名称。
        /// </summary>
        [HtmlAttributeName(AttributeName)]
        public string PermissionName { get; set; }

        /// <summary>
        /// 异步访问并呈现当前标签实例。
        /// </summary>
        /// <param name="context">当前HTML标签上下文，包含当前HTML相关信息。</param>
        /// <param name="output">当前标签输出实例，用于呈现标签相关信息。</param>
        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            if (string.IsNullOrWhiteSpace(PermissionName))
                return;
            if (await _permissionManager.IsAuthorizedAsync(PermissionName.Trim()))
                return;
            output.SuppressOutput();
        }
    }
}