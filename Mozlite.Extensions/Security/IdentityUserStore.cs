using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Mozlite.Data;

namespace Mozlite.Extensions.Security
{
    /// <summary>
    /// 用户存储类。
    /// </summary>
    /// <typeparam name="TUser">用户类型。</typeparam>
    /// <typeparam name="TRole">用户组类型。</typeparam>
    /// <typeparam name="TUserClaim">用户声明。</typeparam>
    /// <typeparam name="TUserRole">用户和用户组类型。</typeparam>
    /// <typeparam name="TUserLogin">用户登入类型。</typeparam>
    /// <typeparam name="TUserToken">用户标识存储类型。</typeparam>
    public class IdentityUserStore<TUser, TRole, TUserClaim, TUserRole, TUserLogin, TUserToken> :
        IUserLoginStore<TUser>,
        IUserRoleStore<TUser>,
        IUserClaimStore<TUser>,
        IUserPasswordStore<TUser>,
        IUserSecurityStampStore<TUser>,
        IUserEmailStore<TUser>,
        IUserLockoutStore<TUser>,
        IUserPhoneNumberStore<TUser>,
        IUserTwoFactorStore<TUser>,
        IUserAuthenticationTokenStore<TUser>
        where TUser : IdentityUser, new()
        where TRole : IdentityRole, new()
        where TUserClaim : IdentityUserClaim, new()
        where TUserRole : IdentityUserRole, new()
        where TUserLogin : IdentityUserLogin, new()
        where TUserToken : IdentityUserToken, new()
    {
        private readonly IRepository<TUser> _users;
        private readonly IRepository<TUserClaim> _userClaims;
        private readonly IRepository<TUserRole> _userRoles;
        private readonly IRepository<TUserToken> _userTokens;
        private readonly IRepository<TRole> _roles;
        private readonly IRepository<TUserLogin> _userLogins;

        /// <summary>
        /// 初始化类<see cref="IdentityUserStore{TUser, TRole, TUserClaim, TUserRole, TUserLogin,  TUserToken}"/>。
        /// </summary>
        /// <param name="users">用户数据操作接口实例。</param>
        /// <param name="userClaims">用户声明数据操作接口实例。</param>
        /// <param name="userRoles">用户组数据操作接口实例。</param>
        /// <param name="userTokens">用户标识数据操作接口实例。</param>
        /// <param name="roles">用户组数据操作接口实例。</param>
        /// <param name="userLogins">用户登入数据操作接口实例。</param>
        /// <param name="describer">错误描述实例。</param>
        public IdentityUserStore(IRepository<TUser> users, IRepository<TUserClaim> userClaims, IRepository<TUserRole> userRoles, IRepository<TUserToken> userTokens, IRepository<TRole> roles, IRepository<TUserLogin> userLogins, IdentityErrorDescriber describer = null)
        {
            _users = users;
            _userClaims = userClaims;
            _userRoles = userRoles;
            _userTokens = userTokens;
            _roles = roles;
            _userLogins = userLogins;
            ErrorDescriber = describer ?? new IdentityErrorDescriber();
        }

        /// <summary>
        /// 当前操作错误描述实例。
        /// </summary>
        public IdentityErrorDescriber ErrorDescriber { get; set; }

        public virtual Task AddClaimsAsync(TUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            return _userClaims.BeginTransactionAsync(async repository =>
            {
                foreach (var claim in claims)
                {
                    var userClaim = new TUserClaim { ClaimType = claim.Type, ClaimValue = claim.Value, UserId = user.UserId };
                    await _userClaims.CreateAsync(userClaim, cancellationToken);
                }
                return true;
            }, cancellationToken: cancellationToken);
        }

        public virtual Task AddLoginAsync(TUser user, UserLoginInfo login, CancellationToken cancellationToken)
        {
            var userLogin = new TUserLogin
            {
                UserId = user.UserId,
                ProviderKey = login.ProviderKey,
                LoginProvider = login.LoginProvider,
                ProviderDisplayName = login.ProviderDisplayName
            };
            return _userLogins.CreateAsync(userLogin, cancellationToken);
        }

        public virtual async Task AddToRoleAsync(TUser user, string roleName, CancellationToken cancellationToken)
        {
            roleName = roleName.ToUpper();
            var role = await _roles.FindAsync(r => r.NormalizedRoleName == roleName, cancellationToken);
            if (role == null)
                return;
            await _userRoles.CreateAsync(new TUserRole { RoleId = role.RoleId, UserId = user.UserId }, cancellationToken);
        }

        public virtual async Task<IdentityResult> CreateAsync(TUser user, CancellationToken cancellationToken)
        {
            await _users.CreateAsync(user, cancellationToken);
            return IdentityResult.Success;
        }

        public virtual async Task<IdentityResult> DeleteAsync(TUser user, CancellationToken cancellationToken)
        {
            await _users.DeleteAsync(u => u.UserId == user.UserId, cancellationToken);
            return IdentityResult.Success;
        }

        public virtual void Dispose()
        {

        }

        public virtual Task<TUser> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken = default)
        {
            return _users.FindAsync(u => u.NormalizedEmail == normalizedEmail, cancellationToken);
        }

