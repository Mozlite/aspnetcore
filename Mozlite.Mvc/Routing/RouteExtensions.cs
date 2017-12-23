using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Mozlite.Mvc.Routing
{
    /// <summary>
    /// 路由扩展类。
    /// </summary>
    public static class RouteExtensions
    {
        /// <summary>
        /// 路由地址。
        /// </summary>
        /// <param name="routeBuilder">路由实例化接口。</param>
        /// <param name="name">名称。</param>
        /// <param name="template">模板。</param>
        /// <returns>返回当前路由实例化对象。</returns>
        public static IRouteBuilder MapLowerCaseRoute(this IRouteBuilder routeBuilder, string name, string template)
        {
            routeBuilder.MapLowerCaseRoute(name, template, null);
            return routeBuilder;
        }

        /// <summary>
        /// 路由地址。
        /// </summary>
        /// <param name="routeBuilder">路由实例化接口。</param>
        /// <param name="name">名称。</param>
        /// <param name="template">模板。</param>
        /// <param name="defaults">默认值。</param>
        /// <returns>返回当前路由实例化对象。</returns>
        public static IRouteBuilder MapLowerCaseRoute(this IRouteBuilder routeBuilder, string name, string template, object defaults)
        {
            return routeBuilder.MapLowerCaseRoute(name, template, defaults, null);
        }

        /// <summary>
        /// 路由地址。
        /// </summary>
        /// <param name="routeBuilder">路由实例化接口。</param>
        /// <param name="name">名称。</param>
        /// <param name="template">模板。</param>
        /// <param name="defaults">默认值。</param>
        /// <param name="constraints">约束表达式。</param>
        /// <returns>返回当前路由实例化对象。</returns>
        public static IRouteBuilder MapLowerCaseRoute(this IRouteBuilder routeBuilder, string name, string template, object defaults, object constraints)
        {
            return routeBuilder.MapLowerCaseRoute(name, template, defaults, constraints, null);
        }

        /// <summary>
        /// 路由地址。
        /// </summary>
        /// <param name="routeBuilder">路由实例化接口。</param>
        /// <param name="name">名称。</param>
        /// <param name="template">模板。</param>
        /// <param name="defaults">默认值。</param>
        /// <param name="constraints">约束表达式。</param>
        /// <param name="dataTokens">其他数据。</param>
        /// <returns>返回当前路由实例化对象。</returns>
        public static IRouteBuilder MapLowerCaseRoute(this IRouteBuilder routeBuilder, string name, string template, object defaults, object constraints, object dataTokens)
        {
            var requiredService = routeBuilder.ServiceProvider.GetRequiredService<IInlineConstraintResolver>();
            routeBuilder.Routes.Add(new LowerCaseRoute(routeBuilder.DefaultHandler, name, template, new RouteValueDictionary(defaults), new RouteValueDictionary(constraints), new RouteValueDictionary(dataTokens), requiredService));
            return routeBuilder;
        }
    }
}