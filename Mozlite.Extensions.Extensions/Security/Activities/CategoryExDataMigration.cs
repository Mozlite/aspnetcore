using Mozlite.Data.Migrations.Builders;

namespace Mozlite.Extensions.Security.Activities
{
    /// <summary>
    /// 数据库迁移类。
    /// </summary>
    public abstract class CategoryExDataMigration<TCategory> : CategoryDataMigration<TCategory>
        where TCategory : CategoryExBase, new()
    {
        /// <summary>
        /// 编辑表格其他属性列。
        /// </summary>
        /// <param name="table">当前表格构建实例对象。</param>
        protected override void Create(CreateTableBuilder<TCategory> table)
        {
            table.Column(x => x.SiteId);
        }
    }
}