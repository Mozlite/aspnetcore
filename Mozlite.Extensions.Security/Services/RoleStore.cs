using Mozlite.Data;
using Mozlite.Extensions.Security.Models;

namespace Mozlite.Extensions.Security.Services
{
    /// <summary>
    /// 用户组数据存储。
    /// </summary>
    public class RoleStore : IdentityIdentityRoleStore<Role, RoleClaim>
    {
        /// <summary>
        /// 初始化类<see cref="RoleStore"/>。
        /// </summary>
        /// <param name="db">用户组数据库操作接口。</param>
        /// <param name="rc">用户组声明数据库操作接口。</param>
        public RoleStore(IDbContext<Role> db, IDbContext<RoleClaim> rc) : base(db, rc)
        {
        }
    }
}