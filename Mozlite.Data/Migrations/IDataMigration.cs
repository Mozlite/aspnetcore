namespace Mozlite.Data.Migrations
{
    /// <summary>
    /// 数据迁移接口。
    /// </summary>
    public interface IDataMigration : IServices
    {
        /// <summary>
        /// 当模型建立时候构建的表格实例。
        /// </summary>
        /// <param name="builder">迁移实例对象。</param>
        void Create(MigrationBuilder builder);

        /// <summary>
        /// 销毁数据表。
        /// </summary>
        /// <param name="builder">迁移实例对象。</param>
        void Destroy(MigrationBuilder builder);

        /// <summary>
        /// 优先级，在两个迁移数据需要先后时候使用。
        /// </summary>
        int Priority { get; }
    }
}