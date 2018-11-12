using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Mozlite.Extensions.Security.Stores;

namespace Mozlite.Extensions.Extensions.Security.Stores
{
    /// <summary>
    /// 数据库存接口。
    /// </summary>
    /// <typeparam name="TRole">用户组类型。</typeparam>
    /// <typeparam name="TUserRole">用户用户组类型。</typeparam>
    /// <typeparam name="TRoleClaim">用户组声明类型。</typeparam>
    public interface IRoleStoreBase<TRole, TUserRole, TRoleClaim> : Mozlite.Extensions.Security.Stores.IRoleStoreBase<TRole, TUserRole, TRoleClaim>
        where TRole : RoleBase
        where TUserRole : IUserRole
        where TRoleClaim : RoleClaimBase, new()
    {
        /// <summary>
        /// 获取所有用户组。
        /// </summary>
        /// <param name="siteId">网站Id。</param>
        /// <returns>返回用户组列表。</returns>
        IEnumerable<TRole> LoadRoles(int siteId);

        /// <summary>
        /// 获取所有用户组。
        /// </summary>
        /// <param name="siteId">网站Id。</param>
        /// <param name="cancellationToken">取消标识。</param>
        /// <returns>返回用户组列表。</returns>
        Task<IEnumerable<TRole>> LoadRolesAsync(int siteId, CancellationToken cancellationToken = default);

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
        /// <param name="cancellationToken">取消标识。</param>
        /// <returns>返回当前用户组实例对象。</returns>
        Task<TRole> FindByNameAsync(int siteId, string normalizedName, CancellationToken cancellationToken = default);
    }
}