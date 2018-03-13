using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mozlite.Extensions.Security.Stores;

namespace Mozlite.Extensions.Security
{
    /// <summary>
    /// 用户管理实现类。
    /// </summary>
    /// <typeparam name="TUser">用户类型。</typeparam>
    /// <typeparam name="TUserClaim">用户声明类型。</typeparam>
    /// <typeparam name="TUserLogin">用户登陆类型。</typeparam>
    /// <typeparam name="TUserToken">用户标识类型。</typeparam>
    public abstract class UserManagerBase<TUser, TUserClaim, TUserLogin, TUserToken>
        : UserManager<TUser>, IUserManager<TUser, TUserClaim, TUserLogin, TUserToken>
        where TUser : UserBase
        where TUserClaim : UserClaimBase, new()
        where TUserLogin : UserLoginBase, new()
        where TUserToken : UserTokenBase, new()
    {
        private readonly IServiceProvider _services;
        private SignInManager<TUser> _signInManager;
        /// <summary>
        /// 登陆管理实例。
        /// </summary>
        protected SignInManager<TUser> SignInManager
        {
            get
            {
                if (_signInManager == null)
                    _signInManager = _services.GetRequiredService<SignInManager<TUser>>();
                return _signInManager;
            }
        }

        private IUserStoreBase<TUser, TUserClaim, TUserLogin, TUserToken> _store;
        /// <summary>
        /// 用户存储实例。
        /// </summary>
        protected new IUserStoreBase<TUser, TUserClaim, TUserLogin, TUserToken> Store
        {
            get
            {
                if (_store == null)
                    _store = base.Store as IUserStoreBase<TUser, TUserClaim, TUserLogin, TUserToken>;
                return _store;
            }
        }

        private HttpContext _httpContext;

        /// <summary>
        /// 当前HTTP上下文。
        /// </summary>
        protected HttpContext HttpContext
        {
            get
            {
                if (_httpContext == null)
                    _httpContext = _services.GetRequiredService<IHttpContextAccessor>().HttpContext;
                return _httpContext;
            }
        }
        
        private readonly Type _currentUserCacheKey = typeof(TUser);
        /// <summary>
        /// 获取当前用户。
        /// </summary>
        /// <returns>返回当前用户实例。</returns>
        public TUser GetUser()
        {
            if (HttpContext.Items.TryGetValue(_currentUserCacheKey, out object user) && user is TUser current)
                return current;
            if (int.TryParse(GetUserId(HttpContext.User), out var userId))
                current = Store.FindUser(userId);
            else
                current = null;
            HttpContext.Items[_currentUserCacheKey] = current;
            return current;
        }

        /// <summary>
        /// 获取当前用户。
        /// </summary>
        /// <returns>返回当前用户实例。</returns>
        public async Task<TUser> GetUserAsync()
        {
            if (HttpContext.Items.TryGetValue(_currentUserCacheKey, out object user) && user is TUser current)
                return current;
            if (int.TryParse(GetUserId(HttpContext.User), out var userId))
                current = await Store.FindUserAsync(userId);
            else
                current = null;
            HttpContext.Items[_currentUserCacheKey] = current;
            return current;
        }

        /// <summary>
        /// 判断当前用户是否已经登陆。
        /// </summary>
        /// <returns>返回判断结果。</returns>
        public bool IsSignedIn()
        {
            return SignInManager.IsSignedIn(HttpContext.User);
        }

        /// <summary>
        /// 密码登陆。
        /// </summary>
        /// <param name="userName">用户名。</param>
        /// <param name="password">密码。</param>
        /// <param name="isRemembered">是否记住登陆状态。</param>
        /// <returns>返回登陆结果。</returns>
        public Task<SignInResult> PasswordSignInAsync(string userName, string password, bool isRemembered)
        {
            return PasswordSignInAsync(userName, password, isRemembered, null);
        }

        /// <summary>
        /// 密码登陆。
        /// </summary>
        /// <param name="userName">用户名。</param>
        /// <param name="password">密码。</param>
        /// <param name="isRemembered">是否记住登陆状态。</param>
        /// <param name="success">成功后执行的方法。</param>
        /// <returns>返回登陆结果。</returns>
        public async Task<SignInResult> PasswordSignInAsync(string userName, string password, bool isRemembered, Func<TUser, Task> success)
        {
            var user = await FindByNameAsync(userName);
            var result = await SignInManager.PasswordSignInAsync(user, PasswordSalt(userName, password), isRemembered, true);
            if (result.Succeeded)
            {
                await UpdateAsync(user.UserId, new
                {
                    LoginIP = HttpContext.GetUserAddress(),
                    LastLoginDate = DateTimeOffset.Now
                });
                if (success != null)
                    await success(user);
            }
            return result;
        }

        /// <summary>
        /// 修改密码。
        /// </summary>
        /// <param name="user">用户实例对象。</param>
        /// <param name="password">原始密码。</param>
        /// <param name="newPassword">新密码。</param>
        /// <returns>返回修改结果。</returns>
        public override async Task<IdentityResult> ChangePasswordAsync(TUser user, string password, string newPassword)
        {
            password = PasswordSalt(user.NormalizedUserName, password);
            newPassword = PasswordSalt(user.NormalizedUserName, newPassword);
            return await base.ChangePasswordAsync(user, password, newPassword);
        }

        /// <summary>
        /// 通过用户ID更新用户列。
        /// </summary>
        /// <param name="userId">用户ID。</param>
        /// <param name="fields">用户列。</param>
        /// <returns>返回更新结果。</returns>
        public virtual bool Update(int userId, object fields)
        {
            return Store.Update(userId, fields);
        }

        /// <summary>
        /// 通过用户ID更新用户列。
        /// </summary>
        /// <param name="userId">用户ID。</param>
        /// <param name="fields">用户列。</param>
        /// <returns>返回更新结果。</returns>
        public virtual Task<bool> UpdateAsync(int userId, object fields)
        {
            return Store.UpdateAsync(userId, fields);
        }

        /// <summary>
        /// 分页加载用户。
        /// </summary>
        /// <typeparam name="TQuery">查询类型。</typeparam>
        /// <param name="query">查询实例。</param>
        /// <returns>返回查询分页实例。</returns>
        public virtual TQuery Load<TQuery>(TQuery query) where TQuery : QueryBase<TUser>
        {
            return Store.Load(query);
        }

        /// <summary>
        /// 分页加载用户。
        /// </summary>
        /// <typeparam name="TQuery">查询类型。</typeparam>
        /// <param name="query">查询实例。</param>
        /// <returns>返回查询分页实例。</returns>
        public virtual Task<TQuery> LoadAsync<TQuery>(TQuery query) where TQuery : QueryBase<TUser>
        {
            return Store.LoadAsync(query);
        }
        
        /// <summary>
        /// 通过用户Id查询用户实例。
        /// </summary>
        /// <param name="userId">用户Id。</param>
        /// <returns>返回用户实例。</returns>
        public virtual Task<TUser> FindByIdAsync(int userId)
        {
            return Store.FindUserAsync(userId);
        }

        /// <summary>
        /// 通过用户Id查询用户实例。
        /// </summary>
        /// <param name="userName">用户名称。</param>
        /// <returns>返回用户实例。</returns>
        public override Task<TUser> FindByNameAsync(string userName)
        {
            return Store.FindByNameAsync(userName);
        }

        /// <summary>
        /// 加密字符串。
        /// </summary>
        /// <param name="user">用户实例。</param>
        /// <returns>返回加密后得字符串。</returns>
        public virtual string HashPassword(TUser user)
        {
            var userName = user.NormalizedUserName ?? NormalizeKey(user.UserName);
            var password = PasswordSalt(userName, user.PasswordHash);
            return PasswordHasher.HashPassword(user, password);
        }

        /// <summary>
        /// 加密字符串。
        /// </summary>
        /// <param name="password">原始密码。</param>
        /// <returns>返回加密后得字符串。</returns>
        public virtual string HashPassword(string password)
        {
            return PasswordHasher.HashPassword(null, password);
        }

        /// <summary>
        /// 验证密码。
        /// </summary>
        /// <param name="hashedPassword">原始已经加密得密码。</param>
        /// <param name="providedPassword">要验证得原始密码。</param>
        /// <returns>验证结果。</returns>
        public virtual bool VerifyHashedPassword(string hashedPassword, string providedPassword)
        {
            return PasswordHasher.VerifyHashedPassword(null, hashedPassword, providedPassword) !=
                   PasswordVerificationResult.Failed;
        }

        /// <summary>
        /// 拼接密码。
        /// </summary>
        /// <param name="userName">当前用户名。</param>
        /// <param name="password">密码。</param>
        /// <returns>返回拼接后得字符串。</returns>
        public virtual string PasswordSalt(string userName, string password)
        {
            return $"{NormalizeKey(userName)}2018{password}";
        }

        /// <summary>
        /// 初始化类<see cref="UserManagerBase{TUser, TUserClaim, TUserLogin, TUserToken}"/>。
        /// </summary>
        /// <param name="store">用户存储接口。</param>
        /// <param name="optionsAccessor"><see cref="T:Microsoft.AspNetCore.Identity.IdentityOptions" />实例对象。</param>
        /// <param name="passwordHasher">密码加密器接口。</param>
        /// <param name="userValidators">用户验证接口。</param>
        /// <param name="passwordValidators">密码验证接口。</param>
        /// <param name="keyNormalizer">唯一键格式化字符串。</param>
        /// <param name="errors">错误实例。</param>
        /// <param name="services">服务提供者接口。</param>
        /// <param name="logger">日志接口。</param>
        protected UserManagerBase(IUserStore<TUser> store, IOptions<IdentityOptions> optionsAccessor, IPasswordHasher<TUser> passwordHasher, IEnumerable<IUserValidator<TUser>> userValidators, IEnumerable<IPasswordValidator<TUser>> passwordValidators, ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, IServiceProvider services, ILogger<UserManager<TUser>> logger)
            : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {
            _services = services;
        }
    }

