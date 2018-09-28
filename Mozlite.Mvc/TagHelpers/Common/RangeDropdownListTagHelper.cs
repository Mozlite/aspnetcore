using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Collections.Generic;

namespace Mozlite.Mvc.TagHelpers.Common
{
    /// <summary>
    /// 范围下拉列表框。
    /// </summary>
    [HtmlTargetElement("moz:range-dropdownlist")]
    public class RangeDropdownListTagHelper : DropdownListTagHelper
    {
        private readonly ILocalizer _localizer;
        /// <summary>
        /// 初始化类<see cref="RangeDropdownListTagHelper"/>。
        /// </summary>
        /// <param name="localizer">本地化接口。</param>
        public RangeDropdownListTagHelper(ILocalizer localizer)
        {
            _localizer = localizer;
        }

        /// <summary>
        /// 开始。
        /// </summary>
        [HtmlAttributeName("from")]
        public int From { get; set; }

        /// <summary>
        /// 结束。
        /// </summary>
        [HtmlAttributeName("to")]
        public int To { get; set; }

        /// <summary>
        /// 资源键，资源格式为：ResourceKey_1。
        /// </summary>
        [HtmlAttributeName("resource")]
        public string ResourceKey { get; set; }

        /// <summary>
        /// 初始化选项列表。
        /// </summary>
        /// <returns>返回选项列表。</returns>
        protected override IEnumerable<SelectListItem> Init()
        {
            for (var i = From; i <= To; i++)
            {
                var text = i.ToString();
                if (ResourceKey != null)
                    text = _localizer.GetString(ResourceKey + "_" + i);
                yield return new SelectListItem(text, i.ToString());
            }
        }
    }
}