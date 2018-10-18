using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Mozlite.Extensions.Security.Stores;

namespace Mozlite.Extensions.Security
{
    /// <summary>
    /// 服务配置基类。
    /// </summary>
    /// <typeparam name="TUser">用户类型。</typeparam>
    /// <typeparam name="TRole">角色类型。</typeparam>
    /// <typeparam name="TUserStore">用户存储。</typeparam>
    /// <typeparam name="TRoleStore">角色存储。</typeparam>
    /// <typeparam name="TUserManager">用户管理类。</typeparam>
    /// <typeparam name="TRoleManager">角色管理类。</typeparam>
    public abstract class ServiceConfigurer<TUser, TRole, TUserStore, TRoleStore, TUserManager, TRoleManager>
        : IServiceConfigurer
        where TUser : UserBase
        where TRole : RoleBase
        where TUserStore : class, IUserStore<TUser>
        where TRoleStore : class, IRoleStore<TRole>
        where TUserManager : UserManager<TUser>
        where TRoleManager : RoleManager<TRole>
    {
        /// <summary>
        /// 配置服务。
        /// </summary>
        /// <param name="services">服务集合。</param>
        /// <param name="configuration">配置接口。</param>
        protected abstract void ConfigureSecurityServices(IServiceCollection services, IConfiguration configuration);

        /// <summary>
        /// 配置服务方法。
        /// </summary>
        /// <param name="services">服务集合实例。</param>
        /// <param name="configuration">配置接口。</param>
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddIdentity<TUser, TRole>()
                .AddUserStore<TUserStore>()
                .AddRoleStore<TRoleStore>()
                .AddUserManager<TUserManager>()
                .AddRoleManager<TRoleManager>()
                .AddErrorDescriber<SecurityErrorDescriptor>()
                .AddDefaultTokenProviders();
            var config = configuration.GetSection("User");
            services.ConfigureApplicationCookie(options =>
                {
                    options.LoginPath = new PathString(config["Login"] ?? "/login");
                    options.LogoutPath = new PathString(config["Logout"] ?? "/logout");
                    options.AccessDeniedPath = new PathString(config["Denied"] ?? "/denied");
                    options.ReturnUrlParameter = config["ReturnUrl"] ?? "returnUrl";
                })
                .Configure<CookiePolicyOptions>(options =>
                {
                    // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                    options.CheckConsentNeeded = context => false;//是否开启GDPR
                    options.MinimumSameSitePolicy = SameSiteMode.None;
                });
            services.AddAuthentication();
            ConfigureSecurityServices(services, configuration);
        }
    }
}