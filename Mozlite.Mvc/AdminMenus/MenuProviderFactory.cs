using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Caching.Memory;

namespace Mozlite.Mvc.AdminMenus
{
    /// <summary>
    /// 菜单提供者工厂实现类。
    /// </summary>
    public class MenuProviderFactory : IMenuProviderFactory
    {
        private readonly IEnumerable<IMenuProvider> _providers;
        private readonly IMemoryCache _cache;

        /// <summary>
        /// 初始化类<see cref="MenuProviderFactory"/>。
        /// </summary>
        /// <param name="providers">菜单提供者接口。</param>
        /// <param name="cache">缓存接口。</param>
        public MenuProviderFactory(IEnumerable<IMenuProvider> providers, IMemoryCache cache)
        {
            _providers = providers;
            _cache = cache;
        }

        /// <summary>
        /// 通过提供者名称获取菜单实例对象。
        /// </summary>
        /// <param name="provider">提供者名称。</param>
        /// <returns>返回当前提供者名称的菜单实例对象。</returns>
        public IEnumerable<MenuItem> GetMenus(string provider)
        {
            return LoadMenus(provider).Values;
        }

        private IDictionary<string, MenuItem> LoadMenus(string provider)
        {
            return _cache.GetOrCreate($"memus[{provider}]", ctx =>
            {
                ctx.SetAbsoluteExpiration(TimeSpan.FromMinutes(3));
                var dic = new Dictionary<string, MenuItem>(StringComparer.OrdinalIgnoreCase);
                var providers =
                    _providers.Where(p => string.Compare(p.Name, provider, StringComparison.OrdinalIgnoreCase) == 0);
                foreach (var menuProvider in providers)
                {
                    AddProvider(dic, menuProvider);
                }
                return dic;
            });
        }

        /// <summary>
        /// 获取当前选项。
        /// </summary>
        /// <param name="provider">提供者名称。</param>
        /// <param name="name">当前菜单唯一Id。</param>
        /// <returns>返回当前菜单项。</returns>
        public MenuItem GetMenu(string provider, string name)
        {
            LoadMenus(provider).TryGetValue(name, out var item);
            return item;
        }

        private void AddProvider(IDictionary<string, MenuItem> dic, IMenuProvider provider)
        {
            var root = new MenuItem();
            provider.Init(root);
            foreach (var menu in root)
            {
                if (dic.TryGetValue(menu.Name, out var item))
                    item.Merge(menu);
                else
                    item = menu;
                AddSubMenus(dic, item);
            }
        }

        private void AddSubMenus(IDictionary<string, MenuItem> dic, MenuItem item)
        {
            dic[item.Name] = item;
            foreach (var it in item)
            {
                AddSubMenus(dic, it);
            }
        }

        /// <summary>
        /// 通过提供者名称获取菜单顶级实例对象。
        /// </summary>
        /// <param name="provider">提供者名称。</param>
        /// <returns>返回当前提供者名称的菜单实例对象。</returns>
        public IEnumerable<MenuItem> GetRoots(string provider)
        {
            return GetMenus(provider)
                .Where(x => x.Level == 0)
                .OrderByDescending(x => x.Priority)
                .ToList();
        }

        /// <summary>
        /// 属性菜单。
        /// </summary>
        /// <param name="provider">菜单提供者。</param>
        public void Refresh(string provider)
        {
            _cache.Remove($"memus[{provider}]");
        }
    }
}