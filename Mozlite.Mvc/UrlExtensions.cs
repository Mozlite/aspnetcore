using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Mozlite.Mvc
{
    /// <summary>
    /// URL扩展类。
    /// </summary>
    public static class UrlExtensions
    {
        /// <summary>
        /// 设置URL地址。
        /// </summary>
        /// <param name="viewContext">试图上下文。</param>
        /// <param name="key">查询键。</param>
        /// <param name="value">查询值。</param>
        /// <returns>返回设置当前键值的URL地址。</returns>
        public static string SetQueryUrl(this ViewContext viewContext, string key, object value)
        {
            return viewContext.SetQueryUrl(x => x[key] = value);
        }

        /// <summary>
        /// 设置URL地址。
        /// </summary>
        /// <param name="viewContext">试图上下文。</param>
        /// <param name="action">设置键值。</param>
        /// <returns>返回设置当前键值的URL地址。</returns>
        public static string SetQueryUrl(this ViewContext viewContext, Action<IDictionary<string, object>> action)
        {
            var query = viewContext.GetQuery();
            action(query);
            return query.ToQueryString();
        }

        private static IDictionary<string, object> GetQuery(this ViewContext context)
        {
            var dic = context.HttpContext.Items[typeof(UrlExtensions)] as Dictionary<string, object>;
            if (dic == null)
            {
                dic = new Dictionary<string, object>();
                foreach (var query in context.HttpContext.Request.Query)
                {
                    dic.Add(query.Key, query.Value);
                }
                context.HttpContext.Items[typeof(UrlExtensions)] = dic;
            }
            return dic;
        }

        private static string ToQueryString(this IDictionary<string, object> query)
        {
            var queryString = string.Join("&", query.Select(x => $"{x.Key}={x.Value}"));
            return "?" + queryString;
        }
    }
}