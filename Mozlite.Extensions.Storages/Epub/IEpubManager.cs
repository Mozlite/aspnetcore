using System;
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
        /// 加载远程路径上得TXT文章。
        /// </summary>
        /// <param name="bookId">电子书Id。</param>
        /// <param name="uri">地址。</param>
        /// <param name="encoding">字符编码。</param>
        /// <param name="title">标题格式："^(第[零一二三四五六七八九十百千0-9]+章)(.*?)\r?\n"。</param>
        /// <returns>返回电子书实例。</returns>
        Task<IEpubFile> LoadUriAsync(string bookId, Uri uri, Encoding encoding = null, Regex title = null);

        /// <summary>
        /// 加载文章。
        /// </summary>
        /// <param name="bookId">电子书Id。</param>
        /// <param name="path">文本文件物理路径。</param>
        /// <param name="encoding">字符编码。</param>
        /// <param name="title">标题格式："^(第[零一二三四五六七八九十百千0-9]+章)(.*?)\r?\n"。</param>
        /// <returns>返回电子书实例。</returns>
        Task<IEpubFile> LoadFileAsync(string bookId, string path, Encoding encoding = null, Regex title = null);

        /// <summary>
        /// 加载文章。
        /// </summary>
        /// <param name="bookId">电子书Id。</param>
        /// <param name="content">内容。</param>
        /// <param name="title">标题格式："^(第[零一二三四五六七八九十百千0-9]+章)(.*?)\r?\n"。</param>
        /// <returns>返回电子书实例。</returns>
        IEpubFile Load(string bookId, string content, Regex title = null);
    }
}