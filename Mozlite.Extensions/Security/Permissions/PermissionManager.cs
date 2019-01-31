using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Mozlite.Data;
using Mozlite.Extensions.Properties;
using Mozlite.Extensions.Security.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        private readonly IDbContext<TUserRole> _urdb;
        private readonly Type _cacheKey = typeof(Permission);
        private readonly Type _valueKey = typeof(PermissionValue);

        /// <summary>
        /// 初始化类<see cref="PermissionManager{TUserRole, TRole}"/>。
        /// </summary>
        /// <param name="db">数据库操作接口实例。</param>
        /// <param name="prdb">数据库操作接口。</param>
        /// <param name="httpContextAccessor">当前HTTP上下文访问器。</param>
        /// <param name="cache">缓存接口。</param>
        /// <param name="rdb">角色数据库操作接口。</param>
        /// <param name="urdb">用户角色数据库操作接口。</param>
        protected PermissionManager(IDbContext<Permission> db, IDbContext<PermissionInRole> prdb, IHttpContextAccessor httpContextAccessor, IMemoryCache cache, IDbContext<TRole> rdb, IDbContext<TUserRole> urdb)
        {
            DbContext = db;
            _prdb = prdb;
            _httpContextAccessor = httpContextAccessor;
            _cache = cache;
            _rdb = rdb;
            _urdb = urdb;
            Init();
        }

        private IEnumerable<Permission> LoadProviderPermissions()
        {
            var providers = _httpContextAccessor.HttpContext.RequestServices.GetServices<IPermissionProvider>();
            providers = providers.OrderBy(x => x.Order);
            var permissions = new Dictionary<string, Permission>(StringComparer.OrdinalIgnoreCase);
            foreach (var provider in providers)
            {
                var providerPermissions = provider.LoadPermissions();
                foreach (var permission in providerPermissions)
                {
                    if (string.IsNullOrEmpty(permission.Category))
                        permission.Category = provider.Category;
                    permissions[permission.Key] = permission;
                }
            }
            return permissions.Values;
        }

        private void EnsureCategories(IEnumerable<Permission> permissions)
        {
            var providerCategories = permissions.Select(x => x.Category).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
            var categoryManager =
                _httpContextAccessor.HttpContext.RequestServices.GetRequiredService<ICategoryManager>();
            var categories = categoryManager.Fetch().ToList();
            foreach (var category in categories)
            {
                if (providerCategories.Contains(category.Name))
                    continue;
                categoryManager.Delete(x => x.Name == category.Name);
                DbContext.Delete(x => x.Category == category.Name);
            }
            categories = categoryManager.Fetch().ToList();
            foreach (var providerCategory in providerCategories)
            {
                if (categories.Any(x => x.Name == providerCategory))
                    continue;
                var category = new Category { Name = providerCategory, Text = providerCategory };
                categoryManager.Save(category);
            }
        }

        private void Init()
        {
            var permissions = LoadProviderPermissions().ToList();
            EnsureCategories(permissions);
            foreach (var permission in permissions)
            {
                var dbPermission = DbContext.Find(x => x.Category == permission.Category && x.Name == permission.Name);
                if (dbPermission == null)
                {
                    permission.Order = DbContext.Max(x => x.Order, x => x.Category == permission.Category) + 1;
                    DbContext.Create(permission);
                }
                else
                    DbContext.Update(x => x.Id == dbPermission.Id, new { permission.Text, permission.Description });
            }
            RefreshOwners();
        }

        /// <summary>
        /// 获取权限值。
        /// </summary>
        /// <param name="roleId">当前角色。</param>
        /// <param name="permissionId">权限Id。</param>
        /// <returns>返回权限值。</returns>
        public virtual PermissionValue GetPermissionValue(int roleId, int permissionId)
        {
            var permissions = LoadCachePermissionValues();
            if (permissions.TryGetValue(GetCacheKey(roleId, permissionId), out var value))
                return value;
            return PermissionValue.NotSet;
        }

        /// <summary>
        /// 获取权限值。
        /// </summary>
        /// <param name="roleId">当前角色。</param>
        /// <param name="permissionId">权限Id。</param>
        /// <returns>返回权限值。</returns>
        public virtual async Task<PermissionValue> GetPermissionValueAsync(int roleId, int permissionId)
        {
            var permissions = await LoadCachePermissionValuesAsync();
            if (permissions.TryGetValue(GetCacheKey(roleId, permissionId), out var value))
                return value;
            return PermissionValue.NotSet;
        }

        /// <summary>
        /// 获取当前用户的权限。
        /// </summary>
        /// <param name="userId">用户Id。</param>
        /// <param name="permissioName">权限名称。</param>
        /// <returns>返回权限结果。</returns>
        public PermissionValue GetUserPermissionValue(int userId, string permissioName)
        {
            var permission = GetOrCreate(permissioName);
            var values = _urdb.Fetch(x => x.UserId == userId)
                .Select(x => GetPermissionValue(x.RoleId, permission.Id))
                .ToList();
            return Merged(values);
        }

        /// <summary>
        /// 获取当前用户的权限。
        /// </summary>
        /// <param name="userId">用户Id。</param>
        /// <param name="permissioName">权限名称。</param>
        /// <returns>返回权限结果。</returns>
        public async Task<PermissionValue> GetUserPermissionValueAsync(int userId, string permissioName)
        {
            var permission = await GetOrCreateAsync(permissioName);
            var roles = await _urdb.FetchAsync(x => x.UserId == userId);
            var values = roles.Select(x => GetPermissionValue(x.RoleId, permission.Id))
                .ToList();
            return Merged(values);
        }

        private PermissionValue Merged(List<PermissionValue> values)
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
                var permission = await GetUserPermissionValueAsync(id, permissionName);
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
                var permission = GetUserPermissionValue(id, permissionName);
                isAuthorized = permission == PermissionValue.Allow;
            }
            _httpContextAccessor.HttpContext.Items[typeof(Permission) + ":" + permissionName] = isAuthorized;
            return isAuthorized.Value;
        }

        /// <summary>
        /// 获取或添加权限。
        /// </summary>
        /// <param name="id">权限Id。</param>
        /// <returns>返回当前名称的权限实例。</returns>
        public virtual Permission GetPermission(int id)
        {
            var permissions = LoadCachePermissions().Values;
            return permissions.SingleOrDefault(x => x.Id == id);
        }

        /// <summary>
        /// 获取或添加权限。
        /// </summary>
        /// <param name="id">权限Id。</param>
        /// <returns>返回当前名称的权限实例。</returns>
        public virtual async Task<Permission> GetPermissionAsync(int id)
        {
            var permissions = await LoadCachePermissionsAsync();
            return permissions.Values.SingleOrDefault(x => x.Id == id);
        }

        /// <summary>
        /// 获取或添加权限。
        /// </summary>
        /// <param name="permissionName">权限名称。</param>
        /// <returns>返回当前名称的权限实例。</returns>
        public Permission GetOrCreate(string permissionName)
        {
            var permission = new Permission(permissionName);
            var permissions = LoadCachePermissions();
            if (permissions.TryGetValue(permission.Key, out var value))
                return value;

            permission.Order = DbContext.Max(x => x.Order, x => x.Category == permission.Category) + 1;
            RemoveCache(DbContext.Create(permission));
            return permission;
        }

        /// <summary>
        /// 获取或添加权限。
        /// </summary>
        /// <param name="permissionName">权限名称。</param>
        /// <returns>返回当前名称的权限实例。</returns>
        public async Task<Permission> GetOrCreateAsync(string permissionName)
        {
            var permission = new Permission(permissionName);
            var permissions = await LoadCachePermissionsAsync();
            if (permissions.TryGetValue(permissionName, out var value))
                return value;

            permission.Order = await DbContext.MaxAsync(x => x.Order, x => x.Category == permission.Category) + 1;
            RemoveCache(await DbContext.CreateAsync(permission));
            return permission;
        }

        /// <summary>
        /// 保存权限。
        /// </summary>
        /// <param name="permission">权限实例对象。</param>
        /// <returns>返回保存结果。</returns>
        public async Task<bool> SaveAsync(Permission permission)
        {
            var permissions = await LoadCachePermissionsAsync();
            bool result;
            if (permissions.Any(x => x.Key == permission.Key))
                result = await DbContext.UpdateAsync(x => x.Name == permission.Name, new { permission.Description });
            else
            {
                permission.Order = await DbContext.MaxAsync(x => x.Order, x => x.Category == permission.Category) + 1;
                result = await DbContext.CreateAsync(permission);
            }
            return RemoveCache(result);
        }

        /// <summary>
        /// 保存权限。
        /// </summary>
        /// <param name="permission">权限实例对象。</param>
        /// <returns>返回保存结果。</returns>
        public bool Save(Permission permission)
        {
            var permissions = LoadCachePermissions();
            bool result;
            if (permissions.Any(x => x.Key == permission.Key))
                result = DbContext.Update(x => x.Name == permission.Name, new { permission.Description });
            else
            {
                permission.Order = DbContext.Max(x => x.Order, x => x.Category == permission.Category) + 1;
                result = DbContext.Create(permission);
            }
            return RemoveCache(result);
        }

        /// <summary>
        /// 更新管理员权限配置。
        /// </summary>
        public async Task RefreshOwnersAsync()
        {
            var role = await _rdb.FindAsync(x => x.NormalizedName == DefaultRole.Owner.NormalizedName);
            if (role == null) return;
            var permissions = await LoadCachePermissionsAsync();
            foreach (var permission in permissions.Values)
            {
                if (await _prdb.AnyAsync(x => x.PermissionId == permission.Id && x.RoleId == role.RoleId))
                    continue;
                await _prdb.CreateAsync(new PermissionInRole { PermissionId = permission.Id, RoleId = role.RoleId, Value = PermissionValue.Allow });
            }
            RemoveCache();
        }

        /// <summary>
        /// 更新管理员权限配置。
        /// </summary>
        public void RefreshOwners()
        {
            var role = _rdb.Find(x => x.NormalizedName == DefaultRole.Owner.NormalizedName);
            if (role == null) return;
            var permissions = LoadCachePermissions().Values;
            foreach (var permission in permissions)
            {
                if (_prdb.Any(x => x.PermissionId == permission.Id && x.RoleId == role.RoleId))
                    continue;
                _prdb.Create(new PermissionInRole { PermissionId = permission.Id, RoleId = role.RoleId, Value = PermissionValue.Allow });
            }
            RemoveCache();
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
                return permissions.Values
                    .Where(x => x.Category.Equals(category, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            return permissions.Values;
        }

        /// <summary>
        /// 保存当前配置角色权限。
        /// </summary>
        /// <param name="roleId">角色Id。</param>
        /// <param name="request">当前请求。</param>
        /// <returns>返回保存结果。</returns>
        public virtual async Task<DataResult> SaveAsync(int roleId, HttpRequest request)
        {
            var role = await _rdb.FindAsync(roleId);
            if (role.NormalizedName == DefaultRole.Owner.NormalizedName)
                return Resources.PermissionSetCannotBeOwner;
            if (await _prdb.BeginTransactionAsync(async db =>
            {
                foreach (var permission in LoadCachePermissions().Values)
                {
                    if (!Enum.TryParse<PermissionValue>(request.Form[$"p-{roleId}-{permission.Id}"], out var value))
                        value = PermissionValue.NotSet;
                    if (await db.AnyAsync(x => x.RoleId == roleId && x.PermissionId == permission.Id))
                        await db.UpdateAsync(x => x.RoleId == roleId && x.PermissionId == permission.Id,
                            new { Value = value });
                    else
                        await db.CreateAsync(new PermissionInRole { PermissionId = permission.Id, RoleId = roleId, Value = value });
                }
                return true;
            }))
            {
                RemoveCache();
                return DataAction.Updated;
            }
            return DataAction.UpdatedFailured;
        }

        /// <summary>
        /// 保存当前配置角色权限。
        /// </summary>
        /// <param name="roleId">角色Id。</param>
        /// <param name="request">当前请求。</param>
        /// <returns>返回保存结果。</returns>
        public virtual DataResult Save(int roleId, HttpRequest request)
        {
            var role = _rdb.Find(roleId);
            if (role.NormalizedName == DefaultRole.Owner.NormalizedName)
                return Resources.PermissionSetCannotBeOwner;
            if (_prdb.BeginTransaction(db =>
            {
                foreach (var permission in LoadCachePermissions().Values)
                {
                    if (!Enum.TryParse<PermissionValue>(request.Form[$"p-{roleId}-{permission.Id}"], out var value))
                        value = PermissionValue.NotSet;
                    if (db.Any(x => x.RoleId == roleId && x.PermissionId == permission.Id))
                        db.Update(x => x.RoleId == roleId && x.PermissionId == permission.Id,
                            new { Value = value });
                    else
                        db.Create(new PermissionInRole { PermissionId = permission.Id, RoleId = roleId, Value = value });
                }
                return true;
            }))
            {
                RemoveCache();
                return DataAction.Updated;
            }
            return DataAction.UpdatedFailured;
        }

        /// <summary>
        /// 上移权限。
        /// </summary>
        /// <param name="id">权限Id。</param>
        /// <param name="category">分类。</param>
        /// <returns>返回移动结果。</returns>
        public virtual bool MoveUp(int id, string category)
        {
            return RemoveCache(DbContext.MoveUp(id, x => x.Order, x => x.Category == category));
        }

        /// <summary>
        /// 上移权限。
        /// </summary>
        /// <param name="id">权限Id。</param>
        /// <param name="category">分类。</param>
        /// <returns>返回移动结果。</returns>
        public virtual async Task<bool> MoveUpAsync(int id, string category)
        {
            return RemoveCache(await DbContext.MoveUpAsync(id, x => x.Order, x => x.Category == category));
        }

        /// <summary>
        /// 下移权限。
        /// </summary>
        /// <param name="id">权限Id。</param>
        /// <param name="category">分类。</param>
        /// <returns>返回移动结果。</returns>
        public virtual bool MoveDown(int id, string category)
        {
            return RemoveCache(DbContext.MoveDown(id, x => x.Order, x => x.Category == category));
        }

        /// <summary>
        /// 下移权限。
        /// </summary>
        /// <param name="id">权限Id。</param>
        /// <param name="category">分类。</param>
        /// <returns>返回移动结果。</returns>
        public virtual async Task<bool> MoveDownAsync(int id, string category)
        {
            return RemoveCache(await DbContext.MoveDownAsync(id, x => x.Order, x => x.Category == category));
        }

        /// <summary>
        /// 判断权限名称是否存在。
        /// </summary>
        /// <param name="permissionName">权限名称。</param>
        /// <returns>返回判断结果。</returns>
        public virtual bool Exist(string permissionName)
        {
            var permission = new Permission(permissionName);
            var permissions = LoadCachePermissions();
            return permissions.ContainsKey(permission.Key);
        }

        /// <summary>
        /// 判断权限名称是否存在。
        /// </summary>
        /// <param name="permissionName">权限名称。</param>
        /// <returns>返回判断结果。</returns>
        public virtual async Task<bool> ExistAsync(string permissionName)
        {
            var permission = new Permission(permissionName);
            var permissions = await LoadCachePermissionsAsync();
            return permissions.ContainsKey(permission.Key);
        }

        private string GetCacheKey(int roleId, int permissionId) => $"{roleId}-{permissionId}";

        private bool RemoveCache(bool result = true)
        {
            if (result)
            {
                _cache.Remove(_valueKey);
                _cache.Remove(_cacheKey);
            }
            return result;
        }

        private IDictionary<string, PermissionValue> LoadCachePermissionValues()
        {
            return _cache.GetOrCreate(_valueKey, ctx =>
            {
                ctx.SetDefaultAbsoluteExpiration();
                var permissions = _prdb.Fetch();
                return permissions.ToDictionary(x => GetCacheKey(x.RoleId, x.PermissionId), x => x.Value, StringComparer.OrdinalIgnoreCase);
            });
        }

        private async Task<IDictionary<string, PermissionValue>> LoadCachePermissionValuesAsync()
        {
            return await _cache.GetOrCreateAsync(_valueKey, async ctx =>
            {
                ctx.SetDefaultAbsoluteExpiration();
                var permissions = await _prdb.FetchAsync();
                return permissions.ToDictionary(x => GetCacheKey(x.RoleId, x.PermissionId), x => x.Value, StringComparer.OrdinalIgnoreCase);
            });
        }

        private IDictionary<string, Permission> LoadCachePermissions()
        {
            return _cache.GetOrCreate(_cacheKey, ctx =>
            {
                ctx.SetDefaultAbsoluteExpiration();
                var permissions = DbContext.Fetch();
                permissions = permissions.OrderByDescending(x => x.Order);
                return permissions.ToDictionary(x => x.Key, StringComparer.OrdinalIgnoreCase);
            });
        }

        private async Task<IDictionary<string, Permission>> LoadCachePermissionsAsync()
        {
            return await _cache.GetOrCreateAsync(_cacheKey, async ctx =>
            {
                ctx.SetDefaultAbsoluteExpiration();
                var permissions = await DbContext.FetchAsync();
                permissions = permissions.OrderByDescending(x => x.Order);
                return permissions.ToDictionary(x => x.Key, StringComparer.OrdinalIgnoreCase);
            });
        }
    }
}