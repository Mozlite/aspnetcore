using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mozlite.Extensions.Categories
{
    /// <summary>
    /// 分类管理接口。
    /// </summary>
    /// <typeparam name="TCategory">分类实例。</typeparam>
    public interface ICategoryManager<TCategory>
        where TCategory : CategoryBase
    {
        /// <summary>
        /// 保存分类。
        /// </summary>
        /// <param name="category">分类实例。</param>
        /// <returns>返回保存结果。</returns>
        DataResult Save(TCategory category);

        /// <summary>
        /// 保存分类。
        /// </summary>
        /// <param name="category">分类实例。</param>
        /// <returns>返回保存结果。</returns>
        Task<DataResult> SaveAsync(TCategory category);

        /// <summary>
        /// 判断是否已经存在。
        /// </summary>
        /// <param name="category">分类实例。</param>
        /// <returns>返回判断结果。</returns>
        bool IsDuplicated(TCategory category);

        /// <summary>
        /// 判断是否已经存在。
        /// </summary>
        /// <param name="category">分类实例。</param>
        /// <returns>返回判断结果。</returns>
        Task<bool> IsDuplicatedAsync(TCategory category);

        /// <summary>
        /// 删除分类。
        /// </summary>
        /// <param name="id">分类Id。</param>
        /// <returns>返回删除结果。</returns>
        DataResult Delete(int id);

        /// <summary>
        /// 删除分类。
        /// </summary>
        /// <param name="id">分类Id。</param>
        /// <returns>返回删除结果。</returns>
        Task<DataResult> DeleteAsync(int id);

        /// <summary>
        /// 删除分类。
        /// </summary>
        /// <param name="ids">分类Id集合，以“,”分隔。</param>
        /// <returns>返回删除结果。</returns>
        DataResult Delete(string ids);

        /// <summary>
        /// 删除分类。
        /// </summary>
        /// <param name="ids">分类Id集合，以“,”分隔。</param>
        /// <returns>返回删除结果。</returns>
        Task<DataResult> DeleteAsync(string ids);

        /// <summary>
        /// 加载所有的分类。
        /// </summary>
        /// <returns>返回分类列表。</returns>
        IEnumerable<TCategory> Fetch();

        /// <summary>
        /// 加载所有的分类。
        /// </summary>
        /// <returns>返回分类列表。</returns>
        Task<IEnumerable<TCategory>> FetchAsync();

        /// <summary>
        /// 获取分类。
        /// </summary>
        /// <param name="id">分类Id。</param>
        /// <returns>返回分类实例。</returns>
        TCategory Get(int id);

        /// <summary>
        /// 获取分类。
        /// </summary>
        /// <param name="id">分类Id。</param>
        /// <returns>返回分类实例。</returns>
        Task<TCategory> GetAsync(int id);
    }
}