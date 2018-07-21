namespace Demo.Extensions.Security.ViewModels
{
    /// <summary>
    /// 登陆用户。
    /// </summary>
    public class LoginUser
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
        /// 验证码。
        /// </summary>
        public string VCode { get; set; }

        /// <summary>
        /// 是否记住登陆状态。
        /// </summary>
        public bool IsRemembered { get; set; }
    }
}