namespace Mozlite.Mvc.TagHelpers.Templates
{
    /// <summary>
    /// 模板节点类型。
    /// </summary>
    public enum TemplateElementType
    {
        /// <summary>
        /// 文档，最高节点。
        /// </summary>
        Document,

        /// <summary>
        /// 文本。
        /// </summary>
        Text,

        /// <summary>
        /// HTML标签。
        /// </summary>
        Html,

        /// <summary>
        /// 代码“{{code}}”。
        /// </summary>
        Code,

        /// <summary>
        /// 语句（有结束符的代码块）“{{code/}}”，"{{code}}text{{/code}}"。
        /// </summary>
        Syntax,
    }
}