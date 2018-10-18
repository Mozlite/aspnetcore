using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Mozlite.Extensions.Security.Stores;

namespace Mozlite.Extensions.Security
{
    /// <summary>
    /// 角色管理接口。
    /// </summary>
    /// <typeparam name="TRole">角色类型。</typeparam>
    /// <typeparam name="TUserRole">用户角色类型。</typeparam>
    /// <typeparam name="TRoleClaim">角色声明类型。</typeparam>
    public interface IRoleManager<TRole, TUserRole, TRoleClaim>
        where TRole : RoleBase
        where TUserRole : IUserRole
        where TRoleClaim : RoleClaimBase, new()
    {
        /// <summary>
        /// 获取所有角色。
        /// </summary>
        /// <returns>返回角色列表。</returns>
        Task<IEnumerable<TRole>> LoadAsync();

        /// <summary>
        /// 获取所有角色。
        /// </summary>
        /// <returns>返回角色列表。</returns>
        IEnumerable<TRole> Load();

        /// <summary>
        /// 通过ID获取角色实例。
        /// </summary>
        /// <param name="id">角色Id。</param>
        /// <returns>返回当前角色实例对象。</returns>
        Task<TRole> FindByIdAsync(int id);

        /// <summary>
        /// 通过角色名称获取角色实例。
        /// </summary>
        /// <param name="normalizedName">角色名称。</param>
        /// <returns>返回当前角色实例对象。</returns>
        Task<TRole> FindByNameAsync(string normalizedName);

        /// <summary>
        /// 添加角色。
        /// </summary>
        /// <param name="role">角色实例。</param>
        /// <returns>返回添加结果。</returns>
        Task<IdentityResult> CreateAsync(TRole role);

        /// <summary>
        /// 更新用户角色。
        /// </summary>
        /// <param name="role">用户角色实例。</param>
        /// <returns>返回角色更新结果。</returns>
        Task<IdentityResult> UpdateAsync(TRole role);

        /// <summary>
        /// 保存角色。
        /// </summary>
        /// <param name="role">角色实例。</param>
        /// <returns>返回角色保存结果。</returns>
        Task<IdentityResult> SaveAsync(TRole role);

        /// <summary>
        /// 正常实例化键。
        /// </summary>
        /// <param name="key">原有键值。</param>
        /// <returns>返回正常化后的字符串。</returns>
        string NormalizeKey(string key);

        /// <summary>
        /// 上移角色。
        /// </summary>
        /// <param name="role">角色实例。</param>
        /// <returns>返回移动结果。</returns>
        bool MoveUp(TRole role);

        /// <summary>
        /// 下移角色。
        /// </summary>
        /// <param name="role">角色实例。</param>
        /// <returns>返回移动结果。</returns>
        bool MoveDown(TRole role);

        /// <summary>
        /// 上移角色。
        /// </summary>
        /// <param name="role">角色实例。</param>
        /// <returns>返回移动结果。</returns>
        Task<bool> MoveUpAsync(TRole role);

        /// <summary>
        /// 下移角色。
        /// </summary>
        /// <param name="role">角色实例。</param>
        /// <returns>返回移动结果。</returns>
        Task<bool> MoveDownAsync(TRole role);

        /// <summary>
        /// 删除角色。
        /// </summary>
        /// <param name="role">角色。</param>
        /// <returns>返回删除结果。</returns>
        Task<IdentityResult> DeleteAsync(TRole role);

        /// <summary>
        /// 判断角色名称或唯一键是否已经存在。
        /// </summary>
        /// <param name="role">当前角色实例。</param>
        /// <returns>返回判断结果。</returns>
        Task<IdentityResult> IsDuplicatedAsync(TRole role);

        /// <summary>
        /// 通过ID获取角色实例。
        /// </summary>
        /// <param name="id">角色Id。</param>
        /// <returns>返回当前角色实例对象。</returns>
        TRole FindById(int id);

        /// <summary>
        /// 通过角色名称获取角色实例。
        /// </summary>
        /// <param name="normalizedName">角色名称。</param>
        /// <returns>返回当前角色实例对象。</returns>
        TRole FindByName(string normalizedName);

        /// <summary>
        /// 保存角色。
        /// </summary>
        /// <param name="role">角色实例。</param>
        /// <returns>返回角色保存结果。</returns>
        IdentityResult Save(TRole role);

        /// <summary>
        /// 添加角色。
        /// </summary>
        /// <param name="role">角色实例。</param>
        /// <returns>返回添加结果。</returns>
        IdentityResult Create(TRole role);

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
        /// <returns>返回角色更新结果。</returns>
        IdentityResult Delete(TRole role);

        /// <summary>
        /// 判断角色名称或唯一键是否已经存在。
        /// </summary>
        /// <param name="role">当前角色实例。</param>
        /// <returns>返回判断结果。</returns>
        IdentityResult IsDuplicated(TRole role);

        /// <summary>
        /// 删除角色。
        /// </summary>
        /// <param name="ids">角色Id。</param>
        /// <returns>返回删除结果。</returns>
        IdentityResult Delete(int[] ids);

        /// <summary>
        /// 删除角色。
        /// </summary>
        /// <param name="ids">角色Id。</param>
        /// <returns>返回删除结果。</returns>
        Task<IdentityResult> DeleteAsync(int[] ids);
    }
}