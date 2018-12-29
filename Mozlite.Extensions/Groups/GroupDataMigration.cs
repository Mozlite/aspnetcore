using Mozlite.Data.Migrations;
using Mozlite.Data.Migrations.Builders;

namespace Mozlite.Extensions.Groups
{
    /// <summary>
    /// 分组数据迁移基类。
    /// </summary>
    /// <typeparam name="TGroup">分组类型。</typeparam>
    public abstract class GroupDataMigration<TGroup> : DataMigration
        where TGroup : GroupBase<TGroup>
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

        /// <summary>
        /// 编辑表格其他属性列。
        /// </summary>
        /// <param name="table">当前表格构建实例对象。</param>
        protected virtual void Create(CreateTableBuilder<TGroup> table) { }
    }
}