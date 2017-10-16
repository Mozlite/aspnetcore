using Mozlite.Data;

namespace Mozlite.Extensions.Security.Models
{
    /// <summary>
    /// 用户组数据存储。
    /// </summary>
    public class RoleStore : IdentityIdentityRoleStore<Role, RoleClaim>
    {
        /// <summary>
        /// 初始化类<see cref="RoleStore"/>。
        /// </summary>
        /// <param name="repository">用户组数据库操作接口。</param>
        /// <param name="rc">用户组声明数据库操作接口。</param>
        public RoleStore(IRepository<Role> repository, IRepository<RoleClaim> rc) : base(repository, rc)
        {
        }
    }
}