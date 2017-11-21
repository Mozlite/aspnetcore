using Mozlite.Data.Migrations;

namespace Mozlite.Extensions.Categories
{
    /// <summary>
    /// 分组数据迁移基类。
    /// </summary>
    /// <typeparam name="TGroup">分组类型。</typeparam>
    public abstract class GroupDataMigration<TGroup> : CategoryDataMigration<TGroup> where TGroup : GroupBase<TGroup>
    {
        /// <summary>
        /// 当模型建立时候构建的表格实例。
        /// </summary>
        /// <param name="builder">迁移实例对象。</param>
        public override void Create(MigrationBuilder builder)
        {
            builder.CreateTable<TGroup>(table =>
            {
                table.Column(x => x.Id)
                    .Column(x => x.Name)
                    .Column(x => x.ParentId);
                Create(table);
                table.UniqueConstraint(x => new { x.ParentId, x.Name });
            });
        }
    }
}