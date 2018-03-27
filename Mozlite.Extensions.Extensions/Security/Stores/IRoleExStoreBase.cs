using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Mozlite.Extensions.Security.Stores;

namespace Mozlite.Extensions.Extensions.Security.Stores
{
    /// <summary>
    /// 数据库存接口。
    /// </summary>
    /// <typeparam name="TRole">角色类型。</typeparam>
    /// <typeparam name="TUserRole">用户角色类型。</typeparam>
    /// <typeparam name="TRoleClaim">角色声明类型。</typeparam>
    public interface IRoleExStoreBase<TRole, TUserRole, TRoleClaim> : IRoleStoreBase<TRole, TUserRole, TRoleClaim>
        where TRole : RoleBase
        where TUserRole : IUserRole
        where TRoleClaim : RoleClaimBase, new()
    {
        /// <summary>
        /// 获取所有角色。
        /// </summary>
        /// <param name="siteId">网站Id。</param>
        /// <returns>返回角色列表。</returns>
        IEnumerable<TRole> LoadRoles(int siteId);

        /// <summary>
        /// 获取所有角色。
        /// </summary>
        /// <param name="siteId">网站Id。</param>
        /// <param name="cancellationToken">取消标识。</param>
        /// <returns>返回角色列表。</returns>
        Task<IEnumerable<TRole>> LoadRolesAsync(int siteId, CancellationToken cancellationToken = default(CancellationToken));

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
        /// <param name="cancellationToken">取消标识。</param>
        /// <returns>返回当前角色实例对象。</returns>
        Task<TRole> FindByNameAsync(int siteId, string normalizedName, CancellationToken cancellationToken = default(CancellationToken));
    }
}