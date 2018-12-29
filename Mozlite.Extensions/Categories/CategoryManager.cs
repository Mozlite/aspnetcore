using Mozlite.Data;
using System.Threading;
using System.Threading.Tasks;

namespace Mozlite.Extensions.Categories
{
    /// <summary>
    /// 分类管理类实现基类。
    /// </summary>
    /// <typeparam name="TCategory">分类类型。</typeparam>
    public abstract class CategoryManager<TCategory> : ObjectManager<TCategory>, ICategoryManager<TCategory>
        where TCategory : CategoryBase
    {
        /// <summary>
        /// 判断是否已经存在。
        /// </summary>
        /// <param name="category">分类实例。</param>
        /// <returns>返回判断结果。</returns>
        public override bool IsDuplicated(TCategory category)
        {
            return Context.Any(x => x.Id != category.Id && x.Name == category.Name);
        }

        /// <summary>
        /// 判断是否已经存在。
        /// </summary>
        /// <param name="category">分类实例。</param>
        /// <param name="cancellationToken">取消标识。</param>
        /// <returns>返回判断结果。</returns>
        public override Task<bool> IsDuplicatedAsync(TCategory category, CancellationToken cancellationToken = default)
        {
            return Context.AnyAsync(x => x.Id != category.Id && x.Name == category.Name, cancellationToken);
        }

        /// <summary>
        /// 初始化类<see cref="CategoryManager{TCategory}"/>。
        /// </summary>
        /// <param name="context">数据库操作实例。</param>
        protected CategoryManager(IDbContext<TCategory> context) : base(context)
        {
        }
    }
}