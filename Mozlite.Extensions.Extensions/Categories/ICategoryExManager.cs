namespace Mozlite.Extensions.Categories
{
    /// <summary>
    /// 分类管理接口。
    /// </summary>
    /// <typeparam name="TCategory">分类实例。</typeparam>
    public interface ICategoryExManager<TCategory> : ICategoryManager<TCategory>
        where TCategory : CategoryExBase
    {
    }
}