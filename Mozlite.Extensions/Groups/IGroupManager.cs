using Mozlite.Extensions.Categories;

namespace Mozlite.Extensions.Groups
{
    /// <summary>
    /// 分组管理接口。
    /// </summary>
    /// <typeparam name="TGroup">分组类型。</typeparam>
    public interface IGroupManager<TGroup> : ICachableObjectManager<TGroup>
        where TGroup : GroupBase<TGroup>
    {

    }
}