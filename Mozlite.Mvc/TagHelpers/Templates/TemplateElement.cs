using System.Collections;
using System.Collections.Generic;

namespace Mozlite.Mvc.TagHelpers.Templates
{
    /// <summary>
    /// 包含子节点的节点基类。
    /// </summary>
    public abstract class TemplateElement : TemplateElementBase, IEnumerable<TemplateElementBase>
    {
        private readonly List<TemplateElementBase> _elements = new List<TemplateElementBase>();
        /// <summary>
        /// 初始化类<see cref="TemplateElement"/>。
        /// </summary>
        /// <param name="position">位置。</param>
        /// <param name="type">类型。</param>
        protected TemplateElement(int position, TemplateElementType type) : base(position, type)
        {
        }

        /// <summary>
        /// 添加子节点。
        /// </summary>
        /// <param name="element">节点实例。</param>
        public void Add(TemplateElementBase element)
        {
            element.Parent = this;
            _elements.Add(element);
        }

        /// <summary>
        /// 添加子节点。
        /// </summary>
        /// <param name="elements">节点实例列表。</param>
        public void AddRange(IEnumerable<TemplateElementBase> elements)
        {
            foreach (var element in elements)
            {
                Add(element);
            }
        }

        /// <summary>返回一个循环访问集合的枚举器。</summary>
        /// <returns>用于循环访问集合的枚举数。</returns>
        public IEnumerator<TemplateElementBase> GetEnumerator()
        {
            return _elements.GetEnumerator();
        }

        /// <summary>返回循环访问集合的枚举数。</summary>
        /// <returns>可用于循环访问集合的 <see cref="T:System.Collections.IEnumerator" /> 对象。</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>返回表示当前对象的字符串。</summary>
        /// <returns>表示当前对象的字符串。</returns>
        public override string ToString()
        {
            return string.Join("\r\n", this);
        }
    }
}