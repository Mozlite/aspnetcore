using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using Mozlite.Data;
using Mozlite.Extensions.Security.Models;

namespace Mozlite.Extensions.Security.Services
{
    /// <summary>
    /// 用户组管理类。
    /// </summary>
    public class RoleManager : IdentityRoleManager<Role>, IRoleManager
    {
        /// <summary>
        /// 初始化类<see cref="RoleManager"/>。
        /// </summary>
        /// <param name="repository">用户组数据库操作接口。</param>
        /// <param name="store">用户组存储接口实例。</param>
        /// <param name="cache">缓存接口。</param>
        public RoleManager(IRepository<Role> repository, IRoleStore<Role> store, IMemoryCache cache)
            : base(repository, store, cache)
        {
        }
    }
}