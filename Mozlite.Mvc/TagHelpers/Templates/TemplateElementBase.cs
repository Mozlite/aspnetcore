using System;

namespace Mozlite.Mvc.TagHelpers.Templates
{
    /// <summary>
    /// 模板元素基类。
    /// </summary>
    public abstract class TemplateElementBase
    {
        /// <summary>
        /// 初始化类<see cref="TemplateElementBase"/>。
        /// </summary>
        /// <param name="position">位置。</param>
        /// <param name="type">类型。</param>
        protected TemplateElementBase(int position, TemplateElementType type)
        {
            Position = position;
            Type = type;
        }

        /// <summary>
        /// 在文档中的位置。
        /// </summary>
        public int Position { get; }

        /// <summary>
        /// 类型。
        /// </summary>
        public TemplateElementType Type { get; }

        /// <summary>
        /// 父级节点。
        /// </summary>
        public TemplateElement Parent { get; internal set; }

        private TemplateDocument _document;
        /// <summary>
        /// 文档节点。
        /// </summary>
        public TemplateDocument Document
        {
            get
            {
                if (_document == null)
                {
                    var element = this;
                    while (element.Type != TemplateElementType.Document)
                    {
                        element = element.Parent;
                    }
                    _document = element as TemplateDocument;
                }
                return _document;
            }
        }

        /// <summary>
        /// 生成脚本。
        /// </summary>
        /// <param name="executor">脚本语法解析器。</param>
        /// <returns>返回生成后的脚本。</returns>
        public abstract string ToJsString(ITemplateExecutor executor);

        /// <summary>
        /// 生成HTML代码。
        /// </summary>
        /// <param name="executor">语法解析器。</param>
        /// <param name="instance">当前实例。</param>
        /// <param name="func">获取实例属性值。</param>
        /// <returns>返回HTML代码。</returns>
        public abstract string ToHtmlString(ITemplateExecutor executor, object instance, Func<object, string, object> func);
    }
}