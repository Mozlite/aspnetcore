using Microsoft.AspNetCore.Identity;
using Mozlite.Data;
using Mozlite.Extensions.Security.Stores;

namespace MS.Extensions.Security.Services
{
    /// <summary>
    /// 用户存储。
    /// </summary>
    public class UserStore : UserStoreBase<User, Role, UserClaim, UserRole, UserLogin, UserToken, RoleClaim>
    {
        /// <summary>
        /// 初始化类<see cref="UserStore"/>。
        /// </summary>
        /// <param name="describer">错误描述<see cref="IdentityErrorDescriber"/>实例。</param>
        /// <param name="userContext">用户数据库接口。</param>
        /// <param name="userClaimContext">用户声明数据库接口。</param>
        /// <param name="userLoginContext">用户登陆数据库接口。</param>
        /// <param name="userTokenContext">用户标识数据库接口。</param>
        /// <param name="roleContext">角色上下文。</param>
        /// <param name="userRoleContext">用户角色数据库操作接口。</param>
        /// <param name="roleClaimContext">角色声明数据库操作接口。</param>
        /// <param name="roleManager">角色管理接口。</param>
        public UserStore(IdentityErrorDescriber describer, IDbContext<User> userContext, IDbContext<UserClaim> userClaimContext, IDbContext<UserLogin> userLoginContext, IDbContext<UserToken> userTokenContext, IDbContext<Role> roleContext, IDbContext<UserRole> userRoleContext, IDbContext<RoleClaim> roleClaimContext, IRoleManager roleManager) 
            : base(describer, userContext, userClaimContext, userLoginContext, userTokenContext, roleContext, userRoleContext, roleClaimContext, roleManager)
        {
        }
    }
}