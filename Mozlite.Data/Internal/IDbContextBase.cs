using System;
using System.Threading;
using Mozlite.Extensions;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace Mozlite.Data.Internal
{
    /// <summary>
    /// 数据库操作接口。
    /// </summary>
    /// <typeparam name="TModel">模型类型。</typeparam>
    public interface IDbContextBase<TModel> : IDbExecutor
    {
        /// <summary>
        /// 日志接口。
        /// </summary>  
        ILogger Logger { get; }

        /// <summary>
        /// SQL辅助接口。
        /// </summary>
        ISqlHelper SqlHelper { get; }

        /// <summary>
        /// 当前实体类型。
        /// </summary>
        IEntityType EntityType { get; }
        
        /// <summary>
        /// 实例化一个查询实例，这个实例相当于实例化一个查询类，不能当作属性直接调用。
        /// </summary>
        /// <returns>返回模型的一个查询实例。</returns>
        IQueryable<TModel> AsQueryable();

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
        /// <param name="key">主键值，主键必须为一列时候才可使用。</param>
        /// <param name="statement">更新选项实例。</param>
        /// <returns>返回是否更新成功。</returns>
        bool Update(object key, object statement);

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
        /// <param name="statement">更新选项实例，可以为匿名对象或者参数名为字段得表达式。</param>
        /// <returns>返回是否更新成功。</returns>
        /// <example>
        /// public class T{
        ///     public int Id{get;set;}
        ///     public int Views{get;set;}
        ///     public int DayViews{get;set;}
        /// }
        /// db.Update(x=>x.Id == 1, x=>new{Views = x.Views + 1,DayViews = x.DayViews + 1});
        /// db.Update(x=>x.Id == 1, Views=>Views.Views + 1);//这里参数必须为列名称
        /// </example>
        bool Update(Expression<Predicate<TModel>> expression, Expression<Func<TModel, object>> statement);

        /// <summary>
        /// 根据条件更新相应的模型实例对象。
        /// </summary>
        /// <param name="expression">条件表达式。</param>
        /// <param name="statement">更新选项实例，可以为匿名对象或者参数名为字段得表达式。</param>
        /// <param name="cancellationToken">取消标记。</param>
        /// <returns>返回是否更新成功。</returns>
        /// <example>
        /// public class T{
        ///     public int Id{get;set;}
        ///     public int Views{get;set;}
        ///     public int DayViews{get;set;}
        /// }
        /// db.Update(x=>x.Id == 1, x=>new{Views = x.Views + 1,DayViews = x.DayViews + 1});
        /// db.Update(x=>x.Id == 1, Views=>Views.Views + 1);//这里参数必须为列名称
        /// </example>
        Task<bool> UpdateAsync(Expression<Predicate<TModel>> expression, Expression<Func<TModel, object>> statement,
            CancellationToken cancellationToken = default);

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
        /// <param name="key">主键值，主键必须为一列时候才可使用。</param>
        /// <param name="statement">更新选项实例。</param>
        /// <param name="cancellationToken">取消标记。</param>
        /// <returns>返回是否更新成功。</returns>
        Task<bool> UpdateAsync(object key, object statement, CancellationToken cancellationToken = default);

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
        /// 通过SQL语句获取模型实例对象。
        /// </summary>
        /// <param name="sql">SQL语句。</param>
        /// <param name="parameters">参数。</param>
        /// <returns>返回模型实例对象。</returns>
        TModel Query(string sql, object parameters = null);

        /// <summary>
        /// 通过SQL语句获取模型实例对象。
        /// </summary>
        /// <param name="sql">SQL语句。</param>
        /// <param name="parameters">参数。</param>
        /// <param name="cancellationToken">取消标记。</param>
        /// <returns>返回模型实例对象。</returns>
        Task<TModel> QueryAsync(string sql, object parameters = null,
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
        /// 通过SQL语句获取模型实例对象。
        /// </summary>
        /// <param name="sql">SQL语句。</param>
        /// <param name="parameters">参数。</param>
        /// <returns>返回模型实例对象。</returns>
        IEnumerable<TModel> Fetch(string sql, object parameters = null);

        /// <summary>
        /// 通过SQL语句获取模型实例对象。
        /// </summary>
        /// <param name="sql">SQL语句。</param>
        /// <param name="parameters">参数。</param>
        /// <param name="cancellationToken">取消标记。</param>
        /// <returns>返回模型实例对象。</returns>
        Task<IEnumerable<TModel>> FetchAsync(string sql, object parameters = null,
            CancellationToken cancellationToken = default); 

        /// <summary>
        /// 分页获取实例列表。
        /// </summary>
        /// <param name="query">查询实例。</param>
        /// <param name="countExpression">返回总记录数的表达式,用于多表拼接过滤重复记录数。</param>
        /// <returns>返回分页实例列表。</returns>
        TQuery Load<TQuery>(TQuery query, Expression<Func<TModel, object>> countExpression = null) where TQuery : QueryBase<TModel>;

        /// <summary>
        /// 分页获取实例列表。
        /// </summary>
        /// <param name="query">查询实例。</param>
        /// <param name="countExpression">返回总记录数的表达式,用于多表拼接过滤重复记录数。</param>
        /// <param name="cancellationToken">取消标识。</param>
        /// <returns>返回分页实例列表。</returns>
        Task<TQuery> LoadAsync<TQuery>(TQuery query, Expression<Func<TModel, object>> countExpression = null, CancellationToken cancellationToken = default) where TQuery : QueryBase<TModel>;

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

        /// <summary>
        /// 获取条件表达式的数量。
        /// </summary>
        /// <param name="expression">条件表达式。</param>
        /// <returns>返回计算结果。</returns>
        int Count(Expression<Predicate<TModel>> expression);

        /// <summary>
        /// 获取条件表达式的数量。
        /// </summary>
        /// <param name="expression">条件表达式。</param>
        /// <param name="cancellationToken">取消标记。</param>
        /// <returns>返回计算结果。</returns>
        Task<int> CountAsync(Expression<Predicate<TModel>> expression,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// 通过条件表达式获取聚合实例对象。
        /// </summary>
        /// <param name="convertFunc">转换函数。</param>
        /// <param name="column">当前列。</param>
        /// <param name="expression">条件表达式。</param>
        /// <param name="scalarMethod">聚合方法。</param>
        /// <returns>返回聚合结果。</returns>
        TValue GetScalar<TValue>(string scalarMethod, Expression<Func<TModel, object>> column, Expression<Predicate<TModel>> expression, Func<object, TValue> convertFunc);

        /// <summary>
        /// 通过条件表达式获取聚合实例对象。
        /// </summary>
        /// <param name="convertFunc">转换函数。</param>
        /// <param name="column">当前列。</param>
        /// <param name="expression">条件表达式。</param>
        /// <param name="scalarMethod">聚合方法。</param>
        /// <param name="cancellationToken">取消标记。</param>
        /// <returns>返回聚合结果。</returns>
        Task<TValue> GetScalarAsync<TValue>(string scalarMethod, Expression<Func<TModel, object>> column, Expression<Predicate<TModel>> expression, 
            Func<object, TValue> convertFunc,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// 上移一个位置。
        /// </summary>
        /// <param name="key">主键值。</param>
        /// <param name="order">排序。</param>
        /// <param name="expression">条件表达式。</param>
        /// <returns>返回移动结果。</returns>
        bool MoveUp(object key, Expression<Func<TModel, object>> order, Expression<Predicate<TModel>> expression = null);

        /// <summary>
        /// 下移一个位置。
        /// </summary>
        /// <param name="key">主键值。</param>
        /// <param name="order">排序。</param>
        /// <param name="expression">条件表达式。</param>
        /// <returns>返回移动结果。</returns>
        bool MoveDown(object key, Expression<Func<TModel, object>> order, Expression<Predicate<TModel>> expression = null);

        /// <summary>
        /// 上移一个位置。
        /// </summary>
        /// <param name="key">主键值。</param>
        /// <param name="order">排序。</param>
        /// <param name="expression">条件表达式。</param>
        /// <param name="cancellationToken">取消标志。</param>
        /// <returns>返回移动结果。</returns>
        Task<bool> MoveUpAsync(object key, Expression<Func<TModel, object>> order, Expression<Predicate<TModel>> expression = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// 下移一个位置。
        /// </summary>
        /// <param name="key">主键值。</param>
        /// <param name="order">排序。</param>
        /// <param name="expression">条件表达式。</param>
        /// <param name="cancellationToken">取消标志。</param>
        /// <returns>返回移动结果。</returns>
        Task<bool> MoveDownAsync(object key, Expression<Func<TModel, object>> order, Expression<Predicate<TModel>> expression = null, CancellationToken cancellationToken = default);
    }
}