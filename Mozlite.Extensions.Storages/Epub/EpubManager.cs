using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Mozlite.Extensions.Storages.Epub
{
    /// <summary>
    /// Epub管理实现类。
    /// </summary>
    public class EpubManager : IEpubManager
    {
        private readonly IStorageDirectory _storageDirectory;
        private readonly string _epubDirectory;

        public EpubManager(IStorageDirectory storageDirectory)
        {
            _storageDirectory = storageDirectory;
            _epubDirectory = storageDirectory.GetPhysicalPath("epub").MakeDirectory();
        }

        private string GetTempPath(string bookId)
        {
            var path = _storageDirectory.GetTempPath(bookId);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            return path;
        }

        /// <summary>
        /// 加载电子书解压后得物理文件夹。
        /// </summary>
        /// <param name="bookId">电子书Id。</param>
        /// <returns>返回电子书组成配置文件实例。</returns>
        public IEpubFile Create(string bookId)
        {
            var path = GetTempPath(bookId);
            var jsonPath = Path.Combine(path, "epub.json");
            if (File.Exists(jsonPath))
            {
                var file = Cores.FromJsonString<EpubFile>(StorageHelper.ReadText(jsonPath));
                if (file != null)
                    return new EpubFileEntry(file, path);
            }
            var instance = new EpubFile { BookId = bookId };
            StorageHelper.SaveText(jsonPath, instance.ToJsonString());
            return new EpubFileEntry(instance, path);
        }

        /// <summary>
        /// 加载电子书解压后得物理文件夹。
        /// </summary>
        /// <param name="bookId">电子书Id。</param>
        /// <returns>返回电子书组成配置文件实例。</returns>
        public async Task<IEpubFile> CreateAsync(string bookId)
        {
            var path = GetTempPath(bookId);
            var jsonPath = Path.Combine(path, "epub.json");
            if (File.Exists(jsonPath))
            {
                var file = Cores.FromJsonString<EpubFile>(await StorageHelper.ReadTextAsync(jsonPath));
                if (file != null)
                    return new EpubFileEntry(file, path);
            }
            var instance = new EpubFile { BookId = bookId };
            await StorageHelper.SaveTextAsync(jsonPath, instance.ToJsonString());
            return new EpubFileEntry(instance, path);
        }

        private static readonly Regex _regex = new Regex("^.*(第[零一二三四五六七八九十百千0-9]+章)(.*?)\r?\n", RegexOptions.Multiline);
        /// <summary>
        /// 分析电子书文件，返回章节和文章列表。
        /// </summary>
        /// <param name="uri">电子书URL地址。</param>
        /// <param name="encoding">字符编码。</param>
        /// <param name="title">标题格式："^.*(第[零一二三四五六七八九十百千0-9]+章)(.*?)\r?\n"。</param>
        /// <param name="minLength">内容最少字符数。</param>
        /// <returns>返回章节和文章列表。</returns>
        public async Task<IDictionary<string, string>> LoadFileAsync(Uri uri, Encoding encoding = null, Regex title = null, int minLength = 1000)
        {
            var file = await _storageDirectory.SaveToTempAsync(uri);
            return await LoadFileAsync(file.FullName, encoding, title, minLength);
        }

        /// <summary>
        /// 分析电子书文件，返回章节和文章列表。
        /// </summary>
        /// <param name="path">电子书TXT文件物理路径。</param>
        /// <param name="encoding">字符编码。</param>
        /// <param name="title">标题格式："^.*(第[零一二三四五六七八九十百千0-9]+章)(.*?)\r?\n"。</param>
        /// <param name="minLength">内容最少字符数。</param>
        /// <returns>返回章节和文章列表。</returns>
        public async Task<IDictionary<string, string>> LoadFileAsync(string path, Encoding encoding = null, Regex title = null, int minLength = 1000)
        {
            if (path.IndexOf(":\\") != 1)
                path = _storageDirectory.GetTempPath(path);
            encoding = encoding ?? StorageHelper.GetEncoding(path);
            var content = await StorageHelper.ReadTextAsync(path, encoding);
            var chapters = new Dictionary<string, string>();
            if (string.IsNullOrEmpty(content))
                return chapters;
            title = title ?? _regex;
            string chapter = null;
            foreach (Match match in title.Matches(content))
            {
                var current = match.Groups[0].Value;
                var index = content.IndexOf(current);
                var body = content.Substring(0, index);
                body = GetFormattedContent(body, minLength);
                content = content.Substring(index + current.Length);
                if (chapter != null && body != null)
                    chapters[chapter] = body;
                chapter = $"{match.Groups[1].Value.Trim()} {match.Groups[2].Value.Trim()}";
            }
            content = GetFormattedContent(content, minLength);
            if (chapter != null && content != null)
                chapters[chapter] = content;
            return chapters;
        }

        /// <summary>
        /// 分析电子书文件，返回章节和文章列表。
        /// </summary>
        /// <param name="file">电子书表单文件。</param>
        /// <param name="encoding">字符编码。</param>
        /// <param name="title">标题格式："^.*(第[零一二三四五六七八九十百千0-9]+章)(.*?)\r?\n"。</param>
        /// <param name="minLength">内容最少字符数。</param>
        /// <returns>返回章节和文章列表。</returns>
        public async Task<IDictionary<string, string>> LoadFileAsync(IFormFile file, Encoding encoding = null, Regex title = null, int minLength = 1000)
        {
            var info = await _storageDirectory.SaveToTempAsync(file);
            return await LoadFileAsync(info.FullName, encoding, title, minLength);
        }

        private string GetFormattedContent(string content, int minLength)
        {
            if (content.Length < minLength)
                return null;
            content = content.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).Where(x => x.Length > 0).Join("</p><p>");
            content = $"<p>{content}</p>";
            return content;
        }

        /// <summary>
        /// 获取电子书的物理文件实例。
        /// </summary>
        /// <param name="bookId">电子书Id。</param>
        /// <returns>返回电子书物理文件。</returns>
        public StoredPhysicalFile Load(string bookId)
        {
            var file = new StoredPhysicalFile();
            if (bookId.Length >= 3)
                file.PhysicalPath = Path.Combine(_epubDirectory, $"{bookId[0]}/{bookId[1]}/{bookId[2]}", bookId + ".moz");
            else if(bookId.Length>=2)
                file.PhysicalPath = Path.Combine(_epubDirectory, $"{bookId[0]}/{bookId[1]}", bookId + ".moz");
            else
                file.PhysicalPath = Path.Combine(_epubDirectory, $"{bookId[0]}", bookId + ".moz");
            file.FileName = $"{bookId}.epub";
            file.ContentType = "application/epub+zip";
            return file;
        }
    }
}