namespace Mozlite.Mvc.TagHelpers.Templates
{
    /// <summary>
    /// HTML属性。
    /// </summary>
    public class TemplateHtmlAttribute
    {
        /// <summary>
        /// 属性名称。
        /// </summary>
        public string Name { get; protected set; }

        /// <summary>
        /// 属性值。
        /// </summary>
        public string Value { get; protected set; }

        /// <summary>
        /// 所属元素。
        /// </summary>
        public TemplateHtmlElement Element { get; set; }

        /// <summary>
        /// 类型。
        /// </summary>
        public TemplateHtmlAttributeType Type { get; }

        /// <summary>
        /// 初始化类<see cref="TemplateHtmlAttribute"/>。
        /// </summary>
        /// <param name="name">属性名称。</param>
        /// <param name="value">属性值。</param>
        /// <param name="type">类型。</param>
        public TemplateHtmlAttribute(string name, string value,
            TemplateHtmlAttributeType type = TemplateHtmlAttributeType.Text)
        {
            Name = name;
            Value = value;
            Type = type;
        }

        /// <summary>
        /// 初始化类<see cref="TemplateHtmlAttribute"/>。
        /// </summary>
        /// <param name="type">类型。</param>
        internal TemplateHtmlAttribute(TemplateHtmlAttributeType type)
        {
            Type = type;
        }

        /// <summary>返回表示当前对象的字符串。</summary>
        /// <returns>表示当前对象的字符串。</returns>
        public override string ToString()
        {
            return $"{Name}=\"{Value}\"";
        }
    }
}