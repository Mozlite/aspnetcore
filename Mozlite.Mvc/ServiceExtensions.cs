using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Mozlite.Mvc.Routing;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace Mozlite.Mvc
{
    /// <summary>
    /// 服务扩展类。
    /// </summary>
    public static class ServiceExtensions
    {
        /// <summary>
        /// 添加MVC服务。
        /// </summary>
        /// <param name="builder">当前服务集合构建实例。</param>
        /// <param name="action">MVC选项实例化方法。</param>
        /// <returns>返回MVC构建实例。</returns>
        public static IMozliteBuilder AddMozliteMvc(this IMozliteBuilder builder, Action<IMvcBuilder> action = null)
        {
            return builder.AddServices(services =>
            {
                //防止添加用户活动日志的时候未注册HTTP上下文实例。
                services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
                var mvc = services.AddMvc()
                    .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
                    .AddDataAnnotationsLocalization()
                    .AddRazorOptions(options =>
                    {//网站的程序集名称，约定扩展程序集名称必须为“网站程序集名称.Extensions.当前扩展区域名称”
                        var assemblyName = Assembly.GetEntryAssembly().GetName().Name;
                        ViewLocation(options, assemblyName);
                        PageLocation(options);
                    })
                    .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
                action?.Invoke(mvc);
            });
        }

        private static void PageLocation(RazorViewEngineOptions options)
        {
            options.PageViewLocationFormats.Clear();
            options.AreaPageViewLocationFormats.Clear();
            options.PageViewLocationFormats.Add("/Pages/{1}/{0}" + RazorViewEngine.ViewExtension);
            options.PageViewLocationFormats.Add("/Pages/Shared/{0}" + RazorViewEngine.ViewExtension);
            options.AreaPageViewLocationFormats.Add("/Areas/{2}/Pages/{1}/{0}" + RazorViewEngine.ViewExtension);
            options.AreaPageViewLocationFormats.Add("/Areas/{2}/Pages/Shared/{0}" + RazorViewEngine.ViewExtension);
            options.AreaPageViewLocationFormats.Add("/Pages/Shared/{0}" + RazorViewEngine.ViewExtension);
        }

        private static void ViewLocation(RazorViewEngineOptions options, string assemblyName)
        {
            options.ViewLocationFormats.Clear();
            options.AreaViewLocationFormats.Clear();
            options.ViewLocationFormats.Add("/Views/{1}/{0}" + RazorViewEngine.ViewExtension);
            options.ViewLocationFormats.Add("/Views/Shared/{0}" + RazorViewEngine.ViewExtension);
            //区域试图路径：Extensions下面的路径，可以将每个区域改为扩展，这样会降低程序集的耦合度
            options.AreaViewLocationFormats.Add("/Extensions/" + assemblyName +
                                                ".Extensions.{2}/Views/{1}/{0}" +
                                                RazorViewEngine.ViewExtension);
            options.AreaViewLocationFormats.Add("/Extensions/" + assemblyName +
                                                ".Extensions.{2}/Views/Shared/{0}" +
                                                RazorViewEngine.ViewExtension);
            options.AreaViewLocationFormats.Add("/Extensions/{2}/Views/{1}/{0}" +
                                                RazorViewEngine.ViewExtension);
            options.AreaViewLocationFormats.Add("/Extensions/{2}/Views/Shared/{0}" +
                                                RazorViewEngine.ViewExtension);
            options.AreaViewLocationFormats.Add("/Views/Shared/{0}" + RazorViewEngine.ViewExtension);
        }

        /// <summary>
        /// 添加默认Mvc。
        /// </summary>
        /// <param name="app">APP构建实例对象。</param>
        /// <param name="configuration">配置实例对象。</param>
        /// <returns>APP构建实例对象。</returns>
        public static IApplicationBuilder UseMozliteMvc(this IApplicationBuilder app, IConfiguration configuration)
        {
            //配置程序集
            app.UseMozlite(configuration);
            //MVC
            app.UseMvc(builder => builder
                .MapLowerCaseRoute("dashboard-area-default", RouteSettings.Dashboard + "/{area}/{controller:regex(^admin.*)=Admin}/{action=Index}/{id?}")
                .MapLowerCaseRoute("dashboard-default", RouteSettings.Dashboard + "/{controller:regex(^admin.*)=Admin}/{action=Index}/{id?}")
                .MapLowerCaseRoute("area-default", "{area:exists}/{controller}/{action=Index}/{id?}")
                .MapLowerCaseRoute("default", "{controller=Home}/{action=Index}/{id?}"));
            return app;
        }

        /// <summary>
        /// 获取配置中支持的语言。
        /// </summary>
        /// <returns>返回配置中支持的语言。</returns>
        /// <param name="configuration">配置接口。</param>
        public static IDictionary<string, string> GetSupportedCultures(this IConfiguration configuration)
        {
            var cultures = configuration.GetSection("Cultures:Supports");
            var languages = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var item in cultures.GetChildren())
            {
                languages[item.Key] = item.Value;
            }
            return languages;
        }

        /// <summary>
        /// 添加语言支持。
        /// </summary>
        /// <returns>返回服务构建实例。</returns>
        /// <param name="builder">服务构建实例。</param>
        public static IMozliteBuilder AddSupportedCultures(this IMozliteBuilder builder)
        {
            var defaultLanguage = builder.Configuration["Cultures:Default"] ?? "zh-CN";
            var supportedCultures = builder.Configuration.GetSupportedCultures().Keys.Select(x => new CultureInfo(x)).ToArray();
            return builder.AddServices(services =>
            {
                services.AddLocalization(options => options.ResourcesPath = "Resources");
                services.Configure<RequestLocalizationOptions>(options =>
                {
                    options.DefaultRequestCulture = new RequestCulture(defaultLanguage);
                    options.SupportedCultures = supportedCultures;
                    options.SupportedUICultures = supportedCultures;
                    options.RequestCultureProviders = new List<IRequestCultureProvider>
                    {
                        new QueryStringRequestCultureProvider(),
                        new CookieRequestCultureProvider()
                    };
                });
            });
        }
    }
}