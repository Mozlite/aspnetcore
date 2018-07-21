using Microsoft.AspNetCore.Identity;
using Mozlite.Data;
using Mozlite.Extensions.Security.Stores;

namespace Demo.Extensions.Security.Stores
{
    /// <summary>
    /// 用户组存储。
    /// </summary>
    public class RoleStore : RoleStoreBase<Role, UserRole, RoleClaim>
    {
        /// <summary>
        /// 初始化类<see cref="RoleStore"/>。
        /// </summary>
        /// <param name="describer">错误描述<see cref="IdentityErrorDescriber"/>实例。</param>
        /// <param name="roleContext">用户组数据库操作接口。</param>
        /// <param name="userRoleContext">用户用户组数据库操作接口。</param>
        /// <param name="roleClaimContext">用户声明数据库操作接口。</param>
        public RoleStore(IdentityErrorDescriber describer, IDbContext<Role> roleContext, IDbContext<UserRole> userRoleContext, IDbContext<RoleClaim> roleClaimContext) 
            : base(describer, roleContext, userRoleContext, roleClaimContext)
        {
        }
    }
}