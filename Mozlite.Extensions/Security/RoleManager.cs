using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Mozlite.Extensions.Security.Stores;

namespace Mozlite.Extensions.Security
{
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
        /// 初始化类<see cref="RoleManager{TRole, TUserRole, TRoleClaim}"/>。
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
        public virtual async Task<IdentityResult> CreateAsync(TRole role)
        {
            if (string.IsNullOrEmpty(role.NormalizedName))
                role.NormalizedName = NormalizeKey(role.Name);
            return await Manager.CreateAsync(role);
        }

        /// <summary>
        /// 更新用户角色。
        /// </summary>
        /// <param name="role">用户角色实例。</param>
        /// <returns>返回角色更新结果。</returns>
        public Task<IdentityResult> UpdateAsync(TRole role)
        {
            return Manager.UpdateAsync(role);
        }

        /// <summary>
        /// 保存角色。
        /// </summary>
        /// <param name="role">角色实例。</param>
        /// <returns>返回角色保存结果。</returns>
        public Task<IdentityResult> SaveAsync(TRole role)
        {
            if (role.RoleId > 0)
                return UpdateAsync(role);
            return CreateAsync(role);
        }

        /// <summary>
        /// 正常实例化键。
        /// </summary>
        /// <param name="key">原有键值。</param>
        /// <returns>返回正常化后的字符串。</returns>
        public virtual string NormalizeKey(string key) => Manager.NormalizeKey(key);

        /// <summary>
        /// 上移角色。
        /// </summary>
        /// <param name="roleId">角色Id。</param>
        /// <returns>返回移动结果。</returns>
        public bool MoveUp(int roleId)
        {
            return Store.MoveUp(roleId);
        }

        /// <summary>
        /// 下移角色。
        /// </summary>
        /// <param name="roleId">角色Id。</param>
        /// <returns>返回移动结果。</returns>
        public bool MoveDown(int roleId)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// 上移角色。
        /// </summary>
        /// <param name="roleId">角色Id。</param>
        /// <returns>返回移动结果。</returns>
        public async Task<bool> MoveUpAsync(int roleId)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// 下移角色。
        /// </summary>
        /// <param name="roleId">角色Id。</param>
        /// <returns>返回移动结果。</returns>
        public async Task<bool> MoveDownAsync(int roleId)
        {
            throw new System.NotImplementedException();
        }
    }
}