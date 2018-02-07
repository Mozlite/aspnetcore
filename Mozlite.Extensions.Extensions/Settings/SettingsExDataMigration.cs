using Mozlite.Data.Migrations;

namespace Mozlite.Extensions.Settings
{
    /// <summary>
    /// 数据库迁移。
    /// </summary>
    [Suppress(typeof(SettingsDataMigration))]
    public class SettingsExDataMigration : SettingsDataMigration
    {
        /// <summary>
        /// 当模型建立时候构建的表格实例。
        /// </summary>
        /// <param name="builder">迁移实例对象。</param>
        public override void Create(MigrationBuilder builder)
        {
            builder.CreateTable<SettingsExAdapter>(table => table
                .Column(x => x.SiteId)
                .Column(s => s.SettingKey)
                .Column(s => s.SettingValue)
            );
        }
    }
}