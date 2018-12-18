using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Mozlite.Mvc
{
    /// <summary>
    /// MVC扩展类。
    /// </summary>
    public static class MvcExtensions
    {
        /// <summary>
        /// 获取当前请求的<see cref="Uri"/>实例。
        /// </summary>
        /// <param name="request">当前HTTP请求。</param>
        /// <returns>返回当前请求的<see cref="Uri"/>实例。</returns>
        public static Uri GetUri(this HttpRequest request)
        {
            var builder = new StringBuilder();
            builder.Append(request.Scheme);
            builder.Append("://");
            builder.Append(request.Host.Value);
            builder.Append(request.PathBase.Value);
            builder.Append(request.Path.Value);
            builder.Append(request.QueryString.Value);
            return new Uri(builder.ToString());
        }

        /// <summary>
        /// 获取当前请求的域名或者端口。
        /// </summary>
        /// <param name="request">当前HTTP请求。</param>
        /// <returns>返回当前请求的域名或者端口。</returns>
        public static string GetDomain(this HttpRequest request)
        {
            var uri = request.GetUri();
            if (uri.IsDefaultPort)
                return uri.DnsSafeHost;
            return $"{uri.DnsSafeHost}:{uri.Port}";
        }

        /// <summary>
        /// 判断当前域名是否为本地域名。
        /// </summary>
        /// <param name="domain">域名实例，可包含端口。</param>
        /// <returns>返回判断结果。</returns>
        public static bool IsLocal(this string domain)
        {
            domain = domain.ToLower();
            return domain.Equals("localhost")
                || domain.Equals("127.0.0.1")
                || domain.StartsWith("localhost:")
                || domain.StartsWith("127.0.0.1:");
        }
        
        /// <summary>
        /// 获取或添加当前请求上下文实例。
        /// </summary>
        /// <typeparam name="TCache">当前缓存类型。</typeparam>
        /// <param name="context">HTTP上下文。</param>
        /// <param name="key">缓存键。</param>
        /// <param name="func">新添加的对象。</param>
        /// <returns>返回当前缓存对象。</returns>
        public static TCache GetOrCreate<TCache>(this HttpContext context, object key, Func<TCache> func)
        {
            if (context.Items.TryGetValue(key, out var value) && value is TCache cache)
                return cache;
            cache = func();
            context.Items[key] = cache;
            return cache;
        }

        /// <summary>
        /// 获取或添加当前请求上下文实例。
        /// </summary>
        /// <typeparam name="TCache">当前缓存类型。</typeparam>
        /// <param name="context">HTTP上下文。</param>
        /// <param name="func">新添加的对象。</param>
        /// <returns>返回当前缓存对象。</returns>
        public static TCache GetOrCreate<TCache>(this HttpContext context, Func<TCache> func)
        {
            return context.GetOrCreate(typeof(TCache), func);
        }

        /// <summary>
        /// 获取或添加当前请求上下文实例。
        /// </summary>
        /// <typeparam name="TCache">当前缓存类型。</typeparam>
        /// <param name="context">HTTP上下文。</param>
        /// <param name="key">缓存键。</param>
        /// <param name="func">新添加的对象。</param>
        /// <returns>返回当前缓存对象。</returns>
        public static async Task<TCache> GetOrCreateAsync<TCache>(this HttpContext context, object key, Func<Task<TCache>> func)
        {
            if (context.Items.TryGetValue(key, out var value) && value is TCache cache)
                return cache;
            cache = await func();
            context.Items[key] = cache;
            return cache;
        }

        /// <summary>
        /// 获取或添加当前请求上下文实例。
        /// </summary>
        /// <typeparam name="TCache">当前缓存类型。</typeparam>
        /// <param name="context">HTTP上下文。</param>
        /// <param name="func">新添加的对象。</param>
        /// <returns>返回当前缓存对象。</returns>
        public static Task<TCache> GetOrCreateAsync<TCache>(this HttpContext context, Func<Task<TCache>> func)
        {
            return context.GetOrCreateAsync(typeof(TCache), func);
        }
    }
}