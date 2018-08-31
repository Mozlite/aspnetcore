using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Mozlite.Extensions.Security.Stores;

namespace Mozlite.Extensions.Security
{
    /// <summary>
    /// 用户管理接口。
    /// </summary>
    /// <typeparam name="TUser">用户类型。</typeparam>
    /// <typeparam name="TUserClaim">用户声明类型。</typeparam>
    /// <typeparam name="TUserLogin">用户登陆类型。</typeparam>
    /// <typeparam name="TUserToken">用户标识类型。</typeparam>
    public interface IUserManager<TUser, TUserClaim, TUserLogin, TUserToken>
        where TUser : UserBase
        where TUserClaim : UserClaimBase, new()
        where TUserLogin : UserLoginBase, new()
        where TUserToken : UserTokenBase, new()
    {
        /// <summary>
        /// 通过用户Id查询用户实例。
        /// </summary>
        /// <param name="userId">用户Id。</param>
        /// <returns>返回用户实例。</returns>
        Task<TUser> FindByIdAsync(int userId);

        /// <summary>
        /// 通过用户Id查询用户实例。
        /// </summary>
        /// <param name="userName">用户名称。</param>
        /// <returns>返回用户实例。</returns>
        Task<TUser> FindByNameAsync(string userName);

        /// <summary>
        /// 正常实例化键。
        /// </summary>
        /// <param name="key">原有键值。</param>
        /// <returns>返回正常化后的字符串。</returns>
        string NormalizeKey(string key);

        /// <summary>
        /// 获取当前用户。
        /// </summary>
        /// <returns>返回当前用户实例。</returns>
        TUser GetUser();

        /// <summary>
        /// 获取当前用户。
        /// </summary>
        /// <returns>返回当前用户实例。</returns>
        Task<TUser> GetUserAsync();

        /// <summary>
        /// 判断当前用户是否已经登陆。
        /// </summary>
        /// <returns>返回判断结果。</returns>
        bool IsSignedIn();

        /// <summary>
        /// 加密字符串。
        /// </summary>
        /// <param name="user">用户实例。</param>
        /// <returns>返回加密后得字符串。</returns>
        string HashPassword(TUser user);

        /// <summary>
        /// 加密字符串。
        /// </summary>
        /// <param name="password">原始密码。</param>
        /// <returns>返回加密后得字符串。</returns>
        string HashPassword(string password);

        /// <summary>
        /// 验证密码。
        /// </summary>
        /// <param name="hashedPassword">原始已经加密得密码。</param>
        /// <param name="providedPassword">要验证得原始密码。</param>
        /// <returns>验证结果。</returns>
        bool VerifyHashedPassword(string hashedPassword, string providedPassword);

        /// <summary>
        /// 拼接密码。
        /// </summary>
        /// <param name="userName">当前用户名。</param>
        /// <param name="password">密码。</param>
        /// <returns>返回拼接后得字符串。</returns>
        string PasswordSalt(string userName, string password);

        /// <summary>
        /// 密码登陆。
        /// </summary>
        /// <param name="userName">用户名。</param>
        /// <param name="password">密码。</param>
        /// <param name="isRemembered">是否记住登陆状态。</param>
        /// <returns>返回登陆结果。</returns>
        Task<SignInResult> PasswordSignInAsync(string userName, string password, bool isRemembered);

        /// <summary>
        /// 密码登陆。
        /// </summary>
        /// <param name="userName">用户名。</param>
        /// <param name="password">密码。</param>
        /// <param name="isRemembered">是否记住登陆状态。</param>
        /// <param name="success">成功后执行的方法。</param>
        /// <returns>返回登陆结果。</returns>
        Task<SignInResult> PasswordSignInAsync(string userName, string password, bool isRemembered, Func<TUser, Task> success);

        /// <summary>
        /// 修改密码。
        /// </summary>
        /// <param name="user">用户实例对象。</param>
        /// <param name="password">原始密码。</param>
        /// <param name="newPassword">新密码。</param>
        /// <returns>返回修改结果。</returns>
        Task<IdentityResult> ChangePasswordAsync(TUser user, string password, string newPassword);

        /// <summary>
        /// 添加密码。
        /// </summary>
        /// <param name="user">用户实例对象。</param>
        /// <param name="password">密码。</param>
        /// <returns>返回添加结果。</returns>
        Task<IdentityResult> AddPasswordAsync(TUser user, string password);

        /// <summary>
        /// 重置密码。
        /// </summary>
        /// <param name="user">用户实例对象。</param>
        /// <param name="newPassword">新密码。</param>
        /// <returns>返回修改结果。</returns>
        Task<IdentityResult> ResetPasswordAsync(TUser user, string newPassword);

        /// <summary>
        /// 重置密码。
        /// </summary>
        /// <param name="user">用户实例对象。</param>
        /// <param name="token">修改密码标识。</param>
        /// <param name="newPassword">新密码。</param>
        /// <returns>返回修改结果。</returns>
        Task<IdentityResult> ResetPasswordAsync(TUser user, string token, string newPassword);

        /// <summary>
        /// 生成一个密码重置的标识。
        /// </summary>
        /// <param name="user">用户是对象。</param>
        /// <returns>返回密码重置的标识。</returns>
        Task<string> GeneratePasswordResetTokenAsync(TUser user);

        /// <summary>
        /// 通过用户ID更新用户列。
        /// </summary>
        /// <param name="userId">用户ID。</param>
        /// <param name="fields">用户列。</param>
        /// <returns>返回更新结果。</returns>
        bool Update(int userId, object fields);

        /// <summary>
        /// 通过用户ID更新用户列。
        /// </summary>
        /// <param name="userId">用户ID。</param>
        /// <param name="fields">用户列。</param>
        /// <returns>返回更新结果。</returns>
        Task<bool> UpdateAsync(int userId, object fields);

        /// <summary>
        /// 分页加载用户。
        /// </summary>
        /// <typeparam name="TQuery">查询类型。</typeparam>
        /// <param name="query">查询实例。</param>
        /// <returns>返回查询分页实例。</returns>
        TQuery Load<TQuery>(TQuery query) where TQuery : QueryBase<TUser>;

        /// <summary>
        /// 分页加载用户。
        /// </summary>
        /// <typeparam name="TQuery">查询类型。</typeparam>
        /// <param name="query">查询实例。</param>
        /// <returns>返回查询分页实例。</returns>
        Task<TQuery> LoadAsync<TQuery>(TQuery query) where TQuery : QueryBase<TUser>;

        /// <summary>
        /// 新建用户实例（不会对密码进行加密）。
        /// </summary>
        /// <param name="user">用户实例对象。</param>
        /// <returns>返回添加用户结果。</returns>
        Task<IdentityResult> CreateAsync(TUser user);

        /// <summary>
        /// 新建用户实例。
        /// </summary>
        /// <param name="user">用户实例对象。</param>
        /// <param name="password">未加密时的密码。</param>
        /// <returns>返回添加用户结果。</returns>
        Task<IdentityResult> CreateAsync(TUser user, string password);

        /// <summary>
        /// 新建用户实例。
        /// </summary>
        /// <param name="user">用户实例对象。</param>
        /// <returns>返回添加用户结果。</returns>
        Task<IdentityResult> DeleteAsync(TUser user);

        /// <summary>
        /// 判断当前用户名称是否存在。
        /// </summary>
        /// <param name="user">用户实例。</param>
        /// <returns>返回判断结果。</returns>
        IdentityResult IsDuplicated(TUser user);

        /// <summary>
        /// 判断当前用户名称是否存在。
        /// </summary>
        /// <param name="user">用户实例。</param>
        /// <returns>返回判断结果。</returns>
        Task<IdentityResult> IsDuplicatedAsync(TUser user);

        /// <summary>
        /// 判断当前用户名称是否存在。
        /// </summary>
        /// <param name="userId">用户实例。</param>
        /// <param name="userName">用户名称。</param>
        /// <returns>返回判断结果。</returns>
        IdentityResult IsDuplicated(int userId, string userName);

        /// <summary>
        /// 判断当前用户名称是否存在。
        /// </summary>
        /// <param name="userId">用户实例。</param>
        /// <param name="userName">用户名称。</param>
        /// <returns>返回判断结果。</returns>
        Task<IdentityResult> IsDuplicatedAsync(int userId, string userName);

        /// <summary>
        /// 锁定或者解锁用户。
        /// </summary>
        /// <param name="userId">用户Id。</param>
        /// <param name="lockoutEnd">锁定截至日期。</param>
        /// <returns>返回执行结果。</returns>
        bool Lockout(int userId, DateTimeOffset? lockoutEnd = null);

        /// <summary>
        /// 锁定或者解锁用户。
        /// </summary>
        /// <param name="userId">用户Id。</param>
        /// <param name="lockoutEnd">锁定截至日期。</param>
        /// <returns>返回执行结果。</returns>
        Task<bool> LockoutAsync(int userId, DateTimeOffset? lockoutEnd = null);
    }

