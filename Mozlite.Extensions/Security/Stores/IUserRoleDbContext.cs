namespace Mozlite.Extensions.Security.Stores
{
    /// <summary>
    /// 用户和角色数据库操作上下文接口。
    /// </summary>
    /// <typeparam name="TUser">用户类型。</typeparam>
    /// <typeparam name="TRole">角色类型。</typeparam>
    /// <typeparam name="TUserClaim">用户声明类型。</typeparam>
    /// <typeparam name="TUserRole">用户角色类型。</typeparam>
    /// <typeparam name="TUserLogin">用户登陆类型。</typeparam>
    /// <typeparam name="TUserToken">用户标识类型。</typeparam>
    /// <typeparam name="TRoleClaim">角色声明类型。</typeparam>
    public interface IUserRoleDbContext<TUser, TRole, TUserClaim, TUserRole, TUserLogin, TUserToken, TRoleClaim>
        : IUserDbContext<TUser, TUserClaim, TUserLogin, TUserToken>, IRoleDbContext<TRole, TUserRole, TRoleClaim>
        where TUser : UserBase
        where TRole : RoleBase
        where TUserClaim : UserClaimBase, new()
        where TUserRole : UserRoleBase, new()
        where TUserLogin : UserLoginBase, new()
        where TUserToken : UserTokenBase, new()
        where TRoleClaim : RoleClaimBase, new()
    {
        /// <summary>
        /// 角色管理接口。
        /// </summary>
        IRoleManager<TRole, TUserRole, TRoleClaim> RoleManager { get; }
    }
}