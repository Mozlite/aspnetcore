using Mozlite.Extensions.Categories;

namespace Mozlite.Extensions.Security.Activities
{
    /// <summary>
    /// 分类管理接口。
    /// </summary>
    /// <typeparam name="TCategory">分类类型。</typeparam>
    public interface ICategoryManager<TCategory> : ICachableCategoryManager<TCategory>
        where TCategory : CategoryBase
    {
    }
}