namespace Mozlite.Extensions.Sites
{
    /// <summary>
    /// 当前网站上下文访问器。
    /// </summary>
    public interface ISiteContextAccessorBase : ISingletonService
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