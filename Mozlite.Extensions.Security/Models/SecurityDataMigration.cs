using Microsoft.AspNetCore.Identity;
using Mozlite.Data.Migrations;
using Mozlite.Data.Migrations.Builders;

namespace Mozlite.Extensions.Security.Models
{
    /// <summary>
    /// 数据迁移。
    /// </summary>
    public class SecurityDataMigration : IdentityDataMigration<User, Role, UserClaim, RoleClaim, UserLogin, UserRole, UserToken>
    {
        private readonly IPasswordHasher<AdminUser> _passwordHasher;

        public SecurityDataMigration(IPasswordHasher<AdminUser> passwordHasher)
        {
            _passwordHasher = passwordHasher;
        }

        /// <summary>
        /// 添加用户定义列。
        /// </summary>
        /// <param name="builder">用户表格定义实例。</param>
        protected override void Create(CreateTableBuilder<User> builder)
        {
            builder.Column(x => x.Score)
                .Column(x => x.ConcurrencyStamp, "timestamp");
        }

        /// <summary>
        /// 添加默认角色。
        /// </summary>
        /// <param name="builder">数据迁移构建实例。</param>
        public override void Up1(MigrationBuilder builder)
        {
            base.Up1(builder);
            builder.CreateTable<UserProfile>(table => table
                .Column(x => x.Id)
                .Column(x => x.Intro)
                .Column(x => x.Weibo)
                .Column(x => x.Weixin)
                .Column(x => x.CommentId)
                .Column(x => x.Comments)
                .Column(x => x.EnabledComment)
                .Column(x => x.Follows)
                .Column(x => x.Followeds)
                .Column(x => x.QQ)
                .ForeignKey<User>(x => x.Id, x => x.UserId, onDelete: ReferentialAction.Cascade)
            );
            var user = new AdminUser();
            user.UserName = "@Admin";
            user.PasswordHash = "a123456";
            user.NormalizedUserName = user.UserName.ToUpper();
            user.PasswordHash =
                _passwordHasher.HashPassword(user, SecurityHelper.CreatePassword(user.UserName, user.PasswordHash));
            builder.SqlCreate(user);
            var role1 = new UserRole { RoleId = 1, UserId = 1 };
            builder.SqlCreate(role1);
            var role2 = new UserRole { RoleId = 2, UserId = 1 };
            builder.SqlCreate(role2);
        }
    }
}