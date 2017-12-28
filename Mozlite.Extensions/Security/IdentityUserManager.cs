using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Mozlite.Data;

namespace Mozlite.Extensions.Security
{
    /// <summary>
    /// 用户管理实现类基类。
    /// </summary>
    /// <typeparam name="TUser">用户类型。</typeparam>
    public abstract class IdentityUserManager<TUser> : IIdentityUserManager<TUser> where TUser : IdentityUser, new()
    {
        private readonly UserManager<TUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        /// <summary>
        /// 数据库操作实例。
        /// </summary>
        protected IRepository<TUser> Repository { get; }

        /// <summary>
        /// 初始化类<see cref="IdentityUserManager{TUser}"/>。
        /// </summary>
        /// <param name="userManager">用户管理实例。</param>
        /// <param name="repository">数据库操作接口。</param>
        /// <param name="httpContextAccessor">HTTP上下文访问接口。</param>
        protected IdentityUserManager(UserManager<TUser> userManager, IRepository<TUser> repository, IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            Repository = repository;
        }

        /// <summary>
        /// 异步修改密码。
        /// </summary>
        /// <param name="user">当前用户实例。</param>
        /// <param name="currentPassword">当前密码。</param>
        /// <param name="newPassword">新密码。</param>
        /// <returns>返回修改结果。</returns>
        public Task<IdentityResult> ChangePasswordAsync(TUser user, string currentPassword, string newPassword)
        {
            return _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
        }

        /// <summary>
        /// 重置密码。
        /// </summary>
        /// <param name="user">用户实例。</param>
        /// <param name="token">用户验证标志。</param>
        /// <param name="newPassword">用户新密码。</param>
        /// <returns>返回重置结果。</returns>
        public Task<IdentityResult> ResetPasswordAsync(TUser user, string token, string newPassword)
        {
            return _userManager.ResetPasswordAsync(user, token, newPassword);
        }

        /// <summary>
        /// 重置密码。
        /// </summary>
        /// <param name="user">用户实例。</param>
        /// <param name="newPassword">用户新密码。</param>
        /// <returns>返回重置结果。</returns>
        public async Task<IdentityResult> ResetPasswordAsync(TUser user, string newPassword)
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            return await ResetPasswordAsync(user, token, newPassword);
        }

        /// <summary>
        /// 通过电子邮件查找用户。
        /// </summary>
        /// <param name="email">电子邮件。</param>
        /// <returns>返回当前用户实例。</returns>
        public Task<TUser> FindByEmailAsync(string email)
        {
            return _userManager.FindByEmailAsync(email);
        }

        /// <summary>
        /// 通过Id查找用户。
        /// </summary>
        /// <param name="userId">用户Id。</param>
        /// <returns>返回当前用户实例。</returns>
        public Task<TUser> FindByIdAsync(int userId)
        {
            return Repository.FindAsync(u => u.UserId == userId);
        }

        /// <summary>
        /// 通过Id查找用户。
        /// </summary>
        /// <param name="userId">用户Id。</param>
        /// <returns>返回当前用户实例。</returns>
        public Task<TUser> FindByIdAsync(string userId)
        {
            return _userManager.FindByIdAsync(userId);
        }

        /// <summary>
        /// 通过登入类型查找用户。
        /// </summary>
        /// <param name="loginProvider">登入类型。</param>
        /// <param name="providerKey">登入唯一键。</param>
        /// <returns>返回当前用户实例。</returns>
        public Task<TUser> FindByLoginAsync(string loginProvider, string providerKey)
        {
            return _userManager.FindByLoginAsync(loginProvider, providerKey);
        }

        /// <summary>
        /// 登入成功后写入登入IP和登入时间。
        /// </summary>
        /// <param name="userId">用户Id。</param>
        /// <returns>返回设置是否成功。</returns>
        public bool SignInSuccess(int userId)
        {
            var userAddress = _httpContextAccessor.HttpContext.GetUserAddress();
            return Repository.Update(u => u.UserId == userId,
                new { LoginIP = userAddress, LastLoginDate = DateTimeOffset.Now });
        }

