using Mozlite.Data.Migrations;
using Mozlite.Data.Migrations.Builders;
using Mozlite.Extensions;
using MozliteDemo.Extensions.Security;

namespace MozliteDemo.Extensions.ProjectManager.Projects
{
    /// <summary>
    /// 数据库迁移类。
    /// </summary>
    public class ProjectDataMigration : ObjectDataMigration<Project>
    {
        protected override void Create(CreateTableBuilder<Project> table)
        {
            table.Column(x => x.Name)
                .Column(x => x.Summary)
                .Column(x => x.UserId)
                .Column(x => x.Enabled)
                .Column(x => x.CreatedDate);
        }

        public void Up1(MigrationBuilder builder)
        {
            builder.CreateTable<ProjectUser>(table => table
                .Column(x => x.Id)
                .ForeignKey<User>(x => x.Id, x => x.Id, onDelete: ReferentialAction.Cascade)
            );
        }
    }
}