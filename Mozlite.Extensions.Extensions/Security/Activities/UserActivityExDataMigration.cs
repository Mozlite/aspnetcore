using Mozlite.Data.Migrations;

namespace Mozlite.Extensions.Security.Activities
{
    /// <summary>
    /// 用户活动数据迁移。
    /// </summary>
    [Suppress(typeof(UserActivityDataMigration))]
    public class UserActivityExDataMigration : UserActivityDataMigration
    {
        /// <summary>
        /// 当模型建立时候构建的表格实例。
        /// </summary>
        /// <param name="builder">迁移实例对象。</param>
        public override void Create(MigrationBuilder builder)
        {
            base.Create(builder);
            builder.AddColumn<UserActivityEx>(x => x.SiteId);
            builder.CreateIndex<UserActivityEx>(x => x.SiteId);
        }
    }
}