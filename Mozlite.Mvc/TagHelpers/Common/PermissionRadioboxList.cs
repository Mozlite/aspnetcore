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
        /// <summary>
        /// 初始化类<see cref="PermissionRadioboxList"/>。
        /// </summary>
        /// <param name="localizer">本地化实例。</param>
        public PermissionRadioboxList(ILocalizer localizer)
        {
            _localizer = localizer;
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