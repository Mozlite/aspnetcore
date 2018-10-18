using System;
using System.Linq;
using System.Threading;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Mozlite.Extensions.Security.Stores
{
    /// <summary>
    /// 用户存储类型。
    /// </summary>
    /// <typeparam name="TUser">用户类型。</typeparam>
    /// <typeparam name="TUserClaim">用户声明类型。</typeparam>
    /// <typeparam name="TUserLogin">用户登陆类型。</typeparam>
    /// <typeparam name="TUserToken">用户标识类型。</typeparam>
    public abstract class IdentityUserStoreBase<TUser, TUserClaim, TUserLogin, TUserToken> :
       IUserLoginStore<TUser>,
       IUserClaimStore<TUser>,
       IUserPasswordStore<TUser>,
       IUserSecurityStampStore<TUser>,
       IUserEmailStore<TUser>,
       IUserLockoutStore<TUser>,
       IUserPhoneNumberStore<TUser>,
       IUserTwoFactorStore<TUser>,
       IUserAuthenticationTokenStore<TUser>,
       IUserAuthenticatorKeyStore<TUser>,
       IUserTwoFactorRecoveryCodeStore<TUser>
       where TUser : UserBase
       where TUserClaim : UserClaimBase, new()
       where TUserLogin : UserLoginBase, new()
       where TUserToken : UserTokenBase, new()
    {
        /// <summary>
        /// 初始化类<see cref="IdentityUserStoreBase{TUser,TRole, TUserLogin, TUserToken}"/>。
        /// </summary>
        /// <param name="describer">错误描述<see cref="IdentityErrorDescriber"/>实例。</param>
        protected IdentityUserStoreBase(IdentityErrorDescriber describer)
        {
            ErrorDescriber = describer ?? throw new ArgumentNullException(nameof(describer));
        }

        /// <summary>
        /// 错误描述实例对象。
        /// </summary>
        protected IdentityErrorDescriber ErrorDescriber { get; }

        /// <summary>
        /// 实例化当前用户的声明实例。
        /// </summary>
        /// <param name="user">当前用户实例。</param>
        /// <param name="claim">声明实例。</param>
        /// <returns>返回用户声明实例对象。</returns>
        protected virtual TUserClaim CreateUserClaim(TUser user, Claim claim)
        {
            var userClaim = new TUserClaim { UserId = user.UserId };
            userClaim.InitializeFromClaim(claim);
            return userClaim;
        }

        /// <summary>
        /// 实例化当前用户的登陆信息实例。
        /// </summary>
        /// <param name="user">当前用户实例。</param>
        /// <param name="login">登陆信息实例。</param>
        /// <returns>返回用户登陆信息实例。</returns>
        protected virtual TUserLogin CreateUserLogin(TUser user, UserLoginInfo login)
        {
            return new TUserLogin
            {
                UserId = user.UserId,
                ProviderKey = login.ProviderKey,
                LoginProvider = login.LoginProvider,
                ProviderDisplayName = login.ProviderDisplayName
            };
        }

        /// <summary>
        /// 通过用户实例以及提供者实例化一个用户标识。
        /// </summary>
        /// <param name="user">用户实例对象。</param>
        /// <param name="loginProvider">登陆提供者。</param>
        /// <param name="name">用户标识名称。</param>
        /// <param name="value">用户标识值。</param>
        /// <returns>返回用户标识。</returns>
        protected virtual TUserToken CreateUserToken(TUser user, string loginProvider, string name, string value)
        {
            return new TUserToken
            {
                UserId = user.UserId,
                LoginProvider = loginProvider,
                Name = name,
                Value = value
            };
        }

        /// <summary>
        /// 从用户实例中获取用户Id。
        /// </summary>
        /// <param name="user">用户实例对象。</param>
        /// <param name="cancellationToken">取消标志。</param>
        /// <returns>返回用户ID。</returns>
        public virtual Task<string> GetUserIdAsync(TUser user, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            return Task.FromResult(user.UserId.ToString());
        }

        /// <summary>
        /// 从用户实例中获取用户名称。
        /// </summary>
        /// <param name="user">用户实例对象。</param>
        /// <param name="cancellationToken">取消标志。</param>
        /// <returns>返回用户名称。</returns>
        public virtual Task<string> GetUserNameAsync(TUser user, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            return Task.FromResult(user.UserName);
        }

        /// <summary>
        /// 设置用户名称。
        /// </summary>
        /// <param name="user">用户实例对象。</param>
        /// <param name="userName">用户名称。</param>
        /// <param name="cancellationToken">取消标志。</param>
        public virtual Task SetUserNameAsync(TUser user, string userName, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            user.UserName = userName;
            return Task.CompletedTask;
        }


        /// <summary>
        /// 从用户实例中获取用户验证名称。
        /// </summary>
        /// <param name="user">用户实例对象。</param>
        /// <param name="cancellationToken">取消标志。</param>
        /// <returns>返回用户验证名称。</returns>
        public virtual Task<string> GetNormalizedUserNameAsync(TUser user, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            return Task.FromResult(user.NormalizedUserName);
        }

        /// <summary>
        /// 设置用户验证名称。
        /// </summary>
        /// <param name="user">用户实例对象。</param>
        /// <param name="normalizedName">用户验证名称。</param>
        /// <param name="cancellationToken">取消标志。</param>
        public virtual Task SetNormalizedUserNameAsync(TUser user, string normalizedName, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            user.NormalizedUserName = user.NormalizedUserName ?? normalizedName;//使用此列为登录名
            return Task.CompletedTask;
        }

        /// <summary>
        /// 新建用户实例。
        /// </summary>
        /// <param name="user">用户实例对象。</param>
        /// <param name="cancellationToken">取消标志。</param>
        /// <returns>返回添加用户结果。</returns>
        public abstract Task<IdentityResult> CreateAsync(TUser user, CancellationToken cancellationToken = default);

        /// <summary>
        /// 更新用户实例。
        /// </summary>
        /// <param name="user">当前用户实例。</param>
        /// <param name="cancellationToken">取消标志。</param>
        /// <returns>返回更新结果。</returns>
        public abstract Task<IdentityResult> UpdateAsync(TUser user, CancellationToken cancellationToken = default);

        /// <summary>
        /// 删除用户实例。
        /// </summary>
        /// <param name="user">当前用户实例。</param>
        /// <param name="cancellationToken">取消标志。</param>
        /// <returns>返回删除结果。</returns>
        public abstract Task<IdentityResult> DeleteAsync(TUser user, CancellationToken cancellationToken = default);

        /// <summary>
        /// 通过用户ID查询用户实例。
        /// </summary>
        /// <param name="userId">当前用户ID。</param>
        /// <param name="cancellationToken">取消标志。</param>
        /// <returns>
        /// 返回当前用户实例对象。
        /// </returns>
        public abstract Task<TUser> FindByIdAsync(string userId, CancellationToken cancellationToken = default);

        /// <summary>
        /// 通过用户验证名称查询用户实例。
        /// </summary>
        /// <param name="normalizedUserName">当前验证名称。</param>
        /// <param name="cancellationToken">取消标志。</param>
        /// <returns>
        /// 返回当前用户实例对象。
        /// </returns>
        public abstract Task<TUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken = default);

        /// <summary>
        /// 设置当前用户实例的哈希密码。
        /// </summary>
        /// <param name="user">用户实例对象。</param>
        /// <param name="passwordHash">哈希密码。</param>
        /// <param name="cancellationToken">取消标志。</param>
        public virtual Task SetPasswordHashAsync(TUser user, string passwordHash, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            user.PasswordHash = passwordHash;
            return Task.CompletedTask;
        }

        /// <summary>
        /// 获取当前用户实体的哈希密码。
        /// </summary>
        /// <param name="user">当前用户实例对象。</param>
        /// <param name="cancellationToken">取消标志。</param>
        /// <returns>返回当前用户的哈希密码。</returns>
        public virtual Task<string> GetPasswordHashAsync(TUser user, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            return Task.FromResult(user.PasswordHash);
        }

        /// <summary>
        /// 判断是否有密码。
        /// </summary>
        /// <param name="user">当前用户实例对象。</param>
        /// <param name="cancellationToken">取消标志。</param>
        /// <returns>返回判断结果。</returns>
        public virtual Task<bool> HasPasswordAsync(TUser user, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(user.PasswordHash != null);
        }

        /// <summary>
        /// 通过Id获取用户实例。
        /// </summary>
        /// <param name="userId">用户Id。</param>
        /// <param name="cancellationToken">取消标志。</param>
        /// <returns>返回当前用户实例。</returns>
        public abstract Task<TUser> FindUserAsync(int userId, CancellationToken cancellationToken = default);

        /// <summary>
        /// 获取用户登陆信息。
        /// </summary>
        /// <param name="userId">用户Id。</param>
        /// <param name="loginProvider">登陆提供者名称。</param>
        /// <param name="providerKey">登陆唯一键。</param>
        /// <param name="cancellationToken">取消标志。</param>
        /// <returns>返回当前用户登陆信息。</returns>
        protected abstract Task<TUserLogin> FindUserLoginAsync(int userId, string loginProvider, string providerKey, CancellationToken cancellationToken);

        /// <summary>
        /// 获取用户登陆信息。
        /// </summary>
        /// <param name="loginProvider">登陆提供者名称。</param>
        /// <param name="providerKey">登陆唯一键。</param>
        /// <param name="cancellationToken">取消标志。</param>
        /// <returns>返回当前用户登陆信息。</returns>
        protected abstract Task<TUserLogin> FindUserLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken);

        /// <summary>
        /// 释放资源。
        /// </summary>
        public virtual void Dispose() { }

        /// <summary>
        /// 获取当前用户实例的所有声明列表。
        /// </summary>
        /// <param name="user">用户实例对象。</param>
        /// <param name="cancellationToken">取消标志。</param>
        /// <returns>返回当前用户实例的所有声明列表。</returns>
        public abstract Task<IList<Claim>> GetClaimsAsync(TUser user, CancellationToken cancellationToken = default);

        /// <summary>
        /// 添加用户声明。
        /// </summary>
        /// <param name="user">当前用户实例对象。</param>
        /// <param name="claims">声明列表。</param>
        /// <param name="cancellationToken">取消标志。</param>
        public abstract Task AddClaimsAsync(TUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken = default);

        /// <summary>
        /// 替换用户声明。
        /// </summary>
        /// <param name="user">当前用户实例对象。</param>
        /// <param name="claim">声明实例对象。</param>
        /// <param name="newClaim">新的声明实例对象。</param>
        /// <param name="cancellationToken">取消标志。</param>
        public abstract Task ReplaceClaimAsync(TUser user, Claim claim, Claim newClaim, CancellationToken cancellationToken = default);

        /// <summary>
        /// 移除用户声明。
        /// </summary>
        /// <param name="user">用户实例。</param>
        /// <param name="claims">声明列表。</param>
        /// <param name="cancellationToken">取消标志。</param>
        public abstract Task RemoveClaimsAsync(TUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken = default);

        /// <summary>
        /// 添加用户登陆信息。
        /// </summary>
        /// <param name="user">当前用户实例。</param>
        /// <param name="login">用户登陆信息实例。</param>
        /// <param name="cancellationToken">取消标志。</param>
        public abstract Task AddLoginAsync(TUser user, UserLoginInfo login, CancellationToken cancellationToken = default);

        /// <summary>
        /// 移除用户登陆信息。
        /// </summary>
        /// <param name="user">当前用户实例。</param>
        /// <param name="loginProvider">登陆提供者名称。</param>
        /// <param name="providerKey">登陆唯一键。</param>
        /// <param name="cancellationToken">取消标志。</param>
        public abstract Task RemoveLoginAsync(TUser user, string loginProvider, string providerKey, CancellationToken cancellationToken = default);

        /// <summary>
        /// 获取当前用户的登陆信息列表。
        /// </summary>
        /// <param name="user">当前用户实例。</param>
        /// <param name="cancellationToken">取消标志。</param>
        /// <returns>
        /// 返回当前用户所有登陆信息。
        /// </returns>
        public abstract Task<IList<UserLoginInfo>> GetLoginsAsync(TUser user, CancellationToken cancellationToken = default);

        /// <summary>
        /// 通过登陆提供者和唯一键获取用户实例。
        /// </summary>
        /// <param name="loginProvider">登陆提供者名称。</param>
        /// <param name="providerKey">登陆唯一键。</param>
        /// <param name="cancellationToken">取消标志。</param>
        /// <returns>
        /// 返回用户实例对象。
        /// </returns>
        public virtual async Task<TUser> FindByLoginAsync(string loginProvider, string providerKey,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var userLogin = await FindUserLoginAsync(loginProvider, providerKey, cancellationToken);
            if (userLogin != null)
            {
                return await FindUserAsync(userLogin.UserId, cancellationToken);
            }
            return null;
        }

        /// <summary>
        /// 获取当前用户实体的邮件确认状态。
        /// </summary>
        /// <param name="user">当前用户实例对象。</param>
        /// <param name="cancellationToken">取消标志。</param>
        /// <returns>返回当前用户的邮件确认状态。</returns>
        public virtual Task<bool> GetEmailConfirmedAsync(TUser user, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            return Task.FromResult(user.EmailConfirmed);
        }

        /// <summary>
        /// 设置当前用户实例的邮件确认状态。
        /// </summary>
        /// <param name="user">用户实例对象。</param>
        /// <param name="confirmed">邮件确认状态。</param>
        /// <param name="cancellationToken">取消标志。</param>
        public virtual Task SetEmailConfirmedAsync(TUser user, bool confirmed, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            user.EmailConfirmed = confirmed;
            return Task.CompletedTask;
        }

        /// <summary>
        /// 设置当前用户实例的邮件地址。
        /// </summary>
        /// <param name="user">用户实例对象。</param>
        /// <param name="email">邮件地址。</param>
        /// <param name="cancellationToken">取消标志。</param>
        public virtual Task SetEmailAsync(TUser user, string email, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            user.Email = email;
            return Task.CompletedTask;
        }

        /// <summary>
        /// 获取当前用户实体的邮件地址。
        /// </summary>
        /// <param name="user">当前用户实例对象。</param>
        /// <param name="cancellationToken">取消标志。</param>
        /// <returns>返回当前用户的邮件地址。</returns>
        public virtual Task<string> GetEmailAsync(TUser user, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            return Task.FromResult(user.Email);
        }

        /// <summary>
        /// 获取当前用户实体的验证邮件地址。
        /// </summary>
        /// <param name="user">当前用户实例对象。</param>
        /// <param name="cancellationToken">取消标志。</param>
        /// <returns>返回当前用户的验证邮件地址。</returns>
        public virtual Task<string> GetNormalizedEmailAsync(TUser user, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            return Task.FromResult(user.NormalizedEmail);
        }

        /// <summary>
        /// 设置当前用户实例的验证邮件地址。
        /// </summary>
        /// <param name="user">用户实例对象。</param>
        /// <param name="normalizedEmail">验证邮件地址。</param>
        /// <param name="cancellationToken">取消标志。</param>
        public virtual Task SetNormalizedEmailAsync(TUser user, string normalizedEmail, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            user.NormalizedEmail = normalizedEmail;
            return Task.CompletedTask;
        }

        /// <summary>
        /// 通过电子邮件获取用户实例。
        /// </summary>
        /// <param name="normalizedEmail">验证邮件地址。</param>
        /// <param name="cancellationToken">取消标志。</param>
        /// <returns>返回当前用户实例。</returns>
        public abstract Task<TUser> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken = default);

        /// <summary>
        /// 获取当前用户实体的锁定截至日期。
        /// </summary>
        /// <param name="user">当前用户实例对象。</param>
        /// <param name="cancellationToken">取消标志。</param>
        /// <returns>返回当前用户的锁定截至日期。</returns>
        public virtual Task<DateTimeOffset?> GetLockoutEndDateAsync(TUser user, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            return Task.FromResult(user.LockoutEnd);
        }

        /// <summary>
        /// 设置当前用户实例的锁定截至日期。
        /// </summary>
        /// <param name="user">用户实例对象。</param>
        /// <param name="lockoutEnd">锁定截至日期。</param>
        /// <param name="cancellationToken">取消标志。</param>
        public virtual Task SetLockoutEndDateAsync(TUser user, DateTimeOffset? lockoutEnd, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            user.LockoutEnd = lockoutEnd;
            return Task.CompletedTask;
        }

        /// <summary>
        /// 增加用户登陆失败次数。
        /// </summary>
        /// <param name="user">用户实例对象。</param>
        /// <param name="cancellationToken">取消标志。</param>
        /// <returns>返回用户登陆失败次数。</returns>
        public virtual Task<int> IncrementAccessFailedCountAsync(TUser user, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            user.AccessFailedCount++;
            return Task.FromResult(user.AccessFailedCount);
        }

        /// <summary>
        /// 重置用户登陆失败次数。
        /// </summary>
        /// <param name="user">用户实例对象。</param>
        /// <param name="cancellationToken">取消标志。</param>
        public virtual Task ResetAccessFailedCountAsync(TUser user, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            user.AccessFailedCount = 0;
            return Task.CompletedTask;
        }

        /// <summary>
        /// 获取用户登陆失败次数。
        /// </summary>
        /// <param name="user">用户实例对象。</param>
        /// <param name="cancellationToken">取消标志。</param>
        /// <returns>返回用户登陆失败次数。</returns>
        public virtual Task<int> GetAccessFailedCountAsync(TUser user, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            return Task.FromResult(user.AccessFailedCount);
        }

        /// <summary>
        /// 获取用户登陆失败达到限定次数后是否锁定。
        /// </summary>
        /// <param name="user">用户实例对象。</param>
        /// <param name="cancellationToken">取消标志。</param>
        /// <returns>返回判断结果。</returns>
        public virtual Task<bool> GetLockoutEnabledAsync(TUser user, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            return Task.FromResult(user.LockoutEnabled);
        }

        /// <summary>
        /// 设置当前用户实例的锁定启用状态。
        /// </summary>
        /// <param name="user">用户实例对象。</param>
        /// <param name="enabled">锁定启用状态。</param>
        /// <param name="cancellationToken">取消标志。</param>
        public virtual Task SetLockoutEnabledAsync(TUser user, bool enabled, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            user.LockoutEnabled = enabled;
            return Task.CompletedTask;
        }

        /// <summary>
        /// 设置当前用户实例的电话号码。
        /// </summary>
        /// <param name="user">用户实例对象。</param>
        /// <param name="phoneNumber">电话号码。</param>
        /// <param name="cancellationToken">取消标志。</param>
        public virtual Task SetPhoneNumberAsync(TUser user, string phoneNumber, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            user.PhoneNumber = phoneNumber;
            return Task.CompletedTask;
        }

        /// <summary>
        /// 获取用户电话号码。
        /// </summary>
        /// <param name="user">用户实例对象。</param>
        /// <param name="cancellationToken">取消标志。</param>
        /// <returns>返回电话号码。</returns>
        public virtual Task<string> GetPhoneNumberAsync(TUser user, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            return Task.FromResult(user.PhoneNumber);
        }

        /// <summary>
        /// 获取当前用户实体的电话号码确认状态。
        /// </summary>
        /// <param name="user">当前用户实例对象。</param>
        /// <param name="cancellationToken">取消标志。</param>
        /// <returns>返回当前用户的电话号码确认状态。</returns>
        public virtual Task<bool> GetPhoneNumberConfirmedAsync(TUser user, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            return Task.FromResult(user.PhoneNumberConfirmed);
        }

        /// <summary>
        /// 设置当前用户实例的电话号码确认状态。
        /// </summary>
        /// <param name="user">用户实例对象。</param>
        /// <param name="confirmed">电话号码确认状态。</param>
        /// <param name="cancellationToken">取消标志。</param>
        public virtual Task SetPhoneNumberConfirmedAsync(TUser user, bool confirmed, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            user.PhoneNumberConfirmed = confirmed;
            return Task.CompletedTask;
        }

        /// <summary>
        /// 设置当前用户实例的安全戳。
        /// </summary>
        /// <param name="user">用户实例对象。</param>
        /// <param name="stamp">安全戳。</param>
        /// <param name="cancellationToken">取消标志。</param>
        public virtual Task SetSecurityStampAsync(TUser user, string stamp, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            user.SecurityStamp = stamp ?? throw new ArgumentNullException(nameof(stamp));
            return Task.CompletedTask;
        }

        /// <summary>
        /// 获取当前用户实体的安全戳。
        /// </summary>
        /// <param name="user">当前用户实例对象。</param>
        /// <param name="cancellationToken">取消标志。</param>
        /// <returns>返回当前用户的安全戳。</returns>
        public virtual Task<string> GetSecurityStampAsync(TUser user, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            return Task.FromResult(user.SecurityStamp);
        }

        /// <summary>
        /// 设置当前用户实例的电话/电子邮件验证状态。
        /// </summary>
        /// <param name="user">用户实例对象。</param>
        /// <param name="enabled">电话/电子邮件验证状态。</param>
        /// <param name="cancellationToken">取消标志。</param>
        public virtual Task SetTwoFactorEnabledAsync(TUser user, bool enabled, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            user.TwoFactorEnabled = enabled;
            return Task.CompletedTask;
        }

        /// <summary>
        /// 获取当前用户实体的电话/电子邮件验证状态。
        /// </summary>
        /// <param name="user">当前用户实例对象。</param>
        /// <param name="cancellationToken">取消标志。</param>
        /// <returns>返回当前用户的电话/电子邮件验证状态。</returns>
        public virtual Task<bool> GetTwoFactorEnabledAsync(TUser user, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            return Task.FromResult(user.TwoFactorEnabled);
        }

        /// <summary>
        /// 获取当前声明的所有用户。
        /// </summary>
        /// <param name="claim">声明实例。</param>
        /// <param name="cancellationToken">取消标志。</param>
        /// <returns>
        /// 返回用户列表。 
        /// </returns>
        public abstract Task<IList<TUser>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken = default);

        /// <summary>
        /// 获取用户标识。
        /// </summary>
        /// <param name="user">当前用户实例。</param>
        /// <param name="loginProvider">登陆提供者。</param>
        /// <param name="name">名称。</param>
        /// <param name="cancellationToken">取消标志。</param>
        /// <returns>返回用户标识实例。</returns>
        protected abstract Task<TUserToken> FindTokenAsync(TUser user, string loginProvider, string name, CancellationToken cancellationToken);

        /// <summary>
        /// 添加新的用户标识。
        /// </summary>
        /// <param name="token">用户标识实例对象。</param>
        protected abstract Task AddUserTokenAsync(TUserToken token);

        /// <summary>
        /// 移除用户标识。
        /// </summary>
        /// <param name="token">用户标识实例对象。</param>
        protected abstract Task RemoveUserTokenAsync(TUserToken token);

        /// <summary>
        /// 设置当前用户的登陆标识。
        /// </summary>
        /// <param name="user">用户实例。</param>
        /// <param name="loginProvider">登陆提供者。</param>
        /// <param name="name">名称。</param>
        /// <param name="value">标识的值。</param>
        /// <param name="cancellationToken">取消标志。</param>
        public virtual async Task SetTokenAsync(TUser user, string loginProvider, string name, string value, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var token = await FindTokenAsync(user, loginProvider, name, cancellationToken);
            if (token == null)
            {
                await AddUserTokenAsync(CreateUserToken(user, loginProvider, name, value));
            }
            else
            {
                token.Value = value;
            }
        }

        /// <summary>
        /// 移除用户标识。
        /// </summary>
        /// <param name="user">用户实例对象。</param>
        /// <param name="loginProvider">登陆提供者。</param>
        /// <param name="name">名称。</param>
        /// <param name="cancellationToken">取消标志。</param>
        public virtual async Task RemoveTokenAsync(TUser user, string loginProvider, string name, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            var entry = await FindTokenAsync(user, loginProvider, name, cancellationToken);
            if (entry != null)
            {
                await RemoveUserTokenAsync(entry);
            }
        }

        /// <summary>
        /// 获取用户标识。
        /// </summary>
        /// <param name="user">用户实例对象。</param>
        /// <param name="loginProvider">登陆提供者。</param>
        /// <param name="name">标识名称。</param>
        /// <param name="cancellationToken">取消标志。</param>
        /// <returns>返回当前用户标识值。</returns>
        public virtual async Task<string> GetTokenAsync(TUser user, string loginProvider, string name, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            var entry = await FindTokenAsync(user, loginProvider, name, cancellationToken);
            return entry?.Value;
        }

        private const string InternalLoginProvider = "[AspNetUserStore]";
        private const string AuthenticatorKeyTokenName = "AuthenticatorKey";
        private const string RecoveryCodeTokenName = "RecoveryCodes";

        /// <summary>
        /// 设置用户验证键，主要用于邮件/电话号码验证。
        /// </summary>
        /// <param name="user">用户实例。</param>
        /// <param name="key">验证键。</param>
        /// <param name="cancellationToken">取消标志。</param>
        public virtual Task SetAuthenticatorKeyAsync(TUser user, string key, CancellationToken cancellationToken)
            => SetTokenAsync(user, InternalLoginProvider, AuthenticatorKeyTokenName, key, cancellationToken);

        /// <summary>
        /// 获取用户验证键。
        /// </summary>
        /// <param name="user">用户实例。</param>
        /// <param name="cancellationToken">取消标志。</param>
        /// <returns>返回用户标识的键值。</returns>
        public virtual Task<string> GetAuthenticatorKeyAsync(TUser user, CancellationToken cancellationToken)
            => GetTokenAsync(user, InternalLoginProvider, AuthenticatorKeyTokenName, cancellationToken);

        /// <summary>
        /// 当前用户有的邀请码的个数。
        /// </summary>
        /// <param name="user">当前用户实例对象。</param>
        /// <param name="cancellationToken">取消标志。</param>
        /// <returns>返回用户邀请码的个数。</returns>
        public virtual async Task<int> CountCodesAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            var mergedCodes = await GetTokenAsync(user, InternalLoginProvider, RecoveryCodeTokenName, cancellationToken) ?? "";
            if (mergedCodes.Length > 0)
            {
                return mergedCodes.Split(';').Length;
            }
            return 0;
        }

        /// <summary>
        /// 替换用户的邀请码。
        /// </summary>
        /// <param name="user">当前用户实例对象。</param>
        /// <param name="recoveryCodes">用户的所有邀请码。</param>
        /// <param name="cancellationToken">取消标志。</param>
        public virtual Task ReplaceCodesAsync(TUser user, IEnumerable<string> recoveryCodes, CancellationToken cancellationToken)
        {
            var mergedCodes = string.Join(";", recoveryCodes);
            return SetTokenAsync(user, InternalLoginProvider, RecoveryCodeTokenName, mergedCodes, cancellationToken);
        }

        /// <summary>
        /// 兑换邀请码，邀请码只能使用一次。
        /// </summary>
        /// <param name="user">当前用户实例对象。</param>
        /// <param name="code">邀请码。</param>
        /// <param name="cancellationToken">取消标志。</param>
        /// <returns>是否兑换成功，如果兑换成功将移除用户的邀请码。</returns>
        public virtual async Task<bool> RedeemCodeAsync(TUser user, string code, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (code == null)
            {
                throw new ArgumentNullException(nameof(code));
            }

            var mergedCodes = await GetTokenAsync(user, InternalLoginProvider, RecoveryCodeTokenName, cancellationToken) ?? "";
            var splitCodes = mergedCodes.Split(';');
            if (splitCodes.Contains(code))
            {
                var updatedCodes = new List<string>(splitCodes.Where(s => s != code));
                await ReplaceCodesAsync(user, updatedCodes, cancellationToken);
                return true;
            }
            return false;
        }
    }

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
    public abstract class IdentityUserStoreBase<TUser, TRole, TUserClaim, TUserRole, TUserLogin, TUserToken, TRoleClaim> :
        IdentityUserStoreBase<TUser, TUserClaim, TUserLogin, TUserToken>,
        IUserRoleStore<TUser>
        where TUser : UserBase
        where TRole : RoleBase
        where TUserClaim : UserClaimBase, new()
        where TUserRole : UserRoleBase, new()
        where TUserLogin : UserLoginBase, new()
        where TUserToken : UserTokenBase, new()
        where TRoleClaim : RoleClaimBase, new()
    {
        /// <summary>
        /// 初始化类<see cref="IdentityUserStoreBase{TUser, TRole, TUserClaim, TUserRole, TUserLogin, TUserToken, TRoleClaim}"/>。
        /// </summary>
        /// <param name="describer">错误描述<see cref="IdentityErrorDescriber"/>实例。</param>
        protected IdentityUserStoreBase(IdentityErrorDescriber describer) : base(describer) { }

        /// <summary>
        /// 实例化一个用户角色实例。
        /// </summary>
        /// <param name="user">当前用户实例。</param>
        /// <param name="role">角色实例。</param>
        /// <returns>返回用户角色实例。</returns>
        protected virtual TUserRole CreateUserRole(TUser user, TRole role)
        {
            return new TUserRole
            {
                UserId = user.UserId,
                RoleId = role.RoleId
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
        public abstract Task<IList<TUser>> GetUsersInRoleAsync(string normalizedRoleName, CancellationToken cancellationToken = default);

        /// <summary>
        /// 添加用户角色。
        /// </summary>
        /// <param name="user">当前用户实例。</param>
        /// <param name="normalizedRoleName">验证角色名称。</param>
        /// <param name="cancellationToken">取消标志。</param>
        public abstract Task AddToRoleAsync(TUser user, string normalizedRoleName, CancellationToken cancellationToken = default);

        /// <summary>
        /// 移除用户角色。
        /// </summary>
        /// <param name="user">用户实例对象。</param>
        /// <param name="normalizedRoleName">验证角色名称。</param>
        /// <param name="cancellationToken">取消标志。</param>
        public abstract Task RemoveFromRoleAsync(TUser user, string normalizedRoleName, CancellationToken cancellationToken = default);

        /// <summary>
        /// 获取用户的所有角色。
        /// </summary>
        /// <param name="user">用户实例对象。</param>
        /// <param name="cancellationToken">取消标志。</param>
        /// <returns>返回当前用户的所有角色列表。</returns>
        public abstract Task<IList<string>> GetRolesAsync(TUser user, CancellationToken cancellationToken = default);

        /// <summary>
        /// 判断用户是否包含当前角色。
        /// </summary>
        /// <param name="user">用户实例。</param>
        /// <param name="normalizedRoleName">验证角色名称。</param>
        /// <param name="cancellationToken">取消标志。</param>
        /// <returns>返回判断结果。</returns>
        public abstract Task<bool> IsInRoleAsync(TUser user, string normalizedRoleName, CancellationToken cancellationToken = default);
    }
}