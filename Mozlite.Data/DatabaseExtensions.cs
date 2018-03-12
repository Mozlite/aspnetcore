using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Mozlite.Data.Internal;

namespace Mozlite.Data
{
    /// <summary>
    /// 数据库扩展类。
    /// </summary>
    public static class DatabaseExtensions
    {
        private static int ConvertInt32(object value)
        {
            if (value == null)
                return 0;
            return Convert.ToInt32(value);
        }

        /// <summary>
        /// 获取最大值。
        /// </summary>
        /// <typeparam name="TModel">当前模型类型。</typeparam>
        /// <typeparam name="TValue">返回值。</typeparam>
        /// <param name="db">数据库实例。</param>
        /// <param name="column">当前列。</param>
        /// <param name="expression">条件表达式。</param>
        /// <param name="convert">转换函数。</param>
        /// <returns>返回当前值。</returns>
        public static TValue Max<TModel, TValue>(this IDbContextBase<TModel> db,
            Expression<Func<TModel, object>> column, Expression<Predicate<TModel>> expression = null,
            Func<object, TValue> convert = null)
        {
            return db.GetScalar("MAX", column, expression, convert);
        }

        /// <summary>
        /// 获取最大值。
        /// </summary>
        /// <typeparam name="TModel">当前模型类型。</typeparam>
        /// <param name="db">数据库实例。</param>
        /// <param name="column">当前列。</param>
        /// <param name="expression">条件表达式。</param>
        /// <returns>返回当前值。</returns>
        public static int Max<TModel>(this IDbContextBase<TModel> db,
            Expression<Func<TModel, object>> column, Expression<Predicate<TModel>> expression = null)
        {
            return db.Max(column, expression, Convert.ToInt32);
        }

        /// <summary>
        /// 获取最小值。
        /// </summary>
        /// <typeparam name="TModel">当前模型类型。</typeparam>
        /// <typeparam name="TValue">返回值。</typeparam>
        /// <param name="db">数据库实例。</param>
        /// <param name="column">当前列。</param>
        /// <param name="expression">条件表达式。</param>
        /// <param name="convert">转换函数。</param>
        /// <returns>返回当前值。</returns>
        public static TValue Min<TModel, TValue>(this IDbContextBase<TModel> db,
            Expression<Func<TModel, object>> column, Expression<Predicate<TModel>> expression = null,
            Func<object, TValue> convert = null)
        {
            return db.GetScalar("MIN", column, expression, convert);
        }

        /// <summary>
        /// 获取最小值。
        /// </summary>
        /// <typeparam name="TModel">当前模型类型。</typeparam>
        /// <param name="db">数据库实例。</param>
        /// <param name="column">当前列。</param>
        /// <param name="expression">条件表达式。</param>
        /// <returns>返回当前值。</returns>
        public static int Min<TModel>(this IDbContextBase<TModel> db,
            Expression<Func<TModel, object>> column, Expression<Predicate<TModel>> expression = null)
        {
            return db.Min(column, expression, Convert.ToInt32);
        }

        /// <summary>
        /// 获取求和值。
        /// </summary>
        /// <typeparam name="TModel">当前模型类型。</typeparam>
        /// <typeparam name="TValue">返回值。</typeparam>
        /// <param name="db">数据库实例。</param>
        /// <param name="column">当前列。</param>
        /// <param name="expression">条件表达式。</param>
        /// <param name="convert">转换函数。</param>
        /// <returns>返回当前值。</returns>
        public static TValue Sum<TModel, TValue>(this IDbContextBase<TModel> db,
            Expression<Func<TModel, object>> column, Expression<Predicate<TModel>> expression = null,
            Func<object, TValue> convert = null)
        {
            return db.GetScalar("SUM", column, expression, convert);
        }

        /// <summary>
        /// 获取求和值。
        /// </summary>
        /// <typeparam name="TModel">当前模型类型。</typeparam>
        /// <param name="db">数据库实例。</param>
        /// <param name="column">当前列。</param>
        /// <param name="expression">条件表达式。</param>
        /// <returns>返回当前值。</returns>
        public static int Sum<TModel>(this IDbContextBase<TModel> db,
            Expression<Func<TModel, object>> column, Expression<Predicate<TModel>> expression = null)
        {
            return db.Sum(column, expression, Convert.ToInt32);
        }

