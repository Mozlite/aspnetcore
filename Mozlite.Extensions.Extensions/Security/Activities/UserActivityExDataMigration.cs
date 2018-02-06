using Mozlite.Data.Migrations;
using Mozlite.Data.Migrations.Builders;

namespace Mozlite.Extensions.Security.Activities
{
    /// <summary>
    /// 用户活动数据迁移。
    /// </summary>
    /// <typeparam name="TUserActivity">用户活动类型。</typeparam>
    public abstract class UserActivityExDataMigration<TUserActivity> : UserActivityDataMigration<TUserActivity>
        where TUserActivity : UserActivityEx
    {
        /// <summary>
        /// 当模型建立时候构建的表格实例。
        /// </summary>
        /// <param name="builder">迁移实例对象。</param>
        public override void Create(MigrationBuilder builder)
        {
            base.Create(builder);
            builder.CreateIndex<UserActivityEx>(x => x.SiteId);
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