        /// <summary>
        /// 登入成功后写入登入IP和登入时间。
        /// </summary>
        /// <param name="userId">用户Id。</param>
        /// <returns>返回设置是否成功。</returns>
        public Task<bool> SignInSuccessAsync(int userId)
        {
            var userAddress = _httpContextAccessor.HttpContext.GetUserAddress();
            return Repository.UpdateAsync(u => u.UserId == userId,
                new { LoginIP = userAddress, LastLoginDate = DateTimeOffset.Now });
        }

        /// <summary>
        /// 锁定用户。
        /// </summary>
        /// <param name="userIds">用户Id集合。</param>
        /// <param name="lockoutEnd">锁定过期时间。</param>
        /// <returns>返回锁定结果。</returns>
        public bool LockoutUsers(int[] userIds, DateTimeOffset? lockoutEnd)
        {
            return Repository.Update(user => user.LockoutEnabled && user.UserId.Included(userIds), new { lockoutEnd });
        }

        /// <summary>
        /// 删除用户。
        /// </summary>
        /// <param name="userId">用户Id。</param>
        /// <returns>返回删除结果。</returns>
        public bool DeleteUser(int userId)
        {
            return Repository.Delete(userId);
        }

        /// <summary>
        /// 删除用户。
        /// </summary>
        /// <param name="userIds">用户Id集合。</param>
        /// <returns>返回删除结果。</returns>
        public bool DeleteUsers(int[] userIds)
        {
            return Repository.Delete(u => u.UserId.Included(userIds));
        }

        /// <summary>
        /// 锁定用户。
        /// </summary>
        /// <param name="userIds">用户Id集合。</param>
        /// <param name="lockoutEnd">锁定过期时间。</param>
        /// <returns>返回锁定结果。</returns>
        public Task<bool> LockoutUsersAsync(int[] userIds, DateTimeOffset? lockoutEnd)
        {
            return Repository.UpdateAsync(user => user.LockoutEnabled && user.UserId.Included(userIds), new { lockoutEnd });
        }

        /// <summary>
        /// 删除用户。
        /// </summary>
        /// <param name="userId">用户Id。</param>
        /// <returns>返回删除结果。</returns>
        public Task<bool> DeleteUserAsync(int userId)
        {
            return Repository.DeleteAsync(userId);
        }

        /// <summary>
        /// 删除用户。
        /// </summary>
        /// <param name="userIds">用户Id集合。</param>
        /// <returns>返回删除结果。</returns>
        public Task<bool> DeleteUsersAsync(int[] userIds)
        {
            return Repository.DeleteAsync(u => u.UserId.Included(userIds));
        }

        /// <summary>
        /// 添加用户登入实例。
        /// </summary>
        /// <param name="user">当前用户。</param>
        /// <param name="login">登入信息。</param>
        /// <returns>返回操作结果。</returns>
        public Task<IdentityResult> AddLoginAsync(TUser user, UserLoginInfo login)
        {
            return _userManager.AddLoginAsync(user, login);
        }

        /// <summary>
        /// 确认邮件地址。
        /// </summary>
        /// <param name="user">当前用户。</param>
        /// <param name="token">确认邮件生成的字符串。</param>
        /// <returns>返回确认结果。</returns>
        public Task<IdentityResult> ConfirmEmailAsync(TUser user, string token)
        {
            return _userManager.ConfirmEmailAsync(user, token);
        }

        /// <summary>
        /// 判断当前用户的邮件是否已经确认。
        /// </summary>
        /// <param name="user">当前用户。</param>
        /// <returns>返回判断结果。</returns>
        public Task<bool> IsEmailConfirmedAsync(TUser user)
        {
            return _userManager.IsEmailConfirmedAsync(user);
        }