    /// <summary>
    /// 用户管理实现类。
    /// </summary>
    /// <typeparam name="TUser">用户类型。</typeparam>
    /// <typeparam name="TRole">角色类型。</typeparam>
    /// <typeparam name="TUserClaim">用户声明类型。</typeparam>
    /// <typeparam name="TUserRole">用户角色类型。</typeparam>
    /// <typeparam name="TUserLogin">用户登陆类型。</typeparam>
    /// <typeparam name="TUserToken">用户标识类型。</typeparam>
    /// <typeparam name="TRoleClaim">角色声明类型。</typeparam>
    public abstract class UserManagerBase<TUser, TRole, TUserClaim, TUserRole, TUserLogin, TUserToken, TRoleClaim>
        : UserManagerBase<TUser, TUserClaim, TUserLogin, TUserToken>,
            IUserManager<TUser, TRole, TUserClaim, TUserRole, TUserLogin, TUserToken, TRoleClaim>
        where TUser : UserBase
        where TRole : RoleBase
        where TUserClaim : UserClaimBase, new()
        where TUserRole : UserRoleBase, new()
        where TUserLogin : UserLoginBase, new()
        where TUserToken : UserTokenBase, new()
        where TRoleClaim : RoleClaimBase, new()
    {
        private IUserStoreBase<TUser, TRole, TUserClaim, TUserRole, TUserLogin, TUserToken, TRoleClaim> _store;

        /// <summary>
        /// 用户存储实例。
        /// </summary>
        protected new IUserStoreBase<TUser, TRole, TUserClaim, TUserRole, TUserLogin, TUserToken, TRoleClaim> Store
        {
            get
            {
                if (_store == null)
                {
                    _store =
                        base.Store as IUserStoreBase<TUser, TRole, TUserClaim, TUserRole, TUserLogin, TUserToken,
                            TRoleClaim>;
                }
                return _store;
            }
        }

        /// <summary>
        /// 初始化类<see cref="UserManagerBase{TUser, TRole, TUserClaim, TUserRole, TUserLogin, TUserToken, TRoleClaim}"/>。
        /// </summary>
        /// <param name="store">用户存储接口。</param>
        /// <param name="optionsAccessor"><see cref="T:Microsoft.AspNetCore.Identity.IdentityOptions" />实例对象。</param>
        /// <param name="passwordHasher">密码加密器接口。</param>
        /// <param name="userValidators">用户验证接口。</param>
        /// <param name="passwordValidators">密码验证接口。</param>
        /// <param name="keyNormalizer">唯一键格式化字符串。</param>
        /// <param name="errors">错误实例。</param>
        /// <param name="services">服务提供者接口。</param>
        /// <param name="logger">日志接口。</param>
        protected UserManagerBase(IUserStore<TUser> store, IOptions<IdentityOptions> optionsAccessor, IPasswordHasher<TUser> passwordHasher, IEnumerable<IUserValidator<TUser>> userValidators, IEnumerable<IPasswordValidator<TUser>> passwordValidators, ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, IServiceProvider services, ILogger<UserManager<TUser>> logger) 
            : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {
        }
    }
}