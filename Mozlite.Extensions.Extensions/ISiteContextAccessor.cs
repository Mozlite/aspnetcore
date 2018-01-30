using Mozlite.Extensions.Sites;

namespace Mozlite.Extensions
{
    /// <summary>
    /// 当前网站上下文访问器。
    /// </summary>
    /// <typeparam name="TSite">网站类型。</typeparam>
    /// <typeparam name="TSiteContext">网站上下文。</typeparam>
    public interface ISiteContextAccessor<TSite, TSiteContext> : ISiteContextAccessorBase
        where TSite : SiteBase, new()
        where TSiteContext : SiteContextBase<TSite>, new()
    {
        /// <summary>
        /// 获取当前网站上下文。
        /// </summary>
        new TSiteContext SiteContext { get; }

        /// <summary>
        /// 设置当前上下文实例。
        /// </summary>
        /// <param name="domain">域名地址，如果为空则从HTTP上下文中得到。</param>
        /// <returns>返回当前网站上下文实例。</returns>
        new TSiteContext CreateSiteContext(string domain = null);
    }
    
    /// <summary>
    /// 当前网站上下文访问器。
    /// </summary>
    public interface ISiteContextAccessorBase
    {
        /// <summary>
        /// 获取当前网站上下文。
        /// </summary>
        SiteContextBase SiteContext { get; }

        /// <summary>
        /// 设置当前上下文实例。
        /// </summary>
        /// <param name="domain">域名地址，如果为空则从HTTP上下文中得到。</param>
        /// <returns>返回当前网站上下文实例。</returns>
        SiteContextBase CreateSiteContext(string domain = null);
    }
}