using Mozlite.Data.Migrations.Builders;
using Mozlite.Extensions.Categories;

namespace MozliteDemo.Extensions.ProjectManager.Milestones
{
    /// <summary>
    /// 数据库迁移类。
    /// </summary>
    public class MilestoneDataMigration : CategoryDataMigration<Milestone>
    {
        /// <summary>
        /// 编辑表格其他属性列。
        /// </summary>
        /// <param name="table">当前表格构建实例对象。</param>
        protected override void Create(CreateTableBuilder<Milestone> table)
        {
            table.Column(x => x.UserId)
                .Column(x => x.Completed)
                .Column(x => x.CompletedDate)
                .Column(x => x.CreatedDate)
                .Column(x => x.Issues)
                .Column(x => x.Operator)
                .Column(x => x.ProjectId)
                .Column(x => x.StartDate)
                .Column(x => x.Summary);
        }
    }
}