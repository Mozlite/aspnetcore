using System;
using System.IO;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Mozlite.Mvc.Templates
{
    /// <summary>
    /// 语法写入器。
    /// </summary>
    public interface ISyntaxWriter : ISingletonServices
    {
        /// <summary>
        /// 名称。
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 将文档<paramref name="syntax"/>解析写入到写入器中。
        /// </summary>
        /// <param name="syntax">当前文档实例。</param>
        /// <param name="writer">写入器实例对象。</param>
        /// <param name="model">当前模型实例。</param>
        /// <param name="write">写入子项目。</param>
        void Write(Syntax syntax, TextWriter writer, ViewDataDictionary model, Action<Syntax, TextWriter, ViewDataDictionary> write);
    }
}