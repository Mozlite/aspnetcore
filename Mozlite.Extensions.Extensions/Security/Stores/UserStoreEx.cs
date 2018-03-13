using System;
using System.Linq;
using Mozlite.Data;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using Mozlite.Extensions.Sites;

namespace Mozlite.Extensions.Security.Stores
{
    /// <summary>
    /// 用户存储基类，包含用户角色的相关操作。
    /// </summary>
    /// <typeparam name="TUser">用户类型。</typeparam>
    /// <typeparam name="TRole">角色类型。</typeparam>
    /// <typeparam name="TUserClaim">用户声明类型。</typeparam>
    /// <typeparam name="TUserRole">用户角色类型。</typeparam>
    /// <typeparam name="TUserLogin">用户登陆类型。</typeparam>
    /// <typeparam name="TUserToken">用户标识类型。</typeparam>
    /// <typeparam name="TRoleClaim">角色声明类型。</typeparam>
    public class UserStoreEx<TUser, TRole, TUserClaim, TUserRole, TUserLogin, TUserToken, TRoleClaim>
        : UserStoreBase<TUser, TRole, TUserClaim, TUserRole, TUserLogin, TUserToken, TRoleClaim>
        where TUser : UserExBase
        where TRole : RoleExBase
        where TUserClaim : UserClaimBase, new()
        where TUserRole : UserRoleBase, new()
        where TUserLogin : UserLoginBase, new()
        where TUserToken : UserTokenBase, new()
        where TRoleClaim : RoleClaimBase, new()
    {
        /// <summary>
        /// 初始化类<see cref="UserStoreBase{TUser, TRole, TUserClaim, TUserRole, TUserLogin, TUserToken, TRoleClaim}"/>。
        /// </summary>
        /// <param name="describer">错误描述<see cref="IdentityErrorDescriber"/>实例。</param>
        /// <param name="userContext">用户数据库接口。</param>
        /// <param name="userClaimContext">用户声明数据库接口。</param>
        /// <param name="userLoginContext">用户登陆数据库接口。</param>
        /// <param name="userTokenContext">用户标识数据库接口。</param>
        /// <param name="roleContext">角色数据库操作接口。</param>
        /// <param name="userRoleContext">用户角色数据库操作接口。</param>
        /// <param name="roleClaimContext">用户声明数据库操作接口。</param>
        /// <param name="roleManager">角色管理接口。</param>
        public UserStoreEx(IdentityErrorDescriber describer, IDbContext<TUser> userContext, IDbContext<TUserClaim> userClaimContext, IDbContext<TUserLogin> userLoginContext, IDbContext<TUserToken> userTokenContext, IDbContext<TRole> roleContext, IDbContext<TUserRole> userRoleContext, IDbContext<TRoleClaim> roleClaimContext, IRoleManager<TRole, TUserRole, TRoleClaim> roleManager) : base(describer, userContext, userClaimContext, userLoginContext, userTokenContext, roleContext, userRoleContext, roleClaimContext, roleManager)
        {
        }
    }
}