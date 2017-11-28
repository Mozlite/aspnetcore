namespace Mozlite.Extensions.Searching
{
    /// <summary>
    /// 搜索接口。
    /// </summary>
    public interface ISearchable
    {
        /// <summary>
        /// 唯一Id。
        /// </summary>
        int Id { get; set; }

        /// <summary>
        /// 是否已经索引。
        /// </summary>
        IndexedType SearchIndexed { get; set; }
    }
}