namespace Mozlite.Extensions.Extensions
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
        /// 设置当前上下文实例，后台现场中使用。
        /// </summary>
        /// <param name="siteKey">网站唯一键。</param>
        /// <returns>返回当前网站上下文实例。</returns>
        new TSiteContext GetThreadSiteContext(string siteKey = null);
    }
}