        /// <summary>
        /// 获取最大值。
        /// </summary>
        /// <typeparam name="TModel">当前模型类型。</typeparam>
        /// <typeparam name="TValue">返回值。</typeparam>
        /// <param name="db">数据库实例。</param>
        /// <param name="column">当前列。</param>
        /// <param name="expression">条件表达式。</param>
        /// <param name="convert">转换函数。</param>
        /// <param name="cancellationToken">取消标识。</param>
        /// <returns>返回当前值。</returns>
        public static Task<TValue> MaxAsync<TModel, TValue>(this IDbContextBase<TModel> db,
            Expression<Func<TModel, object>> column, Expression<Predicate<TModel>> expression = null,
            Func<object, TValue> convert = null,
            CancellationToken cancellationToken = default)
        {
            return db.GetScalarAsync("MAX", column, expression, convert, cancellationToken);
        }

        /// <summary>
        /// 获取最大值。
        /// </summary>
        /// <typeparam name="TModel">当前模型类型。</typeparam>
        /// <param name="db">数据库实例。</param>
        /// <param name="column">当前列。</param>
        /// <param name="expression">条件表达式。</param>
        /// <param name="cancellationToken">取消标识。</param>
        /// <returns>返回当前值。</returns>
        public static Task<int> MaxAsync<TModel>(this IDbContextBase<TModel> db,
            Expression<Func<TModel, object>> column, Expression<Predicate<TModel>> expression = null,
            CancellationToken cancellationToken = default)
        {
            return db.MaxAsync(column, expression, ConvertInt32, cancellationToken);
        }

        /// <summary>
        /// 获取最小值。
        /// </summary>
        /// <typeparam name="TModel">当前模型类型。</typeparam>
        /// <typeparam name="TValue">返回值。</typeparam>
        /// <param name="db">数据库实例。</param>
        /// <param name="column">当前列。</param>
        /// <param name="expression">条件表达式。</param>
        /// <param name="convert">转换函数。</param>
        /// <param name="cancellationToken">取消标识。</param>
        /// <returns>返回当前值。</returns>
        public static Task<TValue> MinAsync<TModel, TValue>(this IDbContextBase<TModel> db,
            Expression<Func<TModel, object>> column, Expression<Predicate<TModel>> expression = null,
            Func<object, TValue> convert = null,
            CancellationToken cancellationToken = default)
        {
            return db.GetScalarAsync("MIN", column, expression, convert, cancellationToken);
        }

        /// <summary>
        /// 获取最小值。
        /// </summary>
        /// <typeparam name="TModel">当前模型类型。</typeparam>
        /// <param name="db">数据库实例。</param>
        /// <param name="column">当前列。</param>
        /// <param name="expression">条件表达式。</param>
        /// <param name="cancellationToken">取消标识。</param>
        /// <returns>返回当前值。</returns>
        public static Task<int> MinAsync<TModel>(this IDbContextBase<TModel> db,
            Expression<Func<TModel, object>> column, Expression<Predicate<TModel>> expression = null,
            CancellationToken cancellationToken = default)
        {
            return db.MinAsync(column, expression, ConvertInt32, cancellationToken);
        }

        /// <summary>
        /// 获取求和值。
        /// </summary>
        /// <typeparam name="TModel">当前模型类型。</typeparam>
        /// <typeparam name="TValue">返回值。</typeparam>
        /// <param name="db">数据库实例。</param>
        /// <param name="column">当前列。</param>
        /// <param name="expression">条件表达式。</param>
        /// <param name="convert">转换函数。</param>
        /// <param name="cancellationToken">取消标识。</param>
        /// <returns>返回当前值。</returns>
        public static Task<TValue> SumAsync<TModel, TValue>(this IDbContextBase<TModel> db,
            Expression<Func<TModel, object>> column, Expression<Predicate<TModel>> expression = null,
            Func<object, TValue> convert = null,
            CancellationToken cancellationToken = default)
        {
            return db.GetScalarAsync("SUM", column, expression, convert, cancellationToken);
        }

        /// <summary>
        /// 获取求和值。
        /// </summary>
        /// <typeparam name="TModel">当前模型类型。</typeparam>
        /// <param name="db">数据库实例。</param>
        /// <param name="column">当前列。</param>
        /// <param name="expression">条件表达式。</param>
        /// <param name="cancellationToken">取消标识。</param>
        /// <returns>返回当前值。</returns>
        public static Task<int> SumAsync<TModel>(this IDbContextBase<TModel> db,
            Expression<Func<TModel, object>> column, Expression<Predicate<TModel>> expression = null,
            CancellationToken cancellationToken = default)
        {
            return db.SumAsync(column, expression, ConvertInt32, cancellationToken);
        }
    }
}