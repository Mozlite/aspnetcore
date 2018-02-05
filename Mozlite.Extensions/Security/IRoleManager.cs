using System;
using System.Linq;
using System.Threading.Tasks;
using Mozlite.Extensions.Data;
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
        Task<IEnumerable<TRole>> LoadRolesAsync();

        /// <summary>
        /// 获取所有角色。
        /// </summary>
        /// <returns>返回角色列表。</returns>
        IEnumerable<TRole> LoadRoles();

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
        /// 添加用户组。
        /// </summary>
        /// <param name="role">用户组实例。</param>
        /// <returns>返回添加结果。</returns>
        Task<DataResult> CreateAsync(TRole role);

        /// <summary>
        /// 更新用户角色。
        /// </summary>
        /// <param name="role">用户角色实例。</param>
        /// <returns>返回角色更新结果。</returns>
        Task<DataResult> UpdateAsync(TRole role);

        /// <summary>
        /// 正常实例化键。
        /// </summary>
        /// <param name="key">原有键值。</param>
        /// <returns>返回正常化后的字符串。</returns>
        string NormalizeKey(string key);
    }

    /// <summary>
    /// 角色管理实现类。
    /// </summary>
    /// <typeparam name="TRole">角色类型。</typeparam>
    /// <typeparam name="TUserRole">用户角色类型。</typeparam>
    /// <typeparam name="TRoleClaim">角色声明类型。</typeparam>
    public abstract class RoleManager<TRole, TUserRole, TRoleClaim>
        : IRoleManager<TRole, TUserRole, TRoleClaim>
        where TRole : RoleBase
        where TUserRole : IUserRole
        where TRoleClaim : RoleClaimBase, new()
    {
        /// <summary>
        /// 系统用户角色管理实例。
        /// </summary>
        protected RoleManager<TRole> Manager { get; }

        /// <summary>
        /// 用户角色存储管理接口。
        /// </summary>
        protected IRoleStoreBase<TRole, TUserRole, TRoleClaim> Store { get; }

        /// <summary>
        /// 初始化类<see cref="RoleManager{TRole}"/>。
        /// </summary>
        /// <param name="manager">系统用户角色管理实例。</param>
        /// <param name="store">角色存储接口。</param>
        protected RoleManager(RoleManager<TRole> manager, IRoleStore<TRole> store)
        {
            Manager = manager;
            Store = store as IRoleStoreBase<TRole, TUserRole, TRoleClaim>;
        }

        /// <summary>
        /// 获取所有角色。
        /// </summary>
        /// <returns>返回角色列表。</returns>
        public virtual Task<IEnumerable<TRole>> LoadRolesAsync() => Store.LoadRolesAsync();

        /// <summary>
        /// 获取所有角色。
        /// </summary>
        /// <returns>返回角色列表。</returns>
        public virtual IEnumerable<TRole> LoadRoles() => Store.LoadRoles();
        
        /// <summary>
        /// 通过ID获取角色实例。
        /// </summary>
        /// <param name="id">角色Id。</param>
        /// <returns>返回当前角色实例对象。</returns>
        public virtual async Task<TRole> FindByIdAsync(int id)
        {
            return await Store.FindByIdAsync(id);
        }

        /// <summary>
        /// 通过角色名称获取角色实例。
        /// </summary>
        /// <param name="normalizedName">角色名称。</param>
        /// <returns>返回当前角色实例对象。</returns>
        public virtual async Task<TRole> FindByNameAsync(string normalizedName)
        {
            return await Manager.FindByNameAsync(normalizedName);
        }

        /// <summary>
        /// 添加用户组。
        /// </summary>
        /// <param name="role">用户组实例。</param>
        /// <returns>返回添加结果。</returns>
        public virtual async Task<DataResult> CreateAsync(TRole role)
        {
            if (string.IsNullOrEmpty(role.NormalizedName))
                role.NormalizedName = NormalizeKey(role.Name);
            var roles = await LoadRolesAsync();
            if (roles.Any(x => x.Name.Equals(role.Name, StringComparison.OrdinalIgnoreCase) || x.NormalizedName == role.NormalizedName))
                return DataAction.Duplicate;
            var result = await Store.CreateAsync(role);
            if (result.Succeeded)
                return DataAction.Created;
            return DataAction.CreatedFailured;
        }

        /// <summary>
        /// 更新用户角色。
        /// </summary>
        /// <param name="role">用户角色实例。</param>
        /// <returns>返回角色更新结果。</returns>
        public async Task<DataResult> UpdateAsync(TRole role)
        {
            var result = await Manager.UpdateAsync(role);
            if (result.Succeeded)
                return DataAction.Updated;
            return DataAction.UpdatedFailured;
        }

        /// <summary>
        /// 正常实例化键。
        /// </summary>
        /// <param name="key">原有键值。</param>
        /// <returns>返回正常化后的字符串。</returns>
        public virtual string NormalizeKey(string key) => Manager.NormalizeKey(key);
    }
}