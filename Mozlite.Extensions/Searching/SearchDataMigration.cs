using Mozlite.Data.Migrations;

namespace Mozlite.Extensions.Searching
{
    /// <summary>
    /// 搜索索引数据迁移。
    /// </summary>
    public abstract class SearchDataMigration : DataMigration
    {
        /// <summary>
        /// 当模型建立时候构建的表格实例。
        /// </summary>
        /// <param name="builder">迁移实例对象。</param>
        public override void Create(MigrationBuilder builder)
        {
            builder.CreateTable<SearchDescriptor>(table => table
                .Column(x => x.Id)
                .Column(x => x.IndexedDate)
                .Column(x => x.ProviderName, nullable: false)
                .Column(x => x.Summary)
                .Column(x => x.TargetId)
                .UniqueConstraint(x => new { x.ProviderName, x.TargetId })
            );

            builder.CreateTable<SearchIndex>(table => table
                .Column(x => x.Id)
                .Column(x => x.Name)
                .Column(x => x.Priority)
            );

            builder.CreateTable<SearchInIndex>(table => table
                .Column(x => x.SearchId)
                .Column(x => x.IndexId)
                .ForeignKey<SearchDescriptor>(x => x.SearchId, x => x.Id, onDelete: ReferentialAction.Cascade)
            );
        }
    }
}