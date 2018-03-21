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
    public interface IUserExManager<TUser, TUserClaim, TUserLogin, TUserToken>
        : IUserManager<TUser, TUserClaim, TUserLogin, TUserToken>
        where TUser : UserExBase
        where TUserClaim : UserClaimBase, new()
        where TUserLogin : UserLoginBase, new()
        where TUserToken : UserTokenBase, new()
    {

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
    public interface IUserExManager<TUser, TRole, TUserClaim, TUserRole, TUserLogin, TUserToken, TRoleClaim>
        : IUserManager<TUser, TRole, TUserClaim, TUserRole, TUserLogin, TUserToken, TRoleClaim>
        where TUser : UserExBase
        where TUserClaim : UserClaimBase, new()
        where TUserLogin : UserLoginBase, new()
        where TUserToken : UserTokenBase, new()
    {

    }
}