using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Mozlite.Extensions.Security.Stores;

namespace Mozlite.Extensions.Security
{
    /// <summary>
    /// 服务配置基类。
    /// </summary>
    /// <typeparam name="TUser">用户类型。</typeparam>
    /// <typeparam name="TRole">用户组类型。</typeparam>
    /// <typeparam name="TUserStore">用户存储。</typeparam>
    /// <typeparam name="TRoleStore">用户组存储。</typeparam>
    /// <typeparam name="TUserManager">用户管理类。</typeparam>
    /// <typeparam name="TRoleManager">用户组管理类。</typeparam>
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
        /// 配置服务方法。
        /// </summary>
        /// <param name="services">服务集合实例。</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddIdentity<TUser, TRole>()
                .AddUserStore<TUserStore>()
                .AddRoleStore<TRoleStore>()
                .AddUserManager<TUserManager>()
                .AddRoleManager<TRoleManager>()
                .AddErrorDescriber<SecurityErrorDescriptor>()
                .AddDefaultTokenProviders();
            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = new PathString("/login");
                options.LogoutPath = new PathString("/logout");
                options.AccessDeniedPath = new PathString("/denied");
                options.ReturnUrlParameter = "url";
            })
            .Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => false;//是否开启GDPR
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
            services.AddAuthentication();
            ConfigureSecurityServices(services);
        }

        /// <summary>
        /// 配置服务。
        /// </summary>
        /// <param name="services">服务集合。</param>
        protected abstract void ConfigureSecurityServices(IServiceCollection services);
    }
}