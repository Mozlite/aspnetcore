using System;
using System.Collections.Generic;

namespace Mozlite.Extensions.Searching
{
    /// <summary>
    /// 索引实例提供者基类。
    /// </summary>
    /// <typeparam name="TModel">模型类型。</typeparam>
    public abstract class SearchIndexProvider<TModel> : ISearchIndexProvider
        where TModel : class, ISearchable, new()
    {
        /// <inheritdoc />
        public Type Model => typeof(TModel);

        /// <summary>
        /// 提供者名称。
        /// </summary>
        public virtual string ProviderName => Model.FullName;

        /// <summary>
        /// 返回总结HTML代码。
        /// </summary>
        /// <param name="entry">当前实例对象。</param>
        /// <returns>返回搜索后显示的HTML实体代码。</returns>
        public abstract string Summarized(SearchEntry entry);

        /// <summary>
        /// 分词索引返回需要检索的关键词。
        /// </summary>
        /// <param name="entry">当前实例对象。</param>
        /// <returns>返回需要索引的关键词。</returns>
        public abstract IEnumerable<string> Indexed(SearchEntry entry);
    }
}