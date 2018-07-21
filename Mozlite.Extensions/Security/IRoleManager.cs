using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Mozlite.Extensions.Security.Stores;

namespace Mozlite.Extensions.Security
{
    /// <summary>
    /// 用户组管理接口。
    /// </summary>
    /// <typeparam name="TRole">用户组类型。</typeparam>
    /// <typeparam name="TUserRole">用户用户组类型。</typeparam>
    /// <typeparam name="TRoleClaim">用户组声明类型。</typeparam>
    public interface IRoleManager<TRole, TUserRole, TRoleClaim>
        where TRole : RoleBase
        where TUserRole : IUserRole
        where TRoleClaim : RoleClaimBase, new()
    {
        /// <summary>
        /// 获取所有用户组。
        /// </summary>
        /// <returns>返回用户组列表。</returns>
        Task<IEnumerable<TRole>> LoadAsync();

        /// <summary>
        /// 获取所有用户组。
        /// </summary>
        /// <returns>返回用户组列表。</returns>
        IEnumerable<TRole> Load();

        /// <summary>
        /// 通过ID获取用户组实例。
        /// </summary>
        /// <param name="id">用户组Id。</param>
        /// <returns>返回当前用户组实例对象。</returns>
        Task<TRole> FindByIdAsync(int id);

        /// <summary>
        /// 通过用户组名称获取用户组实例。
        /// </summary>
        /// <param name="normalizedName">用户组名称。</param>
        /// <returns>返回当前用户组实例对象。</returns>
        Task<TRole> FindByNameAsync(string normalizedName);

        /// <summary>
        /// 添加用户组。
        /// </summary>
        /// <param name="role">用户组实例。</param>
        /// <returns>返回添加结果。</returns>
        Task<IdentityResult> CreateAsync(TRole role);

        /// <summary>
        /// 更新用户用户组。
        /// </summary>
        /// <param name="role">用户用户组实例。</param>
        /// <returns>返回用户组更新结果。</returns>
        Task<IdentityResult> UpdateAsync(TRole role);

        /// <summary>
        /// 保存用户组。
        /// </summary>
        /// <param name="role">用户组实例。</param>
        /// <returns>返回用户组保存结果。</returns>
        Task<IdentityResult> SaveAsync(TRole role);

        /// <summary>
        /// 正常实例化键。
        /// </summary>
        /// <param name="key">原有键值。</param>
        /// <returns>返回正常化后的字符串。</returns>
        string NormalizeKey(string key);

        /// <summary>
        /// 上移用户组。
        /// </summary>
        /// <param name="role">用户组实例。</param>
        /// <returns>返回移动结果。</returns>
        bool MoveUp(TRole role);

        /// <summary>
        /// 下移用户组。
        /// </summary>
        /// <param name="role">用户组实例。</param>
        /// <returns>返回移动结果。</returns>
        bool MoveDown(TRole role);

        /// <summary>
        /// 上移用户组。
        /// </summary>
        /// <param name="role">用户组实例。</param>
        /// <returns>返回移动结果。</returns>
        Task<bool> MoveUpAsync(TRole role);

        /// <summary>
        /// 下移用户组。
        /// </summary>
        /// <param name="role">用户组实例。</param>
        /// <returns>返回移动结果。</returns>
        Task<bool> MoveDownAsync(TRole role);

        /// <summary>
        /// 删除用户组。
        /// </summary>
        /// <param name="role">用户组。</param>
        /// <returns>返回删除结果。</returns>
        Task<IdentityResult> DeleteAsync(TRole role);

        /// <summary>
        /// 判断用户组名称或唯一键是否已经存在。
        /// </summary>
        /// <param name="role">当前用户组实例。</param>
        /// <returns>返回判断结果。</returns>
        Task<IdentityResult> IsDuplicatedAsync(TRole role);

        /// <summary>
        /// 通过ID获取用户组实例。
        /// </summary>
        /// <param name="id">用户组Id。</param>
        /// <returns>返回当前用户组实例对象。</returns>
        TRole FindById(int id);

        /// <summary>
        /// 通过用户组名称获取用户组实例。
        /// </summary>
        /// <param name="normalizedName">用户组名称。</param>
        /// <returns>返回当前用户组实例对象。</returns>
        TRole FindByName(string normalizedName);

        /// <summary>
        /// 保存用户组。
        /// </summary>
        /// <param name="role">用户组实例。</param>
        /// <returns>返回用户组保存结果。</returns>
        IdentityResult Save(TRole role);

        /// <summary>
        /// 添加用户组。
        /// </summary>
        /// <param name="role">用户组实例。</param>
        /// <returns>返回添加结果。</returns>
        IdentityResult Create(TRole role);

        /// <summary>
        /// 更新用户用户组。
        /// </summary>
        /// <param name="role">用户用户组实例。</param>
        /// <returns>返回用户组更新结果。</returns>
        IdentityResult Update(TRole role);

        /// <summary>
        /// 更新用户用户组。
        /// </summary>
        /// <param name="role">用户用户组实例。</param>
        /// <returns>返回用户组更新结果。</returns>
        IdentityResult Delete(TRole role);

        /// <summary>
        /// 判断用户组名称或唯一键是否已经存在。
        /// </summary>
        /// <param name="role">当前用户组实例。</param>
        /// <returns>返回判断结果。</returns>
        IdentityResult IsDuplicated(TRole role);
    }
}