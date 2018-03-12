using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using Mozlite.Data;

namespace Mozlite.Extensions.Security.Stores
{
    /// <summary>
    /// 数据库存储类。
    /// </summary>
    /// <typeparam name="TRole">角色类型。</typeparam>
    /// <typeparam name="TUserRole">用户角色类型。</typeparam>
    /// <typeparam name="TRoleClaim">角色声明类型。</typeparam>
    public abstract class RoleStore<TRole, TUserRole, TRoleClaim>
        : RoleStoreBase<TRole, TUserRole, TRoleClaim>,
        IRoleStoreBase<TRole, TUserRole, TRoleClaim>
        where TRole : RoleBase
        where TUserRole : IUserRole
        where TRoleClaim : RoleClaimBase, new()
    {
        /// <summary>
        /// 缓存实例。
        /// </summary>
        public IMemoryCache Cache { get; }

        /// <summary>
        /// 角色数据库操作接口。
        /// </summary>
        public IDbContext<TRole> RoleContext { get; }

        /// <summary>
        /// 用户角色数据库操作接口。
        /// </summary>
        public IDbContext<TUserRole> UserRoleContext { get; }

        /// <summary>
        /// 用户声明数据库操作接口。
        /// </summary>
        public IDbContext<TRoleClaim> RoleClaimContext { get; }

        private static readonly Type _cacheKey = typeof(TRole);
        /// <summary>
        /// 缓存键。
        /// </summary>
        public virtual object CacheKey => _cacheKey;

        /// <summary>
        /// 初始化类<see cref="RoleStore{TRole,TUserRole,TRoleClaim}"/>。
        /// </summary>
        /// <param name="describer">错误描述<see cref="IdentityErrorDescriber"/>实例。</param>
        /// <param name="roleContext">角色数据库操作接口。</param>
        /// <param name="userRoleContext">用户角色数据库操作接口。</param>
        /// <param name="roleClaimContext">用户声明数据库操作接口。</param>
        /// <param name="cache">缓存接口。</param>
        protected RoleStore(IdentityErrorDescriber describer,
            IDbContext<TRole> roleContext,
            IDbContext<TUserRole> userRoleContext,
            IDbContext<TRoleClaim> roleClaimContext,
            IMemoryCache cache) : base(describer)
        {
            Cache = cache;
            RoleContext = roleContext;
            UserRoleContext = userRoleContext;
            RoleClaimContext = roleClaimContext;
        }

        /// <summary>
        /// 添加用户角色。
        /// </summary>
        /// <param name="role">用户角色实例。</param>
        /// <param name="cancellationToken">取消标识。</param>
        /// <returns>返回角色添加结果。</returns>
        public override async Task<IdentityResult> CreateAsync(TRole role, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }
            if (role is IRoleEventHandler<TRole> handler)
            {
                if (await RoleContext.BeginTransactionAsync(async db =>
                 {
                     if (!await db.CreateAsync(role, cancellationToken))
                         return false;
                     if (!await handler.OnCreatedAsync(db, cancellationToken))
                         return false;
                     return true;
                 }, cancellationToken: cancellationToken))
                {
                    Cache.Remove(CacheKey);
                    return IdentityResult.Success;
                }
            }
            else if (await RoleContext.CreateAsync(role, cancellationToken))
            {
                Cache.Remove(CacheKey);
                return IdentityResult.Success;
            }
            return IdentityResult.Failed(ErrorDescriber.DefaultError());
        }

