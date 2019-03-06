using Mozlite.Data.Migrations;
using Mozlite.Data.Migrations.Builders;
using Mozlite.Extensions.Categories;

namespace Mozlite.Extensions.Security.Events
{
    /// <summary>
    /// 事件数据库迁移类。
    /// </summary>
    public class EventDataMigration : CategoryDataMigration<EventType>
    {
        /// <summary>
        /// 编辑表格其他属性列。
        /// </summary>
        /// <param name="table">当前表格构建实例对象。</param>
        protected override void Create(CreateTableBuilder<EventType> table)
        {
            table.Column(x => x.BgColor).Column(x => x.Color).Column(x => x.IconUrl);
        }

        /// <summary>
        /// 当模型建立时候构建的表格实例。
        /// </summary>
        /// <param name="builder">迁移实例对象。</param>
        public override void Create(MigrationBuilder builder)
        {
            base.Create(builder);
            builder.CreateTable<EventMessage>(table => table
                .Column(x => x.Id)
                .Column(x => x.EventId)
                .Column(x => x.UserId)
                .Column(x => x.IPAdress)
                .Column(x => x.CreatedDate)
                .Column(x => x.ExtendProperties)
                .ForeignKey<EventType>(x => x.EventId, x => x.Id, onDelete: ReferentialAction.Cascade)
            );
        }
    }
}