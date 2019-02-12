using Mozlite.Data.Migrations;

namespace Mozlite.Extensions.Messages.SMS
{
    /// <summary>
    /// 短信数据库迁移类。
    /// </summary>
    public abstract class SmsDataMigration : DataMigration
    {
        /// <summary>
        /// 当模型建立时候构建的表格实例。
        /// </summary>
        /// <param name="builder">迁移实例对象。</param>
        public override void Create(MigrationBuilder builder)
        {
            builder.CreateTable<Note>(table => table
                .Column(x => x.Id)
                .Column(x => x.Client)
                .Column(x => x.Status)
                .Column(x => x.Message)
                .Column(x => x.PhoneNumber)
                .Column(x => x.CreatedDate)
                .Column(x => x.HashKey)
                .Column(x => x.TryTimes)
                .Column(x => x.Msg)
                .Column(x => x.SentDate)
            );
            builder.CreateIndex<Note>(x => x.HashKey);
            builder.CreateIndex<Note>(x => new { x.Status, x.CreatedDate });
            builder.CreateTable<SmsSettings>(table => table
                .Column(x => x.Id)
                .Column(x => x.Client)
                .Column(x => x.AppId)
                .Column(x => x.AppSecret)
                .Column(x => x.ExtendProperties)
                .UniqueConstraint(x => x.Client)
            );
        }
    }
}