using System;
using Demo.Extensions.Security.Stores;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Mozlite.Extensions.Security;

namespace Demo.Extensions.Security.Services
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
        protected override void ConfigureSecurityServices(IServiceCollection services)
        {
            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = new PathString("/login");
                options.LogoutPath = new PathString("/logout");
                options.ReturnUrlParameter = "redirectUrl";
            });
            services.AddAuthentication();
            services.Configure<IdentityOptions>(options =>
                {
                    //锁定配置
                    options.Lockout = new LockoutOptions
                    {
                        AllowedForNewUsers = true,
                        DefaultLockoutTimeSpan = TimeSpan.FromHours(1),
                        MaxFailedAccessAttempts = 5
                    };
                    //密码验证
                    //var password = new PasswordOptions();
                    //password.RequireDigit = false;
                    //password.RequireLowercase = false;
                    //password.RequireUppercase = false;
                    //password.RequireNonAlphanumeric = false;
                    //password.RequiredLength = 6;
                    //options.Password = password;
                    //用户配置
                    options.User = new UserOptions
                    {
                        AllowedUserNameCharacters = null,
                        RequireUniqueEmail = false
                    };
                    //需要激活电子邮件
                    options.SignIn.RequireConfirmedEmail = false;
                })
                .AddScoped(service => service.GetRequiredService<IUserManager>().GetUser());
        }
    }
}