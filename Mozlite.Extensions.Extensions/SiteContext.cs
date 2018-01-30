using Mozlite.Extensions.Sites;

namespace Mozlite.Extensions
{
    /// <summary>
    /// 当前网站上下文基类。
    /// </summary>
    public abstract class SiteContextBase
    {
        /// <summary>
        /// 网站Id。
        /// </summary>
        public virtual int SiteId => Site?.SiteId ?? 0;

        /// <summary>
        /// 网站唯一键。
        /// </summary>
        public virtual string SiteKey => Site?.SiteKey;

        /// <summary>
        /// 网站名称。
        /// </summary>
        public virtual string SiteName => Site?.SiteName;

        /// <summary>
        /// 当前域名。
        /// </summary>
        public virtual string Domain { get; internal set; }

        /// <summary>
        /// 是否已经初始化。
        /// </summary>
        public virtual bool Initialized { get; internal set; }

        /// <summary>
        /// 是否为默认域名。
        /// </summary>
        public virtual bool IsDefault { get; internal set; }

        /// <summary>
        /// 是否禁用。
        /// </summary>
        public virtual bool Disabled { get; internal set; }

        /// <summary>
        /// 当前网站实例。
        /// </summary>
        public virtual SiteBase Site { get; internal set; }
    }

    /// <summary>
    /// 当前网站上下文基类。
    /// </summary>
    /// <typeparam name="TSite">网站类型。</typeparam>
    public abstract class SiteContextBase<TSite> : SiteContextBase
        where TSite : SiteBase, new()
    {
        /// <summary>
        /// 当前网站实例。
        /// </summary>
        public new virtual TSite Site
        {
            get => (TSite)base.Site;
            internal set => base.Site = value;
        }
    }
}