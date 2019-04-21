using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Mozlite.Extensions.Storages.Epub
{
    /// <summary>
    /// Epub管理接口。
    /// </summary>
    public interface IEpubManager : ISingletonService
    {
        /// <summary>
        /// 加载电子书解压后得物理文件夹。
        /// </summary>
        /// <param name="bookId">电子书Id。</param>
        /// <returns>返回电子书组成配置文件实例。</returns>
        IEpubFile Create(string bookId);

        /// <summary>
        /// 加载电子书解压后得物理文件夹。
        /// </summary>
        /// <param name="bookId">电子书Id。</param>
        /// <returns>返回电子书组成配置文件实例。</returns>
        Task<IEpubFile> CreateAsync(string bookId);

        /// <summary>
        /// 分析电子书文件，返回章节和文章列表。
        /// </summary>
        /// <param name="uri">电子书URL地址。</param>
        /// <param name="encoding">字符编码。</param>
        /// <param name="title">标题格式："^.*(第[零一二三四五六七八九十百千0-9]+章)(.*?)\r?\n"。</param>
        /// <param name="minLength">内容最少字符数。</param>
        /// <returns>返回章节和文章列表。</returns>
        Task<IDictionary<string, string>> LoadFileAsync(Uri uri, Encoding encoding = null, Regex title = null, int minLength = 1000);

        /// <summary>
        /// 分析电子书文件，返回章节和文章列表。
        /// </summary>
        /// <param name="path">电子书TXT文件物理路径。</param>
        /// <param name="encoding">字符编码。</param>
        /// <param name="title">标题格式："^.*(第[零一二三四五六七八九十百千0-9]+章)(.*?)\r?\n"。</param>
        /// <param name="minLength">内容最少字符数。</param>
        /// <returns>返回章节和文章列表。</returns>
        Task<IDictionary<string, string>> LoadFileAsync(string path, Encoding encoding = null, Regex title = null, int minLength = 1000);

        /// <summary>
        /// 分析电子书文件，返回章节和文章列表。
        /// </summary>
        /// <param name="file">电子书表单文件。</param>
        /// <param name="encoding">字符编码。</param>
        /// <param name="title">标题格式："^.*(第[零一二三四五六七八九十百千0-9]+章)(.*?)\r?\n"。</param>
        /// <param name="minLength">内容最少字符数。</param>
        /// <returns>返回章节和文章列表。</returns>
        Task<IDictionary<string, string>> LoadFileAsync(IFormFile file, Encoding encoding = null, Regex title = null, int minLength = 1000);

        /// <summary>
        /// 获取电子书的物理文件实例。
        /// </summary>
        /// <param name="bookId">电子书Id。</param>
        /// <returns>返回电子书物理文件。</returns>
        StoredPhysicalFile Load(string bookId);
    }
}