using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Mozlite.Extensions.Security.Permissions;
using System.Collections.Generic;
using System.Linq;

namespace Mozlite.Mvc.AdminMenus.TagHelpers
{
    /// <summary>
    /// 管理员菜单标签。
    /// </summary>
    [HtmlTargetElement("moz:menu", Attributes = AttributeName)]
    public class AdminMenuTagHelper : Mvc.TagHelpers.ViewContextableTagHelperBase
    {
        private readonly IMenuProviderFactory _menuProviderFactory;
        private readonly IUrlHelperFactory _factory;
        private readonly IPermissionManager _permissionManager;
        private IUrlHelper _urlHelper;
        private const string AttributeName = "provider";

        /// <summary>
        /// 菜单提供者名称。
        /// </summary>
        [HtmlAttributeName(AttributeName)]
        public string Provider { get; set; }

        /// <summary>
        /// 初始化类<see cref="AdminMenuTagHelper"/>。
        /// </summary>
        /// <param name="menuProviderFactory">菜单提供者工厂接口。</param>
        /// <param name="factory">URL辅助类工厂接口。</param>
        /// <param name="permissionManager">权限管理接口。</param>
        public AdminMenuTagHelper(IMenuProviderFactory menuProviderFactory, IUrlHelperFactory factory, IPermissionManager permissionManager)
        {
            _menuProviderFactory = menuProviderFactory;
            _factory = factory;
            _permissionManager = permissionManager;
        }

        /// <summary>
        /// 访问并呈现当前标签实例。
        /// </summary>
        /// <param name="context">当前HTML标签上下文，包含当前HTML相关信息。</param>
        /// <param name="output">当前标签输出实例，用于呈现标签相关信息。</param>
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            _urlHelper = _factory.GetUrlHelper(ViewContext);
            output.TagName = "ul";
            var items = _menuProviderFactory.GetRoots(Provider)
                .Where(IsAuthorized)//当前项
                .ToList();
            if (items.Count == 0)
                return;
            var current = ViewContext.GetCurrent(_menuProviderFactory, Provider, _urlHelper);
            foreach (var item in items)
            {
                var children = item.Where(IsAuthorized).ToList();
                var li = CreateMenuItem(item, current, children, item.Any());
                if (li == null)
                    continue;

                output.Content.AppendHtml(li);
            }
        }

        private TagBuilder CreateMenuItem(MenuItem item, MenuItem current, List<MenuItem> items, bool hasSub)
        {
            var li = new TagBuilder("li");
            if (items?.Count > 0)
            {
                if (current.IsCurrent(item))
                    li.AddCssClass("opened");
                li.AddCssClass("has-sub");
            }
            else if (hasSub)
            {//包含子菜单，子菜单中没有一个有权限，则主菜单也没有权限
                return null;
            }
            li.AddCssClass("nav-item");
            var isCurrent = current.IsCurrent(item);
            if (isCurrent)
                li.AddCssClass("active");
            var anchor = new TagBuilder("a");
            anchor.MergeAttribute("href", item.LinkUrl(_urlHelper));
            anchor.MergeAttribute("title", item.Text);
            anchor.AddCssClass($"nav-link level-{item.Level}");
            //图标
            if (!string.IsNullOrWhiteSpace(item.IconName))
            {
                if (item.IconName.StartsWith("fa-"))
                    anchor.InnerHtml.AppendHtml($"<i class=\"fa {item.IconName}\"></i>");
                else
                    anchor.InnerHtml.AppendHtml($"<i class=\"{item.IconName}\"></i>");
            }
            //文本
            var span = new TagBuilder("span");
            span.AddCssClass("title");
            span.InnerHtml.Append(item.Text);
            anchor.InnerHtml.AppendHtml(span);
            if (!string.IsNullOrWhiteSpace(item.BadgeText))
            //badge
            {
                var badge = new TagBuilder("span");
                badge.AddCssClass("badge");
                badge.AddCssClass(item.BadgeClassName);
                badge.InnerHtml.Append(item.BadgeText);
                anchor.InnerHtml.AppendHtml(badge);
            }
            li.InnerHtml.AppendHtml(anchor);
            //子菜单
            if (items?.Count > 0)
                CreateChildren(li, items, current);
            return li;
        }

        private void CreateChildren(TagBuilder li, List<MenuItem> items, MenuItem current)
        {
            var ihasSub = false;
            var iul = new TagBuilder("ul");
            foreach (var it in items.OrderByDescending(x => x.Priority))
            {
                var children = it.Where(IsAuthorized).ToList();
                var ili = CreateMenuItem(it, current, children, it.Any());
                if (ili != null)
                {
                    ihasSub = true;
                    iul.InnerHtml.AppendHtml(ili);
                }
            }
            if (ihasSub)
                li.InnerHtml.AppendHtml(iul);
        }

        /// <summary>
        /// 判断是否具有权限。
        /// </summary>
        /// <param name="item">菜单项。</param>
        /// <returns>返回验证结果。</returns>
        public bool IsAuthorized(MenuItem item)
        {
            if (item.PermissionName == null)
                return true;
            return _permissionManager.IsAuthorized($"{item.PermissionName}");
        }
    }
}