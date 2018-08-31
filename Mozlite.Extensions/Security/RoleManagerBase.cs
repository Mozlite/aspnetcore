using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Mozlite.Extensions.Security.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mozlite.Extensions.Security
{
    /// <summary>
    /// 用户组管理实现类。
    /// </summary>
    /// <typeparam name="TRole">用户组类型。</typeparam>
    /// <typeparam name="TUserRole">用户用户组类型。</typeparam>
    /// <typeparam name="TRoleClaim">用户组声明类型。</typeparam>
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

        private readonly IRoleStoreBase<TRole, TUserRole, TRoleClaim> _store;
        /// <summary>
        /// 用户组数据库操作接口。
        /// </summary>
        protected IRoleDbContext<TRole, TUserRole, TRoleClaim> DbContext { get; }

        /// <summary>
        /// 初始化类<see cref="RoleManagerBase{TRole, TUserRole, TRoleClaim}"/>
        /// </summary>
        /// <param name="store">存储接口。</param>
        /// <param name="roleValidators">用户组验证集合。</param>
        /// <param name="keyNormalizer">用户组唯一键格式化接口。</param>
        /// <param name="errors">错误实例。</param>
        /// <param name="logger">日志接口。</param>
        /// <param name="cache">缓存接口。</param>
        protected RoleManagerBase(IRoleStore<TRole> store, IEnumerable<IRoleValidator<TRole>> roleValidators, ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, ILogger<RoleManager<TRole>> logger,
            IMemoryCache cache)
            : base(store, roleValidators, keyNormalizer, errors, logger)
        {
            _store = store as IRoleStoreBase<TRole, TUserRole, TRoleClaim>;
            DbContext = store as IRoleDbContext<TRole, TUserRole, TRoleClaim>;
            Cache = cache;
        }

        /// <summary>
        /// 用户组实例列表。
        /// </summary>
        public virtual IEnumerable<TRole> Load()
        {
            return Cache.GetOrCreate(_cacheKey, ctx =>
            {
                ctx.SetDefaultAbsoluteExpiration();
                return _store.LoadRoles();
            });
        }

        /// <summary>
        /// 通过ID获取用户组实例。
        /// </summary>
        /// <param name="id">用户组Id。</param>
        /// <returns>返回当前用户组实例对象。</returns>
        public virtual Task<TRole> FindByIdAsync(int id)
        {
            return _store.FindByIdAsync(id, CancellationToken);
        }

        /// <summary>
        /// 用户组实例列表。
        /// </summary>
        /// <returns>返回用户组列表。</returns>
        public virtual Task<IEnumerable<TRole>> LoadAsync()
        {
            return Cache.GetOrCreateAsync(_cacheKey, async ctx =>
            {
                ctx.SetDefaultAbsoluteExpiration();
                return await _store.LoadRolesAsync(CancellationToken);
            });
        }

        /// <summary>
        /// 判断用户组名称或唯一键是否已经存在。
        /// </summary>
        /// <param name="role">当前用户组实例。</param>
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
        /// 判断用户组名称或唯一键是否已经存在。
        /// </summary>
        /// <param name="role">当前用户组实例。</param>
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
        /// 通过ID获取用户组实例。
        /// </summary>
        /// <param name="id">用户组Id。</param>
        /// <returns>返回当前用户组实例对象。</returns>
        public virtual TRole FindById(int id)
        {
            return _store.FindById(id);
        }

        /// <summary>
        /// 通过用户组名称获取用户组实例。
        /// </summary>
        /// <param name="normalizedName">用户组名称。</param>
        /// <returns>返回当前用户组实例对象。</returns>
        public virtual TRole FindByName(string normalizedName)
        {
            normalizedName = NormalizeKey(normalizedName);
            return _store.FindByName(normalizedName);
        }

        /// <summary>
        /// 保存用户组。
        /// </summary>
        /// <param name="role">用户组实例。</param>
        /// <returns>返回用户组保存结果。</returns>
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
            return FromResult(_store.Create(role), role);
        }

        /// <summary>
        /// 添加用户组。
        /// </summary>
        /// <param name="role">用户组实例对象。</param>
        /// <returns>返回添加结果。</returns>
        public override async Task<IdentityResult> CreateAsync(TRole role)
        {
            if (role == null)
                throw new ArgumentNullException(nameof(role));
            role.NormalizedName = NormalizeKey(role.NormalizedName);
            var result = await IsDuplicatedAsync(role);
            if (!result.Succeeded)
                return result;
            return FromResult(await _store.CreateAsync(role, CancellationToken), role);
        }

        /// <summary>
        /// 更新用户用户组。
        /// </summary>
        /// <param name="role">用户用户组实例。</param>
        /// <returns>返回用户组更新结果。</returns>
        public virtual IdentityResult Update(TRole role)
        {
            if (role == null)
                throw new ArgumentNullException(nameof(role));
            var result = IsDuplicated(role);
            if (!result.Succeeded)
                return result;
            return FromResult(_store.Update(role), role);
        }

        /// <summary>
        /// 更新用户组。
        /// </summary>
        /// <param name="role">用户组实例对象。</param>
        /// <returns>返回更新结果。</returns>
        public override async Task<IdentityResult> UpdateAsync(TRole role)
        {
            var result = await IsDuplicatedAsync(role);
            if (!result.Succeeded)
                return result;
            return FromResult(await base.UpdateAsync(role), role);
        }

        /// <summary>
        /// 保存用户组。
        /// </summary>
        /// <param name="role">用户组实例。</param>
        /// <returns>返回用户组保存结果。</returns>
        public virtual Task<IdentityResult> SaveAsync(TRole role)
        {
            if (role.RoleId > 0)
                return UpdateAsync(role);
            return CreateAsync(role);
        }

        /// <summary>
        /// 上移用户组。
        /// </summary>
        /// <param name="role">用户组实例。</param>
        /// <returns>返回移动结果。</returns>
        public virtual bool MoveUp(TRole role)
        {
            return FromResult(_store.MoveUp(role), role);
        }

        /// <summary>
        /// 下移用户组。
        /// </summary>
        /// <param name="role">用户组实例。</param>
        /// <returns>返回移动结果。</returns>
        public virtual bool MoveDown(TRole role)
        {
            return FromResult(_store.MoveDown(role), role);
        }

        /// <summary>
        /// 上移用户组。
        /// </summary>
        /// <param name="role">用户组实例。</param>
        /// <returns>返回移动结果。</returns>
        public virtual async Task<bool> MoveUpAsync(TRole role)
        {
            return FromResult(await _store.MoveUpAsync(role), role);
        }

        /// <summary>
        /// 下移用户组。
        /// </summary>
        /// <param name="role">用户组实例。</param>
        /// <returns>返回移动结果。</returns>
        public virtual async Task<bool> MoveDownAsync(TRole role)
        {
            return FromResult(await _store.MoveDownAsync(role), role);
        }

        /// <summary>
        /// 更新用户用户组。
        /// </summary>
        /// <param name="role">用户用户组实例。</param>
        /// <returns>返回用户组更新结果。</returns>
        public virtual IdentityResult Delete(TRole role)
        {
            return FromResult(_store.Delete(role), role);
        }

        /// <summary>
        /// 删除用户组。
        /// </summary>
        /// <param name="role">用户组实例对象。</param>
        /// <returns>返回删除结果。</returns>
        public override async Task<IdentityResult> DeleteAsync(TRole role)
        {
            return FromResult(await base.DeleteAsync(role), role);
        }

        /// <summary>
        /// 如果成功移除缓存。
        /// </summary>
        /// <param name="result">返回结果。</param>
        /// <param name="role">当前用户组实例。</param>
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
        /// <param name="role">当前用户组实例。</param>
        /// <returns>返回结果。</returns>
        protected virtual bool FromResult(bool result, TRole role)
        {
            if (result)
                Cache.Remove(_cacheKey);
            return result;
        }
    }
}