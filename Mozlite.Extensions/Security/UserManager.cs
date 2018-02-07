using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
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
    public abstract class UserManager<TUser, TUserClaim, TUserLogin, TUserToken>
        : IUserManager<TUser, TUserClaim, TUserLogin, TUserToken>
        where TUser : UserBase
        where TUserClaim : UserClaimBase, new()
        where TUserLogin : UserLoginBase, new()
        where TUserToken : UserTokenBase, new()
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IPasswordManager _passwordManager;

        /// <summary>
        /// 系统管理实例。
        /// </summary>
        protected UserManager<TUser> Manager { get; }

        /// <summary>
        /// 登陆管理实例。
        /// </summary>
        protected SignInManager<TUser> SignInManager { get; }

        /// <summary>
        /// 用户存储实例。
        /// </summary>
        protected IUserStoreBase<TUser, TUserClaim, TUserLogin, TUserToken> Store { get; }

        /// <summary>
        /// 当前HTTP上下文。
        /// </summary>
        protected HttpContext HttpContext => _httpContextAccessor.HttpContext;

        /// <summary>
        /// 初始化类<see cref="UserManager{TUser, TUserClaim, TUserLogin, TUserToken}"/>。
        /// </summary>
        /// <param name="manager">系统管理实例。</param>
        /// <param name="signInManager"> 登陆管理实例。</param>
        /// <param name="store">用户存储实例。</param>
        /// <param name="httpContextAccessor">HTTP上下文访问接口。</param>
        /// <param name="passwordManager">密码管理接口。</param>
        protected UserManager(UserManager<TUser> manager, SignInManager<TUser> signInManager, IUserStore<TUser> store, IHttpContextAccessor httpContextAccessor, IPasswordManager passwordManager)
        {
            _httpContextAccessor = httpContextAccessor;
            _passwordManager = passwordManager;
            Manager = manager;
            SignInManager = signInManager;
            Store = store as IUserStoreBase<TUser, TUserClaim, TUserLogin, TUserToken>;
        }

        /// <summary>
        /// 正常实例化键。
        /// </summary>
        /// <param name="key">原有键值。</param>
        /// <returns>返回正常化后的字符串。</returns>
        public virtual string NormalizeKey(string key) => Manager.NormalizeKey(key);

        private readonly Type _currentUserCacheKey = typeof(TUser);
        /// <summary>
        /// 获取当前用户。
        /// </summary>
        /// <returns>返回当前用户实例。</returns>
        public TUser GetUser()
        {
            if (HttpContext.Items.TryGetValue(_currentUserCacheKey, out object user) && user is TUser current)
                return current;
            if (int.TryParse(Manager.GetUserId(HttpContext.User), out var userId))
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
            if (int.TryParse(Manager.GetUserId(HttpContext.User), out var userId))
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
        /// 通过用户ID更新用户列。
        /// </summary>
        /// <param name="userId">用户ID。</param>
        /// <param name="fields">用户列。</param>
        /// <returns>返回更新结果。</returns>
        public virtual bool Update(int userId, object fields)
        {
            return Store.UserContext.Update(x => x.UserId == userId, fields);
        }

        /// <summary>
        /// 通过用户ID更新用户列。
        /// </summary>
        /// <param name="userId">用户ID。</param>
        /// <param name="fields">用户列。</param>
        /// <returns>返回更新结果。</returns>
        public virtual Task<bool> UpdateAsync(int userId, object fields)
        {
            return Store.UserContext.UpdateAsync(x => x.UserId == userId, fields);
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
        public virtual Task<TUser> FindByNameAsync(string userName)
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
            return HashPassword(password);
        }

        /// <summary>
        /// 加密字符串。
        /// </summary>
        /// <param name="password">原始密码。</param>
        /// <returns>返回加密后得字符串。</returns>
        public string HashPassword(string password)
        {
            return _passwordManager.HashPassword(password);
        }

        /// <summary>
        /// 验证密码。
        /// </summary>
        /// <param name="hashedPassword">原始已经加密得密码。</param>
        /// <param name="providedPassword">要验证得原始密码。</param>
        /// <returns>验证结果。</returns>
        public virtual bool VerifyHashedPassword(string hashedPassword, string providedPassword)
        {
            return _passwordManager.VerifyHashedPassword(hashedPassword, providedPassword);
        }

        /// <summary>
        /// 拼接密码。
        /// </summary>
        /// <param name="userName">当前用户名。</param>
        /// <param name="password">密码。</param>
        /// <returns>返回拼接后得字符串。</returns>
        public string PasswordSalt(string userName, string password)
        {
            return _passwordManager.PasswordSalt(userName, password);
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
    public abstract class UserManager<TUser, TRole, TUserClaim, TUserRole, TUserLogin, TUserToken, TRoleClaim>
        : UserManager<TUser, TUserClaim, TUserLogin, TUserToken>,
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
        protected new IUserStoreBase<TUser, TRole, TUserClaim, TUserRole, TUserLogin, TUserToken, TRoleClaim> Store => _store ?? (_store =
                                                                                                                           base.Store as IUserStoreBase<TUser, TRole, TUserClaim, TUserRole, TUserLogin, TUserToken,
                                                                                                                               TRoleClaim>);

        /// <summary>
        /// 初始化类<see cref="UserManager{TUser, TRole, TUserClaim, TUserRole, TUserLogin, TUserToken, TRoleClaim}"/>。
        /// </summary>
        /// <param name="manager">系统管理实例。</param>
        /// <param name="signInManager"> 登陆管理实例。</param>
        /// <param name="store">用户存储实例。</param>
        /// <param name="httpContextAccessor">HTTP上下文访问接口。</param>
        /// <param name="passwordManager">密码管理接口。</param>
        protected UserManager(UserManager<TUser> manager, SignInManager<TUser> signInManager, IUserStore<TUser> store, IHttpContextAccessor httpContextAccessor, IPasswordManager passwordManager) : base(manager, signInManager, store, httpContextAccessor, passwordManager)
        {
        }
    }
}