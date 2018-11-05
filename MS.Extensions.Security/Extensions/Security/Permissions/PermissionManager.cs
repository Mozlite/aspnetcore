using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Mozlite.Data;
using Mozlite.Extensions.Security.Permissions;

namespace MS.Extensions.Security.Permissions
{
    /// <summary>
    /// 权限管理。
    /// </summary>
    public class PermissionManager : PermissionManager<UserRole, Role>
    {
        /// <summary>
        /// 初始化类<see cref="PermissionManager"/>。
        /// </summary>
        /// <param name="db">数据库操作接口实例。</param>
        /// <param name="prdb">数据库操作接口。</param>
        /// <param name="httpContextAccessor">当前HTTP上下文访问器。</param>
        /// <param name="cache">缓存接口。</param>
        /// <param name="rdb">角色数据库操作接口。</param>
        /// <param name="urdb">用户角色数据库操作接口。</param>
        public PermissionManager(IDbContext<Permission> db, IDbContext<PermissionInRole> prdb, IHttpContextAccessor httpContextAccessor, IMemoryCache cache, IDbContext<Role> rdb, IDbContext<UserRole> urdb) 
            : base(db, prdb, httpContextAccessor, cache, rdb, urdb)
        {
        }
    }
}