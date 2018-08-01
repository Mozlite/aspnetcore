using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Mozlite.Mvc
{
    /// <summary>
    /// HTML生成工具接口。
    /// </summary>
    public interface IRazorHtmlGenerator : ISingletonService
    {
        /// <summary>
        /// 生成HTML代码。
        /// </summary>
        /// <param name="path">物理路径。</param>
        /// <param name="tempData">临时数据。</param>
        /// <param name="routeData">路由数据。</param>
        /// <returns>返回当前HTML代码。</returns>
        Task<string> GeneralAsync(string path, IDictionary<string, object> tempData, RouteData routeData);
    }

    /// <summary>
    /// HTML生成工具。
    /// </summary>
    public class RazorHtmlGenerator : IRazorHtmlGenerator
    {
        private readonly IServiceProvider _serviceProvider;
        /// <summary>
        /// 初始化类<see cref="RazorHtmlGenerator"/>。
        /// </summary>
        /// <param name="serviceProvider">服务提供者接口。</param>
        public RazorHtmlGenerator(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task<string> GeneralAsync(string path, IDictionary<string, object> tempData, RouteData routeData)
        {
            var razorViewEngine = _serviceProvider.GetRequiredService<IRazorViewEngine>();
            var tempDataProvider = _serviceProvider.GetRequiredService<ITempDataProvider>();
            var serviceProvider = _serviceProvider.GetRequiredService<IServiceProvider>();
            var httpContext = new DefaultHttpContext { RequestServices = serviceProvider };
            var actionContext = new ActionContext(httpContext, routeData, new ActionDescriptor());

            var viewResult = razorViewEngine.GetView(path, path, true);
            if (!viewResult.Success)
            {
                throw new InvalidOperationException($"找不到视图模板 {path}");
            }

            using (var stringWriter = new StringWriter())
            {
                var tData = new TempDataDictionary(actionContext.HttpContext, tempDataProvider);
                if (tempData != null)
                {
                    foreach (var data in tempData)
                    {
                        tData[data.Key] = data.Value;
                    }
                }
                var viewData = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary());
                var viewContext = new ViewContext(actionContext, viewResult.View, viewData, tData, stringWriter, new HtmlHelperOptions());
                await viewResult.View.RenderAsync(viewContext);
                return stringWriter.ToString();
            }
        }
    }
}