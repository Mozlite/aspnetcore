using System.Collections.Generic;

namespace Mozlite.Extensions.Storages.Epub
{
    /// <summary>
    /// 电子书实例。
    /// </summary>
    public class EpubFile
    {
        /// <summary>
        /// 电子书Id。
        /// </summary>
        public string BookId { get; set; }

        /// <summary>
        /// 封面图片。
        /// </summary>
        public string CoverImage { get; set; }

        /// <summary>
        /// 封面文件。
        /// </summary>
        public string CoverFile { get; set; }

        /// <summary>
        /// 都柏林核心元素集。
        /// </summary>
        public DublinCore DC { get; set; } 

        /// <summary>
        /// 元数据。
        /// </summary>
        public IDictionary<string, string> Metadata { get; set; } 

        /// <summary>
        /// 文件列表。
        /// </summary>
        public List<Manifest> Manifest { get; set; }
    }
}