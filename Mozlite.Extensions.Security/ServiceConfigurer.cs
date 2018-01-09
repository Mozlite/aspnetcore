using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Mozlite.Extensions.Security.Models;
using Mozlite.Extensions.Security.Services;
using Mozlite.Extensions.Settings;

namespace Mozlite.Extensions.Security
{
    /// <summary>
    /// 注册当前登陆用户。
    /// </summary>
    public class ServiceConfigurer : IServiceConfigurer
    {
        /// <summary>
        /// 配置服务方法。
        /// </summary>
        /// <param name="services">服务集合实例。</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddIdentity<User, Role>()
                .AddIdentityStores<UserStore, RoleStore>()
                .AddDefaultTokenProviders();
            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = new PathString("/login");
                options.LogoutPath = new PathString("/logout");
                options.ReturnUrlParameter = "redirectUrl";
            });
            services.AddAuthentication();
            services.Configure<IdentityOptions>(options =>
                {
                    //密码验证
                    var password = new PasswordOptions();
                    password.RequireDigit = false;
                    password.RequireLowercase = false;
                    password.RequireUppercase = false;
                    password.RequireNonAlphanumeric = false;
                    password.RequiredLength = 6;
                    options.Password = password;
                    //用户配置
                    var user = new UserOptions();
                    user.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_";
                    user.RequireUniqueEmail = true;
                    options.User = user;
                    //需要激活电子邮件
                    options.SignIn.RequireConfirmedEmail = true;
                })
                .AddScoped(service => service.GetRequiredService<IUserManager>().GetUser())
                .AddScoped(service =>
                {
                    var userManager = service.GetRequiredService<IUserManager>();
                    var user = userManager.GetUser();
                    return userManager.GetProfile(user);
                })
                .AddScoped(service => service.GetRequiredService<ISettingsManager>().GetSettings<SecuritySettings>());
        }
    }
}