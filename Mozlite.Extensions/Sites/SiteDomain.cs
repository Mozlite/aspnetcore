using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mozlite.Extensions.Sites
{
    /// <summary>
    /// 网站域名。
    /// </summary>
    [Table("core_Sites")]
    public class SiteDomain
    {
        /// <summary>
        /// 网站配置Id。
        /// </summary>
        [Key]
        public int SiteId { get; set; }

        /// <summary>
        /// 网站域名。
        /// </summary>
        [Key]
        [Size(64)]
        public string Domain { get; set; }
    }
}