using System.Threading.Tasks;
using Mozlite.Data.Internal;

namespace Mozlite.Extensions.Extensions
{
    /// <summary>
    /// 当新站添加/删除时候触发得事件。
    /// </summary>
    public interface ISiteEventHandler : ISingletonServices
    {
        /// <summary>
        /// 当网站添加后得处理方法。
        /// </summary>
        /// <param name="db">数据库事务接口实例。</param>
        /// <param name="site">当前网站实例。</param>
        /// <returns>返回处理结果。</returns>
        bool OnCreate(IDbTransactionContext<SiteAdapter> db, SiteBase site);

        /// <summary>
        /// 当网站添加后得处理方法。
        /// </summary>
        /// <param name="db">数据库事务接口实例。</param>
        /// <param name="site">当前网站实例。</param>
        /// <returns>返回处理结果。</returns>
        Task<bool> OnCreateAsync(IDbTransactionContext<SiteAdapter> db, SiteBase site);

        /// <summary>
        /// 当网站删除后得处理方法。
        /// </summary>
        /// <param name="db">数据库事务接口实例。</param>
        /// <param name="siteId">当前网站Id。</param>
        /// <returns>返回处理结果。</returns>
        bool OnDelete(IDbTransactionContext<SiteAdapter> db, int siteId);

        /// <summary>
        /// 当网站删除后得处理方法。
        /// </summary>
        /// <param name="db">数据库事务接口实例。</param>
        /// <param name="siteId">当前网站Id。</param>
        /// <returns>返回处理结果。</returns>
        Task<bool> OnDeleteAsync(IDbTransactionContext<SiteAdapter> db, int siteId);
    }
}