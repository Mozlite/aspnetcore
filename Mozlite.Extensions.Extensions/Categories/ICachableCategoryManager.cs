﻿namespace Mozlite.Extensions.Extensions.Categories
{
    /// <summary>
    /// 缓存分类管理接口。
    /// </summary>
    /// <typeparam name="TCategory">分类实例。</typeparam>
    public interface ICachableCategoryManager<TCategory> : ICategoryManager<TCategory>
        where TCategory : CategoryBase
    {
    }
}