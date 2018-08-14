using System;
using System.IO;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Mozlite.Mvc.Templates
{
    /// <summary>
    /// 语法写入器基类。
    /// </summary>
    /// <typeparam name="TSyntax">语法类型。</typeparam>
    public abstract class SyntaxWriter<TSyntax> : ISyntaxWriter
        where TSyntax : Syntax
    {
        /// <summary>
        /// 初始化类<see cref="SyntaxWriter{TSyntax}"/>。
        /// </summary>
        /// <param name="name">当前语法名词。</param>
        protected SyntaxWriter(string name)
        {
            Name = name;
        }

        /// <summary>
        /// 名称。
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 将文档<paramref name="syntax"/>解析写入到写入器中。
        /// </summary>
        /// <param name="syntax">当前文档实例。</param>
        /// <param name="writer">写入器实例对象。</param>
        /// <param name="model">当前模型实例。</param>
        /// <param name="write">写入子项目。</param>
        public void Write(Syntax syntax, TextWriter writer, ViewDataDictionary model, Action<Syntax, TextWriter, ViewDataDictionary> write)
        {
            if (syntax is TSyntax current)
            {
                if (model == null)
                    WriteRazor(current, writer, write);
                else
                    WriteHtml(current, writer, model, write);
            }
        }

        /// <summary>
        /// 将文档<paramref name="current"/>解析写入到写入器中。
        /// </summary>
        /// <param name="current">当前文档实例。</param>
        /// <param name="writer">写入器实例对象。</param>
        /// <param name="write">写入子项目。</param>
        protected abstract void WriteRazor(TSyntax current, TextWriter writer, Action<Syntax, TextWriter, ViewDataDictionary> write);

        /// <summary>
        /// 将文档<paramref name="current"/>解析写入到写入器中。
        /// </summary>
        /// <param name="current">当前文档实例。</param>
        /// <param name="writer">写入器实例对象。</param>
        /// <param name="model">当前模型实例。</param>
        /// <param name="write">写入子项目。</param>
        protected abstract void WriteHtml(TSyntax current, TextWriter writer, ViewDataDictionary model, Action<Syntax, TextWriter, ViewDataDictionary> write);
    }
}