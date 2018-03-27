using System;
using Mozlite.Data.Migrations;
using Mozlite.Data.Migrations.Builders;
using Mozlite.Extensions.Groups;

namespace Mozlite.Extensions.Documents
{
    /// <summary>
    /// 数据迁移类。
    /// </summary>
    public class CategoryDataMigration : GroupDataMigration<Category>
    {
        /// <summary>
        /// 编辑表格其他属性列。
        /// </summary>
        /// <param name="table">当前表格构建实例对象。</param>
		protected override void Create(CreateTableBuilder<Category> table)
        {
            table.Column(x => x.Title)
                 .Column(x => x.Description)
                 .Column(x => x.Order);
        }
	}
}
