namespace Mozlite.Extensions.Extensions
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
        /// 设置当前上下文实例，后台现场中使用。
        /// </summary>
        /// <param name="siteKey">网站唯一键。</param>
        /// <returns>返回当前网站上下文实例。</returns>
        SiteContextBase GetThreadSiteContext(string siteKey = null);
    }
}