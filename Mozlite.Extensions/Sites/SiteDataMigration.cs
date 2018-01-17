using Mozlite.Data.Migrations;
using Mozlite.Data.Migrations.Builders;

namespace Mozlite.Extensions.Sites
{
    /// <summary>
    /// 网站信息实例数据迁移类。
    /// </summary>
    public abstract class SiteDataMigration : DataMigration
    {
        /// <summary>
        /// 当模型建立时候构建的表格实例。
        /// </summary>
        /// <param name="builder">迁移实例对象。</param>
        public override void Create(MigrationBuilder builder)
        {
            builder.CreateTable<SiteAdapter>(table =>
            {
                table.Column(x => x.SiteId)
                    .Column(x => x.SiteName)
                    .Column(x => x.UpdatedDate)
                    .Column(x => x.SettingValue);
                Create(table);
            });
            builder.CreateTable<SiteDomain>(table =>
            {
                table.Column(x => x.SiteId)
                    .Column(x => x.Domain)
                    .Column(x => x.IsDefault)
                    .Column(x => x.Disabled)
                    .ForeignKey<SiteAdapter>(x => x.SiteId, x => x.SiteId,
                        onDelete: ReferentialAction.Cascade);
                Create(table);
            });
        }

        /// <summary>
        /// 添加网站信息实例列。
        /// </summary>
        /// <param name="table">构建表格实例。</param>
        protected virtual void Create(CreateTableBuilder<SiteAdapter> table) { }

        /// <summary>
        /// 添加网站域名列。
        /// </summary>
        /// <param name="table">构建表格实例。</param>
        protected virtual void Create(CreateTableBuilder<SiteDomain> table) { }
    }
}