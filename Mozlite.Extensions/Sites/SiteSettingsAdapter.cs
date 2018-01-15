namespace Mozlite.Extensions.Sites
{
    /// <summary>
    /// 数据库保存实体。
    /// </summary>
    [Target(typeof(SiteSettingsBase))]
    public class SiteSettingsAdapter : SiteSettingsBase
    {
        /// <summary>
        /// 序列化字符串。
        /// </summary>
        public string SettingsJSON { get; set; }
    }
}