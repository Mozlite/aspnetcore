using System;
using System.Collections.Generic;

namespace Mozlite.Extensions.Searching
{
    /// <summary>
    /// 索引实例提供者。
    /// </summary>
    public interface ISearchIndexProvider : ISingletonServices
    {
        /// <summary>
        /// 当前模型类型。
        /// </summary>
        Type Model { get; }

        /// <summary>
        /// 提供者名称。
        /// </summary>
        string ProviderName { get; }

        /// <summary>
        /// 返回总结HTML代码。
        /// </summary>
        /// <param name="entry">当前实例对象。</param>
        /// <returns>返回搜索后显示的HTML实体代码。</returns>
        string Summarized(SearchEntry entry);

        /// <summary>
        /// 分词索引返回需要检索的关键词。
        /// </summary>
        /// <param name="entry">当前实例对象。</param>
        /// <returns>返回需要索引的关键词。</returns>
        IEnumerable<string> Indexed(SearchEntry entry);
    }
}