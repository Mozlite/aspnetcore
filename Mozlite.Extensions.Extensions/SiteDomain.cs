using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mozlite.Extensions.Extensions
{
    /// <summary>
    /// 网站域名。
    /// </summary>
    [Table("core_Sites_Domains")]
    public class SiteDomain
    {
        /// <summary>
        /// 网站信息实例Id。
        /// </summary>
        [Key]
        public int SiteId { get; set; }

        /// <summary>
        /// 网站域名。
        /// </summary>
        [Key]
        [Size(64)]
        public string Domain { get; set; }

        /// <summary>
        /// 是否为默认域名。
        /// </summary>
        public bool IsDefault { get; set; }

        /// <summary>
        /// 禁用。
        /// </summary>
        public bool Disabled { get; set; }
    }
}