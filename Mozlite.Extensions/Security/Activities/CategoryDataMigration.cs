namespace Mozlite.Extensions.Security.Activities
{
    /// <summary>
    /// 分类迁移类。
    /// </summary>
    /// <typeparam name="TCategory">分类类型。</typeparam>
    public abstract class CategoryDataMigration<TCategory> : Categories.CategoryDataMigration<TCategory>
        where TCategory : CategoryBase
    {
    }
}