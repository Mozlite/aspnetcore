using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Mozlite.Data;

namespace Mozlite.Extensions.Security.Permissions
{
    /// <summary>
    /// 权限管理实现类。
    /// </summary>
    /// <typeparam name="TUserRole">用户角色关联类型。</typeparam>
    /// <typeparam name="TRole">角色类型。</typeparam>
    public abstract class PermissionManager<TUserRole, TRole> : IPermissionManager
        where TUserRole : IdentityUserRole
        where TRole : IdentityRole
    {
        /// <summary>
        /// 数据库操作实例。
        /// </summary>
        // ReSharper disable once InconsistentNaming
        protected readonly IRepository<Permission> db;
        private readonly IRepository<PermissionInRole> _prdb;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMemoryCache _cache;
        private readonly IRepository<TRole> _rdb;

        /// <summary>
        /// 初始化类<see cref="PermissionManager{TUserRole, TRole}"/>。
        /// </summary>
        /// <param name="repository">数据库操作接口实例。</param>
        /// <param name="prdb">数据库操作接口。</param>
        /// <param name="httpContextAccessor">当前HTTP上下文访问器。</param>
        /// <param name="cache">缓存接口。</param>
        /// <param name="rdb">角色数据库操作接口。</param>
        protected PermissionManager(IRepository<Permission> repository, IRepository<PermissionInRole> prdb, IHttpContextAccessor httpContextAccessor, IMemoryCache cache, IRepository<TRole> rdb)
        {
            db = repository;
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
            if (!LoadPermissions().TryGetValue(permissionName, out var permission))
            {
                permission = new Permission { Name = permissionName };
                db.Create(permission);
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
            var permissions = await LoadPermissionsAsync();
            if (!permissions.TryGetValue(permissionName, out var permission))
            {
                permission = new Permission { Name = permissionName };
                await db.CreateAsync(permission);
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
            if ((await LoadPermissionsAsync()).ContainsKey(permission.Name))
                result = await db.UpdateAsync(x => x.Name == permission.Name, new { permission.Description });
            else
                result = await db.CreateAsync(permission);
            if (result) _cache.Remove(typeof(Permission));
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
            if (LoadPermissions().ContainsKey(permission.Name))
                result = db.Update(x => x.Name == permission.Name, new { permission.Description });
            else
                result = db.Create(permission);
            if (result) _cache.Remove(typeof(Permission));
            return result;
        }

        private const string Administrator = "ADMINISTRATOR";
        /// <summary>
        /// 更新管理员权限配置。
        /// </summary>
        public async Task RefreshAdministratorsAsync()
        {
            var role = await _rdb.FindAsync(x => x.NormalizedRoleName == Administrator);
            var permissions = await LoadPermissionsAsync();
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
            var role = _rdb.Find(x => x.NormalizedRoleName == Administrator);
            var permissions = LoadPermissions().Values;
            foreach (var permission in permissions)
            {
                if (_prdb.Any(x => x.PermissionId == permission.Id && x.RoleId == role.RoleId))
                    continue;
                _prdb.Create(new PermissionInRole { PermissionId = permission.Id, RoleId = role.RoleId, Value = PermissionValue.Allow });
            }
        }

        private IDictionary<string, Permission> LoadPermissions()
        {
            return _cache.GetOrCreate(typeof(Permission), ctx =>
            {
                ctx.SetAbsoluteExpiration(TimeSpan.FromMinutes(3));
                var permissions = db.Fetch();
                return permissions.ToDictionary(x => x.Name, StringComparer.OrdinalIgnoreCase);
            });
        }

        private async Task<IDictionary<string, Permission>> LoadPermissionsAsync()
        {
            return await _cache.GetOrCreateAsync(typeof(Permission), async ctx =>
            {
                ctx.SetAbsoluteExpiration(TimeSpan.FromMinutes(3));
                var permissions = await db.FetchAsync();
                return permissions.ToDictionary(x => x.Name, StringComparer.OrdinalIgnoreCase);
            });
        }
    }
}