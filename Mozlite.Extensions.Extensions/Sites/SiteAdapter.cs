using Newtonsoft.Json;

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

        /// <summary>
        /// 转换为网站实例对象。
        /// </summary>
        /// <typeparam name="TSite">网站类型。</typeparam>
        /// <returns>返回网站实例对象。</returns>
        public TSite ToSite<TSite>()
            where TSite : SiteBase, new()
        {
            var site = string.IsNullOrWhiteSpace(SettingValue) ? new TSite()
                : JsonConvert.DeserializeObject<TSite>(SettingValue);
            site.SiteName = SiteName;
            site.SiteKey = SiteKey;
            site.UpdatedDate = UpdatedDate;
            site.SiteId = SiteId;
            return site;
        }
    }
}