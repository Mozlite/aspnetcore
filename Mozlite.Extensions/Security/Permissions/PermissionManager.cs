using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Mozlite.Data;
using Mozlite.Extensions.Security.Stores;

namespace Mozlite.Extensions.Security.Permissions
{
    /// <summary>
    /// 权限管理实现类。
    /// </summary>
    /// <typeparam name="TUserRole">用户角色关联类型。</typeparam>
    /// <typeparam name="TRole">角色类型。</typeparam>
    public abstract class PermissionManager<TUserRole, TRole> : IPermissionManager
        where TUserRole : IUserRole
        where TRole : RoleBase
    {
        /// <summary>
        /// 数据库操作实例。
        /// </summary>
        protected IDbContext<Permission> DbContext { get; }
        private readonly IDbContext<PermissionInRole> _prdb;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMemoryCache _cache;
        private readonly IDbContext<TRole> _rdb;
        private readonly Type _cacheKey = typeof(Permission);

        /// <summary>
        /// 初始化类<see cref="PermissionManager{TUserRole, TRole}"/>。
        /// </summary>
        /// <param name="db">数据库操作接口实例。</param>
        /// <param name="prdb">数据库操作接口。</param>
        /// <param name="httpContextAccessor">当前HTTP上下文访问器。</param>
        /// <param name="cache">缓存接口。</param>
        /// <param name="rdb">角色数据库操作接口。</param>
        protected PermissionManager(IDbContext<Permission> db, IDbContext<PermissionInRole> prdb, IHttpContextAccessor httpContextAccessor, IMemoryCache cache, IDbContext<TRole> rdb)
        {
            DbContext = db;
            _prdb = prdb;
            _httpContextAccessor = httpContextAccessor;
            _cache = cache;
            _rdb = rdb;
        }

        /// <summary>
        /// 获取当前用户的权限。
        /// </summary>
        /// <param name="userId">用户Id。</param>
        /// <param name="permissioName">权限名称。</param>
        /// <returns>返回权限结果。</returns>
        public PermissionValue GetPermission(int userId, string permissioName)
        {
            var permission = GetOrCreate(permissioName);
            var values = _prdb.AsQueryable()
                .InnerJoin<TUserRole>((p, r) => p.RoleId == r.RoleId)
                .Where<TUserRole>(ur => ur.UserId == userId)
                .Where(x => x.PermissionId == permission.Id)
                .Select(x => x.Value)
                .AsEnumerable(r => (PermissionValue)r.GetInt32(0));
            return Merged(values);
        }

        /// <summary>
        /// 获取当前用户的权限。
        /// </summary>
        /// <param name="userId">用户Id。</param>
        /// <param name="permissioName">权限名称。</param>
        /// <returns>返回权限结果。</returns>
        public async Task<PermissionValue> GetPermissionAsync(int userId, string permissioName)
        {
            var permission = await GetOrCreateAsync(permissioName);
            var values = await _prdb.AsQueryable()
                .InnerJoin<TUserRole>((p, r) => p.RoleId == r.RoleId)
                .Where<TUserRole>(ur => ur.UserId == userId)
                .Where(x => x.PermissionId == permission.Id)
                .Select(x => x.Value)
                .AsEnumerableAsync(r => (PermissionValue)r.GetInt32(0));
            return Merged(values);
        }

        private PermissionValue Merged(IEnumerable<PermissionValue> values)
        {
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
        public async Task<bool> IsAuthorizedAsync(string permissionName)
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

        /// <summary>
        /// 判断当前用户是否拥有<paramref name="permissionName"/>权限。
        /// </summary>
        /// <param name="permissionName">权限名称。</param>
        /// <returns>返回判断结果。</returns>
        public bool IsAuthorized(string permissionName)
        {
            var isAuthorized =
                _httpContextAccessor.HttpContext.Items[typeof(Permission) + ":" + permissionName] as bool?;
            if (isAuthorized != null) return isAuthorized.Value;
            isAuthorized = false;
            var id = _httpContextAccessor.HttpContext.User.GetUserId();
            if (id > 0)
            {
                var permission = GetPermission(id, permissionName);
                isAuthorized = permission == PermissionValue.Allow;
            }
            _httpContextAccessor.HttpContext.Items[typeof(Permission) + ":" + permissionName] = isAuthorized;
            return isAuthorized.Value;
        }

        /// <summary>
        /// 获取或添加权限。
        /// </summary>
        /// <param name="permissionName">权限名称。</param>
        /// <returns>返回当前名称的权限实例。</returns>
        public Permission GetOrCreate(string permissionName)
        {
            if (!LoadCachePermissions().TryGetValue(permissionName, out var permission))
            {
                permission = new Permission { Name = permissionName };
                DbContext.Create(permission);
            }
            return permission;
        }

        /// <summary>
        /// 获取或添加权限。
        /// </summary>
        /// <param name="permissionName">权限名称。</param>
        /// <returns>返回当前名称的权限实例。</returns>
        public async Task<Permission> GetOrCreateAsync(string permissionName)
        {
            var permissions = await LoadCachePermissionsAsync();
            if (!permissions.TryGetValue(permissionName, out var permission))
            {
                permission = new Permission { Name = permissionName };
                await DbContext.CreateAsync(permission);
            }
            return permission;
        }

        /// <summary>
        /// 保存权限。
        /// </summary>
        /// <param name="permission">权限实例对象。</param>
        /// <returns>返回保存结果。</returns>
        public async Task<bool> SaveAsync(Permission permission)
        {
            bool result;
            if ((await LoadCachePermissionsAsync()).ContainsKey(permission.Name))
                result = await DbContext.UpdateAsync(x => x.Name == permission.Name, new { permission.Description });
            else
                result = await DbContext.CreateAsync(permission);
            if (result) _cache.Remove(_cacheKey);
            return result;
        }

        /// <summary>
        /// 保存权限。
        /// </summary>
        /// <param name="permission">权限实例对象。</param>
        /// <returns>返回保存结果。</returns>
        public bool Save(Permission permission)
        {
            bool result;
            if (LoadCachePermissions().ContainsKey(permission.Name))
                result = DbContext.Update(x => x.Name == permission.Name, new { permission.Description });
            else
                result = DbContext.Create(permission);
            if (result) _cache.Remove(_cacheKey);
            return result;
        }

        private const string Administrator = "ADMINISTRATOR";
        /// <summary>
        /// 更新管理员权限配置。
        /// </summary>
        public async Task RefreshAdministratorsAsync()
        {
            var role = await _rdb.FindAsync(x => x.NormalizedName == Administrator);
            var permissions = await LoadCachePermissionsAsync();
            foreach (var permission in permissions.Values)
            {
                if (await _prdb.AnyAsync(x => x.PermissionId == permission.Id && x.RoleId == role.RoleId))
                    continue;
                await _prdb.CreateAsync(new PermissionInRole { PermissionId = permission.Id, RoleId = role.RoleId, Value = PermissionValue.Allow });
            }
        }

        /// <summary>
        /// 更新管理员权限配置。
        /// </summary>
        public void RefreshAdministrators()
        {
            var role = _rdb.Find(x => x.NormalizedName == Administrator);
            var permissions = LoadCachePermissions().Values;
            foreach (var permission in permissions)
            {
                if (_prdb.Any(x => x.PermissionId == permission.Id && x.RoleId == role.RoleId))
                    continue;
                _prdb.Create(new PermissionInRole { PermissionId = permission.Id, RoleId = role.RoleId, Value = PermissionValue.Allow });
            }
        }

        /// <summary>
        /// 加载权限列表。
        /// </summary>
        /// <param name="category">权限分类。</param>
        /// <returns>返回权限列表。</returns>
        public virtual IEnumerable<Permission> LoadPermissions(string category = null)
        {
            var permissions = LoadCachePermissions();
            if (category != null)
                return permissions.Values.Where(x => x.Category.Equals(category, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            return permissions.Values;
        }

        /// <summary>
        /// 加载权限列表。
        /// </summary>
        /// <param name="category">权限分类。</param>
        /// <returns>返回权限列表。</returns>
        public virtual async Task<IEnumerable<Permission>> LoadPermissionsAsync(string category = null)
        {
            var permissions = await LoadCachePermissionsAsync();
            if (category != null)
                return permissions.Values.Where(x => x.Category.Equals(category, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            return permissions.Values;
        }

        private IDictionary<string, Permission> LoadCachePermissions()
        {
            return _cache.GetOrCreate(typeof(Permission), ctx =>
            {
                ctx.SetAbsoluteExpiration(TimeSpan.FromMinutes(3));
                var permissions = DbContext.Fetch();
                return permissions.ToDictionary(x => $"{x.Category}.{x.Name}", StringComparer.OrdinalIgnoreCase);
            });
        }

        private async Task<IDictionary<string, Permission>> LoadCachePermissionsAsync()
        {
            return await _cache.GetOrCreateAsync(typeof(Permission), async ctx =>
            {
                ctx.SetAbsoluteExpiration(TimeSpan.FromMinutes(3));
                var permissions = await DbContext.FetchAsync();
                return permissions.ToDictionary(x => $"{x.Category}.{x.Name}", StringComparer.OrdinalIgnoreCase);
            });
        }
    }
}