        /// <summary>
        /// 获取验证提供者名称列表。
        /// </summary>
        /// <param name="user">当前用户。</param>
        /// <returns>返回验证提供者名称列表。</returns>
        public Task<IList<string>> GetValidTwoFactorProvidersAsync(TUser user)
        {
            return _userManager.GetValidTwoFactorProvidersAsync(user);
        }

        /// <summary>
        /// 生成验证标识字符串。
        /// </summary>
        /// <param name="user">当前用户。</param>
        /// <param name="tokenProvider">验证提供者。</param>
        /// <returns>返回生产的代码字符串。</returns>
        public Task<string> GenerateTwoFactorTokenAsync(TUser user, string tokenProvider)
        {
            return _userManager.GenerateTwoFactorTokenAsync(user, tokenProvider);
        }

        /// <summary>
        /// 获取当前用户的电子邮件地址。
        /// </summary>
        /// <param name="user">当前用户。</param>
        /// <returns>当前用户的电子邮件地址。</returns>
        public Task<string> GetEmailAsync(TUser user)
        {
            return _userManager.GetEmailAsync(user);
        }

        /// <summary>
        /// 获取电话号码。
        /// </summary>
        /// <param name="user">当前用户。</param>
        /// <returns>电话号码。</returns>
        public Task<string> GetPhoneNumberAsync(TUser user)
        {
            return _userManager.GetPhoneNumberAsync(user);
        }

        private static readonly TUser _anonymous = new TUser { UserName = IdentitySettings.Anonymous };

