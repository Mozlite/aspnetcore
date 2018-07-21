namespace Demo.Extensions.Security.ViewModels
{
    /// <summary>
    /// 修改密码。
    /// </summary>
    public class ChangePasswordViewModel
    {
        /// <summary>
        /// 原始密码。
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 新密码。
        /// </summary>
        public string NewPassword { get; set; }

        /// <summary>
        /// 确认密码。
        /// </summary>
        public string ConfirmPassword { get; set; }
    }
}