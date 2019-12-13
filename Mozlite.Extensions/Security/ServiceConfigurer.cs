using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
        /// <param name="builder">容器构建实例。</param>
        protected abstract void ConfigureSecurityServices(IMozliteBuilder builder);

        /// <summary>
        /// 配置<see cref="IdentityBuilder"/>实例。
        /// </summary>
        /// <param name="builder"><see cref="IdentityBuilder"/>实例。</param>
        protected virtual void ConfigureIdentityServices(IdentityBuilder builder)
        {
            builder.AddUserStore<TUserStore>()
                .AddRoleStore<TRoleStore>()
                .AddUserManager<TUserManager>()
                .AddRoleManager<TRoleManager>()
                .AddErrorDescriber<SecurityErrorDescriptor>()
                .AddDefaultTokenProviders();
        }

        /// <summary>
        /// 配置服务方法。
        /// </summary>
        /// <param name="builder">容器构建实例。</param>
        public void ConfigureServices(IMozliteBuilder builder)
        {
            builder.AddServices(services =>
            {
                var identityBuilder = services.AddIdentity<TUser, TRole>();
                ConfigureIdentityServices(identityBuilder);
                services.ConfigureApplicationCookie(options => Init(options, builder.Configuration.GetSection("User")))
                    .Configure<CookiePolicyOptions>(options =>
                    {
                        // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                        options.CheckConsentNeeded = context => false;//是否开启GDPR
                        options.MinimumSameSitePolicy = SameSiteMode.None;
                    });
                services.AddAuthentication();
            });
            ConfigureSecurityServices(builder);
        }

        /// <summary>
        /// 配置Cookie验证实例。
        /// </summary>
        /// <param name="options">Cookie验证选项。</param>
        /// <param name="section">用户配置节点。</param>
        protected virtual void Init(CookieAuthenticationOptions options, IConfigurationSection section)
        {
            options.LoginPath = new PathString(section["Login"] ?? "/login");
            options.LogoutPath = new PathString(section["Logout"] ?? "/logout");
            options.AccessDeniedPath = new PathString(section["Denied"] ?? "/denied");
            options.ReturnUrlParameter = section["ReturnUrl"] ?? "returnUrl";
        }
    }
}