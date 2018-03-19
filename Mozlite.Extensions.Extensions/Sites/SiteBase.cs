using System;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mozlite.Extensions.Sites
{
    /// <summary>
    /// 网站信息实例基类。
    /// </summary>
    [Table("core_Sites")]
    public class SiteBase
    {
        /// <summary>
        /// 网站Id。
        /// </summary>
        [Identity]
        [JsonIgnore]
        public int SiteId { get; set; }

        /// <summary>
        /// 网站唯一键。
        /// </summary>
        [Size(64)]
        [JsonIgnore]
        public string SiteKey { get; set; }

        /// <summary>
        /// 网站名称。
        /// </summary>
        [Size(64)]
        [JsonIgnore]
        public string SiteName { get; set; }

        /// <summary>
        /// 更新地址。
        /// </summary>
        [JsonIgnore]
        public DateTimeOffset UpdatedDate { get; set; } = DateTimeOffset.Now;

        /// <summary>
        /// 是否为管理网站。
        /// </summary>
        [JsonIgnore]
        public bool IsAdministrator { get; set; }

        /// <summary>
        /// 是否已经安装完成并初始化。
        /// </summary>
        [JsonIgnore]
        public bool IsInitialized { get; set; }
        
        /// <summary>
        /// 转换为存储实例。
        /// </summary>
        /// <returns>返回存储实例对象。</returns>
        public SiteAdapter ToStore()
        {
            var adapter = new SiteAdapter();
            adapter.SettingValue = JsonConvert.SerializeObject(this);
            adapter.SiteKey = SiteKey;
            adapter.SiteId = SiteId;
            adapter.SiteName = SiteName;
            adapter.UpdatedDate = UpdatedDate;
            adapter.IsAdministrator = IsAdministrator;
            adapter.IsInitialized = IsInitialized;
            return adapter;
        }
    }
}