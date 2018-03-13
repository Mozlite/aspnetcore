using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Mozlite.Extensions.Security.Stores
{
    /// <summary>
    /// 用户角色存储基类。
    /// </summary>
    /// <typeparam name="TRole">角色类型。</typeparam>
    /// <typeparam name="TUserRole">用户角色关联类。</typeparam>
    /// <typeparam name="TRoleClaim">角色声明类型。</typeparam>
    public interface IRoleStoreBase<TRole, TUserRole, TRoleClaim>
        where TRole : RoleBase
        where TUserRole : IUserRole
        where TRoleClaim : RoleClaimBase, new()
    {
        /// <summary>
        /// 获取所有角色。
        /// </summary>
        /// <returns>返回角色列表。</returns>
        IEnumerable<TRole> LoadRoles();

        /// <summary>
        /// 获取所有角色。
        /// </summary>
        /// <param name="cancellationToken">取消标识。</param>
        /// <returns>返回角色列表。</returns>
        Task<IEnumerable<TRole>> LoadRolesAsync(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// 通过ID获取角色实例。
        /// </summary>
        /// <param name="id">角色Id。</param>
        /// <returns>返回当前角色实例对象。</returns>
        TRole FindById(int id);

        /// <summary>
        /// 通过ID获取角色实例。
        /// </summary>
        /// <param name="id">角色Id。</param>
        /// <param name="cancellationToken">取消标识。</param>
        /// <returns>返回当前角色实例对象。</returns>
        Task<TRole> FindByIdAsync(int id, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// 通过角色名称获取角色实例。
        /// </summary>
        /// <param name="normalizedName">角色名称。</param>
        /// <returns>返回当前角色实例对象。</returns>
        TRole FindByName(string normalizedName);

        /// <summary>
        /// 通过角色名称获取角色实例。
        /// </summary>
        /// <param name="normalizedName">角色名称。</param>
        /// <param name="cancellationToken">取消标识。</param>
        /// <returns>返回当前角色实例对象。</returns>
        Task<TRole> FindByNameAsync(string normalizedName, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// 添加用户组。
        /// </summary>
        /// <param name="role">用户组实例。</param>
        /// <returns>返回添加结果。</returns>
        IdentityResult Create(TRole role);

        /// <summary>
        /// 添加用户组。
        /// </summary>
        /// <param name="role">用户组实例。</param>
        /// <param name="cancellationToken">取消标识。</param>
        /// <returns>返回添加结果。</returns>
        Task<IdentityResult> CreateAsync(TRole role, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// 更新用户角色。
        /// </summary>
        /// <param name="role">用户角色实例。</param>
        /// <returns>返回角色更新结果。</returns>
        IdentityResult Update(TRole role);

        /// <summary>
        /// 更新用户角色。
        /// </summary>
        /// <param name="role">用户角色实例。</param>
        /// <param name="cancellationToken">取消标识。</param>
        /// <returns>返回角色更新结果。</returns>
        Task<IdentityResult> UpdateAsync(TRole role, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// 更新用户角色。
        /// </summary>
        /// <param name="role">用户角色实例。</param>
        /// <returns>返回角色更新结果。</returns>
        IdentityResult Delete(TRole role);

        /// <summary>
        /// 更新用户角色。
        /// </summary>
        /// <param name="role">用户角色实例。</param>
        /// <param name="cancellationToken">取消标识。</param>
        /// <returns>返回角色更新结果。</returns>
        Task<IdentityResult> DeleteAsync(TRole role, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// 上移角色。
        /// </summary>
        /// <param name="role">角色实例。</param>
        /// <returns>返回移动结果。</returns>
        bool MoveUp(TRole role);

        /// <summary>
        /// 上移角色。
        /// </summary>
        /// <param name="role">角色实例。</param>
        /// <param name="cancellationToken">取消标识。</param>
        /// <returns>返回移动结果。</returns>
        Task<bool> MoveUpAsync(TRole role, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// 下移角色。
        /// </summary>
        /// <param name="role">角色实例。</param>
        /// <returns>返回移动结果。</returns>
        bool MoveDown(TRole role);

        /// <summary>
        /// 下移角色。
        /// </summary>
        /// <param name="role">角色实例。</param>
        /// <param name="cancellationToken">取消标识。</param>
        /// <returns>返回移动结果。</returns>
        Task<bool> MoveDownAsync(TRole role, CancellationToken cancellationToken = default(CancellationToken));
    }
}