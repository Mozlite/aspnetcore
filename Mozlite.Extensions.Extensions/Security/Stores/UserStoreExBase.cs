using System.Collections.Generic;
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
    public abstract class UserStoreExBase<TUser, TUserClaim, TUserLogin, TUserToken> :
        UserStoreBase<TUser, TUserClaim, TUserLogin, TUserToken>
       where TUser : UserExBase
       where TUserClaim : UserClaimExBase, new()
       where TUserLogin : UserLoginExBase, new()
       where TUserToken : UserTokenExBase, new()
    {
        private readonly ISiteContextAccessorBase _siteContextAccessor;
        /// <summary>
        /// 当前网站实例。
        /// </summary>
        protected SiteContextBase Site => _siteContextAccessor.SiteContext;

        /// <summary>
        /// 初始化类<see cref="UserStoreExBase{TUser,TRole, TUserLogin, TUserToken}"/>。
        /// </summary>
        /// <param name="describer">错误描述<see cref="IdentityErrorDescriber"/>实例。</param>
        /// <param name="siteContextAccessor">网站上下文访问器接口。</param>
        protected UserStoreExBase(IdentityErrorDescriber describer,
            ISiteContextAccessorBase siteContextAccessor) : base(describer)
        {
            _siteContextAccessor = siteContextAccessor;
        }
    }

    /// <summary>
    /// 用户存储基类，包含用户角色的相关操作。
    /// </summary>
    /// <typeparam name="TUser">用户类型。</typeparam>
    /// <typeparam name="TRole">角色类型。</typeparam>
    /// <typeparam name="TUserClaim">用户声明类型。</typeparam>
    /// <typeparam name="TUserRole">用户角色类型。</typeparam>
    /// <typeparam name="TUserLogin">用户登陆类型。</typeparam>
    /// <typeparam name="TUserToken">用户标识类型。</typeparam>
    /// <typeparam name="TRoleClaim">角色声明类型。</typeparam>
    public abstract class UserStoreExBase<TUser, TRole, TUserClaim, TUserRole, TUserLogin, TUserToken, TRoleClaim> :
        UserStoreExBase<TUser, TUserClaim, TUserLogin, TUserToken>,
        IUserRoleStore<TUser>
        where TUser : UserExBase
        where TRole : RoleExBase
        where TUserClaim : UserClaimExBase, new()
        where TUserRole : UserRoleExBase, new()
        where TUserLogin : UserLoginExBase, new()
        where TUserToken : UserTokenExBase, new()
        where TRoleClaim : RoleClaimExBase, new()
    {
        /// <summary>
        /// 初始化类<see cref="UserStoreExBase{TUser, TRole, TUserClaim, TUserRole, TUserLogin, TUserToken, TRoleClaim}"/>。
        /// </summary>
        /// <param name="describer">错误描述<see cref="IdentityErrorDescriber"/>实例。</param>
        /// <param name="siteContextAccessor">网站上下文访问器接口。</param>
        protected UserStoreExBase(IdentityErrorDescriber describer,
            ISiteContextAccessorBase siteContextAccessor) : base(describer, siteContextAccessor)
        {
        }

        /// <summary>
        /// 实例化一个用户角色实例。
        /// </summary>
        /// <param name="user">当前用户实例。</param>
        /// <param name="role">角色实例。</param>
        /// <returns>返回用户角色实例。</returns>
        protected abstract TUserRole CreateUserRole(TUser user, TRole role);

        /// <summary>
        /// 检索当前角色的所有用户列表。
        /// </summary>
        /// <param name="normalizedRoleName">验证角色名称。</param>
        /// <param name="cancellationToken">取消标志。</param>
        /// <returns>
        /// 返回用户列表。 
        /// </returns>
        public abstract Task<IList<TUser>> GetUsersInRoleAsync(string normalizedRoleName, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// 添加用户角色。
        /// </summary>
        /// <param name="user">当前用户实例。</param>
        /// <param name="normalizedRoleName">验证角色名称。</param>
        /// <param name="cancellationToken">取消标志。</param>
        public abstract Task AddToRoleAsync(TUser user, string normalizedRoleName, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// 移除用户角色。
        /// </summary>
        /// <param name="user">用户实例对象。</param>
        /// <param name="normalizedRoleName">验证角色名称。</param>
        /// <param name="cancellationToken">取消标志。</param>
        public abstract Task RemoveFromRoleAsync(TUser user, string normalizedRoleName, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// 获取用户的所有角色。
        /// </summary>
        /// <param name="user">用户实例对象。</param>
        /// <param name="cancellationToken">取消标志。</param>
        /// <returns>返回当前用户的所有角色列表。</returns>
        public abstract Task<IList<string>> GetRolesAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// 判断用户是否包含当前角色。
        /// </summary>
        /// <param name="user">用户实例。</param>
        /// <param name="normalizedRoleName">验证角色名称。</param>
        /// <param name="cancellationToken">取消标志。</param>
        /// <returns>返回判断结果。</returns>
        public abstract Task<bool> IsInRoleAsync(TUser user, string normalizedRoleName, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// 通过验证角色名称获取角色实例。
        /// </summary>
        /// <param name="normalizedRoleName">验证角色名称。</param>
        /// <param name="cancellationToken">取消标志。</param>
        /// <returns>角色实例对象。</returns>
        protected abstract Task<TRole> FindRoleAsync(string normalizedRoleName, CancellationToken cancellationToken);

        /// <summary>
        /// 获取用户角色。
        /// </summary>
        /// <param name="userId">用户ID。</param>
        /// <param name="roleId">角色ID。</param>
        /// <param name="cancellationToken">取消标志。</param>
        /// <returns>用户角色实例对象。</returns>
        protected abstract Task<TUserRole> FindUserRoleAsync(int userId, int roleId, CancellationToken cancellationToken);
    }
}