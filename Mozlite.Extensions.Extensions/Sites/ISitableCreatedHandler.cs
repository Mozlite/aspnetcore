using System.Threading.Tasks;

namespace Mozlite.Extensions.Sites
{
    /// <summary>
    /// 当新站添加时候触发得事件。
    /// </summary>
    public interface ISitableCreatedHandler : ISingletonServices
    {
        /// <summary>
        /// 当网站添加后得处理方法。
        /// </summary>
        /// <param name="site">当前网站实例。</param>
        /// <returns>返回处理结果。</returns>
        void OnCreated(SiteBase site);

        /// <summary>
        /// 当网站添加后得处理方法。
        /// </summary>
        /// <param name="site">当前网站实例。</param>
        /// <returns>返回处理结果。</returns>
        Task OnCreatedAsync(SiteBase site);
    }
}