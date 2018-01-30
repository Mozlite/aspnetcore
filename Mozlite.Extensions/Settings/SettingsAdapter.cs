using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mozlite.Extensions.Settings
{
    /// <summary>
    /// 网站配置数据库操作适配器。
    /// </summary>
    [Table("core_Settings")]
    public class SettingsAdapter
    {
        /// <summary>
        /// 网站配置实例键。
        /// </summary>
        [Key]
        [Size(256)]
        public string SettingKey { get; set; }

        /// <summary>
        /// 配置的字符串或JSON格式化的字符串。
        /// </summary>
        public string SettingValue { get; set; }
    }
}