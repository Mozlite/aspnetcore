using Mozlite.Data;

namespace Mozlite.Extensions.Security.Stores
{
    /// <summary>
    /// 用户数据库操作上下文接口。
    /// </summary>
    /// <typeparam name="TUser">用户类型。</typeparam>
    /// <typeparam name="TUserClaim">用户声明类型。</typeparam>
    /// <typeparam name="TUserLogin">用户登陆类型。</typeparam>
    /// <typeparam name="TUserToken">用户标识类型。</typeparam>
    public interface IUserDbContext<TUser, TUserClaim, TUserLogin, TUserToken>
        where TUser : UserBase
        where TUserClaim : UserClaimBase, new()
        where TUserLogin : UserLoginBase, new()
        where TUserToken : UserTokenBase, new()
    {
        /// <summary>
        /// 用户数据库操作接口。
        /// </summary>
        IDbContext<TUser> UserContext { get; }

        /// <summary>
        /// 用户声明数据库操作接口。
        /// </summary>
        IDbContext<TUserClaim> UserClaimContext { get; }

        /// <summary>
        /// 用户登陆数据库操作接口。
        /// </summary>
        IDbContext<TUserLogin> UserLoginContext { get; }

        /// <summary>
        /// 用户标识数据库操作接口。
        /// </summary>
        IDbContext<TUserToken> UserTokenContext { get; }
    }
}