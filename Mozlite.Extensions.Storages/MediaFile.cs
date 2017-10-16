using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mozlite.Extensions.Storages
{
    /// <summary>
    /// 媒体文件。
    /// </summary>
    [Table("core_Storages")]
    public class MediaFile
    {
        /// <summary>
        /// 文件唯一Id，会暴露给URL地址。
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        /// <summary>
        /// 实体文件Id。
        /// </summary>
        [Size(32)]
        public string FileId { get; set; }

        /// <summary>
        /// 名称。
        /// </summary>
        [Size(256)]
        public string Name { get; set; }

        /// <summary>
        /// 后缀名。
        /// </summary>
        [Size(32)]
        public string Extension { get; set; }

        /// <summary>
        /// 文件名称。
        /// </summary>
        public string FileName => Id.ToString("N") + Extension;

        /// <summary>
        /// 扩展名称。
        /// </summary>
        [Size(32)]
        public string ExtensionName { get; set; }

        /// <summary>
        /// 大小。
        /// </summary>
        public long Length { get; set; }

        /// <summary>
        /// 添加日期。
        /// </summary>
        public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.Now;

        /// <summary>
        /// 访问地址。
        /// </summary>
        public string Url => $"/{ExtensionName}-medias/{FileName}".ToLower();

        /// <summary>
        /// 访问地址。
        /// </summary>
        public string DownloadUrl => $"/{ExtensionName}-attachments/{FileName}".ToLower();
    }
}