        /// <summary>
        /// 更新用户角色。
        /// </summary>
        /// <param name="role">用户角色实例。</param>
        /// <param name="cancellationToken">取消标识。</param>
        /// <returns>返回角色更新结果。</returns>
        public override async Task<IdentityResult> UpdateAsync(TRole role, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }
            if (role is IRoleEventHandler<TRole> handler)
            {
                if (await RoleContext.BeginTransactionAsync(async db =>
                {
                    if (!await handler.OnUpdateAsync(db, cancellationToken))
                        return false;
                    if (!await db.UpdateAsync(role, cancellationToken))
                        return false;
                    return true;
                }, cancellationToken: cancellationToken))
                {
                    Cache.Remove(CacheKey);
                    return IdentityResult.Success;
                }
            }
            else if (await RoleContext.UpdateAsync(role, cancellationToken))
            {
                Cache.Remove(CacheKey);
                return IdentityResult.Success;
            }
            return IdentityResult.Failed(ErrorDescriber.DefaultError());
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
            if (role is IRoleEventHandler<TRole> handler)
            {
                if (await RoleContext.BeginTransactionAsync(async db =>
                {
                    if (!await handler.OnDeleteAsync(db, cancellationToken))
                        return false;
                    if (!await db.DeleteAsync(role.RoleId, cancellationToken))
                        return false;
                    return true;
                }, cancellationToken: cancellationToken))
                {
                    Cache.Remove(CacheKey);
                    return IdentityResult.Success;
                }
            }
            else if (await RoleContext.DeleteAsync(role.RoleId, cancellationToken))
            {
                Cache.Remove(CacheKey);
                return IdentityResult.Success;
            }
            return IdentityResult.Failed(ErrorDescriber.DefaultError());
        }

        /// <summary>
        /// 通过ID获取角色实例。
        /// </summary>
        /// <param name="id">角色Id。</param>
        /// <param name="cancellationToken">取消标识。</param>
        /// <returns>返回当前角色实例对象。</returns>
        public override async Task<TRole> FindByIdAsync(int id, CancellationToken cancellationToken = default(CancellationToken))
        {
            var roles = await LoadRolesAsync();
            return roles.SingleOrDefault(x => x.RoleId == id);
        }

        /// <summary>
        /// 通过角色名称获取角色实例。
        /// </summary>
        /// <param name="normalizedName">角色名称。</param>
        /// <param name="cancellationToken">取消标识。</param>
        /// <returns>返回当前角色实例对象。</returns>
        public override async Task<TRole> FindByNameAsync(string normalizedName, CancellationToken cancellationToken = default(CancellationToken))
        {
            var roles = await LoadRolesAsync();
            return roles.SingleOrDefault(x => x.NormalizedName == normalizedName);
        }

        /// <summary>
        /// 获取角色声明列表。
        /// </summary>
        /// <param name="role">角色实例。</param>
        /// <param name="cancellationToken">取消标志。</param>
        /// <returns>返回当前角色的声明列表。</returns>
        public override async Task<IList<Claim>> GetClaimsAsync(TRole role, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }
            var claims = await RoleClaimContext.FetchAsync(x => x.RoleId == role.RoleId, cancellationToken);
            return claims.Select(x => x.ToClaim()).ToList();
        }

        /// <summary>
        /// 添加角色声明。
        /// </summary>
        /// <param name="role">角色实例对象。</param>
        /// <param name="claim">声明实例。</param>
        /// <param name="cancellationToken">取消标志。</param>
        public override async Task AddClaimAsync(TRole role, Claim claim, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }
            if (claim == null)
            {
                throw new ArgumentNullException(nameof(claim));
            }
            var roleClaim = new TRoleClaim { RoleId = role.RoleId };
            roleClaim.InitializeFromClaim(claim);
            await RoleClaimContext.CreateAsync(roleClaim, cancellationToken);
        }

        /// <summary>
        /// 移除角色声明。
        /// </summary>
        /// <param name="role">角色实例对象。</param>
        /// <param name="claim">声明实例。</param>
        /// <param name="cancellationToken">取消标志。</param>
        public override async Task RemoveClaimAsync(TRole role, Claim claim, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }
            if (claim == null)
            {
                throw new ArgumentNullException(nameof(claim));
            }
            await RoleClaimContext.DeleteAsync(x =>
                x.RoleId == role.RoleId && x.ClaimType == claim.Type && x.ClaimValue == claim.Value, cancellationToken);
        }

        /// <summary>
        /// 获取所有角色。
        /// </summary>
        /// <returns>返回角色列表。</returns>
        public virtual async Task<IEnumerable<TRole>> LoadRolesAsync()
        {
            return await Cache.GetOrCreateAsync(CacheKey, async ctx =>
            {
                ctx.SetDefaultAbsoluteExpiration();
                return await RoleContext.FetchAsync();
            });
        }

        /// <summary>
        /// 获取所有角色。
        /// </summary>
        /// <returns>返回角色列表。</returns>
        public virtual IEnumerable<TRole> LoadRoles()
        {
            return Cache.GetOrCreate(CacheKey, ctx =>
            {
                ctx.SetDefaultAbsoluteExpiration();
                return RoleContext.Fetch();
            });
        }

        /// <summary>
        /// 获取当前角色可查询实例。
        /// </summary>
        public override System.Linq.IQueryable<TRole> Roles => LoadRoles().AsQueryable();
    }
}