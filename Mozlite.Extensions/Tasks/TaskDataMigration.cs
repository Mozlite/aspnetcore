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
                .Column(s => s.Argument)
                .Column(s => s.ExtensionName)
                .Column(s => s.Type)
                .Column(s => s.Interval)
                .Column(s => s.Enabled)
                .Column(s => s.LastExecuted)
                .Column(s => s.NextExecuting));
        }
    }
}