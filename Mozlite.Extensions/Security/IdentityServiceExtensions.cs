using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Mozlite.Extensions.Security
{
    /// <summary>
    /// Identity相关的服务扩展类。
    /// </summary>
    public static class IdentityServiceExtensions
    {
        /// <summary>
        /// 添加Mozlite框架存储服务。
        /// </summary>
        /// <typeparam name="TUserClaim">用户声明类型。</typeparam>
        /// <typeparam name="TUserLogin">用户登入声明类型。</typeparam>
        /// <typeparam name="TUserRole">用户所在组声明类型。</typeparam>
        /// <typeparam name="TRoleClaim">用户组声明类型。</typeparam>
        /// <typeparam name="TUserToken">用户标识。</typeparam>
        /// <param name="builder">Identity构建实例。</param>
        /// <returns>返回Identity构建实例。</returns>
        public static IdentityBuilder AddIdentityStores<TUserClaim, TRoleClaim, TUserRole, TUserLogin, TUserToken>(this IdentityBuilder builder)
            where TUserClaim : IdentityUserClaim, new()
            where TRoleClaim : IdentityRoleClaim, new()
            where TUserRole : IdentityUserRole, new()
            where TUserLogin : IdentityUserLogin, new()
            where TUserToken : IdentityUserToken, new()
        {
            var userStoreType = typeof(IdentityUserStore<,,,,,>).MakeGenericType(builder.UserType, builder.RoleType, typeof(TUserClaim), typeof(TUserLogin), typeof(TUserRole), typeof(TUserToken));
            var roleStoreType = typeof(IdentityIdentityRoleStore<,>).MakeGenericType(builder.RoleType, typeof(TRoleClaim));
            return builder.AddIdentityStores(userStoreType, roleStoreType);
        }

        /// <summary>
        /// 添加Mozlite框架存储服务。
        /// </summary>
        /// <param name="builder">Identity构建实例。</param>
        /// <typeparam name="TUserStore">用户存储类型。</typeparam>
        /// <typeparam name="TRoleStore">用户组存储类型。</typeparam>
        /// <returns>返回Identity构建实例。</returns>
        public static IdentityBuilder AddIdentityStores<TUserStore, TRoleStore>(this IdentityBuilder builder)
        {
            return builder.AddIdentityStores(typeof(TUserStore), typeof(TRoleStore));
        }

        /// <summary>
        /// 添加Mozlite框架存储服务。
        /// </summary>
        /// <param name="builder">Identity构建实例。</param>
        /// <param name="userStoreType">用户存储类型。</param>
        /// <param name="roleStoreType">用户组存储类型。</param>
        /// <returns>返回Identity构建实例。</returns>
        public static IdentityBuilder AddIdentityStores(this IdentityBuilder builder, Type userStoreType, Type roleStoreType)
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
    }
}