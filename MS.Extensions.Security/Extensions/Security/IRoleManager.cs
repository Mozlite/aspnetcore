using Mozlite;
using Mozlite.Extensions.Security;

namespace MS.Extensions.Security
{
    /// <summary>
    /// 角色管理。
    /// </summary>
    public interface IRoleManager : IRoleManager<Role, UserRole, RoleClaim>, IScopedService
    {

    }
}