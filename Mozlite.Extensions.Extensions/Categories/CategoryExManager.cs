using System.Collections.Generic;
using System.Threading.Tasks;
using Mozlite.Data;
using Mozlite.Extensions.Data;

namespace Mozlite.Extensions.Categories
{
    /// <summary>
    /// 分类管理类实现基类。
    /// </summary>
    /// <typeparam name="TCategory">分类类型。</typeparam>
    public abstract class CategoryExManager<TCategory> : ICategoryExManager<TCategory>
        where TCategory : CategoryExBase
    {
        /// <summary>
        /// 数据库操作实例。
        /// </summary>
        // ReSharper disable once InconsistentNaming
        protected readonly IDbContext<TCategory> db;
        /// <summary>
        /// 初始化类<see cref="CategoryExManager{TCategory}"/>。
        /// </summary>
        /// <param name="db">数据库操作接口实例。</param>
        protected CategoryExManager(IDbContext<TCategory> db)
        {
            db = db;
        }

        /// <summary>
        /// 保存分类。
        /// </summary>
        /// <param name="category">分类实例。</param>
        /// <returns>返回保存结果。</returns>
        public virtual DataResult Save(TCategory category)
        {
            if (IsDuplicated(category))
                return DataAction.Duplicate;
            if (category.Id == 0)
                return DataResult.FromResult(db.Create(category), DataAction.Created);
            return DataResult.FromResult(db.Update(category), DataAction.Created);
        }

        /// <summary>
        /// 保存分类。
        /// </summary>
        /// <param name="category">分类实例。</param>
        /// <returns>返回保存结果。</returns>
        public virtual async Task<DataResult> SaveAsync(TCategory category)
        {
            if (await IsDuplicatedAsync(category))
                return DataAction.Duplicate;
            if (category.Id == 0)
                return DataResult.FromResult(await db.CreateAsync(category), DataAction.Created);
            return DataResult.FromResult(await db.UpdateAsync(category), DataAction.Created);
        }

        /// <summary>
        /// 判断是否已经存在。
        /// </summary>
        /// <param name="category">分类实例。</param>
        /// <returns>返回判断结果。</returns>
        public virtual bool IsDuplicated(TCategory category)
        {
            return db.Any(x => x.Name == category.Name && x.Id != category.Id);
        }

        /// <summary>
        /// 判断是否已经存在。
        /// </summary>
        /// <param name="category">分类实例。</param>
        /// <returns>返回判断结果。</returns>
        public virtual Task<bool> IsDuplicatedAsync(TCategory category)
        {
            return db.AnyAsync(x => x.Name == category.Name && x.Id != category.Id);
        }

        /// <summary>
        /// 删除分类。
        /// </summary>
        /// <param name="id">分类Id。</param>
        /// <returns>返回删除结果。</returns>
        public virtual DataResult Delete(int id)
        {
            return DataResult.FromResult(db.Delete(id), DataAction.Deleted);
        }

        /// <summary>
        /// 删除分类。
        /// </summary>
        /// <param name="id">分类Id。</param>
        /// <returns>返回删除结果。</returns>
        public virtual async Task<DataResult> DeleteAsync(int id)
        {
            return DataResult.FromResult(await db.DeleteAsync(id), DataAction.Deleted);
        }

        /// <summary>
        /// 删除分类。
        /// </summary>
        /// <param name="ids">分类Id集合，以“,”分隔。</param>
        /// <returns>返回删除结果。</returns>
        public virtual DataResult Delete(string ids)
        {
            var intIds = ids.SplitToInt32();
            return DataResult.FromResult(db.Delete(x => x.Id.Included(intIds)), DataAction.Deleted);
        }

        /// <summary>
        /// 删除分类。
        /// </summary>
        /// <param name="ids">分类Id集合，以“,”分隔。</param>
        /// <returns>返回删除结果。</returns>
        public virtual async Task<DataResult> DeleteAsync(string ids)
        {
            var intIds = ids.SplitToInt32();
            return DataResult.FromResult(await db.DeleteAsync(x => x.Id.Included(intIds)), DataAction.Deleted);
        }

        /// <summary>
        /// 加载所有的分类。
        /// </summary>
        /// <returns>返回分类列表。</returns>
        public virtual IEnumerable<TCategory> Fetch()
        {
            return db.Fetch();
        }

        /// <summary>
        /// 加载所有的分类。
        /// </summary>
        /// <returns>返回分类列表。</returns>
        public virtual Task<IEnumerable<TCategory>> FetchAsync()
        {
            return db.FetchAsync();
        }

        /// <summary>
        /// 获取分类。
        /// </summary>
        /// <param name="id">分类Id。</param>
        /// <returns>返回分类实例。</returns>
        public virtual TCategory Get(int id)
        {
            return db.Find(id);
        }

        /// <summary>
        /// 获取分类。
        /// </summary>
        /// <param name="id">分类Id。</param>
        /// <returns>返回分类实例。</returns>
        public virtual Task<TCategory> GetAsync(int id)
        {
            return db.FindAsync(id);
        }
    }
}