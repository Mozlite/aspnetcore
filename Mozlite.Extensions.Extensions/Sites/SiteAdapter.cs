namespace Mozlite.Extensions.Sites
{
    /// <summary>
    /// 数据库保存实体。
    /// </summary>
    [Target(typeof(SiteBase))]
    public class SiteAdapter : SiteBase
    {
        /// <summary>
        /// 序列化字符串。
        /// </summary>
        public string SettingValue { get; set; }
    }
}