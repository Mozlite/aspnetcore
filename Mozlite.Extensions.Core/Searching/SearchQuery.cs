using Mozlite.Data;

namespace Mozlite.Extensions.Searching
{
    /// <summary>
    /// 查询实例对象。
    /// </summary>
    public class SearchQuery : QueryBase<SearchDescriptor>
    {
        /// <summary>
        /// 搜索名称。
        /// </summary>
        public string Q { get; set; }
        
        protected override void Init(IQueryContext<SearchDescriptor> context)
        {
            context.InnerJoin<SearchInIndex>((s, i) => s.Id == i.SearchId)
                .InnerJoin<SearchInIndex, SearchIndex>((i, si) => i.IndexId == si.Id)
                .OrderByDescending<SearchIndex>(x => x.Priority)
                .Where<SearchIndex>(s => s.Name.Contains(Q))
                .Select()
                .Select<SearchIndex>(x => x.Priority);
        }
    }
}