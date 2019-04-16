﻿namespace Mozlite.Extensions.Storages.Epub
{
    /// <summary>
    /// Epub文件接口。
    /// </summary>
    public interface IEpubFile
    {
        /// <summary>
        /// 添加一个文本文件。
        /// </summary>
        /// <param name="fileName">文件名称。</param>
        /// <param name="content">文件内容。</param>
        /// <param name="title">标题，如果是文档文件需要标题。</param>
        /// <param name="isSpine">是否为文件档案。</param>
        void AddContent(string fileName, string content, string title, bool isSpine = true);

        /// <summary>
        /// 添加一个文件。
        /// </summary>
        /// <param name="fileName">文件名称。</param>
        /// <param name="path">物理路径。</param>
        void AddFile(string fileName, string path);

        /// <summary>
        /// 添加一个文件。
        /// </summary>
        /// <param name="fileName">文件名称。</param>
        /// <param name="path">物理路径。</param>
        /// <param name="title">标题，如果是文档文件需要标题。</param>
        void AddFile(string fileName, string path, string title);

        /// <summary>
        /// 编译成Epub文件，并返回物理路径。
        /// </summary>
        /// <param name="fileName">生成得文件路径。</param>
        void Save(string fileName);

        /// <summary>
        /// 编译成Epub文件，并返回物理路径。
        /// </summary>
        /// <param name="fileName">生成得文件路径。</param>
        void Compile(string fileName);

        /// <summary>
        /// 移除文件。
        /// </summary>
        /// <param name="fileName">文件名。</param>
        void RemoveFile(string fileName);

        /// <summary>
        /// 添加默认样式文件。
        /// </summary>
        void AddDefaultStyle();

        /// <summary>
        /// 添加默认模板内容。
        /// </summary>
        /// <param name="fileName">文件名。</param>
        /// <param name="content">内容。</param>
        /// <param name="title">标题。</param>
        void AddHtml(string fileName, string content, string title);
    }
}