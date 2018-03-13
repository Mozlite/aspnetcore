using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

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
        /// <param name="cancellationToken">取消标志。</param>
        /// <returns>返回更新结果。</returns>
        Task<bool> UpdateAsync(int userId, object fields, CancellationToken cancellationToken = default(CancellationToken));

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
        /// <param name="cancellationToken">取消标志。</param>
        /// <returns>返回查询分页实例。</returns>
        Task<TQuery> LoadAsync<TQuery>(TQuery query, CancellationToken cancellationToken = default(CancellationToken)) where TQuery : QueryBase<TUser>;
        
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
        /// <param name="cancellationToken">取消标志。</param>
        /// <returns>返回判断结果。</returns>
        Task<IdentityResult> IsDuplicatedAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken));
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
    }
}