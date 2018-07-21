using Mozlite.Extensions.Security;

namespace Demo.Extensions.Security
{
    /// <summary>
    /// 用户管理接口。
    /// </summary>
    public interface IUserManager : IUserManager<User, Role, UserClaim, UserRole, UserLogin, UserToken, RoleClaim>
    {

    }
}