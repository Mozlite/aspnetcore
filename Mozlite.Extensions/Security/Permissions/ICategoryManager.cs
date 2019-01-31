using Mozlite.Extensions.Categories;

namespace Mozlite.Extensions.Security.Permissions
{
    /// <summary>
    /// 分类管理接口。
    /// </summary>
    public interface ICategoryManager : ICachableCategoryManager<Category>, ISingletonService
    {

    }
}