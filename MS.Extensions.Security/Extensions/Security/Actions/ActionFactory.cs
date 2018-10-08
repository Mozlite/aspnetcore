using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace MS.Extensions.Security.Actions
{
    /// <summary>
    /// 操作工厂实现类。
    /// </summary>
    public class ActionFactory : IActionFactory
    {
        private readonly ConcurrentDictionary<int, IActionProvider> _providers;
        /// <summary>
        /// 初始化类<see cref="ActionFactory"/>。
        /// </summary>
        /// <param name="providers">提供者列表。</param>
        public ActionFactory(IEnumerable<IActionProvider> providers)
        {
            providers = providers.Where(x => x.Action > 0);
            _providers = new ConcurrentDictionary<int, IActionProvider>(providers.ToDictionary(x => x.Action));
        }

        /// <summary>
        /// 尝试获取当前操作接口。
        /// </summary>
        /// <param name="action">当前操作码。</param>
        /// <param name="provider">操作提供者。</param>
        /// <returns>返回获取结果。</returns>
        public virtual bool TryGetProvider(int action, out IActionProvider provider)
        {
            return _providers.TryGetValue(action, out provider);
        }

        /// <summary>
        /// 获取所有操作提供者。
        /// </summary>
        /// <returns>返回操作提供者列表。</returns>
        public virtual IEnumerable<IActionProvider> LoadProviders()
        {
            return _providers.Values;
        }
    }
}