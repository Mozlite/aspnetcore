using System.Collections.Generic;
using Microsoft.AspNetCore.Routing;

namespace Mozlite.Mvc.Routing
{
    /// <summary>
    /// 生成小写字母URL地址的路由实例。
    /// </summary>
    public class LowerCaseRoute : Route
    {
        /// <summary>
        /// 初始化类<see cref="LowerCaseRoute"/>。
        /// </summary>
        /// <param name="target">路由实例对象。</param>
        /// <param name="routeName">路由名称。</param>
        /// <param name="routeTemplate">模板。</param>
        /// <param name="defaults">默认值。</param>
        /// <param name="constraints">正则表达式约束。</param>
        /// <param name="dataTokens">其他数据信息。</param>
        /// <param name="inlineConstraintResolver">内部约束转换器。</param>
        public LowerCaseRoute(IRouter target, string routeName, string routeTemplate, RouteValueDictionary defaults, IDictionary<string, object> constraints, RouteValueDictionary dataTokens, IInlineConstraintResolver inlineConstraintResolver)
            : base(target, routeName, routeTemplate, defaults, constraints, dataTokens, inlineConstraintResolver)
        {
        }

        /// <summary>
        /// 获取虚拟路径数据。
        /// </summary>
        /// <param name="context">虚拟路径上下文。</param>
        /// <returns>返回虚拟数据实例。</returns>
        public override VirtualPathData GetVirtualPath(VirtualPathContext context)
        {
            var virtualPathData = base.GetVirtualPath(context);
            if (virtualPathData != null)
                return new LowerCaseVirtualPathData(virtualPathData);
            return null;
        }

        private class LowerCaseVirtualPathData : VirtualPathData
        {
            public LowerCaseVirtualPathData(VirtualPathData virtualPathData)
                : this(virtualPathData.Router, virtualPathData.VirtualPath?.ToLower(), virtualPathData.DataTokens)
            {

            }

            public LowerCaseVirtualPathData(IRouter router, string virtualPath)
                : base(router, virtualPath)
            {
            }

            public LowerCaseVirtualPathData(IRouter router, string virtualPath, RouteValueDictionary dataTokens)
                : base(router, virtualPath, dataTokens)
            {
            }
        }
    }
}