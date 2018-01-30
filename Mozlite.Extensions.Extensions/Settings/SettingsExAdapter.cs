using System.ComponentModel.DataAnnotations;

namespace Mozlite.Extensions.Settings
{
    /// <summary>
    /// 网站配置数据库操作适配器。
    /// </summary>
    [Target(typeof(SettingsAdapter))]
    public class SettingsExAdapter : SettingsAdapter
    {
        /// <summary>
        /// 网站ID。
        /// </summary>
        [Key]
        public int SiteId { get; set; }
    }
}