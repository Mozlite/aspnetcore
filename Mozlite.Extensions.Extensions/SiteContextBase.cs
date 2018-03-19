using Mozlite.Extensions.Sites;

namespace Mozlite.Extensions
{
    /// <summary>
    /// 当前网站上下文接口。
    /// </summary>
    public abstract class SiteContextBase
    {
        /// <summary>
        /// 网站Id。
        /// </summary>
        public virtual int SiteId => Site.SiteId;

        /// <summary>
        /// 网站唯一键。
        /// </summary>
        public virtual string SiteKey => Site.SiteKey;

        /// <summary>
        /// 网站名称。
        /// </summary>
        public virtual string SiteName => Site.SiteName;

        /// <summary>
        /// 是否为管理网站。
        /// </summary>
        public virtual bool IsAdministrator => Site.IsAdministrator;

        /// <summary>
        /// 是否已经安装完成并初始化。
        /// </summary>
        public virtual bool IsInitialized => Site.IsInitialized;

        /// <summary>
        /// 当前域名。
        /// </summary>
        public SiteDomain Domain { get; internal set; }

        /// <summary>
        /// 当前网站实例。
        /// </summary>
        public SiteBase Site { get; internal set; }
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
        public new TSite Site
        {
            get => (TSite)base.Site;
            internal set => base.Site = value;
        }
    }
}