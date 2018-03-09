using System.Collections.Generic;
using System.Threading.Tasks;
using Mozlite.Extensions.Security.Stores;

namespace Mozlite.Extensions.Security
{
    /// <summary>
    /// 角色管理接口。
    /// </summary>
    /// <typeparam name="TRole">角色类型。</typeparam>
    /// <typeparam name="TUserRole">用户角色类型。</typeparam>
    /// <typeparam name="TRoleClaim">角色声明类型。</typeparam>
    public interface IRoleExManager<TRole, TUserRole, TRoleClaim>
        : IRoleManager<TRole, TUserRole, TRoleClaim>
        where TRole : RoleExBase
        where TUserRole : IUserRole
        where TRoleClaim : RoleClaimBase, new()
    {
        /// <summary>
        /// 获取角色实例。
        /// </summary>
        /// <param name="siteId">网站Id。</param>
        /// <param name="roleId">角色Id。</param>
        /// <returns>返回当前角色实例。</returns>
        TRole GetRole(int siteId, int roleId);

        /// <summary>
        /// 获取角色实例。
        /// </summary>
        /// <param name="siteId">网站Id。</param>
        /// <param name="roleId">角色Id。</param>
        /// <returns>返回当前角色实例。</returns>
        Task<TRole> GetRoleAsync(int siteId, int roleId);

        /// <summary>
        /// 获取所有角色。
        /// </summary>
        /// <param name="siteId">网站Id。</param>
        /// <returns>返回角色列表。</returns>
        Task<IEnumerable<TRole>> LoadRolesAsync(int siteId);

        /// <summary>
        /// 获取所有角色。
        /// </summary>
        /// <param name="siteId">网站Id。</param>
        /// <returns>返回角色列表。</returns>
        IEnumerable<TRole> LoadRoles(int siteId);
    }
}