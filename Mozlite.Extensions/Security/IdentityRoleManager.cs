using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
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
        /// <summary>
        /// 用户组数据库操作接口。
        /// </summary>
        protected IRepository<TRole> Repository { get; }
        private readonly IRoleClaimStore<TRole> _store;

        /// <summary>
        /// 初始化类<see cref="IdentityRoleManager{TRole}"/>。
        /// </summary>
        /// <param name="repository">用户组数据库操作接口。</param>
        /// <param name="store">用户组存储接口实例。</param>
        /// <param name="cache">缓存接口。</param>
        protected IdentityRoleManager(IRepository<TRole> repository, IRoleStore<TRole> store, IMemoryCache cache)
        {
            _cache = cache;
            Repository = repository;
            _store = store as IRoleClaimStore<TRole>;
        }

        /// <summary>
        /// 添加用户组声明。
        /// </summary>
        /// <param name="roleId">用户组ID。</param>
        /// <param name="claim">声明实例对象。</param>
        /// <param name="cancellationToken">取消标志。</param>
        /// <returns>返回添加任务实例。</returns>
        public virtual Task AddClaimAsync(int roleId, Claim claim, CancellationToken cancellationToken = new CancellationToken())
        {
            return _store.AddClaimAsync(new TRole { RoleId = roleId }, claim, cancellationToken);
        }

        /// <summary>
        /// 新建用户组。
        /// </summary>
        /// <param name="role">用户组实例对象。</param>
        /// <param name="cancellationToken">取消标志。</param>
        /// <returns>返回新建结果。</returns>
        public virtual async Task<IdentityResult> CreateAsync(TRole role, CancellationToken cancellationToken = new CancellationToken())
        {
            var result = await _store.CreateAsync(role, cancellationToken);
            if (result.Succeeded)
                _cache.Remove(typeof(TRole));
            return result;
        }

        /// <summary>
        /// 删除用户组。
        /// </summary>
        /// <param name="roleId">用户组ID。</param>
        /// <param name="cancellationToken">取消标志。</param>
        /// <returns>返回删除结果。</returns>
        public virtual async Task<IdentityResult> DeleteAsync(int roleId, CancellationToken cancellationToken = new CancellationToken())
        {
            var result = await _store.DeleteAsync(new TRole { RoleId = roleId }, cancellationToken);
            if (result.Succeeded)
                _cache.Remove(typeof(TRole));
            return result;
        }

        /// <summary>
        /// 删除用户组。
        /// </summary>
        /// <param name="roleIds">用户组ID集合。</param>
        /// <param name="cancellationToken">取消标志。</param>
        /// <returns>返回删除结果。</returns>
        public Task<bool> DeleteAsync(string roleIds, CancellationToken cancellationToken = new CancellationToken())
        {
            var intIds = roleIds.ConvertToInt32s();
            return Repository.DeleteAsync(x => x.RoleId.Included(intIds), cancellationToken);
        }

        /// <summary>
        /// 查询用户组。
        /// </summary>
        /// <param name="normalizedRoleName">用户组名称。</param>
        /// <param name="cancellationToken">取消标志。</param>
        /// <returns>返回用户组实例对象。</returns>
        public virtual Task<TRole> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken = new CancellationToken())
        {
            return _store.FindByNameAsync(normalizedRoleName, cancellationToken);
        }

        /// <summary>
        /// 查询用户组。
        /// </summary>
        /// <param name="roleId">用户组ID。</param>
        /// <param name="cancellationToken">取消标志。</param>
        /// <returns>返回用户组实例对象。</returns>
        public virtual Task<TRole> FindByIdAsync(int roleId, CancellationToken cancellationToken = new CancellationToken())
        {
            return Repository.FindAsync(r => r.RoleId == roleId, cancellationToken);
        }

        /// <summary>
        /// 查询用户组。
        /// </summary>
        /// <param name="roleName">用户组名称。</param>
        /// <param name="cancellationToken">取消标志。</param>
        /// <returns>返回用户组实例对象。</returns>
        public virtual Task<TRole> FindByRoleNameAsync(string roleName, CancellationToken cancellationToken = new CancellationToken())
        {
            return FindByNameAsync(roleName.ToUpper(), cancellationToken);
        }

        /// <summary>
        /// 获取当前用户组的声明列表实例。
        /// </summary>
        /// <param name="roleId">用户组ID。</param>
        /// <param name="cancellationToken">取消标志。</param>
        /// <returns>返回用户组声明列表实例。</returns>
        public virtual Task<IList<Claim>> GetClaimsAsync(int roleId, CancellationToken cancellationToken = new CancellationToken())
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
        public virtual Task RemoveClaimAsync(int roleId, Claim claim, CancellationToken cancellationToken = new CancellationToken())
        {
            return _store.RemoveClaimAsync(new TRole { RoleId = roleId }, claim, cancellationToken);
        }

        /// <summary>
        /// 更新用户组。
        /// </summary>
        /// <param name="role">用户组实例对象。</param>
        /// <param name="cancellationToken">取消标志。</param>
        /// <returns>返回更新结果。</returns>
        public virtual Task<IdentityResult> UpdateAsync(TRole role, CancellationToken cancellationToken = new CancellationToken())
        {
            return _store.UpdateAsync(role, cancellationToken);
        }

        /// <summary>
        /// 加载所有用户组。
        /// </summary>
        /// <returns>所有用户组。</returns>
        public IEnumerable<TRole> Load()
        {
            return Repository.Fetch().OrderByDescending(x => x.Priority);
        }
    }
}