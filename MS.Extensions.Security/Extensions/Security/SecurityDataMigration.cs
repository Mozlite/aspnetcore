using Mozlite.Extensions.Security.Stores;

namespace MS.Extensions.Security
{
    /// <summary>
    /// 数据迁移类。
    /// </summary>
    public class SecurityDataMigration : StoreDataMigration<User, Role, UserClaim, RoleClaim, UserLogin, UserRole, UserToken>
    {

    }
}