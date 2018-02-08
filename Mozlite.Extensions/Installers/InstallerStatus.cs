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