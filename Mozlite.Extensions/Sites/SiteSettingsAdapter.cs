namespace Mozlite.Extensions.Sites
{
    /// <summary>
    /// 数据库保存实体。
    /// </summary>
    [Target(typeof(SiteSettings))]
    public class SiteSettingsAdapter : SiteSettings
    {
        [Size(256)]
        public string Type { get; set; }

        /// <summary>
        /// 序列化字符串。
        /// </summary>
        public string SettingsJSON { get; set; }
    }
}