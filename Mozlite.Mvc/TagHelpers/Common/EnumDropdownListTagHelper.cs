using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Mozlite.Mvc.Properties;
using System;
using System.Collections.Generic;

namespace Mozlite.Mvc.TagHelpers.Common
{
    /// <summary>
    /// 枚举下拉列表框。
    /// </summary>
    [HtmlTargetElement("moz:enum-dropdownlist")]
    public class EnumDropdownListTagHelper : DropdownListTagHelper
    {
        private readonly ILocalizer _localizer;
        /// <summary>
        /// 初始化类<see cref="EnumDropdownListTagHelper"/>。
        /// </summary>
        /// <param name="localizer">本地化接口。</param>
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
            if (type.IsNullableType())
            {
                if (DefaultText == null)
                    DefaultText = Resources.SelectDefaultText;
                type = Nullable.GetUnderlyingType(type);
            }
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