using Mozlite.Extensions.Security;

namespace Demo.Extensions.Security
{
    /// <summary>
    /// 用户组管理。
    /// </summary>
    public interface IRoleManager : IRoleManager<Role, UserRole, RoleClaim>
    {

    }
}