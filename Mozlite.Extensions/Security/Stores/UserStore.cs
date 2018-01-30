using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using Mozlite.Data;
using Mozlite.Data.Internal;

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
    public class UserStore<TUser, TRole, TUserClaim, TUserRole, TUserLogin, TUserToken, TRoleClaim>
        : UserStoreBase<TUser, TRole, TUserClaim, TUserRole, TUserLogin, TUserToken, TRoleClaim>
        where TUser : UserBase
        where TRole : RoleBase
        where TUserClaim : UserClaimBase, new()
        where TUserRole : UserRoleBase, new()
        where TUserLogin : UserLoginBase, new()
        where TUserToken : UserTokenBase, new()
        where TRoleClaim : RoleClaimBase, new()
    {
        private readonly IMemoryCache _cache;

        /// <summary>
        /// 用户数据库操作接口。
        /// </summary>
        protected IDbContext<TUser> UserContext { get; }
        /// <summary>
        /// 用户声明数据库操作接口。
        /// </summary>
        protected IDbContext<TUserClaim> UserClaimContext { get; }
        /// <summary>
        /// 用户登陆数据库操作接口。
        /// </summary>
        protected IDbContext<TUserLogin> UserLoginContext { get; }
        /// <summary>
        /// 用户标识数据库操作接口。
        /// </summary>
        protected IDbContext<TUserToken> UserTokenContext { get; }

        /// <summary>
        /// 角色数据库操作接口。
        /// </summary>
        protected IDbContext<TRole> RoleContext { get; }

        /// <summary>
        /// 用户角色数据库操作接口。
        /// </summary>
        protected IDbContext<TUserRole> UserRoleContext { get; }

        /// <summary>
        /// 用户声明数据库操作接口。
        /// </summary>
        protected IDbContext<TRoleClaim> RoleClaimContext { get; }

        /// <summary>
        /// 初始化类<see cref="UserStore{TUser, TRole, TUserClaim, TUserRole, TUserLogin, TUserToken, TRoleClaim}"/>。
        /// </summary>
        /// <param name="describer">错误描述<see cref="IdentityErrorDescriber"/>实例。</param>
        /// <param name="userContext">用户数据库接口。</param>
        /// <param name="userClaimContext">用户声明数据库接口。</param>
        /// <param name="userLoginContext">用户登陆数据库接口。</param>
        /// <param name="userTokenContext">用户标识数据库接口。</param>
        /// <param name="roleContext">角色数据库操作接口。</param>
        /// <param name="userRoleContext">用户角色数据库操作接口。</param>
        /// <param name="roleClaimContext">用户声明数据库操作接口。</param>
        /// <param name="cache">缓存接口。</param>
        public UserStore(IdentityErrorDescriber describer,
            IDbContext<TUser> userContext,
            IDbContext<TUserClaim> userClaimContext,
            IDbContext<TUserLogin> userLoginContext,
            IDbContext<TUserToken> userTokenContext,
            IDbContext<TRole> roleContext,
            IDbContext<TUserRole> userRoleContext,
            IDbContext<TRoleClaim> roleClaimContext,
            IMemoryCache cache) : base(describer)
        {
            _cache = cache;
            UserContext = userContext;
            UserClaimContext = userClaimContext;
            UserLoginContext = userLoginContext;
            UserTokenContext = userTokenContext;
            RoleContext = roleContext;
            UserRoleContext = userRoleContext;
            RoleClaimContext = roleClaimContext;
        }

        /// <summary>
        /// 新建用户实例。
        /// </summary>
        /// <param name="user">用户实例对象。</param>
        /// <param name="cancellationToken">取消标志。</param>
        /// <returns>返回添加用户结果。</returns>
        public override async Task<IdentityResult> CreateAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (user is IUserExtendable<TUser> extendable)
            {
                await UserContext.BeginTransactionAsync(async db =>
                {
                    if (!await db.CreateAsync(user, cancellationToken))
                        return false;
                    if (!await extendable.OnCreateAsync(db))
                        return false;
                    if (!await OnCreateAsync(db))
                        return false;
                    return true;
                }, cancellationToken: cancellationToken);
            }
            else
                await UserContext.CreateAsync(user, cancellationToken);
            return IdentityResult.Success;
        }

        /// <summary>
        /// 当用户添加后触发得方法。
        /// </summary>
        /// <param name="context">数据库事务操作实例。</param>
        /// <returns>返回操作结果，返回<c>true</c>表示操作成功，将自动提交事务，如果<c>false</c>或发生错误，则回滚事务。</returns>
        protected virtual Task<bool> OnCreateAsync(IDbTransactionContext<TUser> context)
        {
            return Task.FromResult(true);
        }

        /// <summary>
        /// 更新用户实例。
        /// </summary>
        /// <param name="user">当前用户实例。</param>
        /// <param name="cancellationToken">取消标志。</param>
        /// <returns>返回更新结果。</returns>
        public override async Task<IdentityResult> UpdateAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            await UserContext.UpdateAsync(user, cancellationToken);
            return IdentityResult.Success;
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
            await UserContext.DeleteAsync(user.UserId, cancellationToken);
            return IdentityResult.Success;
        }

        /// <summary>
        /// 通过用户ID查询用户实例。
        /// </summary>
        /// <param name="userId">当前用户ID。</param>
        /// <param name="cancellationToken">取消标志。</param>
        /// <returns>
        /// 返回当前用户实例对象。
        /// </returns>
        public override async Task<TUser> FindByIdAsync(string userId, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (int.TryParse(userId, out var id))
                return await FindUserAsync(id, cancellationToken);
            return null;
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
            return UserContext.FindAsync(x => x.NormalizedUserName == normalizedUserName, cancellationToken);
        }

        /// <summary>
        /// 通过Id获取用户实例。
        /// </summary>
        /// <param name="userId">用户Id。</param>
        /// <param name="cancellationToken">取消标志。</param>
        /// <returns>返回当前用户实例。</returns>
        protected override Task<TUser> FindUserAsync(int userId, CancellationToken cancellationToken)
        {
            return UserContext.FindAsync(userId, cancellationToken);
        }

        /// <summary>
        /// 获取用户登陆信息。
        /// </summary>
        /// <param name="userId">用户Id。</param>
        /// <param name="loginProvider">登陆提供者名称。</param>
        /// <param name="providerKey">登陆唯一键。</param>
        /// <param name="cancellationToken">取消标志。</param>
        /// <returns>返回当前用户登陆信息。</returns>
        protected override Task<TUserLogin> FindUserLoginAsync(int userId, string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            return UserLoginContext.FindAsync(
                x => x.UserId == userId && x.LoginProvider == loginProvider && x.ProviderKey == providerKey,
                cancellationToken);
        }

        /// <summary>
        /// 获取用户登陆信息。
        /// </summary>
        /// <param name="loginProvider">登陆提供者名称。</param>
        /// <param name="providerKey">登陆唯一键。</param>
        /// <param name="cancellationToken">取消标志。</param>
        /// <returns>返回当前用户登陆信息。</returns>
        protected override Task<TUserLogin> FindUserLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            return UserLoginContext.FindAsync(
                x => x.LoginProvider == loginProvider && x.ProviderKey == providerKey,
                cancellationToken);
        }

        /// <summary>
        /// 获取当前用户实例的所有声明列表。
        /// </summary>
        /// <param name="user">用户实例对象。</param>
        /// <param name="cancellationToken">取消标志。</param>
        /// <returns>返回当前用户实例的所有声明列表。</returns>
        public override async Task<IList<Claim>> GetClaimsAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            var claims = await UserClaimContext.FetchAsync(x => x.UserId == user.UserId, cancellationToken);
            return claims.Select(x => x.ToClaim()).ToList();
        }

        /// <summary>
        /// 添加用户声明。
        /// </summary>
        /// <param name="user">当前用户实例对象。</param>
        /// <param name="claims">声明列表。</param>
        /// <param name="cancellationToken">取消标志。</param>
        public override async Task AddClaimsAsync(TUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (claims == null)
            {
                throw new ArgumentNullException(nameof(claims));
            }
            await UserClaimContext.BeginTransactionAsync(async db =>
            {
                foreach (var claim in claims)
                {
                    var dbClaim = CreateUserClaim(user, claim);
                    await db.CreateAsync(dbClaim, cancellationToken);
                }
                return true;
            }, cancellationToken: cancellationToken);
        }

        /// <summary>
        /// 替换用户声明。
        /// </summary>
        /// <param name="user">当前用户实例对象。</param>
        /// <param name="claim">声明实例对象。</param>
        /// <param name="newClaim">新的声明实例对象。</param>
        /// <param name="cancellationToken">取消标志。</param>
        public override async Task ReplaceClaimAsync(TUser user, Claim claim, Claim newClaim,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (claim == null)
            {
                throw new ArgumentNullException(nameof(claim));
            }
            if (newClaim == null)
            {
                throw new ArgumentNullException(nameof(newClaim));
            }
            await UserClaimContext.UpdateAsync(
                uc => uc.UserId == user.UserId && uc.ClaimValue == claim.Value && uc.ClaimType == claim.Type,
                new { ClaimType = newClaim.Type, ClaimValue = newClaim.Value }, cancellationToken);
        }

        /// <summary>
        /// 移除用户声明。
        /// </summary>
        /// <param name="user">用户实例。</param>
        /// <param name="claims">声明列表。</param>
        /// <param name="cancellationToken">取消标志。</param>
        public override async Task RemoveClaimsAsync(TUser user, IEnumerable<Claim> claims,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (claims == null)
            {
                throw new ArgumentNullException(nameof(claims));
            }
            await UserClaimContext.BeginTransactionAsync(async db =>
            {
                foreach (var claim in claims)
                {
                    await db.DeleteAsync(
                        uc => uc.UserId == user.UserId && uc.ClaimType == claim.Type && uc.ClaimValue == claim.Value, cancellationToken);
                }
                return true;
            }, cancellationToken: cancellationToken);
        }

        /// <summary>
        /// 获取当前声明的所有用户。
        /// </summary>
        /// <param name="claim">声明实例。</param>
        /// <param name="cancellationToken">取消标志。</param>
        /// <returns>
        /// 返回用户列表。 
        /// </returns>
        public override async Task<IList<TUser>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (claim == null)
            {
                throw new ArgumentNullException(nameof(claim));
            }

            var users = await UserContext.AsQueryable().InnerJoin<TUserClaim>((u, c) => u.UserId == c.UserId)
                .Where<TUserClaim>(x => x.ClaimType == claim.Type && x.ClaimValue == claim.Value)
                .AsEnumerableAsync(cancellationToken);

            return users.ToList();
        }

        /// <summary>
        /// 获取用户标识。
        /// </summary>
        /// <param name="user">当前用户实例。</param>
        /// <param name="loginProvider">登陆提供者。</param>
        /// <param name="name">名称。</param>
        /// <param name="cancellationToken">取消标志。</param>
        /// <returns>返回用户标识实例。</returns>
        protected override Task<TUserToken> FindTokenAsync(TUser user, string loginProvider, string name, CancellationToken cancellationToken)
        {
            return UserTokenContext.FindAsync(
                x => x.UserId == user.UserId && x.LoginProvider == loginProvider && x.Name == name, cancellationToken);
        }

        /// <summary>
        /// 添加新的用户标识。
        /// </summary>
        /// <param name="token">用户标识实例对象。</param>
        protected override async Task AddUserTokenAsync(TUserToken token)
        {
            await UserTokenContext.CreateAsync(token);
        }

        /// <summary>
        /// 移除用户标识。
        /// </summary>
        /// <param name="token">用户标识实例对象。</param>
        protected override async Task RemoveUserTokenAsync(TUserToken token)
        {
            await UserTokenContext.DeleteAsync(x =>
                x.UserId == token.UserId && x.LoginProvider == token.LoginProvider && x.Name == token.Name);
        }

        /// <summary>
        /// 添加用户登陆信息。
        /// </summary>
        /// <param name="user">当前用户实例。</param>
        /// <param name="login">用户登陆信息实例。</param>
        /// <param name="cancellationToken">取消标志。</param>
        public override async Task AddLoginAsync(TUser user, UserLoginInfo login, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (login == null)
            {
                throw new ArgumentNullException(nameof(login));
            }
            await UserLoginContext.CreateAsync(CreateUserLogin(user, login), cancellationToken);
        }

        /// <summary>
        /// 移除用户登陆信息。
        /// </summary>
        /// <param name="user">当前用户实例。</param>
        /// <param name="loginProvider">登陆提供者名称。</param>
        /// <param name="providerKey">登陆唯一键。</param>
        /// <param name="cancellationToken">取消标志。</param>
        public override async Task RemoveLoginAsync(TUser user, string loginProvider, string providerKey,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            await UserLoginContext.DeleteAsync(
                x => x.UserId == user.UserId && x.LoginProvider == loginProvider && x.ProviderKey == providerKey,
                cancellationToken);
        }

        /// <summary>
        /// 获取当前用户的登陆信息列表。
        /// </summary>
        /// <param name="user">当前用户实例。</param>
        /// <param name="cancellationToken">取消标志。</param>
        /// <returns>
        /// 返回当前用户所有登陆信息。
        /// </returns>
        public override async Task<IList<UserLoginInfo>> GetLoginsAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            var loginInfos = await UserLoginContext.FetchAsync(x => x.UserId == user.UserId, cancellationToken);
            return loginInfos.Select(x => new UserLoginInfo(x.LoginProvider, x.ProviderKey, x.ProviderDisplayName)).ToList();
        }

        /// <summary>
        /// 通过电子邮件获取用户实例。
        /// </summary>
        /// <param name="normalizedEmail">验证邮件地址。</param>
        /// <param name="cancellationToken">取消标志。</param>
        /// <returns>返回当前用户实例。</returns>
        public override Task<TUser> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken = default(CancellationToken))
        {
            return UserContext.FindAsync(x => x.NormalizedEmail == normalizedEmail, cancellationToken);
        }

        /// <summary>
        /// 判断用户是否包含当前角色。
        /// </summary>
        /// <param name="user">用户实例。</param>
        /// <param name="normalizedRoleName">验证角色名称。</param>
        /// <param name="cancellationToken">取消标志。</param>
        /// <returns>返回判断结果。</returns>
        public override async Task<bool> IsInRoleAsync(TUser user, string normalizedRoleName,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var role = await FindRoleAsync(normalizedRoleName, cancellationToken);
            if (role == null) return false;
            return await UserRoleContext.AnyAsync(x => x.UserId == user.UserId && x.RoleId == role.RoleId, cancellationToken);
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
            return roles.SingleOrDefault(x =>
                x.NormalizedName.Equals(normalizedRoleName, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// 获取用户角色。
        /// </summary>
        /// <param name="userId">用户ID。</param>
        /// <param name="roleId">角色ID。</param>
        /// <param name="cancellationToken">取消标志。</param>
        /// <returns>用户角色实例对象。</returns>
        protected override Task<TUserRole> FindUserRoleAsync(int userId, int roleId, CancellationToken cancellationToken)
        {
            return UserRoleContext.FindAsync(x => x.UserId == userId && x.RoleId == roleId, cancellationToken);
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
                .AsEnumerableAsync(cancellationToken);
            return users.ToList();
        }

        /// <summary>
        /// 添加用户角色。
        /// </summary>
        /// <param name="user">当前用户实例。</param>
        /// <param name="normalizedRoleName">验证角色名称。</param>
        /// <param name="cancellationToken">取消标志。</param>
        public override async Task AddToRoleAsync(TUser user, string normalizedRoleName,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var role = await FindRoleAsync(normalizedRoleName, cancellationToken);
            if (role == null || await IsInRoleAsync(user, normalizedRoleName, cancellationToken))
                return;
            await UserRoleContext.CreateAsync(CreateUserRole(user, role), cancellationToken);
        }

        /// <summary>
        /// 移除用户角色。
        /// </summary>
        /// <param name="user">用户实例对象。</param>
        /// <param name="normalizedRoleName">验证角色名称。</param>
        /// <param name="cancellationToken">取消标志。</param>
        public override async Task RemoveFromRoleAsync(TUser user, string normalizedRoleName,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var role = await FindRoleAsync(normalizedRoleName, cancellationToken);
            if (role != null)
                await UserRoleContext.DeleteAsync(x => x.UserId == user.UserId && x.RoleId == role.RoleId, cancellationToken);
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
        /// 获取所有角色。
        /// </summary>
        /// <returns>返回角色列表。</returns>
        protected virtual async Task<IEnumerable<TRole>> LoadRolesAsync()
        {
            return await _cache.GetOrCreateAsync(typeof(TRole), async ctx =>
            {
                ctx.SetDefaultAbsoluteExpiration();
                return await RoleContext.FetchAsync();
            });
        }
    }
}