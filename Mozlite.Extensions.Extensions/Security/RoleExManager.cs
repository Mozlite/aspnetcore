using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Mozlite.Extensions.Security.Stores;
using Mozlite.Extensions.Sites;

namespace Mozlite.Extensions.Security
{
    /// <summary>
    /// 角色管理实现类。
    /// </summary>
    /// <typeparam name="TRole">角色类型。</typeparam>
    /// <typeparam name="TUserRole">用户角色类型。</typeparam>
    /// <typeparam name="TRoleClaim">角色声明类型。</typeparam>
    public abstract class RoleExManager<TRole, TUserRole, TRoleClaim>
        : RoleManager<TRole, TUserRole, TRoleClaim>, IRoleExManager<TRole, TUserRole, TRoleClaim>
        where TRole : RoleExBase
        where TUserRole : IUserRole
        where TRoleClaim : RoleClaimBase, new()
    {
        private readonly ISiteContextAccessorBase _siteContextAccessor;

        /// <summary>
        /// 初始化类<see cref="RoleExManager{TRole, TUserRole, TRoleClaim}"/>。
        /// </summary>
        /// <param name="manager">系统用户角色管理实例。</param>
        /// <param name="store">角色存储接口。</param>
        /// <param name="siteContextAccessor">网站上下文访问器。</param>
        protected RoleExManager(RoleManager<TRole> manager, IRoleStore<TRole> store, ISiteContextAccessorBase siteContextAccessor) 
            : base(manager, store)
        {
            _siteContextAccessor = siteContextAccessor;
        }

        /// <summary>
        /// 当前网站实例。
        /// </summary>
        protected SiteContextBase Site => _siteContextAccessor.SiteContext;

        /// <summary>
        /// 获取所有角色。
        /// </summary>
        /// <param name="siteId">网站Id。</param>
        /// <returns>返回角色列表。</returns>
        public virtual async Task<IEnumerable<TRole>> LoadRolesAsync(int siteId)
        {
            if (siteId == 0)
                return await LoadRolesAsync();
            return await Store.RoleContext.FetchAsync();
        }

        /// <summary>
        /// 获取所有角色。
        /// </summary>
        /// <param name="siteId">网站Id。</param>
        /// <returns>返回角色列表。</returns>
        public virtual IEnumerable<TRole> LoadRoles(int siteId)
        {
            if (siteId == 0)
                return LoadRoles();
            return Store.RoleContext.Fetch();
        }
    }
}