using System.IO;

namespace Mozlite.Extensions.Storages.Epub
{
    /// <summary>
    /// 文件。
    /// </summary>
    public class Manifest
    {
        private string _id;
        /// <summary>
        /// 文件Id。
        /// </summary>
        public string Id { get => _id ?? (_id = Href.Replace("/", ".").Replace("\\", ".").ToLower()); }

        /// <summary>
        /// 文件相对路径。
        /// </summary>
        public string Href { get; set; }

        private string _mediaType;
        /// <summary>
        /// 文件媒体类型。
        /// </summary>
        public string MediaType { get => _mediaType ?? (_mediaType = GetMediaType(Href)); }

        /// <summary>
        /// 是否为文件档案。
        /// </summary>
        public bool IsSpine { get; set; }

        /// <summary>
        /// 标题。
        /// </summary>
        public string Title { get; set; }

        private string GetMediaType(string fileName)
        {
            var extension = Path.GetExtension(fileName)?.ToLower();
            if (extension == ".html")
                return "application/xhtml+xml";
            return extension.GetContentType();
        }
    }
}