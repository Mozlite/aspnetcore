using System;
using System.Linq;
using System.Threading;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using Mozlite.Data;

namespace Mozlite.Extensions.Security
{
    /// <summary>
    /// 用户组管理接口。
    /// </summary>
    /// <typeparam name="TRole">用户组类型。</typeparam>
    public abstract class IdentityRoleManager<TRole> : IIdentityRoleManager<TRole> where TRole : IdentityRole, new()
    {
        private readonly IMemoryCache _cache;
        private static readonly Type _cacheKey = typeof(TRole);
        /// <summary>
        /// 用户组数据库操作接口。
        /// </summary>
        protected IDbContext<TRole> Repository { get; }
        private readonly IRoleClaimStore<TRole> _store;

        /// <summary>
        /// 初始化类<see cref="IdentityRoleManager{TRole}"/>。
        /// </summary>
        /// <param name="db">用户组数据库操作接口。</param>
        /// <param name="store">用户组存储接口实例。</param>
        /// <param name="cache">缓存接口。</param>
        protected IdentityRoleManager(IDbContext<TRole> db, IRoleStore<TRole> store, IMemoryCache cache)
        {
            _cache = cache;
            Repository = db;
            _store = store as IRoleClaimStore<TRole>;
        }

        /// <summary>
        /// 添加用户组声明。
        /// </summary>
        /// <param name="roleId">用户组ID。</param>
        /// <param name="claim">声明实例对象。</param>
        /// <param name="cancellationToken">取消标志。</param>
        /// <returns>返回添加任务实例。</returns>
        public virtual Task AddClaimAsync(int roleId, Claim claim, CancellationToken cancellationToken = default)
        {
            return _store.AddClaimAsync(new TRole { RoleId = roleId }, claim, cancellationToken);
        }

        /// <summary>
        /// 新建用户组。
        /// </summary>
        /// <param name="role">用户组实例对象。</param>
        /// <param name="cancellationToken">取消标志。</param>
        /// <returns>返回新建结果。</returns>
        public virtual async Task<IdentityResult> CreateAsync(TRole role, CancellationToken cancellationToken = default)
        {
            var result = await _store.CreateAsync(role, cancellationToken);
            if (result.Succeeded)
                _cache.Remove(_cacheKey);
            return result;
        }

        /// <summary>
        /// 删除用户组。
        /// </summary>
        /// <param name="roleId">用户组ID。</param>
        /// <param name="cancellationToken">取消标志。</param>
        /// <returns>返回删除结果。</returns>
        public virtual async Task<IdentityResult> DeleteAsync(int roleId, CancellationToken cancellationToken = default)
        {
            var result = await _store.DeleteAsync(new TRole { RoleId = roleId }, cancellationToken);
            if (result.Succeeded)
                _cache.Remove(_cacheKey);
            return result;
        }

        /// <summary>
        /// 删除用户组。
        /// </summary>
        /// <param name="roleIds">用户组ID集合。</param>
        /// <param name="cancellationToken">取消标志。</param>
        /// <returns>返回删除结果。</returns>
        public async Task<DataResult> DeleteAsync(string roleIds, CancellationToken cancellationToken = default)
        {
            var intIds = roleIds.SplitToInt32();
            var result = await Repository.DeleteAsync(x => x.RoleId.Included(intIds), cancellationToken);
            if (result)
                _cache.Remove(_cacheKey);
            return DataResult.FromResult(result, DataAction.Deleted);
        }

        /// <summary>
        /// 查询用户组。
        /// </summary>
        /// <param name="normalizedRoleName">用户组名称。</param>
        /// <param name="cancellationToken">取消标志。</param>
        /// <returns>返回用户组实例对象。</returns>
        public virtual async Task<TRole> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken = default)
        {
            var roles = await LoadAsync(cancellationToken);
            return roles.SingleOrDefault(x => x.NormalizedRoleName == normalizedRoleName);
        }

        /// <summary>
        /// 查询用户组。
        /// </summary>
        /// <param name="roleId">用户组ID。</param>
        /// <param name="cancellationToken">取消标志。</param>
        /// <returns>返回用户组实例对象。</returns>
        public virtual async Task<TRole> FindByIdAsync(int roleId, CancellationToken cancellationToken = default)
        {
            var roles = await LoadAsync(cancellationToken);
            return roles.SingleOrDefault(x => x.RoleId == roleId);
        }

        /// <summary>
        /// 查询用户组。
        /// </summary>
        /// <param name="roleName">用户组名称。</param>
        /// <param name="cancellationToken">取消标志。</param>
        /// <returns>返回用户组实例对象。</returns>
        public virtual async Task<TRole> FindByRoleNameAsync(string roleName, CancellationToken cancellationToken = default)
        {
            var roles = await LoadAsync(cancellationToken);
            return roles.SingleOrDefault(x => x.RoleName.Equals(roleName, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// 获取当前用户组的声明列表实例。
        /// </summary>
        /// <param name="roleId">用户组ID。</param>
        /// <param name="cancellationToken">取消标志。</param>
        /// <returns>返回用户组声明列表实例。</returns>
        public virtual Task<IList<Claim>> GetClaimsAsync(int roleId, CancellationToken cancellationToken = default)
        {
            return _store.GetClaimsAsync(new TRole { RoleId = roleId }, cancellationToken);
        }

        /// <summary>
        /// 移除当前用户组的一个声明实例。
        /// </summary>
        /// <param name="roleId">用户组ID。</param>
        /// <param name="claim">声明实例。</param>
        /// <param name="cancellationToken">取消标志。</param>
        /// <returns>返回移除声明的任务。</returns>
        public virtual Task RemoveClaimAsync(int roleId, Claim claim, CancellationToken cancellationToken = default)
        {
            return _store.RemoveClaimAsync(new TRole { RoleId = roleId }, claim, cancellationToken);
        }

        /// <summary>
        /// 更新用户组。
        /// </summary>
        /// <param name="role">用户组实例对象。</param>
        /// <param name="cancellationToken">取消标志。</param>
        /// <returns>返回更新结果。</returns>
        public virtual async Task<IdentityResult> UpdateAsync(TRole role, CancellationToken cancellationToken = default)
        {
            var result = await _store.UpdateAsync(role, cancellationToken);
            if (result.Succeeded)
                _cache.Remove(_cacheKey);
            return result;
        }

        /// <summary>
        /// 加载所有用户组。
        /// </summary>
        /// <returns>所有用户组。</returns>
        public virtual IEnumerable<TRole> Load()
        {
            return _cache.GetOrCreate(_cacheKey, ctx =>
            {
                ctx.SetAbsoluteExpiration(TimeSpan.FromMinutes(3));
                return Repository.Fetch().OrderByDescending(x => x.Priority);
            });
        }

        /// <summary>
        /// 加载所有用户组。
        /// </summary>
        /// <param name="cancellationToken">取消标志。</param>
        /// <returns>所有用户组。</returns>
        public virtual async Task<IEnumerable<TRole>> LoadAsync(CancellationToken cancellationToken = default)
        {
            return await _cache.GetOrCreateAsync(_cacheKey, async ctx =>
            {
                ctx.SetAbsoluteExpiration(TimeSpan.FromMinutes(3));
                var roles = await Repository.FetchAsync(cancellationToken: cancellationToken);
                return roles.OrderByDescending(x => x.Priority);
            });
        }
    }
}