        /// <summary>
        /// 获取当前登入用户。
        /// </summary>
        /// <returns>返回当前登入用户实例。</returns>
        public async Task<TUser> GetUserAsync()
        {
            return (await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User)) ?? _anonymous;
        }

        /// <inheritdoc />
        public TUser GetUser()
        {
            var userId = _userManager.GetUserId(_httpContextAccessor.HttpContext.User);
            if (int.TryParse(userId, out var id))
                return Repository.Find(u => u.UserId == id) ?? _anonymous;
            return _anonymous;
        }

        /// <summary>
        /// 生成一个电子邮件验证码。
        /// </summary>
        /// <param name="user">当前用户实例。</param>
        /// <returns>返回验证码。</returns>
        public Task<string> GenerateEmailConfirmationTokenAsync(TUser user)
        {
            return _userManager.GenerateEmailConfirmationTokenAsync(user);
        }

        /// <summary>
        /// 分页检索用户。
        /// </summary>
        /// <typeparam name="TQuery">用户查询类型。</typeparam>
        /// <param name="query">用户查询实例对象。</param>
        /// <returns>返回分页实例。</returns>
        public TQuery Load<TQuery>(TQuery query) where TQuery : QueryBase<TUser>
        {
            return Repository.Load(query);
        }

        /// <summary>
        /// 修改昵称。
        /// </summary>
        /// <param name="id">用户Id。</param>
        /// <param name="nickName">昵称。</param>
        /// <returns>返回修改结果。</returns>
        public async Task<DataResult> ChangeNickNameAsync(int id, string nickName)
        {
            if (await Repository.AnyAsync(x => x.UserId != id && x.NickName == nickName))
                return DataAction.Duplicate;
            return DataResult.FromResult(await Repository.UpdateAsync(x => x.UserId == id, new { nickName, updateddate = DateTimeOffset.Now }), DataAction.Updated);
        }

        /// <summary>
        /// 通过名称查找用户。
        /// </summary>
        /// <param name="userName">用户名称。</param>
        /// <returns>返回当前用户实例。</returns>
        public Task<TUser> FindByNameAsync(string userName)
        {
            return _userManager.FindByNameAsync(userName);
        }

        /// <summary>
        /// 新建用户，不包含密码。
        /// </summary>
        /// <param name="user">当前用户实例。</param>
        /// <param name="roleName">用户组名称。</param>
        /// <returns>返回新建结果。</returns>
        public async Task<IdentityResult> CreateAsync(TUser user, string roleName)
        {
            SetUser(user);
            var result = await _userManager.CreateAsync(user);
            if (result.Succeeded)
            {
                return await _userManager.AddToRoleAsync(user, roleName);
            }
            return result;
        }

        /// <summary>
        /// 新建用户。
        /// </summary>
        /// <param name="user">当前用户实例。</param>
        /// <param name="password">用户密码。</param>
        /// <param name="roleName">用户组名称。</param>
        /// <returns>返回新建结果。</returns>
        public async Task<IdentityResult> CreateAsync(TUser user, string password, string roleName)
        {
            SetUser(user);
            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                return await _userManager.AddToRoleAsync(user, roleName);
            }
            return result;
        }

        /// <summary>
        /// 新建用户。
        /// </summary>
        /// <param name="user">当前用户实例。</param>
        /// <param name="password">用户密码。</param>
        /// <param name="roles">用户组名称。</param>
        /// <returns>返回新建结果。</returns>
        public async Task<IdentityResult> CreateAsync(TUser user, string password, IEnumerable<string> roles)
        {
            SetUser(user);
            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                return await _userManager.AddToRolesAsync(user, roles);
            }
            return result;
        }

        private void SetUser(TUser user)
        {
            user.CreatedDate = DateTimeOffset.Now;
            user.UpdatedDate = DateTimeOffset.Now;
            user.CreatedIP = _httpContextAccessor.HttpContext.GetUserAddress();
        }

        /// <summary>
        /// 获取当前用户声明的所有用户列表。
        /// </summary>
        /// <param name="claim">当前用户声明实例。</param>
        /// <returns>返回当前用户声明的所有用户列表。</returns>
        public Task<IList<TUser>> GetUsersForClaimAsync(Claim claim)
        {
            return _userManager.GetUsersForClaimAsync(claim);
        }

        /// <summary>
        /// 获取当前用户组的所有用户列表。
        /// </summary>
        /// <param name="roleName">当前用户组名称。</param>
        /// <returns>返回当前用户组的所有用户列表。</returns>
        public Task<IList<TUser>> GetUsersInRoleAsync(string roleName)
        {
            return _userManager.GetUsersInRoleAsync(roleName);
        }

        /// <summary>
        /// 判断名称是否已经存在。
        /// </summary>
        /// <param name="id">用户Id。</param>
        /// <param name="name">用户名称。</param>
        /// <returns>返回判断结果。</returns>
        public Task<bool> IsDuplicatedNameAsync(int id, string name)
        {
            return Repository.AnyAsync(x => x.UserId != id && (x.NickName == name || x.UserName == name));
        }

        /// <summary>
        /// 更新用户的相关列。
        /// </summary>
        /// <param name="id">用户Id。</param>
        /// <param name="user">用户实例列。</param>
        /// <returns>返回保存结果。</returns>
        public async Task<DataResult> UpdateAsync(int id, object user)
        {
            return DataResult.FromResult(await Repository.UpdateAsync(x => x.UserId == id, user), DataAction.Updated);
        }

        /// <summary>
        /// 获取用户Id。
        /// </summary>
        /// <param name="userName">用户名称。</param>
        /// <returns>返回用户Id。</returns>
        public int GetUserId(string userName)
        {
            userName = userName.ToUpper();
            return Repository.AsQueryable().Where(x => x.NormalizedUserName == userName).Select(x => x.UserId)
                .SingleOrDefault(x => x.GetInt32(0));
        }

        /// <summary>
        /// 获取用户Id。
        /// </summary>
        /// <param name="userName">用户名称。</param>
        /// <returns>返回用户Id。</returns>
        public Task<int> GetUserIdAsync(string userName)
        {
            userName = userName.ToUpper();
            return Repository.AsQueryable().Where(x => x.NormalizedUserName == userName).Select(x => x.UserId)
                .SingleOrDefaultAsync(x => x.GetInt32(0));
        }
    }
}