using Mozlite.Data.Migrations;

namespace Mozlite.Extensions.Sites
{
    /// <summary>
    /// 网站配置数据迁移类。
    /// </summary>
    public class SiteDataMigration : DataMigration
    {
        /// <summary>
        /// 当模型建立时候构建的表格实例。
        /// </summary>
        /// <param name="builder">迁移实例对象。</param>
        public override void Create(MigrationBuilder builder)
        {
            builder.CreateTable<SiteSettingsAdapter>(table => table.Column(x => x.SettingsId)
                .Column(x => x.SiteName)
                .Column(x => x.UpdatedDate)
                .Column(x => x.SettingsJSON)
            );
            builder.CreateTable<SiteDomain>(table => table.Column(x => x.SiteId)
                .Column(x => x.Domain)
                .ForeignKey<SiteSettingsAdapter>(x => x.SiteId, x => x.SettingsId, onDelete: ReferentialAction.Cascade)
            );
        }
    }
}