using System;
using Mozlite.Data;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using Mozlite.Extensions.Sites;

namespace Mozlite.Extensions.Security.Stores
{
    /// <summary>
    /// 数据库存储类。
    /// </summary>
    /// <typeparam name="TRole">角色类型。</typeparam>
    /// <typeparam name="TUserRole">用户角色类型。</typeparam>
    /// <typeparam name="TRoleClaim">角色声明类型。</typeparam>
    public class RoleStoreEx<TRole, TUserRole, TRoleClaim> : RoleStore<TRole, TUserRole, TRoleClaim>
        where TRole : RoleExBase
        where TUserRole : IUserRole
        where TRoleClaim : RoleClaimBase, new()
    {
        private readonly ISiteContextAccessorBase _siteContextAccessor;

        /// <summary>
        /// 缓存实例。
        /// </summary>
        protected SiteContextBase Site => _siteContextAccessor.SiteContext;

        /// <summary>
        /// 缓存键。
        /// </summary>
        public override object CacheKey => new Tuple<int, Type>(Site.SiteId, typeof(TRole));

        /// <summary>
        /// 添加用户角色。
        /// </summary>
        /// <param name="role">用户角色实例。</param>
        /// <param name="cancellationToken">取消标识。</param>
        /// <returns>返回角色添加结果。</returns>
        public override Task<IdentityResult> CreateAsync(TRole role, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (role?.SiteId == 0)
                role.SiteId = Site.SiteId;
            return base.CreateAsync(role, cancellationToken);
        }

        /// <summary>
        /// 更新用户角色。
        /// </summary>
        /// <param name="role">用户角色实例。</param>
        /// <param name="cancellationToken">取消标识。</param>
        /// <returns>返回角色更新结果。</returns>
        public override Task<IdentityResult> UpdateAsync(TRole role, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (role?.SiteId == 0)
                role.SiteId = Site.SiteId;
            return base.UpdateAsync(role, cancellationToken);
        }

        /// <summary>
        /// 删除用户角色。
        /// </summary>
        /// <param name="role">用户角色实例。</param>
        /// <param name="cancellationToken">取消标识。</param>
        /// <returns>返回角色删除结果。</returns>
        public override async Task<IdentityResult> DeleteAsync(TRole role, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }
            if (role.SiteId == 0)
                role.SiteId = Site.SiteId;
            await RoleContext.DeleteAsync(x => x.RoleId == role.RoleId && x.SiteId == role.SiteId, cancellationToken);
            Cache.Remove(CacheKey);
            return IdentityResult.Success;
        }

        /// <summary>
        /// 获取所有角色。
        /// </summary>
        /// <returns>返回角色列表。</returns>
        public override async Task<IEnumerable<TRole>> LoadRolesAsync()
        {
            return await Cache.GetOrCreateAsync(CacheKey, async ctx =>
            {
                ctx.SetDefaultAbsoluteExpiration();
                return await RoleContext.FetchAsync(x => x.SiteId == Site.SiteId);
            });
        }

        /// <summary>
        /// 获取所有角色。
        /// </summary>
        /// <returns>返回角色列表。</returns>
        public override IEnumerable<TRole> LoadRoles()
        {
            return Cache.GetOrCreate(CacheKey, ctx =>
            {
                ctx.SetDefaultAbsoluteExpiration();
                return RoleContext.Fetch(x => x.SiteId == Site.SiteId);
            });
        }
        
        /// <summary>
        /// 初始化类<see cref="RoleStore{TRole,TUserRole,TRoleClaim}"/>。
        /// </summary>
        /// <param name="describer">错误描述<see cref="IdentityErrorDescriber"/>实例。</param>
        /// <param name="roleContext">角色数据库操作接口。</param>
        /// <param name="userRoleContext">用户角色数据库操作接口。</param>
        /// <param name="roleClaimContext">用户声明数据库操作接口。</param>
        /// <param name="cache">缓存接口。</param>
        /// <param name="siteContextAccessor">网站上下文访问器接口。</param>
        public RoleStoreEx(IdentityErrorDescriber describer, IDbContext<TRole> roleContext, IDbContext<TUserRole> userRoleContext, IDbContext<TRoleClaim> roleClaimContext, IMemoryCache cache, ISiteContextAccessorBase siteContextAccessor) 
            : base(describer, roleContext, userRoleContext, roleClaimContext, cache)
        {
            _siteContextAccessor = siteContextAccessor;
        }
    }
}