using System;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace Mozlite.Extensions.Sites
{
    /// <summary>
    /// 网站配置基类。
    /// </summary>
    [Table("core_Sites_Settings")]
    public class SiteSettingsBase
    {
        /// <summary>
        /// 配置Id。
        /// </summary>
        [Identity]
        [JsonIgnore]
        public int SettingsId { get; set; }

        /// <summary>
        /// 网站名称。
        /// </summary>
        [Size(64)]
        [JsonIgnore]
        public string SiteName { get; set; }

        /// <summary>
        /// 更新地址。
        /// </summary>
        [JsonIgnore]
        public DateTimeOffset UpdatedDate { get; set; } = DateTimeOffset.Now;
    }
}