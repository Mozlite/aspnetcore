using System;
using System.IO;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Mozlite.Mvc.Templates
{
    /// <summary>
    /// 语法管理接口。
    /// </summary>
    public interface ISyntaxManager : ISingletonService
    {
        /// <summary>
        /// 解析模板语法。
        /// </summary>
        /// <param name="source">模板源代码。</param>
        /// <returns>返回当前文档实例。</returns>
        Syntax Parse(string source);

        /// <summary>
        /// 加载模板文件。
        /// </summary>
        /// <param name="path">文件物理路径。</param>
        /// <returns>返回当前文档实例。</returns>
        Syntax Load(string path);

        /// <summary>
        /// 将文档<paramref name="syntax"/>解析写入到写入器中。
        /// </summary>
        /// <param name="syntax">当前文档实例。</param>
        /// <param name="writer">写入器实例对象。</param>
        /// <param name="viewData">当前模型实例。</param>
        void Write(Syntax syntax, TextWriter writer, Action<ViewDataDictionary> viewData);

        /// <summary>
        /// 获取呈现的代码字符串。
        /// </summary>
        /// <param name="syntax">当前文档实例。</param>
        /// <param name="viewData">当前模型实例。</param>
        /// <returns>返回呈现的代码字符串。</returns>
        string Render(Syntax syntax, Action<ViewDataDictionary> viewData);
    }
}