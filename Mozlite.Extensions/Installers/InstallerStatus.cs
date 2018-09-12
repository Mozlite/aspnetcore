namespace Mozlite.Extensions.Installers
{
    /// <summary>
    /// 验证结果。
    /// </summary>
    public enum InstallerStatus
    {
        /// <summary>
        /// 初始化。
        /// </summary>
        Initializing,

        /// <summary>
        /// 安装配置。
        /// </summary>
        Setup,

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

        /// <summary>
        /// 未注册。
        /// </summary>
        Unregister,
    }
}