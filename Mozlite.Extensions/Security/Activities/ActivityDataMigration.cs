using Mozlite.Data.Migrations;
using Mozlite.Data.Migrations.Builders;

namespace Mozlite.Extensions.Security.Activities
{
    /// <summary>
    /// 用户活动数据迁移。
    /// </summary>
    /// <typeparam name="TUserActivity">用户活动类型。</typeparam>
    public abstract class ActivityDataMigration<TUserActivity> : DataMigration
        where TUserActivity : UserActivity
    {
        /// <summary>
        /// 当模型建立时候构建的表格实例。
        /// </summary>
        /// <param name="builder">迁移实例对象。</param>
        public override void Create(MigrationBuilder builder)
        {
            builder.CreateTable<TUserActivity>(table =>
            {
                table
                    .Column(x => x.Id)
                    .Column(x => x.CategoryId)
                    .Column(x => x.Activity)
                    .Column(x => x.CreatedDate)
                    .Column(x => x.IPAdress)
                    .Column(x => x.UserId);
                Create(table);
            });
        }

        /// <summary>
        /// 添加其他列。
        /// </summary>
        /// <param name="table">表格构建实例。</param>
        protected virtual void Create(CreateTableBuilder<TUserActivity> table) { }
    }
}