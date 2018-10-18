using System;
using System.Threading;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Mozlite.Extensions.Security.Stores
{
    /// <summary>
    /// 用户角色存储基类。
    /// </summary>
    /// <typeparam name="TRole">用户角色类型。</typeparam>
    /// <typeparam name="TUserRole">用户角色关联类。</typeparam>
    /// <typeparam name="TRoleClaim">角色声明类型。</typeparam>
    public abstract class IdentityRoleStoreBase<TRole, TUserRole, TRoleClaim> :
        IRoleClaimStore<TRole>
        where TRole : RoleBase
        where TUserRole : IUserRole
        where TRoleClaim : RoleClaimBase, new()
    {
        /// <summary>
        /// 初始化类<see cref="IdentityRoleStoreBase{TRole,TUserRole,TRoleClaim}"/>。
        /// </summary>
        /// <param name="describer">错误描述<see cref="IdentityErrorDescriber"/>实例。</param>
        protected IdentityRoleStoreBase(IdentityErrorDescriber describer)
        {
            ErrorDescriber = describer ?? throw new ArgumentNullException(nameof(describer));
        }

        /// <summary>
        /// 错误描述实例对象。
        /// </summary>
        protected IdentityErrorDescriber ErrorDescriber { get; }

        /// <summary>
        /// 添加用户角色。
        /// </summary>
        /// <param name="role">用户角色实例。</param>
        /// <param name="cancellationToken">取消标识。</param>
        /// <returns>返回角色添加结果。</returns>
        public abstract Task<IdentityResult> CreateAsync(TRole role, CancellationToken cancellationToken = default);

        /// <summary>
        /// 更新用户角色。
        /// </summary>
        /// <param name="role">用户角色实例。</param>
        /// <param name="cancellationToken">取消标识。</param>
        /// <returns>返回角色更新结果。</returns>
        public abstract Task<IdentityResult> UpdateAsync(TRole role, CancellationToken cancellationToken = default);

        /// <summary>
        /// 删除用户角色。
        /// </summary>
        /// <param name="role">用户角色实例。</param>
        /// <param name="cancellationToken">取消标识。</param>
        /// <returns>返回角色删除结果。</returns>
        public abstract Task<IdentityResult> DeleteAsync(TRole role, CancellationToken cancellationToken = default);

        /// <summary>
        /// 获取角色ID。
        /// </summary>
        /// <param name="role">用户角色实例。</param>
        /// <param name="cancellationToken">取消标识。</param>
        /// <returns>返回角色ID。</returns>
        public virtual Task<string> GetRoleIdAsync(TRole role, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }
            return Task.FromResult(role.RoleId.ToString());
        }

        /// <summary>
        /// 获取角色名称。
        /// </summary>
        /// <param name="role">用户角色实例。</param>
        /// <param name="cancellationToken">取消标识。</param>
        /// <returns>返回角色名称。</returns>
        public virtual Task<string> GetRoleNameAsync(TRole role, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }
            return Task.FromResult(role.Name);
        }

        /// <summary>
        /// 设置角色名称。
        /// </summary>
        /// <param name="role">用户角色实例。</param>
        /// <param name="roleName">角色名称。</param>
        /// <param name="cancellationToken">取消标识。</param>
        public virtual Task SetRoleNameAsync(TRole role, string roleName, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }
            role.Name = roleName;
            return Task.CompletedTask;
        }

        /// <summary>
        /// 通过ID获取角色实例。
        /// </summary>
        /// <param name="id">角色Id。</param>
        /// <param name="cancellationToken">取消标识。</param>
        /// <returns>返回当前角色实例对象。</returns>
        public virtual async Task<TRole> FindByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            if (int.TryParse(id, out var roleId))
                return await FindByIdAsync(roleId, cancellationToken);
            return null;
        }

        /// <summary>
        /// 通过ID获取角色实例。
        /// </summary>
        /// <param name="id">角色Id。</param>
        /// <param name="cancellationToken">取消标识。</param>
        /// <returns>返回当前角色实例对象。</returns>
        public abstract Task<TRole> FindByIdAsync(int id, CancellationToken cancellationToken = default);

        /// <summary>
        /// 通过角色名称获取角色实例。
        /// </summary>
        /// <param name="normalizedName">角色名称。</param>
        /// <param name="cancellationToken">取消标识。</param>
        /// <returns>返回当前角色实例对象。</returns>
        public abstract Task<TRole> FindByNameAsync(string normalizedName, CancellationToken cancellationToken = default);

        /// <summary>
        /// 通过角色实例获取角色验证名称。
        /// </summary>
        /// <param name="role">角色实例。</param>
        /// <param name="cancellationToken">取消标识。</param>
        /// <returns>返回当前角色验证名称。</returns>
        public virtual Task<string> GetNormalizedRoleNameAsync(TRole role, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }
            return Task.FromResult(role.NormalizedName);
        }

        /// <summary>
        /// 设置角色验证名称。
        /// </summary>
        /// <param name="role">用户角色实例。</param>
        /// <param name="normalizedName">角色验证名称。</param>
        /// <param name="cancellationToken">取消标识。</param>
        public virtual Task SetNormalizedRoleNameAsync(TRole role, string normalizedName, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }
            role.NormalizedName = normalizedName;
            return Task.CompletedTask;
        }

        /// <summary>
        /// 释放资源。
        /// </summary>
        public virtual void Dispose() { }

        /// <summary>
        /// 获取角色声明列表。
        /// </summary>
        /// <param name="role">角色实例。</param>
        /// <param name="cancellationToken">取消标志。</param>
        /// <returns>返回当前角色的声明列表。</returns>
        public abstract Task<IList<Claim>> GetClaimsAsync(TRole role, CancellationToken cancellationToken = default);

        /// <summary>
        /// 添加角色声明。
        /// </summary>
        /// <param name="role">角色实例对象。</param>
        /// <param name="claim">声明实例。</param>
        /// <param name="cancellationToken">取消标志。</param>
        public abstract Task AddClaimAsync(TRole role, Claim claim, CancellationToken cancellationToken = default);

        /// <summary>
        /// 移除角色声明。
        /// </summary>
        /// <param name="role">角色实例对象。</param>
        /// <param name="claim">声明实例。</param>
        /// <param name="cancellationToken">取消标志。</param>
        public abstract Task RemoveClaimAsync(TRole role, Claim claim, CancellationToken cancellationToken = default);

        /// <summary>
        /// 通过角色和声明实例转换为声明实例。
        /// </summary>
        /// <param name="role">当前角色实例。</param>
        /// <param name="claim">声明实例。</param>
        /// <returns>返回<see cref="TRoleClaim"/>实例对象。</returns>
        protected virtual TRoleClaim CreateRoleClaim(TRole role, Claim claim)
        {
            var roleClaim = new TRoleClaim { RoleId = role.RoleId };
            roleClaim.InitializeFromClaim(claim);
            return roleClaim;
        }
    }
}