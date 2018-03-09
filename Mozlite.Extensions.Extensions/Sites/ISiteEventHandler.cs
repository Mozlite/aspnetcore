using System.Threading.Tasks;

namespace Mozlite.Extensions.Sites
{
    /// <summary>
    /// 当新站添加/删除时候触发得事件。
    /// </summary>
    public interface ISiteEventHandler : ISingletonServices
    {
        /// <summary>
        /// 当网站添加后得处理方法。
        /// </summary>
        /// <param name="site">当前网站实例。</param>
        /// <returns>返回处理结果。</returns>
        bool OnCreate(SiteBase site);

        /// <summary>
        /// 当网站添加后得处理方法。
        /// </summary>
        /// <param name="site">当前网站实例。</param>
        /// <returns>返回处理结果。</returns>
        Task<bool> OnCreateAsync(SiteBase site);

        /// <summary>
        /// 当网站删除后得处理方法。
        /// </summary>
        /// <param name="siteId">当前网站Id。</param>
        /// <returns>返回处理结果。</returns>
        bool OnDelete(int siteId);

        /// <summary>
        /// 当网站删除后得处理方法。
        /// </summary>
        /// <param name="siteId">当前网站Id。</param>
        /// <returns>返回处理结果。</returns>
        Task<bool> OnDeleteAsync(int siteId);
    }
}