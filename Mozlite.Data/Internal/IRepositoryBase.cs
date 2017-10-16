using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Mozlite.Extensions;

namespace Mozlite.Data.Internal
{
    /// <summary>
    /// 数据库操作接口。
    /// </summary>
    /// <typeparam name="TModel">模型类型。</typeparam>
    public interface IRepositoryBase<TModel> : IDbExecutor
    {
        /// <summary>
        /// 当前实体类型。
        /// </summary>
        IEntityType EntityType { get; }
        
        /// <summary>
        /// 新建实例对象。
        /// </summary>
        /// <param name="model">模型实例对象。</param>
        /// <returns>返回是否成功新建实例。</returns>
        bool Create(TModel model);

        /// <summary>
        /// 新建实例对象。
        /// </summary>
        /// <param name="model">模型实例对象。</param>
        /// <param name="cancellationToken">取消标记。</param>
        /// <returns>返回是否成功新建实例。</returns>
        Task<bool> CreateAsync(TModel model, CancellationToken cancellationToken = default);

        /// <summary>
        /// 更新模型实例，此实例必须包含自增长ID或主键实例对象。
        /// </summary>
        /// <param name="model">模型实例对象。</param>
        /// <returns>返回是否更新成功。</returns>
        bool Update(TModel model);

        /// <summary>
        /// 更新模型实例，此实例必须包含自增长ID或主键实例对象。
        /// </summary>
        /// <param name="model">模型实例对象。</param>
        /// <param name="cancellationToken">取消标记。</param>
        /// <returns>返回是否更新成功。</returns>
        Task<bool> UpdateAsync(TModel model, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// 更新所有模型实例对象。
        /// </summary>
        /// <param name="statement">更新选项实例。</param>
        /// <returns>返回是否更新成功。</returns>
        bool Update(object statement);

        /// <summary>
        /// 根据条件更新相应的模型实例对象。
        /// </summary>
        /// <param name="expression">条件表达式。</param>
        /// <param name="statement">更新选项实例。</param>
        /// <returns>返回是否更新成功。</returns>
        bool Update(Expression<Predicate<TModel>> expression, object statement);

        /// <summary>
        /// 根据条件更新相应的模型实例对象。
        /// </summary>
        /// <param name="expression">条件表达式。</param>
        /// <param name="statement">更新选项实例。</param>
        /// <param name="cancellationToken">取消标记。</param>
        /// <returns>返回是否更新成功。</returns>
        Task<bool> UpdateAsync(Expression<Predicate<TModel>> expression, object statement,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// 更新所有的模型实例对象。
        /// </summary>
        /// <param name="statement">更新选项实例。</param>
        /// <param name="cancellationToken">取消标记。</param>
        /// <returns>返回是否更新成功。</returns>
        Task<bool> UpdateAsync(object statement, CancellationToken cancellationToken = default);

        /// <summary>
        /// 根据条件删除模型实例对象。
        /// </summary>
        /// <param name="expression">条件表达式。</param>
        /// <returns>判断是否删除成功。</returns>
        bool Delete(Expression<Predicate<TModel>> expression = null);

        /// <summary>
        /// 根据条件删除模型实例对象。
        /// </summary>
        /// <param name="expression">条件表达式。</param>
        /// <param name="cancellationToken">取消标记。</param>
        /// <returns>判断是否删除成功。</returns>
        Task<bool> DeleteAsync(Expression<Predicate<TModel>> expression = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// 通过条件表达式获取模型实例对象。
        /// </summary>
        /// <param name="expression">条件表达式。</param>
        /// <returns>返回模型实例对象。</returns>
        TModel Find(Expression<Predicate<TModel>> expression);

        /// <summary>
        /// 通过条件表达式获取模型实例对象。
        /// </summary>
        /// <param name="expression">条件表达式。</param>
        /// <param name="cancellationToken">取消标记。</param>
        /// <returns>返回模型实例对象。</returns>
        Task<TModel> FindAsync(Expression<Predicate<TModel>> expression,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// 根据主键删除模型实例对象。
        /// </summary>
        /// <param name="key">主键值，主键必须为一列时候才可使用。</param>
        /// <returns>判断是否删除成功。</returns>
        bool Delete(object key);

        /// <summary>
        /// 根据主键删除模型实例对象。
        /// </summary>
        /// <param name="key">主键值，主键必须为一列时候才可使用。</param>
        /// <param name="cancellationToken">取消标记。</param>
        /// <returns>判断是否删除成功。</returns>
        Task<bool> DeleteAsync(object key,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// 通过主键获取模型实例对象。
        /// </summary>
        /// <param name="key">主键值，主键必须为一列时候才可使用。</param>
        /// <returns>返回模型实例对象。</returns>
        TModel Find(object key);

        /// <summary>
        /// 通过主键获取模型实例对象。
        /// </summary>
        /// <param name="key">主键值，主键必须为一列时候才可使用。</param>
        /// <param name="cancellationToken">取消标记。</param>
        /// <returns>返回模型实例对象。</returns>
        Task<TModel> FindAsync(object key,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// 通过条件表达式获取模型实例对象。
        /// </summary>
        /// <param name="expression">条件表达式。</param>
        /// <returns>返回模型实例对象。</returns>
        IEnumerable<TModel> Fetch(Expression<Predicate<TModel>> expression = null);

        /// <summary>
        /// 通过条件表达式获取模型实例对象。
        /// </summary>
        /// <param name="expression">条件表达式。</param>
        /// <param name="cancellationToken">取消标记。</param>
        /// <returns>返回模型实例对象。</returns>
        Task<IEnumerable<TModel>> FetchAsync(Expression<Predicate<TModel>> expression = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// 通过条件表达式判断是否存在实例对象。
        /// </summary>
        /// <param name="expression">条件表达式。</param>
        /// <returns>返回判断结果。</returns>
        bool Any(Expression<Predicate<TModel>> expression = null);

        /// <summary>
        /// 通过条件表达式判断是否存在实例对象。
        /// </summary>
        /// <param name="expression">条件表达式。</param>
        /// <param name="cancellationToken">取消标记。</param>
        /// <returns>返回判断结果。</returns>
        Task<bool> AnyAsync(Expression<Predicate<TModel>> expression = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// 通过条件表达式判断是否存在实例对象。
        /// </summary>
        /// <param name="key">主键值，主键必须为一列时候才可使用。</param>
        /// <returns>返回判断结果。</returns>
        bool Any(object key);

        /// <summary>
        /// 通过条件表达式判断是否存在实例对象。
        /// </summary>
        /// <param name="key">主键值，主键必须为一列时候才可使用。</param>
        /// <param name="cancellationToken">取消标记。</param>
        /// <returns>返回判断结果。</returns>
        Task<bool> AnyAsync(object key,
            CancellationToken cancellationToken = default);
    }
}