using Microsoft.Extensions.Caching.Memory;
using Mozlite.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Mozlite.Extensions.Categories
{
    /// <summary>
    /// 缓存分类管理实现类基类。
    /// </summary>
    /// <typeparam name="TCategory">分类类型。</typeparam>
    public abstract class CachableCategoryManager<TCategory> : CachableObjectManager<TCategory>, ICachableCategoryManager<TCategory>
        where TCategory : CategoryBase
    {
        /// <summary>
        /// 判断是否已经存在。
        /// </summary>
        /// <param name="category">分类实例。</param>
        /// <returns>返回判断结果。</returns>
        public override bool IsDuplicated(TCategory category)
        {
            var categories = Fetch();
            return categories.Any(x => x.Id != category.Id && x.Name == category.Name);
        }

        /// <summary>
        /// 判断是否已经存在。
        /// </summary>
        /// <param name="category">分类实例。</param>
        /// <param name="cancellationToken">取消标识。</param>
        /// <returns>返回判断结果。</returns>
        public override async Task<bool> IsDuplicatedAsync(TCategory category, CancellationToken cancellationToken = default)
        {
            var categories = await FetchAsync(cancellationToken: cancellationToken);
            return categories.Any(x => x.Id != category.Id && x.Name == category.Name);
        }

        /// <summary>
        /// 初始化类<see cref="CachableCategoryManager{TModel}"/>。
        /// </summary>
        /// <param name="context">数据库操作实例。</param>
        /// <param name="cache">缓存接口。</param>
        protected CachableCategoryManager(IDbContext<TCategory> context, IMemoryCache cache)
            : base(context, cache)
        {
        }
    }
}