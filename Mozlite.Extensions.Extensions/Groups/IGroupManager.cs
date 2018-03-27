using Mozlite.Extensions.Extensions.Categories;

namespace Mozlite.Extensions.Extensions.Groups
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