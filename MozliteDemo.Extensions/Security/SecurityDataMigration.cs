using Mozlite.Extensions.Security;

namespace MozliteDemo.Extensions.Security
{
    /// <summary>
    /// 数据迁移类。
    /// </summary>
    public class SecurityDataMigration : IdentityDataMigration<User, Role, UserClaim, RoleClaim, UserLogin, UserRole, UserToken>
    {
        /// <summary>
        /// 优先级，在两个迁移数据需要先后时候使用。
        /// </summary>
        public override int Priority => 200;
    }
}