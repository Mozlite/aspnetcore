using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Mozlite.Data;

namespace Mozlite.Extensions.Security
{
    /// <summary>
    /// 用户组存储类。
    /// </summary>
    /// <typeparam name="TRole">用户组类型。</typeparam>
    /// <typeparam name="TRoleClaim">用户组声明类型。</typeparam>
    public abstract class IdentityIdentityRoleStore<TRole, TRoleClaim> : IRoleClaimStore<TRole>
        where TRole : IdentityRole, new()
        where TRoleClaim : IdentityRoleClaim, new()
    {
        private readonly IRepository<TRole> _repository;
        private readonly IRepository<TRoleClaim> _rc;
        /// <summary>
        /// 初始化类<see cref="IdentityIdentityRoleStore{TRole,TRoleClaim}"/>。
        /// </summary>
        /// <param name="repository">用户组数据库操作接口。</param>
        /// <param name="rc">用户组声明数据库操作接口。</param>
        protected IdentityIdentityRoleStore(IRepository<TRole> repository, IRepository<TRoleClaim> rc)
        {
            _repository = repository;
            _rc = rc;
        }

        /// <summary>
        /// 添加用户组声明。
        /// </summary>
        /// <param name="role">用户组实例对象。</param>
        /// <param name="claim">声明实例对象。</param>
        /// <param name="cancellationToken">取消标志。</param>
        /// <returns>返回添加任务实例。</returns>
        public virtual async Task AddClaimAsync(TRole role, Claim claim, CancellationToken cancellationToken = default)
        {
            var rc = new TRoleClaim { ClaimType = claim.Type, ClaimValue = claim.Value, RoleId = role.RoleId };
            await _rc.CreateAsync(rc, cancellationToken);
        }

        /// <summary>
        /// 新建用户组。
        /// </summary>
        /// <param name="role">用户组实例对象。</param>
        /// <param name="cancellationToken">取消标志。</param>
        /// <returns>返回新建结果。</returns>
        public virtual async Task<IdentityResult> CreateAsync(TRole role, CancellationToken cancellationToken)
        {
            try
            {
                if (role.NormalizedRoleName == null)
                    role.NormalizedRoleName = role.RoleName.ToUpper();
                await _repository.CreateAsync(role, cancellationToken);
                return IdentityResult.Success;
            }
            catch (Exception exception)
            {
                return
                    IdentityResult.Failed(new IdentityError { Code = "CreateRoleError", Description = exception.Message });
            }
        }

        /// <summary>
        /// 删除用户组。
        /// </summary>
        /// <param name="role">用户组实例对象。</param>
        /// <param name="cancellationToken">取消标志。</param>
        /// <returns>返回删除结果。</returns>
        public virtual async Task<IdentityResult> DeleteAsync(TRole role, CancellationToken cancellationToken)
        {
            try
            {
                await _repository.DeleteAsync(r => r.RoleId == role.RoleId, cancellationToken);
                return IdentityResult.Success;
            }
            catch (Exception exception)
            {
                return
                    IdentityResult.Failed(new IdentityError { Code = "DeleteRoleError", Description = exception.Message });
            }
        }

        /// <summary>
        /// 执行与释放或重置非托管资源关联的应用程序定义的任务。
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {

        }

        /// <summary>
        /// 查询用户组。
        /// </summary>
        /// <param name="roleId">用户组ID。</param>
        /// <param name="cancellationToken">取消标志。</param>
        /// <returns>返回用户组实例对象。</returns>
        public virtual async Task<TRole> FindByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            if (int.TryParse(roleId?.Trim(), out var id))
                return await FindByIdAsync(id, cancellationToken);
            return null;
        }

        /// <summary>
        /// 查询用户组。
        /// </summary>
        /// <param name="normalizedRoleName">用户组名称。</param>
        /// <param name="cancellationToken">取消标志。</param>
        /// <returns>返回用户组实例对象。</returns>
        public virtual Task<TRole> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            return _repository.FindAsync(r => r.NormalizedRoleName == normalizedRoleName, cancellationToken);
        }

        /// <summary>
        /// 获取当前用户组的声明列表实例。
        /// </summary>
        /// <param name="role">用户组实例对象。</param>
        /// <param name="cancellationToken">取消标志。</param>
        /// <returns>返回用户组声明列表实例。</returns>
        public virtual async Task<IList<Claim>> GetClaimsAsync(TRole role, CancellationToken cancellationToken = default)
        {
            var roleClaims = await _rc.FetchAsync(r => r.RoleId == role.RoleId, cancellationToken);
            return roleClaims.Select(rc => new Claim(rc.ClaimType, rc.ClaimValue)).ToList();
        }

        public Task<string> GetNormalizedRoleNameAsync(TRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }
            return Task.FromResult(role.NormalizedRoleName);
        }

        public Task<string> GetRoleIdAsync(TRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }
            return Task.FromResult(role.RoleId.ToString());
        }

        public Task<string> GetRoleNameAsync(TRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }
            return Task.FromResult(role.RoleName);
        }

        /// <summary>
        /// 移除当前用户组的一个声明实例。
        /// </summary>
        /// <param name="role">用户组实例对象。</param>
        /// <param name="claim">声明实例。</param>
        /// <param name="cancellationToken">取消标志。</param>
        /// <returns>返回移除声明的任务。</returns>
        public virtual async Task RemoveClaimAsync(TRole role, Claim claim, CancellationToken cancellationToken = default)
        {
            await _rc.DeleteAsync(rc => rc.RoleId == role.RoleId && rc.ClaimType == claim.Type, cancellationToken);
        }

        public Task SetNormalizedRoleNameAsync(TRole role, string normalizedName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }
            role.NormalizedRoleName = normalizedName;
            return Task.FromResult(0);
        }

        public Task SetRoleNameAsync(TRole role, string roleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }
            role.RoleName = roleName;
            return Task.FromResult(0);
        }

        /// <summary>
        /// 更新用户组。
        /// </summary>
        /// <param name="role">用户组实例对象。</param>
        /// <param name="cancellationToken">取消标志。</param>
        /// <returns>返回更新结果。</returns>
        public virtual async Task<IdentityResult> UpdateAsync(TRole role, CancellationToken cancellationToken)
        {
            try
            {
                await
                    _repository.UpdateAsync(r => r.RoleId == role.RoleId, new { role.DisplayName, role.Priority },
                        cancellationToken);
                return IdentityResult.Success;
            }
            catch (Exception exception)
            {
                return
                    IdentityResult.Failed(new IdentityError { Code = "UpdateRoleError", Description = exception.Message });
            }
        }

        /// <summary>
        /// 查询用户组。
        /// </summary>
        /// <param name="roleId">用户组ID。</param>
        /// <param name="cancellationToken">取消标志。</param>
        /// <returns>返回用户组实例对象。</returns>
        public virtual Task<TRole> FindByIdAsync(int roleId, CancellationToken cancellationToken)
        {
            return _repository.FindAsync(x => x.RoleId == roleId, cancellationToken);
        }

        /// <summary>
        /// 查询用户组。
        /// </summary>
        /// <param name="roleName">用户组名称。</param>
        /// <param name="cancellationToken">取消标志。</param>
        /// <returns>返回用户组实例对象。</returns>
        public Task<TRole> FindByRoleNameAsync(string roleName, CancellationToken cancellationToken)
        {
            roleName = roleName?.ToUpper();
            return _repository.FindAsync(r => r.NormalizedRoleName == roleName, cancellationToken);
        }
    }
}