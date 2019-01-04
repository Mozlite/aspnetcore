using System;
using System.Linq.Expressions;

namespace Mozlite.Data
{
    /// <summary>
    /// 查询上下文接口。
    /// </summary>
    /// <typeparam name="TModel">模型类型。</typeparam>
    public interface IQueryContext<TModel>
    {
        /// <summary>
        /// 设置表格关联。
        /// </summary>
        /// <typeparam name="TForeign">拼接类型。</typeparam>
        /// <param name="onExpression">关联条件表达式。</param>
        /// <returns>返回当前查询实例对象。</returns>
        IQueryContext<TModel> InnerJoin<TForeign>(Expression<Func<TModel, TForeign, bool>> onExpression);

        /// <summary>
        /// 设置表格关联。
        /// </summary>
        /// <typeparam name="TPrimary">主键所在的模型类型。</typeparam>
        /// <typeparam name="TForeign">外键所在的模型类型。</typeparam>
        /// <param name="onExpression">关联条件表达式。</param>
        /// <returns>返回当前查询实例对象。</returns>
        IQueryContext<TModel> InnerJoin<TPrimary, TForeign>(Expression<Func<TPrimary, TForeign, bool>> onExpression);

        /// <summary>
        /// 设置表格左关联。
        /// </summary>
        /// <typeparam name="TForeign">拼接类型。</typeparam>
        /// <param name="onExpression">关联条件表达式。</param>
        /// <returns>返回当前查询实例对象。</returns>
        IQueryContext<TModel> LeftJoin<TForeign>(Expression<Func<TModel, TForeign, bool>> onExpression);

        /// <summary>
        /// 设置表格左关联。
        /// </summary>
        /// <typeparam name="TPrimary">主键所在的模型类型。</typeparam>
        /// <typeparam name="TForeign">外键所在的模型类型。</typeparam>
        /// <param name="onExpression">关联条件表达式。</param>
        /// <returns>返回当前查询实例对象。</returns>
        IQueryContext<TModel> LeftJoin<TPrimary, TForeign>(Expression<Func<TPrimary, TForeign, bool>> onExpression);

        /// <summary>
        /// 设置表格右关联。
        /// </summary>
        /// <typeparam name="TForeign">拼接类型。</typeparam>
        /// <param name="onExpression">关联条件表达式。</param>
        /// <returns>返回当前查询实例对象。</returns>
        IQueryContext<TModel> RightJoin<TForeign>(Expression<Func<TModel, TForeign, bool>> onExpression);

        /// <summary>
        /// 设置表格右关联。
        /// </summary>
        /// <typeparam name="TPrimary">主键所在的模型类型。</typeparam>
        /// <typeparam name="TForeign">外键所在的模型类型。</typeparam>
        /// <param name="onExpression">关联条件表达式。</param>
        /// <returns>返回当前查询实例对象。</returns>
        IQueryContext<TModel> RightJoin<TPrimary, TForeign>(Expression<Func<TPrimary, TForeign, bool>> onExpression);

        /// <summary>
        /// 设置选择列。
        /// </summary>
        /// <typeparam name="TEntity">模型类型。</typeparam>
        /// <param name="field">列表达式。</param>
        /// <param name="alias">别名。</param>
        /// <returns>返回当前查询实例对象。</returns>
        IQueryContext<TModel> Select<TEntity>(Expression<Func<TEntity, object>> field, string alias);

        /// <summary>
        /// 设置选择列。
        /// </summary>
        /// <typeparam name="TEntity">模型类型。</typeparam>
        /// <param name="fields">列表达式。</param>
        /// <returns>返回当前查询实例对象。</returns>
        IQueryContext<TModel> Select<TEntity>(Expression<Func<TEntity, object>> fields);

        /// <summary>
        /// 设置选择列。
        /// </summary>
        /// <param name="fields">列表达式。</param>
        /// <returns>返回当前查询实例对象。</returns>
        IQueryContext<TModel> Select(Expression<Func<TModel, object>> fields);

        /// <summary>
        /// 设置选择列。
        /// </summary>
        /// <returns>返回当前查询实例对象。</returns>
        IQueryContext<TModel> Select();

