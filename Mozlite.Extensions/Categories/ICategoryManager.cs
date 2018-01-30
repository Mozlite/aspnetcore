using System.Collections.Generic;
using Mozlite.Extensions.Data;

namespace Mozlite.Extensions.Categories
{
    /// <summary>
    /// 分类管理接口。
    /// </summary>
    /// <typeparam name="TCategory">分类实例。</typeparam>
    public interface ICategoryManager<TCategory>
        : IObjectManager<TCategory>
        where TCategory : CategoryBase
    {
        /// <summary>
        /// 当前分类实例。
        /// </summary>
        IEnumerable<TCategory> Categories { get; }
    }
}