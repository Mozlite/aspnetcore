using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Mozlite.Extensions.Security.Activities;

namespace Mozlite.Extensions.Security
{
    /// <summary>
    /// Identity相关的服务扩展类。
    /// </summary>
    public static class ServiceExtensions
    {
        /// <summary>
        /// 添加Mozlite框架存储服务。
        /// </summary>
        /// <param name="builder">Identity构建实例。</param>
        /// <typeparam name="TUserStore">用户存储类型。</typeparam>
        /// <typeparam name="TRoleStore">用户组存储类型。</typeparam>
        /// <returns>返回Identity构建实例。</returns>
        public static IdentityBuilder AddSecurityStores<TUserStore, TRoleStore>(this IdentityBuilder builder)
        {
            return builder.AddSecurityStores(typeof(TUserStore), typeof(TRoleStore));
        }

        /// <summary>
        /// 添加Mozlite框架存储服务。
        /// </summary>
        /// <param name="builder">Identity构建实例。</param>
        /// <param name="userStoreType">用户存储类型。</param>
        /// <param name="roleStoreType">用户组存储类型。</param>
        /// <returns>返回Identity构建实例。</returns>
        public static IdentityBuilder AddSecurityStores(this IdentityBuilder builder, Type userStoreType, Type roleStoreType)
        {
            var services = new ServiceCollection();
            services.AddScoped(
                typeof(IUserStore<>).MakeGenericType(builder.UserType),
                userStoreType);
            services.AddScoped(
                typeof(IRoleStore<>).MakeGenericType(builder.RoleType),
                roleStoreType);
            builder.Services.TryAdd(services);
            return builder;
        }

        /// <summary>
        /// 使用用户状态日志记录。
        /// </summary>
        /// <param name="app">应用程序构建实例。</param>
        /// <returns>返回应用程序构建实例。</returns>
        public static IApplicationBuilder UseUserActivity(this IApplicationBuilder app)
        {
            var factory = app.ApplicationServices.GetRequiredService<ILoggerFactory>();
            factory.AddProvider(new ActivityLoggerProvider(app.ApplicationServices));
            return app;
        }
    }
}