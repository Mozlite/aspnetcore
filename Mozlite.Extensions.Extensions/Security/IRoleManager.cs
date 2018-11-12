using System.Threading.Tasks;
using System.Collections.Generic;
using Mozlite.Extensions.Security.Stores;
using Mozlite.Extensions.Extensions.Security.Stores;

namespace Mozlite.Extensions.Extensions.Security
{
    /// <summary>
    /// 用户组管理接口。
    /// </summary>
    /// <typeparam name="TRole">用户组类型。</typeparam>
    /// <typeparam name="TUserRole">用户用户组类型。</typeparam>
    /// <typeparam name="TRoleClaim">用户组声明类型。</typeparam>
    public interface IRoleManager<TRole, TUserRole, TRoleClaim>
        : Mozlite.Extensions.Security.IRoleManager<TRole, TUserRole, TRoleClaim>
        where TRole : Stores.RoleBase
        where TUserRole : IUserRole
        where TRoleClaim : RoleClaimBase, new()
    {
        /// <summary>
        /// 通过用户组名称获取用户组实例。
        /// </summary>
        /// <param name="siteId">网站Id。</param>
        /// <param name="normalizedName">用户组名称。</param>
        /// <returns>返回当前用户组实例对象。</returns>
        TRole FindByName(int siteId, string normalizedName);

        /// <summary>
        /// 通过用户组名称获取用户组实例。
        /// </summary>
        /// <param name="siteId">网站Id。</param>
        /// <param name="normalizedName">用户组名称。</param>
        /// <returns>返回当前用户组实例对象。</returns>
        Task<TRole> FindByNameAsync(int siteId, string normalizedName);

        /// <summary>
        /// 获取所有用户组。
        /// </summary>
        /// <param name="siteId">网站Id。</param>
        /// <returns>返回用户组列表。</returns>
        Task<IEnumerable<TRole>> LoadAsync(int siteId);

        /// <summary>
        /// 获取所有用户组。
        /// </summary>
        /// <param name="siteId">网站Id。</param>
        /// <returns>返回用户组列表。</returns>
        IEnumerable<TRole> Load(int siteId);
    }
}