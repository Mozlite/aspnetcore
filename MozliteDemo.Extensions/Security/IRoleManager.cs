using Mozlite;
using Mozlite.Extensions.Security;

namespace MozliteDemo.Extensions.Security
{
    /// <summary>
    /// 角色管理。
    /// </summary>
    public interface IRoleManager : IRoleManager<Role, UserRole, RoleClaim>, IScopedService
    {

    }
}