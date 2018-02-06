using System.Threading.Tasks;
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
        /// 正常实例化键。
        /// </summary>
        /// <param name="key">原有键值。</param>
        /// <returns>返回正常化后的字符串。</returns>
        string NormalizeKey(string key);
    }

    /// <summary>
    /// 用户管理接口。
    /// </summary>
    /// <typeparam name="TUser">用户类型。</typeparam>
    /// <typeparam name="TRole">角色类型。</typeparam>
    /// <typeparam name="TUserClaim">用户声明类型。</typeparam>
    /// <typeparam name="TUserRole">用户角色类型。</typeparam>
    /// <typeparam name="TUserLogin">用户登陆类型。</typeparam>
    /// <typeparam name="TUserToken">用户标识类型。</typeparam>
    /// <typeparam name="TRoleClaim">角色声明类型。</typeparam>
    public interface IUserManager<TUser, TRole, TUserClaim, TUserRole, TUserLogin, TUserToken, TRoleClaim>
        : IUserManager<TUser, TUserClaim, TUserLogin, TUserToken>
        where TUser : UserBase
        where TUserClaim : UserClaimBase, new()
        where TUserLogin : UserLoginBase, new()
        where TUserToken : UserTokenBase, new()
    {

    }
}