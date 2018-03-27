namespace Mozlite.Extensions.Extensions.Categories
{
    /// <summary>
    /// 分类管理接口。
    /// </summary>
    /// <typeparam name="TCategory">分类实例。</typeparam>
    public interface ICategoryManager<TCategory> : Mozlite.Extensions.Categories.ICategoryManager<TCategory>
        where TCategory : CategoryBase
    {
    }
}