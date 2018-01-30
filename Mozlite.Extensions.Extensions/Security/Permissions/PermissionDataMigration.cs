using Mozlite.Data.Migrations;

namespace Mozlite.Extensions.Security.Permissions
{
    /// <summary>
    /// 权限数据库迁移类。
    /// </summary>
    public class PermissionDataMigration : DataMigration
    {
        /// <summary>
        /// 当模型建立时候构建的表格实例。
        /// </summary>
        /// <param name="builder">迁移实例对象。</param>
        public override void Create(MigrationBuilder builder)
        {
            builder.CreateTable<Permission>(table => table
                .Column(x => x.Id)
                .Column(x => x.Name)
                .Column(x => x.Description));

            builder.CreateTable<PermissionInRole>(table => table
                .Column(x => x.PermissionId)
                .Column(x => x.RoleId)
                .Column(x => x.Value)
                .ForeignKey<Permission>(x => x.PermissionId, x => x.Id, onDelete: ReferentialAction.Cascade));
        }
    }
}