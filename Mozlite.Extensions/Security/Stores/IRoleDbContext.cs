using Mozlite.Data;

namespace Mozlite.Extensions.Security.Stores
{
    /// <summary>
    /// 角色数据库操作上下文接口。
    /// </summary>
    /// <typeparam name="TRole">角色类型。</typeparam>
    /// <typeparam name="TUserRole">用户角色类型。</typeparam>
    /// <typeparam name="TRoleClaim">角色声明类型。</typeparam>
    public interface IRoleDbContext<TRole, TUserRole, TRoleClaim>
        where TRole : RoleBase
        where TUserRole : IUserRole
        where TRoleClaim : RoleClaimBase, new()
    {
        /// <summary>
        /// 角色数据库操作接口。
        /// </summary>
        IDbContext<TRole> RoleContext { get; }

        /// <summary>
        /// 用户角色数据库操作接口。
        /// </summary>
        IDbContext<TUserRole> UserRoleContext { get; }

        /// <summary>
        /// 用户声明数据库操作接口。
        /// </summary>
        IDbContext<TRoleClaim> RoleClaimContext { get; }
    }
}