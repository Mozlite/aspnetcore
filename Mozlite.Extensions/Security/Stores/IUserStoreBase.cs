using Microsoft.Extensions.Caching.Memory;
using Mozlite.Data;

namespace Mozlite.Extensions.Security.Stores
{
    /// <summary>
    /// 用户存储类型。
    /// </summary>
    /// <typeparam name="TUser">用户类型。</typeparam>
    /// <typeparam name="TUserClaim">用户声明类型。</typeparam>
    /// <typeparam name="TUserLogin">用户登陆类型。</typeparam>
    /// <typeparam name="TUserToken">用户标识类型。</typeparam>
    public interface IUserStoreBase<TUser, TUserClaim, TUserLogin, TUserToken>
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
    /// <summary>
    /// 用户存储接口。
    /// </summary>
    /// <typeparam name="TUser">用户类型。</typeparam>
    /// <typeparam name="TRole">角色类型。</typeparam>
    /// <typeparam name="TUserClaim">用户声明类型。</typeparam>
    /// <typeparam name="TUserRole">用户角色类型。</typeparam>
    /// <typeparam name="TUserLogin">用户登陆类型。</typeparam>
    /// <typeparam name="TUserToken">用户标识类型。</typeparam>
    /// <typeparam name="TRoleClaim">角色声明类型。</typeparam>
    public interface IUserStoreBase<TUser, TRole, TUserClaim, TUserRole, TUserLogin, TUserToken, TRoleClaim> :
        IUserStoreBase<TUser, TUserClaim, TUserLogin, TUserToken>
        where TUser : UserBase
        where TRole : RoleBase
        where TUserClaim : UserClaimBase, new()
        where TUserRole : UserRoleBase, new()
        where TUserLogin : UserLoginBase, new()
        where TUserToken : UserTokenBase, new()
        where TRoleClaim : RoleClaimBase, new()
    {
        /// <summary>
        /// 缓存实例。
        /// </summary>
        IMemoryCache Cache { get; }

        /// <summary>
        /// 角色数据库操作接口。
        /// </summary>
        IDbContext<TRole> RoleContext { get; }

        /// <summary>
        /// 用户角色数据库操作接口。
        /// </summary>
        IDbContext<TUserRole> UserRoleContext { get; }

        /// <summary>
        /// 用户声明数据库操作接口。
        /// </summary>
        IDbContext<TRoleClaim> RoleClaimContext { get; }
    }
}