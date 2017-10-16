namespace Mozlite.Extensions.Security.Models
{
    /// <summary>
    /// 用户组管理接口。
    /// </summary>
    public interface IRoleManager : IIdentityRoleManager<Role>, IScopedService
    {

    }
}