        /// <summary>
        /// 设置选择列。
        /// </summary>
        /// <typeparam name="TEntity">模型类型。</typeparam>
        /// <param name="fields">不包含的列表达式。</param>
        /// <returns>返回当前查询实例对象。</returns>
        IQueryContext<TModel> Exclude<TEntity>(Expression<Func<TEntity, object>> fields);

        /// <summary>
        /// 设置选择列。
        /// </summary>
        /// <param name="fields">不包含的列表达式。</param>
        /// <returns>返回当前查询实例对象。</returns>
        IQueryContext<TModel> Exclude(Expression<Func<TModel, object>> fields);

        /// <summary>
        /// 设置选择列(不重复)。
        /// </summary>
        /// <typeparam name="TEntity">模型类型。</typeparam>
        /// <param name="fields">列表达式。</param>
        /// <returns>返回当前查询实例对象。</returns>
        IQueryContext<TModel> Distinct<TEntity>(Expression<Func<TEntity, object>> fields);

        /// <summary>
        /// 设置选择列(不重复)。
        /// </summary>
        /// <param name="fields">列表达式。</param>
        /// <returns>返回当前查询实例对象。</returns>
        IQueryContext<TModel> Distinct(Expression<Func<TModel, object>> fields);

        /// <summary>
        /// 添加条件表达式。
        /// </summary>
        /// <typeparam name="TEntity">模型类型。</typeparam>
        /// <param name="expression">条件表达式。</param>
        /// <returns>返回当前查询实例对象。</returns>
        IQueryContext<TModel> Where<TEntity>(Expression<Predicate<TEntity>> expression);

        /// <summary>
        /// 添加条件表达式。
        /// </summary>
        /// <param name="where">条件语句。</param>
        /// <returns>返回当前查询上下文。</returns>
        IQueryContext<TModel> Where(string where);

        /// <summary>
        /// 添加条件表达式。
        /// </summary>
        /// <param name="expression">条件表达式。</param>
        /// <returns>返回当前查询实例对象。</returns>
        IQueryContext<TModel> Where(Expression<Predicate<TModel>> expression);

        /// <summary>
        /// 添加排序规则。
        /// </summary>
        /// <typeparam name="TEntity">模型类型。</typeparam>
        /// <param name="expression">列名称表达式。</param>
        /// <returns>返回当前查询实例对象。</returns>
        IQueryContext<TModel> OrderBy<TEntity>(Expression<Func<TEntity, object>> expression);

        /// <summary>
        /// 添加排序规则。
        /// </summary>
        /// <typeparam name="TEntity">模型类型。</typeparam>
        /// <param name="expression">列名称表达式。</param>
        /// <returns>返回当前查询实例对象。</returns>
        IQueryContext<TModel> OrderByDescending<TEntity>(Expression<Func<TEntity, object>> expression);

        /// <summary>
        /// 添加排序规则。
        /// </summary>
        /// <param name="expression">列名称表达式。</param>
        /// <returns>返回当前查询实例对象。</returns>
        IQueryContext<TModel> OrderBy(Expression<Func<TModel, object>> expression);

        /// <summary>
        /// 添加排序规则。
        /// </summary>
        /// <param name="expression">列名称表达式。</param>
        /// <returns>返回当前查询实例对象。</returns>
        IQueryContext<TModel> OrderByDescending(Expression<Func<TModel, object>> expression);

        /// <summary>
        /// 添加排序规则。
        /// </summary>
        /// <typeparam name="TEntity">模型类型。</typeparam>
        /// <param name="expression">列名称表达式。</param>
        /// <param name="isDesc">是否为降序。</param>
        /// <returns>返回当前查询实例对象。</returns>
        IQueryContext<TModel> OrderBy<TEntity>(Expression<Func<TEntity, object>> expression, bool isDesc);

        /// <summary>
        /// 添加排序规则。
        /// </summary>
        /// <param name="expression">列名称表达式。</param>
        /// <param name="isDesc">是否为降序。</param>
        /// <returns>返回当前查询实例对象。</returns>
        IQueryContext<TModel> OrderBy(Expression<Func<TModel, object>> expression, bool isDesc);

        /// <summary>
        /// 忽略锁（脏查询）。
        /// </summary>
        /// <returns>返回当前查询实例对象。</returns>
        IQueryContext<TModel> WithNolock();
    }
}