        public virtual async Task<TUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            if (int.TryParse(userId, out var id))
                return await _users.FindAsync(u => u.UserId == id, cancellationToken);
            return null;
        }

        public virtual Task<TUser> FindByLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            return _users.AsQueryable().InnerJoin<TUserLogin>((u, l) => u.UserId == l.UserId)
                .Where<TUserLogin>(x => x.LoginProvider == loginProvider && x.ProviderKey == providerKey)
                .SingleOrDefaultAsync(cancellationToken);
        }

        public virtual Task<TUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken = default)
        {
            return _users.FindAsync(u => u.NormalizedUserName == normalizedUserName, cancellationToken);
        }

        public virtual Task<int> GetAccessFailedCountAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            return Task.FromResult(user.AccessFailedCount);
        }

        public virtual async Task<IList<Claim>> GetClaimsAsync(TUser user, CancellationToken cancellationToken)
        {
            var userClaims = await _userClaims.FetchAsync(uc => uc.UserId == user.UserId, cancellationToken);
            return userClaims?.Select(c => new Claim(c.ClaimType, c.ClaimValue)).ToList();
        }

        public virtual Task<string> GetEmailAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            return Task.FromResult(user.Email);
        }

        public virtual Task<bool> GetEmailConfirmedAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            return Task.FromResult(user.EmailConfirmed);
        }

        public virtual Task<bool> GetLockoutEnabledAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            return Task.FromResult(user.LockoutEnabled);
        }

        public virtual Task<DateTimeOffset?> GetLockoutEndDateAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            return Task.FromResult(user.LockoutEnd);
        }

        public virtual async Task<IList<UserLoginInfo>> GetLoginsAsync(TUser user, CancellationToken cancellationToken)
        {
            var userLogins = await _userLogins.FetchAsync(ul => ul.UserId == user.UserId, cancellationToken);
            return
                userLogins?.Select(l => new UserLoginInfo(l.LoginProvider, l.ProviderKey, l.ProviderDisplayName))
                    .ToList();
        }

        public virtual Task<string> GetNormalizedEmailAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            return Task.FromResult(user.NormalizedEmail);
        }

        public virtual Task<string> GetNormalizedUserNameAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            return Task.FromResult(user.NormalizedUserName);
        }

        public virtual Task<string> GetPasswordHashAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            return Task.FromResult(user.PasswordHash);
        }

        public virtual Task<string> GetPhoneNumberAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            return Task.FromResult(user.PhoneNumber);
        }

        public virtual Task<bool> GetPhoneNumberConfirmedAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            return Task.FromResult(user.PhoneNumberConfirmed);
        }

        public virtual async Task<IList<string>> GetRolesAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return (await GetRolesAsync(user.UserId, cancellationToken))
                .Select(r => r.RoleName)
                .ToList();
        }

        public virtual Task<string> GetSecurityStampAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            return Task.FromResult(user.SecurityStamp);
        }

        public virtual async Task<string> GetTokenAsync(TUser user, string loginProvider, string name, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            var entry = await _userTokens.FindAsync(t => t.UserId == user.UserId && t.LoginProvider == loginProvider && t.Name == name, cancellationToken);
            return entry?.Value;
        }

        public virtual Task<bool> GetTwoFactorEnabledAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            return Task.FromResult(user.TwoFactorEnabled);
        }

        public virtual Task<string> GetUserIdAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            return Task.FromResult(user.UserId.ToString());
        }

        public virtual Task<string> GetUserNameAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            return Task.FromResult(user.UserName);
        }

        public virtual async Task<IList<TUser>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken)
        {
            var users = await _users.AsQueryable().InnerJoin<TUserClaim>((u, c) => u.UserId == c.UserId)
                .Where<TUserClaim>(x => x.ClaimType == claim.Type && x.ClaimValue == claim.Value)
                .AsEnumerableAsync(cancellationToken);
            return users.ToList();
        }

        public virtual async Task<IList<TUser>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
        {
            roleName = roleName.ToUpper();
            var users = await _users.AsQueryable().InnerJoin<TUserRole>((u, x) => u.UserId == x.UserId)
                .InnerJoin<TUserRole, TRole>((ur, r) => ur.RoleId == r.RoleId)
                .Where<TRole>(r => r.NormalizedRoleName == roleName)
                .AsEnumerableAsync(cancellationToken);
            return users.ToList();
        }

        public virtual Task<bool> HasPasswordAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(user.PasswordHash != null);
        }

        public virtual Task<int> IncrementAccessFailedCountAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            user.AccessFailedCount++;
            return Task.FromResult(user.AccessFailedCount);
        }

        public virtual async Task<bool> IsInRoleAsync(TUser user, string roleName, CancellationToken cancellationToken)
        {
            var userRole = await _userRoles.AsQueryable().InnerJoin<TRole>((ur, r) => ur.RoleId == r.RoleId)
                .Where(x => x.UserId == user.UserId)
                .Where<TRole>(x => x.RoleName == roleName)
                .SingleOrDefaultAsync(cancellationToken);
            return userRole != null;
        }

        public virtual async Task RemoveClaimsAsync(TUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (claims == null)
            {
                throw new ArgumentNullException(nameof(claims));
            }
            await _userClaims.BeginTransactionAsync(async repository =>
            {
                foreach (var claim in claims)
                {
                    await repository.DeleteAsync(
                            uc => uc.UserId == user.UserId && uc.ClaimType == claim.Type && uc.ClaimValue == claim.Value, cancellationToken);
                }
                return true;
            }, cancellationToken: cancellationToken);
        }

        public virtual async Task RemoveFromRoleAsync(TUser user, string roleName, CancellationToken cancellationToken)
        {
            var role = await _roles.FindAsync(r => r.NormalizedRoleName == roleName.ToUpper(), cancellationToken);
            if (role == null)
                return;
            await _userRoles.DeleteAsync(ur => ur.UserId == user.UserId && ur.RoleId == role.RoleId, cancellationToken);
        }

        public virtual Task RemoveLoginAsync(TUser user, string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            return _userLogins.DeleteAsync(userLogin => userLogin.UserId == user.UserId && userLogin.LoginProvider == loginProvider && userLogin.ProviderKey == providerKey, cancellationToken);
        }

        public virtual Task RemoveTokenAsync(TUser user, string loginProvider, string name, CancellationToken cancellationToken)
        {
            return _userTokens.DeleteAsync(l => l.UserId == user.UserId && l.LoginProvider == loginProvider && l.Name == name, cancellationToken);
        }

        public virtual async Task ReplaceClaimAsync(TUser user, Claim claim, Claim newClaim, CancellationToken cancellationToken)
        {
            await _userClaims.UpdateAsync(
                    uc => uc.UserId == user.UserId && uc.ClaimValue == claim.Value && uc.ClaimType == claim.Type,
                    new { ClaimType = newClaim.Type, ClaimValue = newClaim.Value }, cancellationToken);
        }

        public virtual Task ResetAccessFailedCountAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            user.AccessFailedCount = 0;
            return Task.FromResult(0);
        }

        public virtual Task SetEmailAsync(TUser user, string email, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            user.Email = email;
            return Task.FromResult(0);
        }

        public virtual Task SetEmailConfirmedAsync(TUser user, bool confirmed, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            user.EmailConfirmed = confirmed;
            return Task.FromResult(0);
        }

        public virtual Task SetLockoutEnabledAsync(TUser user, bool enabled, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            user.LockoutEnabled = enabled;
            return Task.FromResult(0);
        }

        public virtual Task SetLockoutEndDateAsync(TUser user, DateTimeOffset? lockoutEnd, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            user.LockoutEnd = lockoutEnd;
            return Task.FromResult(0);
        }

        public virtual Task SetNormalizedEmailAsync(TUser user, string normalizedEmail, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            user.NormalizedEmail = normalizedEmail;
            return Task.FromResult(0);
        }

        public virtual Task SetNormalizedUserNameAsync(TUser user, string normalizedName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            user.NormalizedUserName = normalizedName;
            return Task.FromResult(0);
        }

        public virtual Task SetPasswordHashAsync(TUser user, string passwordHash, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            user.PasswordHash = passwordHash;
            return Task.FromResult(0);
        }

        public virtual Task SetPhoneNumberAsync(TUser user, string phoneNumber, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            user.PhoneNumber = phoneNumber;
            return Task.FromResult(0);
        }

        public virtual Task SetPhoneNumberConfirmedAsync(TUser user, bool confirmed, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            user.PhoneNumberConfirmed = confirmed;
            return Task.FromResult(0);
        }

        public virtual Task SetSecurityStampAsync(TUser user, string stamp, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            user.SecurityStamp = stamp;
            return Task.FromResult(0);
        }

        public virtual async Task SetTokenAsync(TUser user, string loginProvider, string name, string value, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (
                await
                    _userTokens.AnyAsync(
                        ut => ut.UserId == user.UserId && ut.LoginProvider == loginProvider && ut.Name == name,
                        cancellationToken))
                await
                    _userTokens.UpdateAsync(
                        ut => ut.UserId == user.UserId && ut.LoginProvider == loginProvider && ut.Name == name,
                        new { Value = value },
                        cancellationToken);
            else
                await
                    _userTokens.CreateAsync(new TUserToken
                    {
                        LoginProvider = loginProvider,
                        Name = name,
                        UserId = user.UserId,
                        Value = value
                    },
                        cancellationToken);
        }

        public virtual Task SetTwoFactorEnabledAsync(TUser user, bool enabled, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            user.TwoFactorEnabled = enabled;
            return Task.FromResult(0);
        }

        public virtual Task SetUserNameAsync(TUser user, string userName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            user.UserName = userName;
            return Task.FromResult(0);
        }

        public virtual async Task<IdentityResult> UpdateAsync(TUser user, CancellationToken cancellationToken)
        {
            await _users.UpdateAsync(user, cancellationToken);
            return IdentityResult.Success;
        }

        /// <summary>
        /// 获取用户组列表。
        /// </summary>
        /// <param name="userId">用户ID。</param>
        /// <param name="cancellationToken">取消标识。</param>
        /// <returns>返回用户组列表。</returns>
        public virtual Task<IEnumerable<TRole>> GetRolesAsync(int userId, CancellationToken cancellationToken = default)
        {
            return _roles.AsQueryable().InnerJoin<TUserRole>((r, ur) => r.RoleId == ur.RoleId)
                .Where<TUserRole>(u => u.UserId == userId).AsEnumerableAsync(cancellationToken);
        }
    }
}