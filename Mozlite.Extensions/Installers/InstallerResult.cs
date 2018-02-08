namespace Mozlite.Extensions.Installers
{
    /// <summary>
    /// 验证结果。
    /// </summary>
    public enum InstallerResult
    {
        /// <summary>
        /// 数据迁移。
        /// </summary>
        Data,

        /// <summary>
        /// 新产品。
        /// </summary>
        New,

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