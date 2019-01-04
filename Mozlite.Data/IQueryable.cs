using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Mozlite.Extensions;

namespace Mozlite.Data
{
    /// <summary>
    /// 查询实例。
    /// </summary>
    /// <typeparam name="TModel">实体类型。</typeparam>
    public interface IQueryable<TModel> : IQueryContext<TModel>
    {
        /// <summary>
        /// 设置表格关联。
        /// </summary>
        /// <typeparam name="TForeign">拼接类型。</typeparam>
        /// <param name="onExpression">关联条件表达式。</param>
        /// <returns>返回当前查询实例对象。</returns>
        new IQueryable<TModel> InnerJoin<TForeign>(Expression<Func<TModel, TForeign, bool>> onExpression);

        /// <summary>
        /// 设置表格关联。
        /// </summary>
        /// <typeparam name="TPrimary">主键所在的模型类型。</typeparam>
        /// <typeparam name="TForeign">外键所在的模型类型。</typeparam>
        /// <param name="onExpression">关联条件表达式。</param>
        /// <returns>返回当前查询实例对象。</returns>
        new IQueryable<TModel> InnerJoin<TPrimary, TForeign>(Expression<Func<TPrimary, TForeign, bool>> onExpression);

        /// <summary>
        /// 设置表格左关联。
        /// </summary>
        /// <typeparam name="TForeign">拼接类型。</typeparam>
        /// <param name="onExpression">关联条件表达式。</param>
        /// <returns>返回当前查询实例对象。</returns>
        new IQueryable<TModel> LeftJoin<TForeign>(Expression<Func<TModel, TForeign, bool>> onExpression);

        /// <summary>
        /// 设置表格左关联。
        /// </summary>
        /// <typeparam name="TPrimary">主键所在的模型类型。</typeparam>
        /// <typeparam name="TForeign">外键所在的模型类型。</typeparam>
        /// <param name="onExpression">关联条件表达式。</param>
        /// <returns>返回当前查询实例对象。</returns>
        new IQueryable<TModel> LeftJoin<TPrimary, TForeign>(Expression<Func<TPrimary, TForeign, bool>> onExpression);

        /// <summary>
        /// 设置表格右关联。
        /// </summary>
        /// <typeparam name="TForeign">拼接类型。</typeparam>
        /// <param name="onExpression">关联条件表达式。</param>
        /// <returns>返回当前查询实例对象。</returns>
        new IQueryable<TModel> RightJoin<TForeign>(Expression<Func<TModel, TForeign, bool>> onExpression);

        /// <summary>
        /// 设置表格右关联。
        /// </summary>
        /// <typeparam name="TPrimary">主键所在的模型类型。</typeparam>
        /// <typeparam name="TForeign">外键所在的模型类型。</typeparam>
        /// <param name="onExpression">关联条件表达式。</param>
        /// <returns>返回当前查询实例对象。</returns>
        new IQueryable<TModel> RightJoin<TPrimary, TForeign>(Expression<Func<TPrimary, TForeign, bool>> onExpression);

        /// <summary>
        /// 设置选择列。
        /// </summary>
        /// <typeparam name="TEntity">模型类型。</typeparam>
        /// <param name="field">列表达式。</param>
        /// <param name="alias">别名。</param>
        /// <returns>返回当前查询实例对象。</returns>
        new IQueryable<TModel> Select<TEntity>(Expression<Func<TEntity, object>> field, string alias);

        /// <summary>
        /// 设置选择列。
        /// </summary>
        /// <typeparam name="TEntity">模型类型。</typeparam>
        /// <param name="fields">列表达式。</param>
        /// <returns>返回当前查询实例对象。</returns>
        new IQueryable<TModel> Select<TEntity>(Expression<Func<TEntity, object>> fields);

        /// <summary>
        /// 设置选择列。
        /// </summary>
        /// <param name="fields">列表达式。</param>
        /// <returns>返回当前查询实例对象。</returns>
        new IQueryable<TModel> Select(Expression<Func<TModel, object>> fields);

