namespace Mozlite.Extensions.Security
{
    /// <summary>
    /// 缓存用户实例。
    /// </summary>
    public class CachedUser
    {
        /// <summary>
        /// 用户Id。
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 用户名称。
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 头像。
        /// </summary>
        public string Avatar { get; set; }

        /// <summary>
        /// 登录名称。
        /// </summary>
        public string LoginName => NormalizedUserName?.ToLower();

        /// <summary>
        /// 角色名称。
        /// </summary>
        public int RoleId { get; set; }

        /// <summary>
        /// 角色名称。
        /// </summary>
        public string RoleName { get; set; }

        /// <summary>
        /// 登录名称。
        /// </summary>
        public string NormalizedUserName { get; set; }
    }
}