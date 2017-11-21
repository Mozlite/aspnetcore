namespace Mozlite.Extensions.Categories
{
    /// <summary>
    /// 分组管理接口。
    /// </summary>
    /// <typeparam name="TGroup">分组类型。</typeparam>
    public interface IGroupManager<TGroup> : ICachableCategoryManager<TGroup>
        where TGroup : GroupBase<TGroup>
    {

    }
}