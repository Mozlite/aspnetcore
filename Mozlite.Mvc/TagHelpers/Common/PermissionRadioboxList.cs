using System.Collections.Generic;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Mozlite.Extensions.Security.Permissions;
using Mozlite.Mvc.TagHelpers.Bootstrap;

namespace Mozlite.Mvc.TagHelpers.Common
{
    /// <summary>
    /// 权限当选框。
    /// </summary>
    [HtmlTargetElement("moz:permission-radioboxlist")]
    public class PermissionRadioboxList : RadioboxListTagHelper
    {
        private readonly ILocalizer _localizer;
        private readonly IPermissionManager _permissionManager;

        /// <summary>
        /// 角色Id。
        /// </summary>
        [HtmlAttributeName("roleid")]
        public int RoleId { get; set; }

        /// <summary>
        /// 权限Id。
        /// </summary>
        [HtmlAttributeName("permissionid")]
        public int PermissionId { get; set; }

        /// <summary>
        /// 初始化类<see cref="PermissionRadioboxList"/>。
        /// </summary>
        /// <param name="localizer">本地化实例。</param>
        /// <param name="permissionManager">权限管理接口。</param>
        public PermissionRadioboxList(ILocalizer localizer, IPermissionManager permissionManager)
        {
            _localizer = localizer;
            _permissionManager = permissionManager;
        }

        /// <summary>
        /// 初始化当前标签上下文。
        /// </summary>
        /// <param name="context">当前HTML标签上下文，包含当前HTML相关信息。</param>
        public override void Init(TagHelperContext context)
        {
            base.Init(context);
            Name = $"p-{RoleId}-{PermissionId}";
            Value = ((int)_permissionManager.GetPermissionValue(RoleId, PermissionId)).ToString();
        }

        /// <summary>
        /// 附加复选项目列表，文本/值。
        /// </summary>
        /// <param name="items">复选框项目列表实例。</param>
        protected override void Init(IDictionary<string, string> items)
        {
            items.Add(_localizer.GetString(PermissionValue.Allow), ((int)PermissionValue.Allow).ToString());
            items.Add(_localizer.GetString(PermissionValue.Deny), ((int)PermissionValue.Deny).ToString());
            items.Add(_localizer.GetString(PermissionValue.NotSet), ((int)PermissionValue.NotSet).ToString());
        }
    }
}