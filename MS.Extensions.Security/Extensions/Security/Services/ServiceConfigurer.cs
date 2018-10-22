using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Mozlite.Extensions.Security;

namespace MS.Extensions.Security.Services
{
    /// <summary>
    /// 注册当前登陆用户。
    /// </summary>
    public class ServiceConfigurer : ServiceConfigurer<User, Role, UserStore, RoleStore, UserManager, RoleManager>
    {
        /// <summary>
        /// 配置服务。
        /// </summary>
        /// <param name="services">服务集合。</param>
        /// <param name="configuration">配置接口。</param>
        protected override void ConfigureSecurityServices(IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<IdentityOptions>(options =>
            {
                //锁定配置
                options.Lockout = new LockoutOptions
                {
                    AllowedForNewUsers = true,
                    DefaultLockoutTimeSpan = TimeSpan.FromHours(1),
                    MaxFailedAccessAttempts = 5
                };
                //用户配置
                options.User = new UserOptions
                {
                    AllowedUserNameCharacters = null,
                    RequireUniqueEmail = false
                };
                //需要激活电子邮件
                options.SignIn.RequireConfirmedEmail = false;
            })
            .AddScoped(service => service.GetRequiredService<IUserManager>().GetUser() ?? _anonymous);
        }

        private static readonly User _anonymous = new User { UserName = "Anonymous" };
    }
}