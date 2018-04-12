using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using Mozlite.Extensions.Security.Stores;
using Microsoft.Extensions.Caching.Memory;

namespace Mozlite.Extensions.Security
{
    /// <summary>
    /// 角色管理实现类。
    /// </summary>
    /// <typeparam name="TRole">角色类型。</typeparam>
    /// <typeparam name="TUserRole">用户角色类型。</typeparam>
    /// <typeparam name="TRoleClaim">角色声明类型。</typeparam>
    public abstract class RoleManagerBase<TRole, TUserRole, TRoleClaim> : RoleManager<TRole>,
        IRoleManager<TRole, TUserRole, TRoleClaim>
        where TRole : RoleBase
        where TUserRole : IUserRole
        where TRoleClaim : RoleClaimBase, new()
    {
        /// <summary>
        /// 缓存接口。
        /// </summary>
        protected IMemoryCache Cache { get; }

        private IRoleStoreBase<TRole, TUserRole, TRoleClaim> _store;
        /// <summary>
        /// 数据存储接口实例。
        /// </summary>
        protected new IRoleStoreBase<TRole, TUserRole, TRoleClaim> Store
        {
            get
            {
                if (_store == null)
                    _store = base.Store as IRoleStoreBase<TRole, TUserRole, TRoleClaim>;
                return _store;
            }
        }

        /// <summary>
        /// 初始化类<see cref="RoleManagerBase{TRole, TUserRole, TRoleClaim}"/>
        /// </summary>
        /// <param name="store">存储接口。</param>
        /// <param name="roleValidators">角色验证集合。</param>
        /// <param name="keyNormalizer">角色唯一键格式化接口。</param>
        /// <param name="errors">错误实例。</param>
        /// <param name="logger">日志接口。</param>
        /// <param name="cache">缓存接口。</param>
        protected RoleManagerBase(IRoleStore<TRole> store, IEnumerable<IRoleValidator<TRole>> roleValidators, ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, ILogger<RoleManager<TRole>> logger,
            IMemoryCache cache)
            : base(store, roleValidators, keyNormalizer, errors, logger)
        {
            Cache = cache;
        }

        /// <summary>
        /// 角色实例列表。
        /// </summary>
        public virtual IEnumerable<TRole> Load()
        {
            return Cache.GetOrCreate(_cacheKey, ctx =>
            {
                ctx.SetDefaultAbsoluteExpiration();
                return Store.LoadRoles();
            });
        }

        /// <summary>
        /// 通过ID获取角色实例。
        /// </summary>
        /// <param name="id">角色Id。</param>
        /// <returns>返回当前角色实例对象。</returns>
        public virtual Task<TRole> FindByIdAsync(int id)
        {
            return Store.FindByIdAsync(id, CancellationToken);
        }

        /// <summary>
        /// 角色实例列表。
        /// </summary>
        /// <returns>返回角色列表。</returns>
        public virtual Task<IEnumerable<TRole>> LoadAsync()
        {
            return Cache.GetOrCreateAsync(_cacheKey, async ctx =>
            {
                ctx.SetDefaultAbsoluteExpiration();
                return await Store.LoadRolesAsync(CancellationToken);
            });
        }

        /// <summary>
        /// 判断角色名称或唯一键是否已经存在。
        /// </summary>
        /// <param name="role">当前角色实例。</param>
        /// <returns>返回判断结果。</returns>
        public virtual IdentityResult IsDuplicated(TRole role)
        {
            var roles = Load().ToList();
            if (role.Name != null && roles.Any(x => x.RoleId != role.RoleId && x.Name.Equals(role.Name, StringComparison.OrdinalIgnoreCase)))
                return IdentityResult.Failed(ErrorDescriber.DuplicateRoleName(role.Name));
            if (role.NormalizedName != null && roles.Any(x => x.RoleId != role.RoleId && x.NormalizedName.Equals(role.NormalizedName, StringComparison.OrdinalIgnoreCase)))
                return IdentityResult.Failed(ErrorDescriber.DuplicateNormalizedRoleName(role.Name));
            return IdentityResult.Success;
        }

        /// <summary>
        /// 判断角色名称或唯一键是否已经存在。
        /// </summary>
        /// <param name="role">当前角色实例。</param>
        /// <returns>返回判断结果。</returns>
        public virtual async Task<IdentityResult> IsDuplicatedAsync(TRole role)
        {
            var roles = (await LoadAsync()).ToList();
            if (role.Name != null && roles.Any(x => x.RoleId != role.RoleId && x.Name.Equals(role.Name, StringComparison.OrdinalIgnoreCase)))
                return IdentityResult.Failed(ErrorDescriber.DuplicateRoleName(role.Name));
            if (role.NormalizedName != null && roles.Any(x => x.RoleId != role.RoleId && x.NormalizedName.Equals(role.NormalizedName, StringComparison.OrdinalIgnoreCase)))
                return IdentityResult.Failed(ErrorDescriber.DuplicateNormalizedRoleName(role.Name));
            return IdentityResult.Success;
        }

        /// <summary>
        /// 通过ID获取角色实例。
        /// </summary>
        /// <param name="id">角色Id。</param>
        /// <returns>返回当前角色实例对象。</returns>
        public virtual TRole FindById(int id)
        {
            return Store.FindById(id);
        }

        /// <summary>
        /// 通过角色名称获取角色实例。
        /// </summary>
        /// <param name="normalizedName">角色名称。</param>
        /// <returns>返回当前角色实例对象。</returns>
        public virtual TRole FindByName(string normalizedName)
        {
            normalizedName = NormalizeKey(normalizedName);
            return Store.FindByName(normalizedName);
        }

        /// <summary>
        /// 保存角色。
        /// </summary>
        /// <param name="role">角色实例。</param>
        /// <returns>返回角色保存结果。</returns>
        public virtual IdentityResult Save(TRole role)
        {
            if (role.RoleId > 0)
                return Update(role);
            return Create(role);
        }

        /// <summary>
        /// 添加用户组。
        /// </summary>
        /// <param name="role">用户组实例。</param>
        /// <returns>返回添加结果。</returns>
        public virtual IdentityResult Create(TRole role)
        {
            if (role == null)
                throw new ArgumentNullException(nameof(role));
            role.NormalizedName = NormalizeKey(role.NormalizedName);
            var result = IsDuplicated(role);
            if (!result.Succeeded)
                return result;
            return FromResult(Store.Create(role), role);
        }

        /// <summary>
        /// 添加角色。
        /// </summary>
        /// <param name="role">角色实例对象。</param>
        /// <returns>返回添加结果。</returns>
        public override async Task<IdentityResult> CreateAsync(TRole role)
        {
            if (role == null)
                throw new ArgumentNullException(nameof(role));
            role.NormalizedName = NormalizeKey(role.NormalizedName);
            var result = await IsDuplicatedAsync(role);
            if (!result.Succeeded)
                return result;
            return FromResult(await Store.CreateAsync(role, CancellationToken), role);
        }

        /// <summary>
        /// 更新用户角色。
        /// </summary>
        /// <param name="role">用户角色实例。</param>
        /// <returns>返回角色更新结果。</returns>
        public virtual IdentityResult Update(TRole role)
        {
            if (role == null)
                throw new ArgumentNullException(nameof(role));
            var result = IsDuplicated(role);
            if (!result.Succeeded)
                return result;
            return FromResult(Store.Update(role), role);
        }

        /// <summary>
        /// 更新角色。
        /// </summary>
        /// <param name="role">角色实例对象。</param>
        /// <returns>返回更新结果。</returns>
        public override async Task<IdentityResult> UpdateAsync(TRole role)
        {
            var result = await IsDuplicatedAsync(role);
            if (!result.Succeeded)
                return result;
            return FromResult(await base.UpdateAsync(role), role);
        }

        /// <summary>
        /// 保存角色。
        /// </summary>
        /// <param name="role">角色实例。</param>
        /// <returns>返回角色保存结果。</returns>
        public virtual Task<IdentityResult> SaveAsync(TRole role)
        {
            if (role.RoleId > 0)
                return UpdateAsync(role);
            return CreateAsync(role);
        }

        /// <summary>
        /// 上移角色。
        /// </summary>
        /// <param name="role">角色实例。</param>
        /// <returns>返回移动结果。</returns>
        public virtual bool MoveUp(TRole role)
        {
            return FromResult(Store.MoveUp(role), role);
        }

        /// <summary>
        /// 下移角色。
        /// </summary>
        /// <param name="role">角色实例。</param>
        /// <returns>返回移动结果。</returns>
        public virtual bool MoveDown(TRole role)
        {
            return FromResult(Store.MoveDown(role), role);
        }

        /// <summary>
        /// 上移角色。
        /// </summary>
        /// <param name="role">角色实例。</param>
        /// <returns>返回移动结果。</returns>
        public virtual async Task<bool> MoveUpAsync(TRole role)
        {
            return FromResult(await Store.MoveUpAsync(role), role);
        }

        /// <summary>
        /// 下移角色。
        /// </summary>
        /// <param name="role">角色实例。</param>
        /// <returns>返回移动结果。</returns>
        public virtual async Task<bool> MoveDownAsync(TRole role)
        {
            return FromResult(await Store.MoveDownAsync(role), role);
        }

        /// <summary>
        /// 更新用户角色。
        /// </summary>
        /// <param name="role">用户角色实例。</param>
        /// <returns>返回角色更新结果。</returns>
        public virtual IdentityResult Delete(TRole role)
        {
            return FromResult(Store.Delete(role), role);
        }

        /// <summary>
        /// 删除角色。
        /// </summary>
        /// <param name="role">角色实例对象。</param>
        /// <returns>返回删除结果。</returns>
        public override async Task<IdentityResult> DeleteAsync(TRole role)
        {
            return FromResult(await base.DeleteAsync(role), role);
        }

        /// <summary>
        /// 如果成功移除缓存。
        /// </summary>
        /// <param name="result">返回结果。</param>
        /// <param name="role">当前角色实例。</param>
        /// <returns>返回结果。</returns>
        protected IdentityResult FromResult(IdentityResult result, TRole role)
        {
            FromResult(result.Succeeded, role);
            return result;
        }

        private static readonly Type _cacheKey = typeof(TRole);
        /// <summary>
        /// 如果成功移除缓存。
        /// </summary>
        /// <param name="result">返回结果。</param>
        /// <param name="role">当前角色实例。</param>
        /// <returns>返回结果。</returns>
        protected virtual bool FromResult(bool result, TRole role)
        {
            if (result)
                Cache.Remove(_cacheKey);
            return result;
        }
    }
}