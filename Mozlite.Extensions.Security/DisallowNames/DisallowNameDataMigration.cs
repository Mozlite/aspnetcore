using Mozlite.Data.Migrations;

namespace Mozlite.Extensions.Security.DisallowNames
{
    /// <summary>
    /// 禁用名称数据迁移类。
    /// </summary>
    public class DisallowNameDataMigration : DataMigration
    {
        /// <summary>
        /// 当模型建立时候构建的表格实例。
        /// </summary>
        /// <param name="builder">迁移实例对象。</param>
        public override void Create(MigrationBuilder builder)
        {
            builder.CreateTable<DisallowName>(table => table.Column(x => x.Id).Column(x => x.Name));
        }
    }
}