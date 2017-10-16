using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Mozlite.Data;

namespace Mozlite.Extensions.Security.Permissions
{
    /// <summary>
    /// 权限管理接口。
    /// </summary>
    public interface IPermissionManager : ISingletonService
    {
        /// <summary>
        /// 获取当前用户的权限。
        /// </summary>
        /// <param name="userId">用户Id。</param>
        /// <param name="permissioName">权限名称。</param>
        /// <returns>返回权限结果。</returns>
        Task<PermissionValue> GetPermissionAsync(int userId, string permissioName);

        /// <summary>
        /// 判断当前用户是否拥有<paramref name="permissionName"/>权限。
        /// </summary>
        /// <param name="permissionName">权限名称。</param>
        /// <returns>返回判断结果。</returns>
        Task<bool> IsAuthorized(string permissionName);
    }

    /// <summary>
    /// 权限管理实现类。
    /// </summary>
    public abstract class PermissionManager<TUserRole> : IPermissionManager
        where TUserRole : IdentityUserRole
    {
        private readonly IRepository<Permission> _repository;
        private readonly IRepository<PermissionInRole> _pirs;
        private readonly IHttpContextAccessor _httpContextAccessor;

        /// <summary>
        /// 初始化类<see cref="PermissionManager{TUserRole}"/>。
        /// </summary>
        /// <param name="repository">数据库操作接口实例。</param>
        /// <param name="pirs">数据库操作接口。</param>
        /// <param name="httpContextAccessor">当前HTTP上下文访问器。</param>
        protected PermissionManager(IRepository<Permission> repository, IRepository<PermissionInRole> pirs, IHttpContextAccessor httpContextAccessor)
        {
            _repository = repository;
            _pirs = pirs;
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// 获取当前用户的权限。
        /// </summary>
        /// <param name="userId">用户Id。</param>
        /// <param name="permissioName">权限名称。</param>
        /// <returns>返回权限结果。</returns>
        public async Task<PermissionValue> GetPermissionAsync(int userId, string permissioName)
        {
            var values = await _pirs.AsQueryable()
                .InnerJoin<TUserRole>((p, r) => p.RoleId == r.RoleId)
                .InnerJoin<Permission>((pi, p) => pi.PermissionId == p.Id)
                .Where<TUserRole>(ur => ur.UserId == userId)
                .Where<Permission>(p => p.Name == permissioName)
                .Select(x => x.Value)
                .AsEnumerableAsync(r => (PermissionValue)r.GetInt32(0));
            if (values.Any(x => x == PermissionValue.Deny))
                return PermissionValue.Deny;
            if (values.Any(x => x == PermissionValue.Allow))
                return PermissionValue.Allow;
            return PermissionValue.NotSet;
        }

        /// <summary>
        /// 判断当前用户是否拥有<paramref name="permissionName"/>权限。
        /// </summary>
        /// <param name="permissionName">权限名称。</param>
        /// <returns>返回判断结果。</returns>
        public async Task<bool> IsAuthorized(string permissionName)
        {
            var isAuthorized =
                _httpContextAccessor.HttpContext.Items[typeof(Permission) + ":" + permissionName] as bool?;
            if (isAuthorized != null) return isAuthorized.Value;
            isAuthorized = false;
            var id = _httpContextAccessor.HttpContext.User.GetUserId();
            if (id > 0)
            {
                var permission = await GetPermissionAsync(id, permissionName);
                isAuthorized = permission == PermissionValue.Allow;
            }
            _httpContextAccessor.HttpContext.Items[typeof(Permission) + ":" + permissionName] = isAuthorized;
            return isAuthorized.Value;
        }
    }
}