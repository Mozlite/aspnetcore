using Mozlite.Data.Migrations.Builders;

namespace Mozlite.Extensions.Extensions.Categories
{
    /// <summary>
    /// 分类数据迁移基类。
    /// </summary>
    /// <typeparam name="TCategory">分类类型。</typeparam>
    public abstract class CategoryDataMigration<TCategory> : ObjectDataMigration<TCategory>
        where TCategory : CategoryBase
    {
        /// <summary>
        /// 添加列。
        /// </summary>
        /// <param name="table">表格构建实例。</param>
        protected override void Create(CreateTableBuilder<TCategory> table)
        {
            table.Column(x => x.Name)
                .UniqueConstraint(x => new { x.SiteId, x.Name });
        }
    }
}