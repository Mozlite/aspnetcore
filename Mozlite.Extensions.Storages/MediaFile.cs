using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mozlite.Extensions.Storages
{
    /// <summary>
    /// 媒体文件。
    /// </summary>
    [Table("core_Storages_Medias")]
    public class MediaFile
    {
        /// <summary>
        /// 文件唯一Id，会暴露给URL地址。
        /// </summary>
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

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
        /// 关联Id。
        /// </summary>
        public int? TargetId { get; set; }

        /// <summary>
        /// 添加日期。
        /// </summary>
        public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.Now;

        /// <summary>
        /// 获取缩略图地址。
        /// </summary>
        /// <param name="width">宽度。</param>
        /// <param name="height">高度。</param>
        /// <returns>返回缩略图地址。</returns>
        public string GetThumbUrl(int width, int height) => $"/s-medias/{width}x{height}/{FileName}".ToLower();

        /// <summary>
        /// 访问地址。
        /// </summary>
        public string Url => $"/s-medias/{FileName}".ToLower();

        /// <summary>
        /// 访问地址。
        /// </summary>
        public string DownloadUrl => $"/s-download/{FileName}".ToLower();
    }
}