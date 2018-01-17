using System;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace Mozlite.Extensions.Sites
{
    /// <summary>
    /// 网站信息实例基类。
    /// </summary>
    [Table("core_Sites")]
    public class SiteBase
    {
        /// <summary>
        /// 网站Id。
        /// </summary>
        [Identity]
        [JsonIgnore]
        public int SiteId { get; set; }

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