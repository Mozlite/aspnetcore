using Microsoft.AspNetCore.Identity;
using Mozlite.Extensions.Security.Stores;

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
        : IUserManager<TUser, TUserClaim, TUserLogin, TUserToken>
        where TUser : UserBase
        where TUserClaim : UserClaimBase, new()
        where TUserLogin : UserLoginBase, new()
        where TUserToken : UserTokenBase, new()
    {
        /// <summary>
        /// 系统管理实例。
        /// </summary>
        protected UserManager<TUser> Manager { get; }

        /// <summary>
        /// 登陆管理实例。
        /// </summary>
        protected SignInManager<TUser> SignInManager { get; }

        /// <summary>
        /// 用户存储实例。
        /// </summary>
        protected IUserStoreBase<TUser, TUserClaim, TUserLogin, TUserToken> Store { get; }

        /// <summary>
        /// 初始化类<see cref="UserManager{TUser}"/>。
        /// </summary>
        /// <param name="manager">系统管理实例。</param>
        /// <param name="signInManager"> 登陆管理实例。</param>
        /// <param name="store">用户存储实例。</param>
        protected UserManager(UserManager<TUser> manager, SignInManager<TUser> signInManager, IUserStore<TUser> store)
        {
            Manager = manager;
            SignInManager = signInManager;
            Store = store as IUserStoreBase<TUser, TUserClaim, TUserLogin, TUserToken>;
        }
        
        /// <summary>
        /// 正常实例化键。
        /// </summary>
        /// <param name="key">原有键值。</param>
        /// <returns>返回正常化后的字符串。</returns>
        public virtual string NormalizeKey(string key) => Manager.NormalizeKey(key);
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
            IUserManager<TUser, TRole, TUserClaim, TUserRole, TUserLogin, TUserToken, TRoleClaim>
        where TUser : UserBase
        where TRole : RoleBase
        where TUserClaim : UserClaimBase, new()
        where TUserRole : UserRoleBase, new()
        where TUserLogin : UserLoginBase, new()
        where TUserToken : UserTokenBase, new()
        where TRoleClaim : RoleClaimBase, new()
    {
        /// <summary>
        /// 初始化类<see cref="UserManager{TUser}"/>。
        /// </summary>
        /// <param name="manager">系统管理实例。</param>
        /// <param name="signInManager"> 登陆管理实例。</param>
        /// <param name="store">用户存储实例。</param>
        protected UserManager(UserManager<TUser> manager, SignInManager<TUser> signInManager, IUserStore<TUser> store)
            : base(manager, signInManager, store)
        {
        }

        private IUserStoreBase<TUser, TRole, TUserClaim, TUserRole, TUserLogin, TUserToken, TRoleClaim> _store;
        /// <summary>
        /// 用户存储实例。
        /// </summary>
        protected new IUserStoreBase<TUser, TRole, TUserClaim, TUserRole, TUserLogin, TUserToken, TRoleClaim> Store => _store ?? (_store =
                                                                                                                           base.Store as IUserStoreBase<TUser, TRole, TUserClaim, TUserRole, TUserLogin, TUserToken,
                                                                                                                               TRoleClaim>);
    }
}