using System.Collections.Generic;

namespace Mozlite.Mvc.TagHelpers.Templates
{
    /// <summary>
    /// 代码属性，以moz:开头的属性。
    /// </summary>
    public class TemplateHtmlCodeAttribute : TemplateHtmlAttribute
    {
        /// <summary>
        /// 被影响的HTML属性名称。
        /// </summary>
        public string AttributeName { get; }

        private readonly List<string> _args = new List<string>();

        /// <summary>
        /// 初始化类<see cref="TemplateHtmlCodeAttribute"/>。
        /// </summary>
        /// <param name="name">属性名称。</param>
        /// <param name="value">属性值。</param>
        public TemplateHtmlCodeAttribute(string name, string value)
            : base(TemplateHtmlAttributeType.Code)
        {
            var index = name.IndexOf('-');
            if (index == -1)
                Name = name;
            else
            {
                Name = name.Substring(0, index);
                AttributeName = name.Substring(index + 1);
            }
            value = value.Trim();
            var condition = new TemplateString(value);
            Value = condition.ReadUntil(':', '?');
            while (condition.CanRead)
            {
                _args.Add(condition.ReadUntil(':'));
            }
        }

        /// <summary>返回表示当前对象的字符串。</summary>
        /// <returns>表示当前对象的字符串。</returns>
        public override string ToString()
        {
            if (AttributeName == null)
                return $"{Name}=\"{{{{{Value}}}}}\"";
            return $"{Name}-{AttributeName}=\"{{{{{Value}}}}}\"";
        }
        
        /// <summary>
        /// 参数个数。
        /// </summary>
        public int Count => _args.Count;

        /// <summary>
        /// 获取当前索引的参数值。
        /// </summary>
        /// <param name="index">当前索引值。</param>
        /// <returns>返回当前索引的参数值。</returns>
        public string this[int index]
        {
            get
            {
                if (index < Count) return _args[index];
                return null;
            }
        }
    }
}