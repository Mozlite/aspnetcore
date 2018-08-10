using System;
using System.Collections.Generic;

namespace Mozlite.Mvc.Templates.Codings
{
    /// <summary>
    /// 源代码。
    /// </summary>
    public class SourceCodeSyntax : CodeSyntax
    {
        /// <summary>
        /// 源代码。
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// 将<paramref name="syntaxs"/>添加到尾部。
        /// </summary>
        /// <param name="syntaxs">当前函数列表。</param>
        public override void Append(IEnumerable<Syntax> syntaxs)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// 将<paramref name="syntax"/>添加到尾部。
        /// </summary>
        /// <param name="syntax">当前函数。</param>
        public override void Append(Syntax syntax)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// 移除当前索引的子语法项。
        /// </summary>
        /// <param name="index">当前索引位置。</param>
        public override void Remove(int index)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// 在特定的位置插入<paramref name="syntax"/>。
        /// </summary>
        /// <param name="index">当前位置。</param>
        /// <param name="syntax">当前函数。</param>
        public override void Insert(int index, Syntax syntax)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// 迭代器。
        /// </summary>
        public override IEnumerator<Syntax> GetEnumerator()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// 当前语法的呈现字符串。
        /// </summary>
        /// <returns>返回当前语法的呈现字符串。</returns>
        public override string ToString()
        {
            return Name + Source;
        }
    }
}