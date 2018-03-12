using System;
using Mozlite.Data;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
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
        /// 获取当前最大角色等级。
        /// </summary>
        /// <param name="role">当前角色实例。</param>
        /// <returns>返回最大角色等级。</returns>
        protected override Task<int> GetMaxRoleLevelAsync(TRole role)
        {
            return RoleContext.MaxAsync(x => x.RoleLevel, x => x.SiteId == role.SiteId && x.RoleLevel < int.MaxValue);
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
        /// 上移角色。
        /// </summary>
        /// <param name="roleId">角色Id。</param>
        /// <returns>返回移动结果。</returns>
        public override bool MoveUp(int roleId)
        {
            var site = RoleContext.Find(roleId);
            if (RoleContext.MoveUp(roleId, x => x.RoleLevel, x => x.SiteId == site.SiteId && x.RoleLevel > 0 && x.RoleLevel < int.MaxValue))
            {
                Cache.Remove(CacheKey);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 下移角色。
        /// </summary>
        /// <param name="roleId">角色Id。</param>
        /// <returns>返回移动结果。</returns>
        public override bool MoveDown(int roleId)
        {
            var site = RoleContext.Find(roleId);
            if (RoleContext.MoveDown(roleId, x => x.RoleLevel, x => x.SiteId == site.SiteId && x.RoleLevel > 0 && x.RoleLevel < int.MaxValue))
            {
                Cache.Remove(CacheKey);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 上移角色。
        /// </summary>
        /// <param name="roleId">角色Id。</param>
        /// <param name="cancellationToken">取消标识。</param>
        /// <returns>返回移动结果。</returns>
        public override async Task<bool> MoveUpAsync(int roleId, CancellationToken cancellationToken = default(CancellationToken))
        {
            var site = await RoleContext.FindAsync(roleId, cancellationToken);
            if (await RoleContext.MoveUpAsync(roleId, x => x.RoleLevel, x => x.SiteId == site.SiteId && x.RoleLevel > 0 && x.RoleLevel < int.MaxValue, cancellationToken))
            {
                Cache.Remove(CacheKey);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 下移角色。
        /// </summary>
        /// <param name="roleId">角色Id。</param>
        /// <param name="cancellationToken">取消标识。</param>
        /// <returns>返回移动结果。</returns>
        public override async Task<bool> MoveDownAsync(int roleId, CancellationToken cancellationToken = default(CancellationToken))
        {
            var site = await RoleContext.FindAsync(roleId, cancellationToken);
            if (await RoleContext.MoveDownAsync(roleId, x => x.RoleLevel, x => x.SiteId == site.SiteId && x.RoleLevel > 0 && x.RoleLevel < int.MaxValue, cancellationToken))
            {
                Cache.Remove(CacheKey);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 判断角色名称或唯一键是否已经存在。
        /// </summary>
        /// <param name="role">当前角色实例。</param>
        /// <returns>返回判断结果。</returns>
        public override async Task<IdentityResult> IsDuplicatedAsync(TRole role)
        {
            if (await RoleContext.AnyAsync(x => x.RoleId != role.RoleId && x.Name == role.Name && x.SiteId == role.SiteId))
                return IdentityResult.Failed(ErrorDescriber.DuplicateRoleName(role.Name));
            if (await RoleContext.AnyAsync(x => x.RoleId != role.RoleId && x.NormalizedName == role.NormalizedName && x.SiteId == role.SiteId))
                return IdentityResult.Failed(ErrorDescriber.DuplicateNormalizedRoleName(role.Name));
            return IdentityResult.Success;
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