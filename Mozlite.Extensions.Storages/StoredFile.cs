using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mozlite.Extensions.Storages
{
    /// <summary>
    /// 存储实体文件。
    /// </summary>
    [Table("core_Storages")]
    public class StoredFile
    {
        /// <summary>
        /// MD5值。
        /// </summary>
        [Key]
        [Size(32)]
        public string FileId { get; set; }

        /// <summary>
        /// 大小。
        /// </summary>
        public long Length { get; set; }

        /// <summary>
        /// 内容类型。
        /// </summary>
        [Size(256)]
        public string ContentType { get; set; }

        private string _path;
        /// <summary>
        /// 媒体路径。
        /// </summary>
        public string Path
        {
            get
            {
                if (_path == null && FileId != null)
                    _path = FileId.MakedPath();
                return _path;
            }
        }
    }
}