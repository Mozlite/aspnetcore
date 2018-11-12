using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mozlite.Extensions.Security.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        : UserManager<TUser>, IUserManager<TUser>
        where TUser : UserBase, new()
        where TUserClaim : UserClaimBase, new()
        where TUserLogin : UserLoginBase, new()
        where TUserToken : UserTokenBase, new()
    {
        private readonly IServiceProvider _serviceProvider;
        /// <summary>
        /// 获取当前接口。
        /// </summary>
        /// <typeparam name="TService">当前服务类型。</typeparam>
        /// <returns>返回当前服务实例。</returns>
        protected TService GetService<TService>() => _serviceProvider.GetService<TService>();

        /// <summary>
        /// 获取当前接口。
        /// </summary>
        /// <typeparam name="TService">当前服务类型。</typeparam>
        /// <returns>返回当前服务实例。</returns>
        protected TService GetRequiredService<TService>() => _serviceProvider.GetRequiredService<TService>();

        private SignInManager<TUser> _signInManager;
        /// <summary>
        /// 登陆管理实例。
        /// </summary>
        public SignInManager<TUser> SignInManager
        {
            get
            {
                if (_signInManager == null)
                    _signInManager = _serviceProvider.GetRequiredService<SignInManager<TUser>>();
                return _signInManager;
            }
        }

        private readonly IUserStoreBase<TUser, TUserClaim, TUserLogin, TUserToken> _store;
        /// <summary>
        /// 数据库操作接口。
        /// </summary>
        protected IUserDbContext<TUser, TUserClaim, TUserLogin, TUserToken> DbContext { get; }

        private HttpContext _httpContext;

        /// <summary>
        /// 当前HTTP上下文。
        /// </summary>
        protected HttpContext HttpContext
        {
            get
            {
                if (_httpContext == null)
                    _httpContext = _serviceProvider.GetRequiredService<IHttpContextAccessor>().HttpContext;
                return _httpContext;
            }
        }

        /// <summary>
        /// 新建用户实例（不会对密码进行加密）。
        /// </summary>
        /// <param name="user">用户实例对象。</param>
        /// <returns>返回添加用户结果。</returns>
        public override Task<IdentityResult> CreateAsync(TUser user)
        {
            if (user.CreatedIP == null)
                user.CreatedIP = HttpContext.GetUserAddress();
            return base.CreateAsync(user);
        }

        /// <summary>
        /// 新建用户实例。
        /// </summary>
        /// <param name="user">用户实例对象。</param>
        /// <param name="password">未加密时的密码。</param>
        /// <returns>返回添加用户结果。</returns>
        public override Task<IdentityResult> CreateAsync(TUser user, string password)
        {
            if (user.CreatedIP == null)
                user.CreatedIP = HttpContext.GetUserAddress();
            return base.CreateAsync(user, password);
        }

        /// <summary>
        /// 获取当前用户。
        /// </summary>
        /// <returns>返回当前用户实例。</returns>
        public TUser GetUser() => HttpContext.GetUser<TUser>();

        /// <summary>
        /// 获取当前用户。
        /// </summary>
        /// <returns>返回当前用户实例。</returns>
        public Task<TUser> GetUserAsync() => HttpContext.GetUserAsync<TUser>();

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
            var result = await SignInManager.PasswordSignInAsync(user, password, isRemembered, true);
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
        /// 登出。
        /// </summary>
        public virtual Task SignOutAsync()
        {
            return SignInManager.SignOutAsync();
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
        /// 重置密码。
        /// </summary>
        /// <param name="user">用户实例对象。</param>
        /// <param name="newPassword">新密码。</param>
        /// <returns>返回修改结果。</returns>
        public virtual Task<IdentityResult> ResetPasswordAsync(TUser user, string newPassword)
        {
            return ResetPasswordAsync(user, null, newPassword);
        }

        /// <summary>
        /// 重置密码。
        /// </summary>
        /// <param name="user">用户实例对象。</param>
        /// <param name="token">修改密码标识。</param>
        /// <param name="newPassword">新密码。</param>
        /// <returns>返回修改结果。</returns>
        public override async Task<IdentityResult> ResetPasswordAsync(TUser user, string token, string newPassword)
        {
            if (user.PasswordHash == null || user.NormalizedUserName == null)
                user = await FindByIdAsync(user.UserId);
            if (token == null)
                token = await GeneratePasswordResetTokenAsync(user);
            newPassword = PasswordSalt(user.NormalizedUserName, newPassword);
            return await base.ResetPasswordAsync(user, token, newPassword);
        }

        /// <summary>
        /// 通过用户ID更新用户列。
        /// </summary>
        /// <param name="userId">用户ID。</param>
        /// <param name="fields">用户列。</param>
        /// <returns>返回更新结果。</returns>
        public virtual bool Update(int userId, object fields)
        {
            return _store.Update(userId, fields);
        }

        /// <summary>
        /// 通过用户ID更新用户列。
        /// </summary>
        /// <param name="userId">用户ID。</param>
        /// <param name="fields">用户列。</param>
        /// <returns>返回更新结果。</returns>
        public virtual Task<bool> UpdateAsync(int userId, object fields)
        {
            return _store.UpdateAsync(userId, fields);
        }

        /// <summary>
        /// 分页加载用户。
        /// </summary>
        /// <typeparam name="TQuery">查询类型。</typeparam>
        /// <param name="query">查询实例。</param>
        /// <returns>返回查询分页实例。</returns>
        public virtual TQuery Load<TQuery>(TQuery query) where TQuery : UserQuery<TUser>
        {
            return _store.Load(query);
        }

        /// <summary>
        /// 分页加载用户。
        /// </summary>
        /// <typeparam name="TQuery">查询类型。</typeparam>
        /// <param name="query">查询实例。</param>
        /// <returns>返回查询分页实例。</returns>
        public virtual Task<TQuery> LoadAsync<TQuery>(TQuery query) where TQuery : UserQuery<TUser>
        {
            return _store.LoadAsync(query);
        }

        /// <summary>
        /// 判断当前用户名称是否存在。
        /// </summary>
        /// <param name="user">用户实例。</param>
        /// <returns>返回判断结果。</returns>
        public virtual IdentityResult IsDuplicated(TUser user)
        {
            return _store.IsDuplicated(user);
        }

        /// <summary>
        /// 判断当前用户名称是否存在。
        /// </summary>
        /// <param name="user">用户实例。</param>
        /// <returns>返回判断结果。</returns>
        public virtual Task<IdentityResult> IsDuplicatedAsync(TUser user)
        {
            return _store.IsDuplicatedAsync(user, CancellationToken);
        }

        /// <summary>
        /// 判断当前用户名称是否存在。
        /// </summary>
        /// <param name="userId">用户实例。</param>
        /// <param name="userName">用户名称。</param>
        /// <returns>返回判断结果。</returns>
        public virtual IdentityResult IsDuplicated(int userId, string userName)
        {
            if (DbContext.UserContext.Any(x => x.UserId != userId && (x.UserName == userName || x.NormalizedUserName == userName)))
                return IdentityResult.Failed(ErrorDescriber.DuplicateUserName(userName));
            return IdentityResult.Success;
        }

        /// <summary>
        /// 判断当前用户名称是否存在。
        /// </summary>
        /// <param name="userId">用户实例。</param>
        /// <param name="userName">用户名称。</param>
        /// <returns>返回判断结果。</returns>
        public virtual async Task<IdentityResult> IsDuplicatedAsync(int userId, string userName)
        {
            if (await DbContext.UserContext.AnyAsync(x => x.UserId != userId && (x.UserName == userName || x.NormalizedUserName == userName)))
                return IdentityResult.Failed(ErrorDescriber.DuplicateUserName(userName));
            return IdentityResult.Success;
        }

        /// <summary>
        /// 锁定或者解锁用户。
        /// </summary>
        /// <param name="userId">用户Id。</param>
        /// <param name="lockoutEnd">锁定截至日期。</param>
        /// <returns>返回执行结果。</returns>
        public virtual bool Lockout(int userId, DateTimeOffset? lockoutEnd = null)
        {
            return _store.Update(userId, new { LockoutEnd = lockoutEnd });
        }

        /// <summary>
        /// 锁定或者解锁用户。
        /// </summary>
        /// <param name="userId">用户Id。</param>
        /// <param name="lockoutEnd">锁定截至日期。</param>
        /// <returns>返回执行结果。</returns>
        public virtual Task<bool> LockoutAsync(int userId, DateTimeOffset? lockoutEnd = null)
        {
            return _store.UpdateAsync(userId, new { LockoutEnd = lockoutEnd });
        }

        /// <summary>
        /// 删除用户。
        /// </summary>
        /// <param name="ids">用户Id。</param>
        /// <returns>返回删除结果。</returns>
        public virtual IdentityResult Delete(int[] ids)
        {
            if (DbContext.UserContext.Delete(x => x.UserId.Included(ids)))
                return IdentityResult.Success;
            return IdentityResult.Failed(ErrorDescriber.DefaultError());
        }

        /// <summary>
        /// 删除用户。
        /// </summary>
        /// <param name="id">用户Id。</param>
        /// <returns>返回删除结果。</returns>
        public virtual IdentityResult Delete(int id)
        {
            if (DbContext.UserContext.Delete(id))
                return IdentityResult.Success;
            return IdentityResult.Failed(ErrorDescriber.DefaultError());
        }

        /// <summary>
        /// 删除用户。
        /// </summary>
        /// <param name="ids">用户Id。</param>
        /// <returns>返回删除结果。</returns>
        public virtual async Task<IdentityResult> DeleteAsync(int[] ids)
        {
            if (await DbContext.UserContext.DeleteAsync(x => x.UserId.Included(ids)))
                return IdentityResult.Success;
            return IdentityResult.Failed(ErrorDescriber.DefaultError());
        }

        /// <summary>
        /// 删除用户。
        /// </summary>
        /// <param name="id">用户Id。</param>
        /// <returns>返回删除结果。</returns>
        public virtual async Task<IdentityResult> DeleteAsync(int id)
        {
            if (await DbContext.UserContext.DeleteAsync(id))
                return IdentityResult.Success;
            return IdentityResult.Failed(ErrorDescriber.DefaultError());
        }

        /// <summary>
        /// 通过用户Id查询用户实例。
        /// </summary>
        /// <param name="userId">用户Id。</param>
        /// <returns>返回用户实例。</returns>
        public virtual Task<TUser> FindByIdAsync(int userId)
        {
            return _store.FindUserAsync(userId);
        }

        /// <summary>
        /// 通过用户Id查询用户实例。
        /// </summary>
        /// <param name="userName">用户名称。</param>
        /// <returns>返回用户实例。</returns>
        public override Task<TUser> FindByNameAsync(string userName)
        {
            return _store.FindByNameAsync(userName);
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
            return $"{NormalizeKey(userName)}2O.l8{password}";
        }

        /// <summary>
        /// 验证密码。
        /// </summary>
        /// <param name="user">当前用户。</param>
        /// <param name="password">当前密码。</param>
        /// <returns>返回判断结果。</returns>
        public override Task<bool> CheckPasswordAsync(TUser user, string password)
        {
            password = PasswordSalt(user.NormalizedUserName, password);
            return base.CheckPasswordAsync(user, password);
        }

        private const string TwoFactorTokenName = "TwoFactor";

        /// <summary>
        /// 二次登陆验证判定。
        /// </summary>
        /// <param name="user">用户实例对象。</param>
        /// <param name="verificationCode">验证码。</param>
        /// <returns>返回判定结果。</returns>
        public virtual Task<bool> VerifyTwoFactorTokenAsync(TUser user, string verificationCode)
        {
            return VerifyTwoFactorTokenAsync(user, Options.Tokens.AuthenticatorTokenProvider, verificationCode);
        }

        /// <summary>
        /// 初始化类<see cref="UserManager{TUser, TUserClaim, TUserLogin, TUserToken}"/>。
        /// </summary>
        /// <param name="store">用户存储接口。</param>
        /// <param name="optionsAccessor"><see cref="T:Microsoft.AspNetCore.Identity.IdentityOptions" />实例对象。</param>
        /// <param name="passwordHasher">密码加密器接口。</param>
        /// <param name="userValidators">用户验证接口。</param>
        /// <param name="passwordValidators">密码验证接口。</param>
        /// <param name="keyNormalizer">唯一键格式化字符串。</param>
        /// <param name="errors">错误实例。</param>
        /// <param name="serviceProvider">服务提供者接口。</param>
        protected UserManager(IUserStore<TUser> store, IOptions<IdentityOptions> optionsAccessor, IPasswordHasher<TUser> passwordHasher, IEnumerable<IUserValidator<TUser>> userValidators, IEnumerable<IPasswordValidator<TUser>> passwordValidators, ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, IServiceProvider serviceProvider)
            : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, serviceProvider, serviceProvider.GetRequiredService<ILogger<UserManager<TUser>>>())
        {
            _serviceProvider = serviceProvider;
            _store = store as IUserStoreBase<TUser, TUserClaim, TUserLogin, TUserToken>;
            DbContext = store as IUserDbContext<TUser, TUserClaim, TUserLogin, TUserToken>;
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
            IUserManager<TUser, TRole>
        where TUser : UserBase, new()
        where TRole : RoleBase, new()
        where TUserClaim : UserClaimBase, new()
        where TUserRole : UserRoleBase, new()
        where TUserLogin : UserLoginBase, new()
        where TUserToken : UserTokenBase, new()
        where TRoleClaim : RoleClaimBase, new()
    {
        private readonly IUserStoreBase<TUser, TRole, TUserClaim, TUserRole, TUserLogin, TUserToken, TRoleClaim> _store;
        /// <summary>
        /// 数据库操作接口。
        /// </summary>
        protected new IUserRoleDbContext<TUser, TRole, TUserClaim, TUserRole, TUserLogin, TUserToken, TRoleClaim> DbContext { get; }

        /// <summary>
        /// 初始化类<see cref="UserManager{TUser, TRole, TUserClaim, TUserRole, TUserLogin, TUserToken, TRoleClaim}"/>。
        /// </summary>
        /// <param name="store">用户存储接口。</param>
        /// <param name="optionsAccessor"><see cref="T:Microsoft.AspNetCore.Identity.IdentityOptions" />实例对象。</param>
        /// <param name="passwordHasher">密码加密器接口。</param>
        /// <param name="userValidators">用户验证接口。</param>
        /// <param name="passwordValidators">密码验证接口。</param>
        /// <param name="keyNormalizer">唯一键格式化字符串。</param>
        /// <param name="errors">错误实例。</param>
        /// <param name="serviceProvider">服务提供者接口。</param>
        protected UserManager(IUserStore<TUser> store, IOptions<IdentityOptions> optionsAccessor, IPasswordHasher<TUser> passwordHasher, IEnumerable<IUserValidator<TUser>> userValidators, IEnumerable<IPasswordValidator<TUser>> passwordValidators, ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, IServiceProvider serviceProvider)
            : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, serviceProvider)
        {
            _store = store as IUserStoreBase<TUser, TRole, TUserClaim, TUserRole, TUserLogin, TUserToken, TRoleClaim>;
            DbContext = store as IUserRoleDbContext<TUser, TRole, TUserClaim, TUserRole, TUserLogin, TUserToken, TRoleClaim>;
        }

        /// <summary>
        /// 获取角色Id。
        /// </summary>
        /// <param name="userId">用户Id。</param>
        /// <returns>返回角色Id集合。</returns>
        public virtual int[] GetRoleIds(int userId)
        {
            var roles = GetRoles(userId);
            return roles.Select(x => x.RoleId).ToArray();
        }

        /// <summary>
        /// 获取角色Id。
        /// </summary>
        /// <param name="userId">用户Id。</param>
        /// <returns>返回角色Id集合。</returns>
        public virtual async Task<int[]> GetRoleIdsAsync(int userId)
        {
            var roles = await GetRolesAsync(userId);
            return roles.Select(x => x.RoleId).ToArray();
        }

        /// <summary>
        /// 获取角色列表。
        /// </summary>
        /// <param name="userId">用户Id。</param>
        /// <returns>返回角色Id集合。</returns>
        public virtual IEnumerable<TRole> GetRoles(int userId)
        {
            return _store.GetRoles(userId);
        }

        /// <summary>
        /// 获取角色列表。
        /// </summary>
        /// <param name="userId">用户Id。</param>
        /// <returns>返回角色Id集合。</returns>
        public virtual Task<IEnumerable<TRole>> GetRolesAsync(int userId)
        {
            return _store.GetRolesAsync(userId);
        }

        /// <summary>
        /// 获取最高级角色实例。
        /// </summary>
        /// <param name="userId">用户Id。</param>
        /// <returns>返回用户实例对象。</returns>
        public virtual TRole GetMaxRole(int userId)
        {
            return GetRoles(userId).OrderByDescending(x => x.RoleLevel).FirstOrDefault();
        }

        /// <summary>
        /// 获取最高级角色实例。
        /// </summary>
        /// <param name="userId">用户Id。</param>
        /// <returns>返回用户实例对象。</returns>
        public virtual async Task<TRole> GetMaxRoleAsync(int userId)
        {
            var roles = await GetRolesAsync(userId);
            return roles.OrderByDescending(x => x.RoleLevel).FirstOrDefault();
        }

        /// <summary>
        /// 将用户添加到角色中。
        /// </summary>
        /// <param name="userId">用户Id。</param>
        /// <param name="roleIds">角色Id列表。</param>
        /// <returns>返回添加结果。</returns>
        public virtual bool AddUserToRoles(int userId, int[] roleIds)
        {
            return _store.AddUserToRoles(userId, roleIds);
        }

        /// <summary>
        /// 将用户添加到角色中。
        /// </summary>
        /// <param name="userId">用户Id。</param>
        /// <param name="roleIds">角色Id列表。</param>
        /// <returns>返回添加结果。</returns>
        public virtual Task<bool> AddUserToRolesAsync(int userId, int[] roleIds)
        {
            return _store.AddUserToRolesAsync(userId, roleIds);
        }

        /// <summary>
        /// 设置用户角色。
        /// </summary>
        /// <param name="userId">用户Id。</param>
        /// <param name="roleIds">角色Id列表。</param>
        /// <returns>返回添加结果。</returns>
        public virtual bool SetUserToRoles(int userId, int[] roleIds)
        {
            return _store.SetUserToRoles(userId, roleIds);
        }

        /// <summary>
        /// 设置用户角色。
        /// </summary>
        /// <param name="userId">用户Id。</param>
        /// <param name="roleIds">角色Id列表。</param>
        /// <returns>返回设置结果。</returns>
        public virtual Task<bool> SetUserToRolesAsync(int userId, int[] roleIds)
        {
            return _store.SetUserToRolesAsync(userId, roleIds);
        }

        /// <summary>
        /// 添加所有者账号。
        /// </summary>
        /// <param name="userName">用户名。</param>
        /// <param name="loginName">登录名称。</param>
        /// <param name="password">密码。</param>
        /// <param name="init">实例化用户方法。</param>
        /// <returns>返回添加结果。</returns>
        public virtual async Task<bool> CreateOwnerAsync(string userName, string loginName, string password, Action<TUser> init = null)
        {
            var user = Activator.CreateInstance(typeof(TUser)) as TUser;
            user.UserName = userName;
            user.NormalizedUserName = loginName;
            user.PasswordHash = password;
            user.EmailConfirmed = true;
            user.PhoneNumberConfirmed = true;
            user.CreatedIP = "127.0.0.1";
            user.CreatedDate = DateTimeOffset.Now;
            user.RoleId = 1;
            user.RoleName = DefaultRole.Owner.Name;
            user.NormalizedUserName = NormalizeKey(user.NormalizedUserName);
            user.PasswordHash = HashPassword(user);
            return await _store.UserContext.BeginTransactionAsync(async db =>
            {
                var rdb = db.As<TRole>();
                var owner = DefaultRole.Owner.As<TRole>();
                var member = DefaultRole.Member.As<TRole>();
                await rdb.CreateAsync(owner);
                await rdb.CreateAsync(member);
                await db.CreateAsync(user);
                var urdb = db.As<TUserRole>();
                await urdb.CreateAsync(new TUserRole { UserId = user.UserId, RoleId = owner.RoleId });
                await urdb.CreateAsync(new TUserRole { UserId = user.UserId, RoleId = member.RoleId });
                return true;
            });
        }
    }
}