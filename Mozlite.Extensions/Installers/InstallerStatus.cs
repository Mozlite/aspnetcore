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
        Data,

        /// <summary>
        /// 数据迁移错误。
        /// </summary>
        DataError,

        /// <summary>
        /// 数据迁移完成。
        /// </summary>
        DataSuccess,

        /// <summary>
        /// 新产品。
        /// </summary>
        New,

        /// <summary>
        /// 配置。
        /// </summary>
        Config,

        /// <summary>
        /// 成功。
        /// </summary>
        Success,

        /// <summary>
        /// 过期。
        /// </summary>
        Expired,

        /// <summary>
        /// 注册码错误。
        /// </summary>
        Failured,
    }
}