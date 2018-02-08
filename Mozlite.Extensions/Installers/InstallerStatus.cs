namespace Mozlite.Extensions.Installers
{
    /// <summary>
    /// 验证结果。
    /// </summary>
    public enum InstallerStatus
    {
        /// <summary>
        /// 数据迁移。
        /// </summary>
        DataMigration,

        /// <summary>
        /// 数据库完成。
        /// </summary>
        MigrationCompleted,

        /// <summary>
        /// 初始化。
        /// </summary>
        Initialize,

        /// <summary>
        /// 成功。
        /// </summary>
        Success,

        /// <summary>
        /// 过期。
        /// </summary>
        Expired,

        /// <summary>
        /// 失败。
        /// </summary>
        Failured,
    }
}