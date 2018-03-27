using Mozlite.Data.Migrations;
using BaseSettingsDataMigration = Mozlite.Extensions.Settings.SettingsDataMigration;

namespace Mozlite.Extensions.Extensions.Settings
{
    /// <summary>
    /// 数据库迁移。
    /// </summary>
    [Suppress(typeof(BaseSettingsDataMigration))]
    public class SettingsDataMigration : BaseSettingsDataMigration
    {
        /// <summary>
        /// 当模型建立时候构建的表格实例。
        /// </summary>
        /// <param name="builder">迁移实例对象。</param>
        public override void Create(MigrationBuilder builder)
        {
            builder.CreateTable<SettingsAdapter>(table => table
                .Column(x => x.SiteId)
                .Column(s => s.SettingKey)
                .Column(s => s.SettingValue)
            );
        }
    }
}