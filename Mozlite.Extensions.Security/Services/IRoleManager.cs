using Mozlite.Extensions.Security.Models;

namespace Mozlite.Extensions.Security.Services
{
    /// <summary>
    /// 用户组管理接口。
    /// </summary>
    public interface IRoleManager : IIdentityRoleManager<Role>, IScopedService
    {

    }
}