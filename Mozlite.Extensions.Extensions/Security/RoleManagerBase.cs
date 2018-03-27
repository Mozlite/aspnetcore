using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using Mozlite.Extensions.Security.Stores;
using Microsoft.Extensions.Caching.Memory;
using Mozlite.Extensions.Extensions.Security.Stores;

namespace Mozlite.Extensions.Extensions.Security
{
    /// <summary>
    /// 角色管理实现类。
    /// </summary>
    /// <typeparam name="TRole">角色类型。</typeparam>
    /// <typeparam name="TUserRole">用户角色类型。</typeparam>
    /// <typeparam name="TRoleClaim">角色声明类型。</typeparam>
    public abstract class RoleManagerBase<TRole, TUserRole, TRoleClaim>
        : Mozlite.Extensions.Security.RoleManagerBase<TRole, TUserRole, TRoleClaim>, IRoleManager<TRole, TUserRole, TRoleClaim>
        where TRole : RoleExBase
        where TUserRole : IUserRole
        where TRoleClaim : RoleClaimBase, new()
    {
        private readonly ISiteContextAccessorBase _siteContextAccessor;

        /// <summary>
        /// 当前网站实例。
        /// </summary>
        protected SiteContextBase Site => _siteContextAccessor.SiteContext;


        private IRoleExStoreBase<TRole, TUserRole, TRoleClaim> _store;
        /// <summary>
        /// 数据存储接口实例。
        /// </summary>
        protected new IRoleExStoreBase<TRole, TUserRole, TRoleClaim> Store
        {
            get
            {
                if (_store == null)
                    _store = base.Store as IRoleExStoreBase<TRole, TUserRole, TRoleClaim>;
                return _store;
            }
        }

        /// <summary>
        /// 获取缓存键。
        /// </summary>
        /// <param name="siteId">当前网站Id。</param>
        /// <returns>返回缓存键实例。</returns>
        protected object GetCacheKey(int siteId)
        {
            return new Tuple<int, Type>(siteId, typeof(TRole));
        }

        /// <summary>
        /// 通过角色名称获取角色实例。
        /// </summary>
        /// <param name="normalizedName">角色名称。</param>
        /// <returns>返回当前角色实例对象。</returns>
        public override TRole FindByName(string normalizedName)
        {
            return FindByName(Site.SiteId, normalizedName);
        }

        /// <summary>
        /// 通过角色名称获取角色实例。
        /// </summary>
        /// <param name="siteId">网站Id。</param>
        /// <param name="normalizedName">角色名称。</param>
        /// <returns>返回当前角色实例对象。</returns>
        public virtual TRole FindByName(int siteId, string normalizedName)
        {
            normalizedName = NormalizeKey(normalizedName);
            return Store.FindByName(siteId, normalizedName);
        }

        /// <summary>
        /// 通过角色名称获取角色实例。
        /// </summary>
        /// <param name="normalizedName">角色名称。</param>
        /// <returns>返回当前角色实例对象。</returns>
        public override Task<TRole> FindByNameAsync(string normalizedName)
        {
            normalizedName = NormalizeKey(normalizedName);
            return FindByNameAsync(Site.SiteId, normalizedName);
        }

        /// <summary>
        /// 通过角色名称获取角色实例。
        /// </summary>
        /// <param name="siteId">网站Id。</param>
        /// <param name="normalizedName">角色名称。</param>
        /// <returns>返回当前角色实例对象。</returns>
        public virtual Task<TRole> FindByNameAsync(int siteId, string normalizedName)
        {
            normalizedName = NormalizeKey(normalizedName);
            return Store.FindByNameAsync(siteId, normalizedName, CancellationToken);
        }

        /// <summary>
        /// 获取所有角色。
        /// </summary>
        /// <returns>返回角色列表。</returns>
        public override Task<IEnumerable<TRole>> LoadAsync()
        {
            return LoadAsync(Site.SiteId);
        }

        /// <summary>
        /// 获取所有角色。
        /// </summary>
        /// <param name="siteId">网站Id。</param>
        /// <returns>返回角色列表。</returns>
        public virtual Task<IEnumerable<TRole>> LoadAsync(int siteId)
        {
            return Cache.GetOrCreateAsync(GetCacheKey(siteId), ctx =>
            {
                ctx.SetDefaultAbsoluteExpiration();
                return Store.LoadRolesAsync(siteId);
            });
        }

        /// <summary>
        /// 获取所有角色。
        /// </summary>
        /// <returns>返回角色列表。</returns>
        public override IEnumerable<TRole> Load()
        {
            return Load(Site.SiteId);
        }

        /// <summary>
        /// 获取所有角色。
        /// </summary>
        /// <param name="siteId">网站Id。</param>
        /// <returns>返回角色列表。</returns>
        public virtual IEnumerable<TRole> Load(int siteId)
        {
            return Cache.GetOrCreate(GetCacheKey(siteId), ctx =>
            {
                ctx.SetDefaultAbsoluteExpiration();
                return Store.LoadRoles(siteId);
            });
        }

        /// <summary>
        /// 如果成功移除缓存。
        /// </summary>
        /// <param name="result">返回结果。</param>
        /// <param name="role">当前角色实例。</param>
        /// <returns>返回结果。</returns>
        protected override bool FromResult(bool result, TRole role)
        {
            if (result)
                Cache.Remove(GetCacheKey(role.SiteId));
            return result;
        }

        /// <summary>
        /// 初始化类<see cref="RoleManagerBase{TRole,TUserRole,TRoleClaim}"/>
        /// </summary>
        /// <param name="store">存储接口。</param>
        /// <param name="roleValidators">角色验证集合。</param>
        /// <param name="keyNormalizer">角色唯一键格式化接口。</param>
        /// <param name="errors">错误实例。</param>
        /// <param name="logger">日志接口。</param>
        /// <param name="cache">缓存接口。</param>
        /// <param name="siteContextAccessor">网站上下文访问接口。</param>
        protected RoleManagerBase(IRoleStore<TRole> store, IEnumerable<IRoleValidator<TRole>> roleValidators, ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, ILogger<RoleManager<TRole>> logger, IMemoryCache cache, ISiteContextAccessorBase siteContextAccessor)
            : base(store, roleValidators, keyNormalizer, errors, logger, cache)
        {
            _siteContextAccessor = siteContextAccessor;
        }
    }
}