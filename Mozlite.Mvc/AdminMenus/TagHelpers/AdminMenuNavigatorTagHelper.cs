using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Mozlite.Mvc.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Mozlite.Mvc.AdminMenus.TagHelpers
{
    /// <summary>
    /// 后台管理导航。
    /// </summary>
    [HtmlTargetElement("moz:menu-navigator", Attributes = AttributeName)]
    public class AdminMenuNavigatorTagHelper : ViewContextableTagHelperBase
    {
        private const string AttributeName = "provider";
        private const string HomeAttributeName = "home";
        private const string HomeHrefAttributeName = "href";
        private readonly IMenuProviderFactory _factory;
        private readonly IUrlHelperFactory _urlHelperFactory;

        /// <summary>
        /// 初始化类<see cref="AdminMenuNavigatorTagHelper"/>。
        /// </summary>
        public AdminMenuNavigatorTagHelper(IMenuProviderFactory factory, IUrlHelperFactory urlHelperFactory)
        {
            _factory = factory;
            _urlHelperFactory = urlHelperFactory;
        }

        /// <summary>
        /// 呈现标记。
        /// </summary>
        /// <param name="context">标记辅助上下文。</param>
        /// <param name="output">输出实例。</param>
        /// <returns>返回执行任务。</returns>
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var urlHelper = _urlHelperFactory.GetUrlHelper(ViewContext);
            output.TagName = "ul";
            output.AddCssClass("breadcrumb");
            var current = ViewContext.GetCurrent(_factory, Provider, urlHelper);
            var navigators = LoadNavigators(current).OrderBy(n => n.Level).ToList();
            if (navigators.Count == 0)
                return;
            var links = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var navigator in navigators)
            {
                if (navigator.LinkUrl(urlHelper, null) == null || current.Name == navigator.Name && navigator.Text == Title)
                    continue;
                links[navigator.Text] = navigator.LinkUrl(urlHelper, null);
            }
            links[Title] = null;
            links.Remove(Home);
            output.Content.AppendHtml($"<li><a href=\"{Href}\">{Home}</a></li>");
            foreach (var link in links)
            {
                output.Content.AppendHtml(CreateLink(link.Value, link.Key));
            }
        }

        private string _title;
        /// <summary>
        /// 标题。
        /// </summary>
        protected string Title
        {
            get
            {
                if (_title == null)
                    _title = ViewContext.ViewData["Title"] as string;
                return _title;
            }
        }

        private TagBuilder CreateLink(string linkUrl, string text)
        {
            var builder = new TagBuilder("li");
            if (linkUrl != null)
            {
                var achor = new TagBuilder("a");
                achor.MergeAttribute("href", linkUrl);
                achor.InnerHtml.AppendHtml(text);
                builder.InnerHtml.AppendHtml(achor);
            }
            else
                builder.InnerHtml.AppendHtml(text);
            return builder;
        }

        private IEnumerable<MenuItem> LoadNavigators(MenuItem current)
        {
            while (current != null && current.Level >= 0)
            {
                yield return current;
                current = current.Parent;
            }
        }

        /// <summary>
        /// 菜单提供者名称。
        /// </summary>
        [HtmlAttributeName(AttributeName)]
        public string Provider { get; set; }

        /// <summary>
        /// 首页链接地址。
        /// </summary>
        [HtmlAttributeName(HomeHrefAttributeName)]
        public string Href { get; set; }

        /// <summary>
        /// 首页名称。
        /// </summary>
        [HtmlAttributeName(HomeAttributeName)]
        public string Home { get; set; }
    }
}