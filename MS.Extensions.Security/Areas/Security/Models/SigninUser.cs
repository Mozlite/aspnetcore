using System.ComponentModel.DataAnnotations;

namespace MS.Areas.Security.Models
{
    /// <summary>
    /// 登陆用户模型。
    /// </summary>
    public class SigninUser
    {
        /// <summary>
        /// 用户名。
        /// </summary>
        [Required(ErrorMessage = "用户名不能为空!")]
        public string UserName { get; set; }

        /// <summary>
        /// 密码。
        /// </summary>
        [Required(ErrorMessage = "密码不能为空!")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        /// <summary>
        /// 验证码。
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 登陆状态。
        /// </summary>
        public bool RememberMe { get; set; }
    }
}