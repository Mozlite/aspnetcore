using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mozlite.Extensions.Searching
{
    /// <summary>
    /// 搜索管理。
    /// </summary>
    public interface ISearchManager : ISingletonService
    {
        /// <summary>
        /// 加载索引实体。
        /// </summary>
        /// <param name="entityType">模型实例对象。</param>
        /// <returns>返回当前索引实体列表。</returns>
        Task<SearchEntry> GetIndexAsync(IEntityType entityType);

        /// <summary>
        /// 保存索引。
        /// </summary>
        /// <param name="entityType">模型实例对象。</param>
        /// <param name="search">搜索实例对象。</param>
        /// <param name="indexes">索引分词。</param>
        /// <returns>返回保存结果。</returns>
        Task<bool> SaveAsync(IEntityType entityType, SearchDescriptor search, IEnumerable<string> indexes);

        /// <summary>
        /// 分页加载搜索信息。
        /// </summary>
        /// <param name="query">搜索实例对象。</param>
        /// <returns>返回搜索实例对象。</returns>
        Task<SearchQuery> LoadAsync(SearchQuery query);

        /// <summary>
        /// 提示搜索列。
        /// </summary>
        /// <param name="name">搜索字符串。</param>
        /// <returns>返回检索的关键词列表。</returns>
        Task<IEnumerable<SearchIndex>> LoadSuggestions(string name);
    }
}