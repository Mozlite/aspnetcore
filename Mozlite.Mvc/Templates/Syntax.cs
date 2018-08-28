using System.Collections;
using System.Collections.Generic;
using System.Text;
using Mozlite.Mvc.Templates.Declarings;

namespace Mozlite.Mvc.Templates
{
    /// <summary>
    /// 语法。
    /// </summary>
    public abstract class Syntax : IEnumerable<Syntax>
    {
        /// <summary>
        /// 父级语法。
        /// </summary>
        public Syntax Parent { get; set; }

        /// <summary>
        /// 名称。
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 将<paramref name="syntax"/>添加到尾部。
        /// </summary>
        /// <param name="syntax">当前函数。</param>
        public virtual void Append(Syntax syntax)
        {
            IsBlock = true;
            syntax.Parent = this;
            syntax._indent = _indent + 1;
            _children.Add(syntax);
        }

        private int _indent;
        /// <summary>
        /// 缩进字符串。
        /// </summary>
        /// <param name="indent">缩进字符串。</param>
        /// <returns>返回缩进的字符串。</returns>
        public string Indent(string indent = "\t")
        {
            var builder = new StringBuilder();
            for (var i = 1; i < _indent; i++)
            {
                builder.Append(indent);
            }
            return builder.ToString();
        }

        /// <summary>
        /// 将<paramref name="syntaxs"/>添加到尾部。
        /// </summary>
        /// <param name="syntaxs">当前函数列表。</param>
        public virtual void Append(IEnumerable<Syntax> syntaxs)
        {
            foreach (var syntax in syntaxs)
            {
                Append(syntax);
            }
        }

        /// <summary>
        /// 在特定的位置插入<paramref name="syntax"/>。
        /// </summary>
        /// <param name="index">当前位置。</param>
        /// <param name="syntax">当前函数。</param>
        public virtual void Insert(int index, Syntax syntax)
        {
            IsBlock = true;
            syntax.Parent = this;
            syntax._indent = _indent + 1;
            _children.Insert(index, syntax);
        }

        /// <summary>
        /// 移除当前索引的子语法项。
        /// </summary>
        /// <param name="index">当前索引位置。</param>
        public virtual void Remove(int index)
        {
            _children.RemoveAt(index);
        }

        /// <summary>
        /// 是否为块语法。
        /// </summary>
        public bool IsBlock { get; internal set; }

        private readonly IList<Syntax> _children = new List<Syntax>();

        /// <summary>
        /// 迭代器。
        /// </summary>
        public virtual IEnumerator<Syntax> GetEnumerator()
        {
            return _children.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// 声明语法。
        /// </summary>
        public List<Declaring> Declarings { get; } = new List<Declaring>();

        /// <summary>
        /// 添加声明。
        /// </summary>
        /// <param name="declarings">添加声明列表。</param>
        public virtual void Append(IEnumerable<Declaring> declarings)
        {
            foreach (var declaring in declarings)
            {
                declaring.Parent = this;
                Declarings.Add(declaring);
            }
        }

        /// <summary>
        /// 当前语法的呈现字符串。
        /// </summary>
        /// <returns>返回当前语法的呈现字符串。</returns>
        public override string ToString()
        {
            return Name;
        }
    }
}