        /// <summary>
        /// 设置选择列。
        /// </summary>
        /// <returns>返回当前查询实例对象。</returns>
        new IQueryable<TModel> Select();

        /// <summary>
        /// 设置选择列。
        /// </summary>
        /// <typeparam name="TEntity">模型类型。</typeparam>
        /// <param name="fields">不包含的列表达式。</param>
        /// <returns>返回当前查询实例对象。</returns>
        new IQueryable<TModel> Exclude<TEntity>(Expression<Func<TEntity, object>> fields);

        /// <summary>
        /// 设置选择列。
        /// </summary>
        /// <param name="fields">不包含的列表达式。</param>
        /// <returns>返回当前查询实例对象。</returns>
        new IQueryable<TModel> Exclude(Expression<Func<TModel, object>> fields);

        /// <summary>
        /// 设置选择列(不重复)。
        /// </summary>
        /// <typeparam name="TEntity">模型类型。</typeparam>
        /// <param name="fields">列表达式。</param>
        /// <returns>返回当前查询实例对象。</returns>
        new IQueryable<TModel> Distinct<TEntity>(Expression<Func<TEntity, object>> fields);

        /// <summary>
        /// 设置选择列(不重复)。
        /// </summary>
        /// <param name="fields">列表达式。</param>
        /// <returns>返回当前查询实例对象。</returns>
        new IQueryable<TModel> Distinct(Expression<Func<TModel, object>> fields);

        /// <summary>
        /// 添加条件表达式。
        /// </summary>
        /// <typeparam name="TEntity">模型类型。</typeparam>
        /// <param name="expression">条件表达式。</param>
        /// <returns>返回当前查询实例对象。</returns>
        new IQueryable<TModel> Where<TEntity>(Expression<Predicate<TEntity>> expression);

        /// <summary>
        /// 添加条件表达式。
        /// </summary>
        /// <param name="where">条件语句。</param>
        /// <returns>返回当前查询上下文。</returns>
        new IQueryable<TModel> Where(string where);

        /// <summary>
        /// 添加条件表达式。
        /// </summary>
        /// <param name="expression">条件表达式。</param>
        /// <returns>返回当前查询实例对象。</returns>
        new IQueryable<TModel> Where(Expression<Predicate<TModel>> expression);

        /// <summary>
        /// 添加排序规则。
        /// </summary>
        /// <typeparam name="TEntity">模型类型。</typeparam>
        /// <param name="expression">列名称表达式。</param>
        /// <returns>返回当前查询实例对象。</returns>
        new IQueryable<TModel> OrderBy<TEntity>(Expression<Func<TEntity, object>> expression);

        /// <summary>
        /// 添加排序规则。
        /// </summary>
        /// <typeparam name="TEntity">模型类型。</typeparam>
        /// <param name="expression">列名称表达式。</param>
        /// <returns>返回当前查询实例对象。</returns>
        new IQueryable<TModel> OrderByDescending<TEntity>(Expression<Func<TEntity, object>> expression);

        /// <summary>
        /// 添加排序规则。
        /// </summary>
        /// <param name="expression">列名称表达式。</param>
        /// <returns>返回当前查询实例对象。</returns>
        new IQueryable<TModel> OrderBy(Expression<Func<TModel, object>> expression);

        /// <summary>
        /// 添加排序规则。
        /// </summary>
        /// <param name="expression">列名称表达式。</param>
        /// <returns>返回当前查询实例对象。</returns>
        new IQueryable<TModel> OrderByDescending(Expression<Func<TModel, object>> expression);

        /// <summary>
        /// 添加排序规则。
        /// </summary>
        /// <typeparam name="TEntity">模型类型。</typeparam>
        /// <param name="expression">列名称表达式。</param>
        /// <param name="isDesc">是否为降序。</param>
        /// <returns>返回当前查询实例对象。</returns>
        new IQueryable<TModel> OrderBy<TEntity>(Expression<Func<TEntity, object>> expression, bool isDesc);

