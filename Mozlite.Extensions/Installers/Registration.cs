using System;

namespace Mozlite.Extensions.Installers
{
    /// <summary>
    /// 注册码。
    /// </summary>
    public class Registration
    {
        /// <summary>
        /// 用户名。
        /// </summary>
        public string UserName { get; set; } = "mozlite";

        /// <summary>
        /// 注册码。
        /// </summary>
        public string Password { get; set; } = Cores.Md5("mozltie for aspnetcore");

        /// <summary>
        /// 过期时间。
        /// </summary>
        public DateTimeOffset Expired { get; set; } = DateTimeOffset.MaxValue;

        /// <summary>
        /// 安装状态。
        /// </summary>
        public InstallerStatus Status { get; set; }
    }
}