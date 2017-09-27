using Microsoft.Extensions.Logging;

namespace Mozlite.Data.Migrations
{
    /// <summary>
    /// 数据迁移基类。
    /// </summary>
    public abstract class DataMigration : IDataMigration
    {
        /// <summary>
        /// 当模型建立时候构建的表格实例。
        /// </summary>
        /// <param name="builder">迁移实例对象。</param>
        public abstract void Create(MigrationBuilder builder);

        /// <summary>
        /// 销毁数据表。
        /// </summary>
        /// <param name="builder">迁移实例对象。</param>
        public abstract void Destroy(MigrationBuilder builder);

        /// <summary>
        /// 优先级，在两个迁移数据需要先后时候使用。
        /// </summary>
        public virtual int Priority => 0;

        /// <summary>
        /// 日志接口。
        /// </summary>
        protected internal ILogger Logger { get; internal set; }
    }

    /// <summary>
    /// 数据库迁移基类。
    /// </summary>
    /// <typeparam name="TEntity">模型类型。</typeparam>
    public abstract class DataMigration<TEntity> : DataMigration
    {
        /// <summary>
        /// 当模型建立时候构建的表格实例。
        /// </summary>
        /// <param name="builder">迁移实例对象。</param>
        public override void Create(MigrationBuilder builder)
        {
            Create(new MigrationBuilder<TEntity>(builder));
        }

        /// <summary>
        /// 当模型建立时候构建的表格实例。
        /// </summary>
        /// <param name="builder">迁移实例对象。</param>
        protected abstract void Create(MigrationBuilder<TEntity> builder);

        /// <summary>
        /// 销毁数据表。
        /// </summary>
        /// <param name="builder">迁移实例对象。</param>
        public override void Destroy(MigrationBuilder builder)
        {
            builder.DropTable<TEntity>();
        }
    }
}