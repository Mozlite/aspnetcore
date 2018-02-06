using Mozlite.Data.Migrations;
using Mozlite.Data.Migrations.Builders;

namespace Mozlite.Extensions.Categories
{
    /// <summary>
    /// 分类数据迁移基类。
    /// </summary>
    /// <typeparam name="TCategory">分类类型。</typeparam>
    public abstract class CategoryDataMigration<TCategory> : DataMigration
        where TCategory : CategoryBase
    {
        /// <summary>
        /// 当模型建立时候构建的表格实例。
        /// </summary>
        /// <param name="builder">迁移实例对象。</param>
        public override void Create(MigrationBuilder builder)
        {
            builder.CreateTable<TCategory>(table =>
            {
                table.Column(x => x.Id).Column(x => x.Name);
                Create(table);
                table.UniqueConstraint(x => x.Name);
            });
        }

        /// <summary>
        /// 编辑表格其他属性列。
        /// </summary>
        /// <param name="table">当前表格构建实例对象。</param>
        protected virtual void Create(CreateTableBuilder<TCategory> table) { }
    }
}