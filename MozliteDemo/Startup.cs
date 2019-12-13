using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Mozlite;
using Mozlite.Data.SqlServer;
using Mozlite.Mvc.Apis;

namespace MozliteDemo
{
    /// <summary>
    /// 启动类。
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// 初始化类<see cref="Startup"/>。
        /// </summary>
        /// <param name="configuration">配置实例。</param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// 配置接口实例。
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// 配置IOC服务。
        /// </summary>
        /// <param name="services">服务实例对象。</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMozlite(Configuration)
                    .AddSqlServer();
            services.AddRazorPages()
                .AddApis();
        }

        /// <summary>
        /// 配置管道环境。
        /// </summary>
        /// <param name="app">应用程序构建实例。</param>
        /// <param name="env">环境变量。</param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();

            //下面两个位置一定要放对
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseMozlite(Configuration);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }
    }
}
