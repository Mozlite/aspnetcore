using Mozlite.Data.Migrations;

namespace Mozlite.Extensions.Tasks
{
    /// <summary>
    /// 后台服务数据迁移类型。
    /// </summary>
    public class TaskMigration : DataMigration
    {
        /// <summary>
        /// 创建操作。
        /// </summary>
        /// <param name="builder">迁移构建实例对象。</param>
        public override void Create(MigrationBuilder builder)
        {
            builder.CreateTable<TaskDescriptor>(table => table
                .Column(s => s.Id)
                .Column(s => s.Name)
                .Column(s => s.Description)
                .Column(s => s.DependenceArgument)
                .Column(s => s.ExtensionName)
                .Column(s => s.Type)
                .Column(s => s.Interval)
                .Column(s => s.LastExecuted)
                .Column(s => s.NextExecuting));

            builder.CreateTable<TaskArgument>(table => table
                .Column(x => x.Id)
                .Column(x => x.TaskId)
                .Column(x => x.ExtensionName)
                .Column(x => x.TryTimes)
                .Column(x => x.Status)
                .Column(x => x.LastExecuted)
                .Column(x => x.Argument, nullable: false)
                .Column(x => x.Error)
                .ForeignKey<TaskDescriptor>(x => x.TaskId, x => x.Id, onDelete: ReferentialAction.Cascade)
            );

            builder.CreateIndex<TaskArgument>(x => new { x.ExtensionName, x.Status });
        }

        /// <summary>
        /// 销毁数据表。
        /// </summary>
        /// <param name="builder">迁移实例对象。</param>
        public override void Destroy(MigrationBuilder builder)
        {
            builder.DropTable<TaskArgument>();
            base.Destroy(builder);
        }
    }
}