using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Mozlite.Data;

namespace Mozlite.Extensions.Security.Stores
{
    /// <summary>
    /// 用户存储类型。
    /// </summary>
    /// <typeparam name="TUser">用户类型。</typeparam>
    /// <typeparam name="TUserClaim">用户声明类型。</typeparam>
    /// <typeparam name="TUserLogin">用户登陆类型。</typeparam>
    /// <typeparam name="TUserToken">用户标识类型。</typeparam>
    public abstract class UserOnlyStore<TUser, TUserClaim, TUserLogin, TUserToken>
        : UserStoreBase<TUser, TUserClaim, TUserLogin, TUserToken>,
        IUserStoreBase<TUser, TUserClaim, TUserLogin, TUserToken>
        where TUser : UserBase
        where TUserClaim : UserClaimBase, new()
        where TUserLogin : UserLoginBase, new()
        where TUserToken : UserTokenBase, new()
    {
        /// <summary>
        /// 用户数据库操作接口。
        /// </summary>
        public IDbContext<TUser> UserContext { get; }
        /// <summary>
        /// 用户声明数据库操作接口。
        /// </summary>
        public IDbContext<TUserClaim> UserClaimContext { get; }
        /// <summary>
        /// 用户登陆数据库操作接口。
        /// </summary>
        public IDbContext<TUserLogin> UserLoginContext { get; }
        /// <summary>
        /// 用户标识数据库操作接口。
        /// </summary>
        public IDbContext<TUserToken> UserTokenContext { get; }

        /// <summary>
        /// 初始化类<see cref="UserOnlyStore{TUser,TRole, TUserLogin, TUserToken}"/>。
        /// </summary>
        /// <param name="describer">错误描述<see cref="IdentityErrorDescriber"/>实例。</param>
        /// <param name="userContext">用户数据库接口。</param>
        /// <param name="userClaimContext">用户声明数据库接口。</param>
        /// <param name="userLoginContext">用户登陆数据库接口。</param>
        /// <param name="userTokenContext">用户标识数据库接口。</param>
        protected UserOnlyStore(IdentityErrorDescriber describer,
            IDbContext<TUser> userContext,
            IDbContext<TUserClaim> userClaimContext,
            IDbContext<TUserLogin> userLoginContext,
            IDbContext<TUserToken> userTokenContext) : base(describer)
        {
            UserContext = userContext;
            UserClaimContext = userClaimContext;
            UserLoginContext = userLoginContext;
            UserTokenContext = userTokenContext;
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
            await UserContext.CreateAsync(user, cancellationToken);
            return IdentityResult.Success;
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
        public override Task<TUser> FindUserAsync(int userId, CancellationToken cancellationToken = default(CancellationToken))
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
    }
}