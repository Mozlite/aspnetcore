namespace Mozlite.Extensions.Security.Stores
{
    /// <summary>
    /// 用户角色接口。
    /// </summary>
    public interface IUserRole
    {
        /// <summary>
        /// 角色ID。
        /// </summary>
        int RoleId { get; }

        /// <summary>
        /// 用户ID。
        /// </summary>
        int UserId { get; }
    }
}