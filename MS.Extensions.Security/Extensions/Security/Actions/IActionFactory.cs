using System.Collections.Generic;
using Mozlite;

namespace MS.Extensions.Security.Actions
{
    /// <summary>
    /// 操作工厂接口。
    /// </summary>
    public interface IActionFactory : ISingletonService
    {
        /// <summary>
        /// 尝试获取当前操作接口。
        /// </summary>
        /// <param name="action">当前操作码。</param>
        /// <param name="provider">操作提供者。</param>
        /// <returns>返回获取结果。</returns>
        bool TryGetProvider(int action, out IActionProvider provider);

        /// <summary>
        /// 获取所有操作提供者。
        /// </summary>
        /// <returns>返回操作提供者列表。</returns>
        IEnumerable<IActionProvider> LoadProviders();
    }
}