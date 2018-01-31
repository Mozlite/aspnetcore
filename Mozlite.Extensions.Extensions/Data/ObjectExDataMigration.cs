using Mozlite.Data.Migrations;
using Mozlite.Data.Migrations.Builders;

namespace Mozlite.Extensions.Data
{
    /// <summary>
    /// 数据迁移基类。
    /// </summary>
    /// <typeparam name="TObject">当前对象类型。</typeparam>
    /// <typeparam name="TKey">Id类型。</typeparam>
    public abstract class ObjectExDataMigration<TObject, TKey> : DataMigration
        where TObject : IIdSiteObject<TKey>
    {
        /// <summary>
        /// 当模型建立时候构建的表格实例。
        /// </summary>
        /// <param name="builder">迁移实例对象。</param>
        public override void Create(MigrationBuilder builder)
        {
            builder.CreateTable<TObject>(table =>
            {
                table.Column(x => x.Id)
                    .Column(x => x.SiteId);
                Create(table);
            });

            //建立索引
            builder.CreateIndex<TObject>(x => x.SiteId, true);
        }

        /// <summary>
        /// 添加列。
        /// </summary>
        /// <param name="table">表格构建实例。</param>
        protected abstract void Create(CreateTableBuilder<TObject> table);
    }

    /// <summary>
    /// 数据迁移基类。
    /// </summary>
    /// <typeparam name="TObject">当前对象类型。</typeparam>
    public abstract class ObjectExDataMigration<TObject> : ObjectExDataMigration<TObject, int> where TObject : IIdSiteObject
    {
    }
}