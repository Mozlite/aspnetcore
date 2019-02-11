using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;

namespace Mozlite.Mvc.TagHelpers.Bootstrap
{
    /// <summary>
    /// 枚举当选框列表。
    /// </summary>
    [HtmlTargetElement("moz:enum-radioboxlist")]
    public class EnumRadioboxListTagHelper : RadioboxListTagHelper
    {
        private readonly ILocalizer _localizer;
        /// <summary>
        /// 初始化类<see cref="EnumRadioboxListTagHelper"/>。
        /// </summary>
        /// <param name="localizer">本地化接口。</param>
        public EnumRadioboxListTagHelper(ILocalizer localizer)
        {
            _localizer = localizer;
        }

        /// <summary>
        /// 附加复选项目列表，文本/值。
        /// </summary>
        /// <param name="items">复选框项目列表实例。</param>
        protected override void Init(IDictionary<string, string> items)
        {
            if (For != null)
                Init(items, For.ModelExplorer.ModelType);
            else if (Value is Enum value)
                Init(items, value.GetType());
            else
                throw new Exception("无法找到枚举类型，需要设置Value值或者使用For指定枚举属性！");
        }

        private object _value;
        /// <summary>
        /// 当前值。
        /// </summary>
        [HtmlAttributeName("value")]
        public new object Value
        {
            get => _value;
            set
            {
                _value = value;
                base.Value = _value?.ToString();
            }
        }

        /// <summary>
        /// 最小值。
        /// </summary>
        [HtmlAttributeName("min")]
        public int MinValue { get; set; }

        private void Init(IDictionary<string, string> items, Type type)
        {
            if (type.IsNullableType())
                type = Nullable.GetUnderlyingType(type);
            foreach (Enum value in Enum.GetValues(type))
            {
                if (MinValue > (int)(object)value)
                    continue;
                items.Add(_localizer.GetString(value), value.ToString());
            }
        }
    }
}