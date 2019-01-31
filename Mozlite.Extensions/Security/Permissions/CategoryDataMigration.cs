using Mozlite.Data.Migrations.Builders;
using Mozlite.Extensions.Categories;

namespace Mozlite.Extensions.Security.Permissions
{
    /// <summary>
    /// 权限分类数据库迁移类。
    /// </summary>
    public class CategoryDataMigration : CategoryDataMigration<Category>
    {
        /// <summary>
        /// 编辑表格其他属性列。
        /// </summary>
        /// <param name="table">当前表格构建实例对象。</param>
        protected override void Create(CreateTableBuilder<Category> table)
        {
            table.Column(x => x.Text).Column(x => x.Disabled);
        }
    }
}