using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Mozlite.Mvc.TagHelpers;
using MS.Extensions.Security.Actions;
using System.Collections.Generic;

namespace MS.Extensions.Security.TagHelpers
{
    /// <summary>
    /// 用户登陆后操作标签。
    /// </summary>
    [HtmlTargetElement("moz:user-action-dropdownlist")]
    public class UserActionDropdownListTagHelper : DropdownListTagHelper
    {
        private readonly IActionFactory _factory;
        /// <summary>
        /// 初始化类<see cref="UserActionDropdownListTagHelper"/>。
        /// </summary>
        /// <param name="factory">当前提供者接口。</param>
        public UserActionDropdownListTagHelper(IActionFactory factory)
        {
            _factory = factory;
        }

        /// <summary>
        /// 初始化选项列表。
        /// </summary>
        /// <returns>返回选项列表。</returns>
        protected override IEnumerable<SelectListItem> Init()
        {
            var actions = _factory.LoadProviders();
            foreach (var action in actions)
            {
                yield return new SelectListItem(action.Name, action.Action.ToString());
            }
        }
    }
}