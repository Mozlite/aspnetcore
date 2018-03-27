using System.ComponentModel.DataAnnotations;
using BaseSettingsAdapter = Mozlite.Extensions.Settings.SettingsAdapter;

namespace Mozlite.Extensions.Extensions.Settings
{
    /// <summary>
    /// 网站配置数据库操作适配器。
    /// </summary>
    [Target(typeof(BaseSettingsAdapter))]
    public class SettingsAdapter : BaseSettingsAdapter
    {
        /// <summary>
        /// 网站ID。
        /// </summary>
        [Key]
        public int SiteId { get; set; }
    }
}