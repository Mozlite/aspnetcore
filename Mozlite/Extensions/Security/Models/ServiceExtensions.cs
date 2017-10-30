using Microsoft.AspNetCore.Identity;

namespace Mozlite.Extensions.Security.Models
{
    /// <summary>
    /// 服务扩展类。
    /// </summary>
    public static class ServiceExtensions
    {
        /// <summary>
        /// 注册Identity容器，实现用户和用户组的数据库存储功能。
        /// </summary>
        /// <param name="builder">容器注册辅助实例。</param>
        /// <returns>返回当前实例。</returns>
        public static IdentityBuilder AddSecurity(this IdentityBuilder builder)
        {
           return builder.AddIdentityStores<UserStore, RoleStore>();
        }
    }
}