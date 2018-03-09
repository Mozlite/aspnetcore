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