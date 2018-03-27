using System.Threading.Tasks;
using System.Collections.Generic;
using Mozlite.Extensions.Security.Stores;
using Mozlite.Extensions.Extensions.Security.Stores;

namespace Mozlite.Extensions.Extensions.Security
{
    /// <summary>
    /// 角色管理接口。
    /// </summary>
    /// <typeparam name="TRole">角色类型。</typeparam>
    /// <typeparam name="TUserRole">用户角色类型。</typeparam>
    /// <typeparam name="TRoleClaim">角色声明类型。</typeparam>
    public interface IRoleManager<TRole, TUserRole, TRoleClaim>
        : Mozlite.Extensions.Security.IRoleManager<TRole, TUserRole, TRoleClaim>
        where TRole : RoleExBase
        where TUserRole : IUserRole
        where TRoleClaim : RoleClaimBase, new()
    {
        /// <summary>
        /// 通过角色名称获取角色实例。
        /// </summary>
        /// <param name="siteId">网站Id。</param>
        /// <param name="normalizedName">角色名称。</param>
        /// <returns>返回当前角色实例对象。</returns>
        TRole FindByName(int siteId, string normalizedName);

        /// <summary>
        /// 通过角色名称获取角色实例。
        /// </summary>
        /// <param name="siteId">网站Id。</param>
        /// <param name="normalizedName">角色名称。</param>
        /// <returns>返回当前角色实例对象。</returns>
        Task<TRole> FindByNameAsync(int siteId, string normalizedName);

        /// <summary>
        /// 获取所有角色。
        /// </summary>
        /// <param name="siteId">网站Id。</param>
        /// <returns>返回角色列表。</returns>
        Task<IEnumerable<TRole>> LoadAsync(int siteId);

        /// <summary>
        /// 获取所有角色。
        /// </summary>
        /// <param name="siteId">网站Id。</param>
        /// <returns>返回角色列表。</returns>
        IEnumerable<TRole> Load(int siteId);
    }
}