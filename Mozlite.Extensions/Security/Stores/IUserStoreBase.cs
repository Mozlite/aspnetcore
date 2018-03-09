using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
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
        /// 错误描述实例对象。
        /// </summary>
        IdentityErrorDescriber ErrorDescriber { get; }

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

        /// <summary>
        /// 通过用户验证名称查询用户实例。
        /// </summary>
        /// <param name="normalizedUserName">当前验证名称。</param>
        /// <returns>
        /// 返回当前用户实例对象。
        /// </returns>
        TUser FindByName(string normalizedUserName);

        /// <summary>
        /// 通过Id获取用户实例。
        /// </summary>
        /// <param name="userId">用户Id。</param>
        /// <returns>返回当前用户实例。</returns>
        TUser FindUser(int userId);

        /// <summary>
        /// 通过用户验证名称查询用户实例。
        /// </summary>
        /// <param name="normalizedUserName">当前验证名称。</param>
        /// <param name="cancellationToken">取消标志。</param>
        /// <returns>
        /// 返回当前用户实例对象。
        /// </returns>
        Task<TUser> FindByNameAsync(string normalizedUserName,
           CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// 通过Id获取用户实例。
        /// </summary>
        /// <param name="userId">用户Id。</param>
        /// <param name="cancellationToken">取消标志。</param>
        /// <returns>返回当前用户实例。</returns>
        Task<TUser> FindUserAsync(int userId, CancellationToken cancellationToken = default(CancellationToken));
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