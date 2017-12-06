using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Mozlite.Extensions.Security.Models;
using Mozlite.Extensions.Security.Services;

namespace Mozlite.Extensions.Security
{
    /// <summary>
    /// 服务扩展类。
    /// </summary>
    public static class ServiceExtensions
    {
        /// <summary>
        /// 添加用户服务。
        /// </summary>
        /// <param name="services">服务集合。</param>
        /// <param name="action">用户服务操作。</param>
        /// <returns>返回服务集合实例对象。</returns>
        public static IServiceCollection AddIdentity(this IServiceCollection services, Action<IdentityBuilder> action = null)
        {
            var builder = services.AddIdentity<User, Role>()
                .AddIdentityStores<UserStore, RoleStore>()
                .AddDefaultTokenProviders();
            action?.Invoke(builder);
            return services;
        }

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