        /// <summary>
        /// 添加排序规则。
        /// </summary>
        /// <param name="expression">列名称表达式。</param>
        /// <param name="isDesc">是否为降序。</param>
        /// <returns>返回当前查询实例对象。</returns>
        new IQueryable<TModel> OrderBy(Expression<Func<TModel, object>> expression, bool isDesc);

        /// <summary>
        /// 忽略锁（脏查询）。
        /// </summary>
        /// <returns>返回当前查询实例对象。</returns>
        new IQueryable<TModel> WithNolock();

        /// <summary>
        /// 查询数据库返回<paramref name="size"/>项结果。
        /// </summary>
        /// <param name="size">返回的记录数。</param>
        /// <returns>返回数据列表。</returns>
        IEnumerable<TModel> AsEnumerable(int size);

        /// <summary>
        /// 查询数据库返回结果。
        /// </summary>
        /// <returns>返回数据列表。</returns>
        TModel FirstOrDefault();

        /// <summary>
        /// 查询数据库返回结果。
        /// </summary>
        /// <param name="cancellationToken">取消标识。</param>
        /// <returns>返回数据列表。</returns>
        Task<TModel> FirstOrDefaultAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// 查询数据库返回结果。
        /// </summary>
        /// <param name="converter">对象转换器。</param>
        /// <returns>返回数据列表。</returns>
        TValue FirstOrDefault<TValue>(Func<DbDataReader, TValue> converter);

        /// <summary>
        /// 查询数据库返回结果。
        /// </summary>
        /// <param name="converter">对象转换器。</param>
        /// <param name="cancellationToken">取消标识。</param>
        /// <returns>返回数据列表。</returns>
        Task<TValue> FirstOrDefaultAsync<TValue>(Func<DbDataReader, TValue> converter, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// 查询数据库返回结果。
        /// </summary>
        /// <param name="pageIndex">页码。</param>
        /// <param name="pageSize">每页显示的记录数。</param>
        /// <param name="count">分页总记录数计算列。</param>
        /// <returns>返回数据列表。</returns>
        IPageEnumerable<TModel> AsEnumerable(int pageIndex, int pageSize, Expression<Func<TModel, object>> count = null);

        /// <summary>
        /// 查询数据库返回结果。
        /// </summary>
        /// <returns>返回数据列表。</returns>
        IEnumerable<TModel> AsEnumerable();

        /// <summary>
        /// 查询数据库返回结果，主要配合查询特定列时候使用。
        /// </summary>
        /// <param name="converter">对象转换器。</param>
        /// <returns>返回数据列表。</returns>
        IEnumerable<TValue> AsEnumerable<TValue>(Func<DbDataReader, TValue> converter);

        /// <summary>
        /// 查询数据库返回<paramref name="size"/>项结果。
        /// </summary>
        /// <param name="size">返回的记录数。</param>
        /// <param name="cancellationToken">取消标识。</param>
        /// <returns>返回数据列表。</returns>
        Task<IEnumerable<TModel>> AsEnumerableAsync(int size,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// 查询数据库返回结果。
        /// </summary>
        /// <param name="pageIndex">页码。</param>
        /// <param name="pageSize">每页显示的记录数。</param>
        /// <param name="count">分页总记录数计算列。</param>
        /// <param name="cancellationToken">取消标识。</param>
        /// <returns>返回数据列表。</returns>
        Task<IPageEnumerable<TModel>> AsEnumerableAsync(int pageIndex, int pageSize,
            Expression<Func<TModel, object>> count = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// 查询数据库返回结果。
        /// </summary>
        /// <param name="cancellationToken">取消标识。</param>
        /// <returns>返回数据列表。</returns>
        Task<IEnumerable<TModel>> AsEnumerableAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// 查询数据库返回结果，主要配合查询特定列时候使用。
        /// </summary>
        /// <param name="converter">对象转换器。</param>
        /// <param name="cancellationToken">取消标识。</param>
        /// <returns>返回数据列表。</returns>
        Task<IEnumerable<TValue>> AsEnumerableAsync<TValue>(Func<DbDataReader, TValue> converter, CancellationToken cancellationToken = default);
    }
}