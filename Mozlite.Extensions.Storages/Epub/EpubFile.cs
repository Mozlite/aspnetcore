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
        public string BookId { get => DC.Identifier; set => DC.Identifier = value; }

        /// <summary>
        /// 都柏林核心元素集。
        /// </summary>
        public DublinCore DC { get; set; } = new DublinCore();

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