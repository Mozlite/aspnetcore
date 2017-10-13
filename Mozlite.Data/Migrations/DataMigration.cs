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
        public virtual void Destroy(MigrationBuilder builder) { }

        /// <summary>
        /// 优先级，在两个迁移数据需要先后时候使用。
        /// </summary>
        public virtual int Priority => 0;

        /// <summary>
        /// 日志接口。
        /// </summary>
        protected internal ILogger Logger { get; internal set; }
    }
}