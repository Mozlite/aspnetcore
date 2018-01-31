using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace Mozlite.Extensions.Security.Stores
{
    /// <summary>
    /// 用户角色存储基类。
    /// </summary>
    /// <typeparam name="TRole">用户角色类型。</typeparam>
    /// <typeparam name="TUserRole">用户角色关联类。</typeparam>
    /// <typeparam name="TRoleClaim">角色声明类型。</typeparam>
    public abstract class RoleStoreExBase<TRole, TUserRole, TRoleClaim> :
        RoleStoreBase<TRole, TUserRole, TRoleClaim>
        where TRole : RoleExBase
        where TUserRole : IUserRoleEx
        where TRoleClaim : RoleClaimExBase, new()
    {
        private readonly ISiteContextAccessorBase _siteContextAccessor;
        /// <summary>
        /// 当前网站上下文接口。
        /// </summary>
        protected SiteContextBase Site => _siteContextAccessor.SiteContext;
        
        /// <summary>
        /// 通过角色和声明实例转换为声明实例。
        /// </summary>
        /// <param name="role">当前角色实例。</param>
        /// <param name="claim">声明实例。</param>
        /// <returns>返回<see cref="TRoleClaim"/>实例对象。</returns>
        protected override TRoleClaim CreateRoleClaim(TRole role, Claim claim)
            => new TRoleClaim { RoleId = role.RoleId, ClaimType = claim.Type, ClaimValue = claim.Value, SiteId = Site.SiteId };

        /// <summary>
        /// 初始化类<see cref="RoleStoreExBase{TRole,TUserRole,TRoleClaim}"/>。
        /// </summary>
        /// <param name="describer">错误描述<see cref="IdentityErrorDescriber"/>实例。</param>
        /// <param name="siteContextAccessor">网站上下文访问器接口。</param>
        protected RoleStoreExBase(IdentityErrorDescriber describer, ISiteContextAccessorBase siteContextAccessor) : base(describer)
        {
            _siteContextAccessor = siteContextAccessor;
        }
    }
}