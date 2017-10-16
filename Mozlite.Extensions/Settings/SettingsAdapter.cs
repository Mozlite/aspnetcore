using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mozlite.Extensions.Settings
{
    /// <summary>
    /// 网站配置数据库操作适配器。
    /// </summary>
    [Table("core_SiteSettings")]
    public class SettingsAdapter
    {
        /// <summary>
        /// 网站配置ID。
        /// </summary>
        [Key]
        [Size(256)]
        public string SettingsId { get; set; }

        /// <summary>
        /// 配置的字符串或JSON格式化的字符串。
        /// </summary>
        public string SettingsValue { get; set; }
    }
}