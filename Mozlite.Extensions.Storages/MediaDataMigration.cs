using Mozlite.Data.Migrations;
using Mozlite.Extensions.Storages.Caching;

namespace Mozlite.Extensions.Storages
{
    /// <summary>
    /// 媒体文件存储数据库迁移类。
    /// </summary>
    public class MediaDataMigration : DataMigration
    {
        /// <summary>
        /// 当模型建立时候构建的表格实例。
        /// </summary>
        /// <param name="builder">迁移实例对象。</param>
        public override void Create(MigrationBuilder builder)
        {
            builder.CreateTable<StoredFile>(table => table
                .Column(x => x.FileId)
                .Column(x => x.Length)
                .Column(x => x.ContentType, nullable: false)
            );

            builder.CreateTable<MediaFile>(table => table
                .Column(x => x.Id)
                .Column(x => x.CreatedDate)
                .Column(x => x.Name)
                .Column(x => x.Extension)
                .Column(x => x.ExtensionName)
                .Column(x => x.FileId)
                .Column(x => x.TargetId)
                .ForeignKey<StoredFile>(x => x.FileId, x => x.FileId)
            );

            builder.CreateIndex<MediaFile>(x => x.FileId);

            builder.CreateTable<StorageCache>(table => table
                .Column(x => x.CacheKey)
                .Column(x => x.ExpiredDate)
                .Column(x => x.Dependency)
                .Column(x => x.CreatedDate)
            );
        }
    }
}