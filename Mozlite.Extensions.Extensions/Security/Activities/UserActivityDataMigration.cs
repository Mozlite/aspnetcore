using Mozlite.Data.Migrations;
using Mozlite.Data.Migrations.Builders;

namespace Mozlite.Extensions.Extensions.Security.Activities
{
    /// <summary>
    /// 用户活动数据迁移。
    /// </summary>
    /// <typeparam name="TUserActivity">用户活动类型。</typeparam>
    public abstract class UserActivityDataMigration<TUserActivity> : Mozlite.Extensions.Security.Activities.UserActivityDataMigration<TUserActivity>
        where TUserActivity : UserActivity
    {
        /// <summary>
        /// 当模型建立时候构建的表格实例。
        /// </summary>
        /// <param name="builder">迁移实例对象。</param>
        public override void Create(MigrationBuilder builder)
        {
            base.Create(builder);
            builder.CreateIndex<UserActivity>(x => x.SiteId);
        }

        /// <summary>
        /// 添加其他列。
        /// </summary>
        /// <param name="table">表格构建实例。</param>
        protected override void Create(CreateTableBuilder<TUserActivity> table)
        {
            table.Column(x => x.SiteId);
        }
    }
}