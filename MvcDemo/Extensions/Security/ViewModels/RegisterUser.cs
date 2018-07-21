namespace Demo.Extensions.Security.ViewModels
{
    /// <summary>
    /// 创建帐户。
    /// </summary>
    public class RegisterUser
    {
        /// <summary>
        /// 用户名。
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 密码。
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 确认密码。
        /// </summary>
        public string ConfirmPassword { get; set; }

        /// <summary>
        /// 邮件地址。
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 验证码。
        /// </summary>
        public string VCode { get; set; }
    }
}