using Mozlite.Data.Migrations;
using Mozlite.Data.Migrations.Builders;

namespace Mozlite.Extensions.Security.Models
{
    /// <summary>
    /// 数据迁移。
    /// </summary>
    public class SecurityDataMigration : IdentityDataMigration<User, Role, UserClaim, RoleClaim, UserLogin, UserRole, UserToken>
    {
        /// <summary>
        /// 添加用户定义列。
        /// </summary>
        /// <param name="builder">用户表格定义实例。</param>
        protected override void Create(CreateTableBuilder<User> builder)
        {
            builder.Column(x => x.Score)
                .Column(x => x.ConcurrencyStamp, "timestamp");
        }

        public void Up2(MigrationBuilder builder)
        {
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
        }

        //public void Up3(MigrationBuilder builder)
        //{
        //    var user = new AdminUser();
        //    user.UserName = "UserAdmin";
        //    user.NormalizedUserName = user.UserName.ToUpper();
        //    user.PasswordHash = "";
        //    builder.SqlCreate(user);
        //}
    }
}