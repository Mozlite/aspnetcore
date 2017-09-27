namespace Mozlite.Data.Migrations.Models
{
    /// <summary>
    /// 数据库迁移表格迁移类型。
    /// </summary>
    internal class CoreDataMigration : DataMigration<Migration>
    {
        /// <summary>
        /// 当模型建立时候构建的表格实例。
        /// </summary>
        /// <param name="builder">迁移实例对象。</param>
        protected override void Create(MigrationBuilder<Migration> builder)
        {
            builder.CreateTable(table => table
                .Column(c => c.Id)
                .Column(c => c.Version)
           );
        }
    }
}