using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Mozlite.Mvc.TagHelpers.Common
{
    /// <summary>
    /// 枚举下拉列表框。
    /// </summary>
    [HtmlTargetElement("moz:enum-dropdownlist")]
    public class EnumDropdownListTagHelper : DropdownListTagHelper
    {
        private readonly ILocalizer _localizer;
        public EnumDropdownListTagHelper(ILocalizer localizer)
        {
            _localizer = localizer;
        }

        /// <summary>
        /// 初始化选项列表。
        /// </summary>
        /// <returns>返回选项列表。</returns>
        protected override IEnumerable<SelectListItem> Init()
        {
            if (For != null)
                return GetEnumItems(For.ModelExplorer.ModelType);
            if (Value is Enum value)
                return GetEnumItems(value.GetType());
            throw new Exception("无法找到枚举类型，需要设置Value值或者使用For指定枚举属性！");
        }

        private IEnumerable<SelectListItem> GetEnumItems(Type type)
        {
            foreach (Enum value in Enum.GetValues(type))
            {
                yield return new SelectListItem
                {
                    Text = _localizer.GetString(value),
                    Value = value.ToString()
                };
            }
        }
    }
}