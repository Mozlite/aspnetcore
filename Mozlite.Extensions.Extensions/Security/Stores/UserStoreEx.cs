using System;
using System.Linq;
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
    /// 用户存储基类，包含用户角色的相关操作。
    /// </summary>
    /// <typeparam name="TUser">用户类型。</typeparam>
    /// <typeparam name="TRole">角色类型。</typeparam>
    /// <typeparam name="TUserClaim">用户声明类型。</typeparam>
    /// <typeparam name="TUserRole">用户角色类型。</typeparam>
    /// <typeparam name="TUserLogin">用户登陆类型。</typeparam>
    /// <typeparam name="TUserToken">用户标识类型。</typeparam>
    /// <typeparam name="TRoleClaim">角色声明类型。</typeparam>
    public class UserStoreEx<TUser, TRole, TUserClaim, TUserRole, TUserLogin, TUserToken, TRoleClaim>
        : UserStore<TUser, TRole, TUserClaim, TUserRole, TUserLogin, TUserToken, TRoleClaim>
        where TUser : UserExBase
        where TRole : RoleExBase
        where TUserClaim : UserClaimBase, new()
        where TUserRole : UserRoleBase, new()
        where TUserLogin : UserLoginBase, new()
        where TUserToken : UserTokenBase, new()
        where TRoleClaim : RoleClaimBase, new()
    {
        private readonly ISiteContextAccessorBase _siteContextAccessor;

        /// <summary>
        /// 当前网站上下文。
        /// </summary>
        protected SiteContextBase Site => _siteContextAccessor.SiteContext;

        /// <summary>
        /// 新建用户实例。
        /// </summary>
        /// <param name="user">用户实例对象。</param>
        /// <param name="cancellationToken">取消标志。</param>
        /// <returns>返回添加用户结果。</returns>
        public override Task<IdentityResult> CreateAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (user?.SiteId == 0)
                user.SiteId = Site.SiteId;
            return base.CreateAsync(user, cancellationToken);
        }

        /// <summary>
        /// 更新用户实例。
        /// </summary>
        /// <param name="user">当前用户实例。</param>
        /// <param name="cancellationToken">取消标志。</param>
        /// <returns>返回更新结果。</returns>
        public override Task<IdentityResult> UpdateAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (user?.SiteId == 0)
                user.SiteId = Site.SiteId;
            return base.UpdateAsync(user, cancellationToken);
        }

        /// <summary>
        /// 删除用户实例。
        /// </summary>
        /// <param name="user">当前用户实例。</param>
        /// <param name="cancellationToken">取消标志。</param>
        /// <returns>返回删除结果。</returns>
        public override async Task<IdentityResult> DeleteAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            await UserContext.DeleteAsync(x => x.UserId == user.UserId && x.SiteId == Site.SiteId, cancellationToken);
            return IdentityResult.Success;
        }

        /// <summary>
        /// 通过用户验证名称查询用户实例。
        /// </summary>
        /// <param name="normalizedUserName">当前验证名称。</param>
        /// <param name="cancellationToken">取消标志。</param>
        /// <returns>
        /// 返回当前用户实例对象。
        /// </returns>
        public override Task<TUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken = default(CancellationToken))
        {
            return UserContext.FindAsync(x => x.NormalizedUserName == normalizedUserName && x.SiteId == Site.SiteId, cancellationToken);
        }

        /// <summary>
        /// 通过Id获取用户实例。
        /// </summary>
        /// <param name="userId">用户Id。</param>
        /// <param name="cancellationToken">取消标志。</param>
        /// <returns>返回当前用户实例。</returns>
        public override Task<TUser> FindUserAsync(int userId, CancellationToken cancellationToken = default(CancellationToken))
        {
            return UserContext.FindAsync(x => x.UserId == userId && x.SiteId == Site.SiteId, cancellationToken);
        }

        /// <summary>
        /// 通过电子邮件获取用户实例。
        /// </summary>
        /// <param name="normalizedEmail">验证邮件地址。</param>
        /// <param name="cancellationToken">取消标志。</param>
        /// <returns>返回当前用户实例。</returns>
        public override Task<TUser> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken = default(CancellationToken))
        {
            return UserContext.FindAsync(x => x.NormalizedEmail == normalizedEmail && x.SiteId == Site.SiteId, cancellationToken);
        }

        /// <summary>
        /// 通过验证角色名称获取角色实例。
        /// </summary>
        /// <param name="normalizedRoleName">验证角色名称。</param>
        /// <param name="cancellationToken">取消标志。</param>
        /// <returns>角色实例对象。</returns>
        protected override async Task<TRole> FindRoleAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            var roles = await LoadRolesAsync();
            return roles.SingleOrDefault(x => x.SiteId == Site.SiteId &&
                x.NormalizedName.Equals(normalizedRoleName, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// 实例化一个用户角色实例。
        /// </summary>
        /// <param name="user">当前用户实例。</param>
        /// <param name="role">角色实例。</param>
        /// <returns>返回用户角色实例。</returns>
        protected override TUserRole CreateUserRole(TUser user, TRole role)
        {
            return new TUserRole
            {
                RoleId = role.RoleId,
                UserId = user.UserId
            };
        }

        /// <summary>
        /// 检索当前角色的所有用户列表。
        /// </summary>
        /// <param name="normalizedRoleName">验证角色名称。</param>
        /// <param name="cancellationToken">取消标志。</param>
        /// <returns>
        /// 返回用户列表。 
        /// </returns>
        public override async Task<IList<TUser>> GetUsersInRoleAsync(string normalizedRoleName, CancellationToken cancellationToken = default(CancellationToken))
        {
            var role = await FindRoleAsync(normalizedRoleName, cancellationToken);
            if (role == null) return null;
            var users = await UserContext.AsQueryable().InnerJoin<TUserRole>((u, l) => u.UserId == l.UserId)
                .Where<TUserRole>(x => x.RoleId == role.RoleId)
                .Where(x => x.SiteId == Site.SiteId)
                .AsEnumerableAsync(cancellationToken);
            return users.ToList();
        }

        /// <summary>
        /// 获取用户的所有角色。
        /// </summary>
        /// <param name="user">用户实例对象。</param>
        /// <param name="cancellationToken">取消标志。</param>
        /// <returns>返回当前用户的所有角色列表。</returns>
        public override async Task<IList<string>> GetRolesAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            var userRoles = await UserRoleContext.FetchAsync(x => x.UserId == user.UserId, cancellationToken);
            var roleIds = userRoles.Select(x => x.RoleId).ToList();
            var roles = await LoadRolesAsync();
            return roles.Where(x => roleIds.Contains(x.RoleId))
                .Select(x => x.Name)
                .ToList();
        }

        /// <summary>
        /// 缓存键。
        /// </summary>
        protected virtual object CacheKey => new Tuple<int, Type>(Site.SiteId, typeof(TRole));

        /// <summary>
        /// 获取所有角色。
        /// </summary>
        /// <returns>返回角色列表。</returns>
        protected override async Task<IEnumerable<TRole>> LoadRolesAsync()
        {
            return await Cache.GetOrCreateAsync(CacheKey, async ctx =>
            {
                ctx.SetDefaultAbsoluteExpiration();
                return await RoleContext.FetchAsync(x => x.SiteId == Site.SiteId);
            });
        }

        /// <summary>
        /// 初始化类<see cref="UserStoreEx{TUser, TRole, TUserClaim, TUserRole, TUserLogin, TUserToken, TRoleClaim}"/>。
        /// </summary>
        /// <param name="describer">错误描述<see cref="IdentityErrorDescriber"/>实例。</param>
        /// <param name="userContext">用户数据库接口。</param>
        /// <param name="userClaimContext">用户声明数据库接口。</param>
        /// <param name="userLoginContext">用户登陆数据库接口。</param>
        /// <param name="userTokenContext">用户标识数据库接口。</param>
        /// <param name="roleContext">角色数据库操作接口。</param>
        /// <param name="userRoleContext">用户角色数据库操作接口。</param>
        /// <param name="roleClaimContext">用户声明数据库操作接口。</param>
        /// <param name="siteContextAccessor">当前网站访问接口。</param>
        /// <param name="cache">缓存接口。</param>
        public UserStoreEx(IdentityErrorDescriber describer, IDbContext<TUser> userContext, IDbContext<TUserClaim> userClaimContext, IDbContext<TUserLogin> userLoginContext, IDbContext<TUserToken> userTokenContext, IDbContext<TRole> roleContext, IDbContext<TUserRole> userRoleContext, IDbContext<TRoleClaim> roleClaimContext, IMemoryCache cache, ISiteContextAccessorBase siteContextAccessor)
            : base(describer, userContext, userClaimContext, userLoginContext, userTokenContext, roleContext, userRoleContext, roleClaimContext, cache)
        {
            _siteContextAccessor = siteContextAccessor;
        }
    }
}