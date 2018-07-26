using Mozlite.Data.Migrations;

namespace Mozlite.Mvc.Apis
{
    /// <summary>
    /// 应用程序数据库迁移类。
    /// </summary>
    public abstract class ApiDataMigration : DataMigration
    {
        /// <summary>
        /// 当模型建立时候构建的表格实例。
        /// </summary>
        /// <param name="builder">迁移实例对象。</param>
        public override void Create(MigrationBuilder builder)
        {
            builder.CreateTable<Application>(table =>
            {
                table.Column(x => x.Id)
                    .Column(x => x.AppSecret)
                    .Column(x => x.Name)
                    .Column(x => x.Token)
                    .Column(x => x.ExpiredDate)
                    .Column(x => x.CreatedDate)
                    .Column(x => x.ExtendProperties);
            });
        }
    }
}