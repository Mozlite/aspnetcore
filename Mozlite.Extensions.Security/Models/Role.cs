namespace Mozlite.Extensions.Security.Models
{
    /// <summary>
    /// 用户组。
    /// </summary>
    public class Role : IdentityRole
    {
        /// <summary>
        /// 是否为系统角色。
        /// </summary>
        public bool IsSystem => Priority == 0 || Priority == int.MaxValue;
    }
}