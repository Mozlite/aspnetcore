using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Mozlite.Extensions.Security.Stores;
using Mozlite.Extensions.Sites;

namespace Mozlite.Extensions.Security
{
    /// <summary>
    /// 用户管理实现类。
    /// </summary>
    /// <typeparam name="TUser">用户类型。</typeparam>
    /// <typeparam name="TUserClaim">用户声明类型。</typeparam>
    /// <typeparam name="TUserLogin">用户登陆类型。</typeparam>
    /// <typeparam name="TUserToken">用户标识类型。</typeparam>
    public abstract class UserExManager<TUser, TUserClaim, TUserLogin, TUserToken>
        : UserManager<TUser, TUserClaim, TUserLogin, TUserToken>, IUserExManager<TUser, TUserClaim, TUserLogin, TUserToken>
        where TUser : UserExBase
        where TUserClaim : UserClaimBase, new()
        where TUserLogin : UserLoginBase, new()
        where TUserToken : UserTokenBase, new()
    {
        /// <summary>
        /// 初始化类<see cref="UserExManager{TUser, TUserClaim, TUserLogin, TUserToken}"/>。
        /// </summary>
        /// <param name="manager">系统管理实例。</param>
        /// <param name="signInManager"> 登陆管理实例。</param>
        /// <param name="store">用户存储实例。</param>
        /// <param name="httpContextAccessor">HTTP上下文访问接口。</param>
        protected UserExManager(UserManager<TUser> manager, SignInManager<TUser> signInManager, IUserStore<TUser> store, IHttpContextAccessor httpContextAccessor) : base(manager, signInManager, store, httpContextAccessor)
        {
            Site = httpContextAccessor.HttpContext.RequestServices.GetRequiredService<ISiteContextAccessorBase>()
                .SiteContext;
        }

        /// <summary>
        /// 当前网站实例。
        /// </summary>
        protected SiteContextBase Site { get; }
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
    public abstract class UserExManager<TUser, TRole, TUserClaim, TUserRole, TUserLogin, TUserToken, TRoleClaim>
        : UserManager<TUser, TRole, TUserClaim, TUserRole, TUserLogin, TUserToken, TRoleClaim>,
            IUserExManager<TUser, TRole, TUserClaim, TUserRole, TUserLogin, TUserToken, TRoleClaim>
        where TUser : UserExBase
        where TRole : RoleExBase
        where TUserClaim : UserClaimBase, new()
        where TUserRole : UserRoleBase, new()
        where TUserLogin : UserLoginBase, new()
        where TUserToken : UserTokenBase, new()
        where TRoleClaim : RoleClaimBase, new()
    {
        /// <summary>
        /// 初始化类<see cref="UserExManager{TUser, TRole, TUserClaim, TUserRole, TUserLogin, TUserToken, TRoleClaim}"/>。
        /// </summary>
        /// <param name="manager">系统管理实例。</param>
        /// <param name="signInManager"> 登陆管理实例。</param>
        /// <param name="store">用户存储实例。</param>
        /// <param name="httpContextAccessor">HTTP上下文访问接口。</param>
        protected UserExManager(UserManager<TUser> manager, SignInManager<TUser> signInManager, IUserStore<TUser> store, IHttpContextAccessor httpContextAccessor) : base(manager, signInManager, store, httpContextAccessor)
        {
            Site = httpContextAccessor.HttpContext.RequestServices.GetRequiredService<ISiteContextAccessorBase>()
                .SiteContext;
        }

        /// <summary>
        /// 当前网站实例。
        /// </summary>
        protected SiteContextBase Site { get; }
    }
}