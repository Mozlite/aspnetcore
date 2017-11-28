using Mozlite.Data.Migrations;

namespace Mozlite.Extensions.Security.Activities
{
    /// <summary>
    /// 用户活动数据迁移。
    /// </summary>
    public class UserActivityDataMiration : DataMigration
    {
        /// <summary>
        /// 当模型建立时候构建的表格实例。
        /// </summary>
        /// <param name="builder">迁移实例对象。</param>
        public override void Create(MigrationBuilder builder)
        {
            builder.CreateTable<UserActivity>(table => table.Column(x => x.Id)
                .Column(x => x.Activity)
                .Column(x => x.CreatedDate)
                .Column(x => x.IPAdress)
                .Column(x => x.UserId));
        }
    }
}