    /// <summary>
    /// 用户管理接口。
    /// </summary>
    /// <typeparam name="TUser">用户类型。</typeparam>
    /// <typeparam name="TRole">用户组类型。</typeparam>
    /// <typeparam name="TUserClaim">用户声明类型。</typeparam>
    /// <typeparam name="TUserRole">用户用户组类型。</typeparam>
    /// <typeparam name="TUserLogin">用户登陆类型。</typeparam>
    /// <typeparam name="TUserToken">用户标识类型。</typeparam>
    /// <typeparam name="TRoleClaim">用户组声明类型。</typeparam>
    public interface IUserManager<TUser, TRole, TUserClaim, TUserRole, TUserLogin, TUserToken, TRoleClaim>
        : IUserManager<TUser, TUserClaim, TUserLogin, TUserToken>
        where TUser : UserBase
        where TUserClaim : UserClaimBase, new()
        where TUserLogin : UserLoginBase, new()
        where TUserToken : UserTokenBase, new()
    {
        /// <summary>
        /// 获取用户的所有用户组。
        /// </summary>
        /// <param name="user">用户实例对象。</param>
        /// <returns>返回当前用户的所有用户组列表。</returns>
        Task<IList<string>> GetRolesAsync(TUser user);

        /// <summary>
        /// 获取用户组Id。
        /// </summary>
        /// <param name="userId">用户Id。</param>
        /// <returns>返回用户组Id集合。</returns>
        int[] GetRoleIds(int userId);

        /// <summary>
        /// 获取用户组Id。
        /// </summary>
        /// <param name="userId">用户Id。</param>
        /// <returns>返回用户组Id集合。</returns>
        Task<int[]> GetRoleIdsAsync(int userId);

        /// <summary>
        /// 获取用户组列表。
        /// </summary>
        /// <param name="userId">用户Id。</param>
        /// <returns>返回用户组Id集合。</returns>
        IEnumerable<TRole> GetRoles(int userId);

        /// <summary>
        /// 获取用户组列表。
        /// </summary>
        /// <param name="userId">用户Id。</param>
        /// <returns>返回用户组Id集合。</returns>
        Task<IEnumerable<TRole>> GetRolesAsync(int userId);

        /// <summary>
        /// 获取最高级用户组实例。
        /// </summary>
        /// <param name="userId">用户Id。</param>
        /// <returns>返回用户实例对象。</returns>
        TRole GetMaxRole(int userId);

        /// <summary>
        /// 获取最高级用户组实例。
        /// </summary>
        /// <param name="userId">用户Id。</param>
        /// <returns>返回用户实例对象。</returns>
        Task<TRole> GetMaxRoleAsync(int userId);

        /// <summary>
        /// 将用户添加到用户组中。
        /// </summary>
        /// <param name="userId">用户Id。</param>
        /// <param name="roleIds">用户组Id列表。</param>
        /// <returns>返回添加结果。</returns>
        bool AddUserToRoles(int userId, int[] roleIds);

        /// <summary>
        /// 将用户添加到用户组中。
        /// </summary>
        /// <param name="userId">用户Id。</param>
        /// <param name="roleIds">用户组Id列表。</param>
        /// <returns>返回添加结果。</returns>
        Task<bool> AddUserToRolesAsync(int userId, int[] roleIds);

        /// <summary>
        /// 设置用户用户组。
        /// </summary>
        /// <param name="userId">用户Id。</param>
        /// <param name="roleIds">用户组Id列表。</param>
        /// <returns>返回添加结果。</returns>
        bool SetUserToRoles(int userId, int[] roleIds);

        /// <summary>
        /// 设置用户用户组。
        /// </summary>
        /// <param name="userId">用户Id。</param>
        /// <param name="roleIds">用户组Id列表。</param>
        /// <returns>返回设置结果。</returns>
        Task<bool> SetUserToRolesAsync(int userId, int[] roleIds);
    }
}