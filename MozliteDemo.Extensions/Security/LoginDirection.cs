namespace MozliteDemo.Extensions.Security
{
    /// <summary>
    /// 登录后的转向。
    /// </summary>
    public enum LoginDirection
    {
        /// <summary>
        /// 首页。
        /// </summary>
        Default,
        /// <summary>
        /// 用户中心。
        /// </summary>
        Account,
        /// <summary>
        /// 后台管理。
        /// </summary>
        Admin,
    }
}