using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace Mozlite.Data.Internal
{
    /// <summary>
    /// 数据库服务提供接口。
    /// </summary>
    public interface IDbExecutor
    {
        /// <summary>
        /// 执行没有返回值的查询实例对象。
        /// </summary>
        /// <param name="commandText">SQL字符串。</param>
        /// <param name="parameters">参数实例对象。</param>
        /// <param name="commandType">命令类型。</param>
        /// <returns>返回是否有执行影响到数据行。</returns>
        bool ExecuteNonQuery(
            string commandText,
            object parameters = null,
            CommandType commandType = CommandType.Text);

        /// <summary>
        /// 查询实例对象。
        /// </summary>
        /// <param name="commandText">SQL字符串。</param>
        /// <param name="parameters">参数实例对象。</param>
        /// <param name="commandType">命令类型。</param>
        /// <returns>返回数据库读取实例接口。</returns>
        DbDataReader ExecuteReader(
            string commandText,
            object parameters = null,
            CommandType commandType = CommandType.Text);

        /// <summary>
        /// 查询数据库聚合值。
        /// </summary>
        /// <param name="commandText">SQL字符串。</param>
        /// <param name="parameters">参数实例对象。</param>
        /// <param name="commandType">命令类型。</param>
        /// <returns>返回聚合值实例对象。</returns>
        object ExecuteScalar(
            string commandText,
            object parameters = null,
            CommandType commandType = CommandType.Text);

        /// <summary>
        /// 执行SQL语句。
        /// </summary>
        /// <param name="commandText">SQL字符串。</param>
        /// <param name="parameters">参数匿名类型。</param>
        /// <param name="commandType">SQL类型。</param>
        /// <param name="cancellationToken">取消标记。</param>
        /// <returns>返回影响的行数。</returns>
        Task<bool> ExecuteNonQueryAsync(
            string commandText,
            object parameters = null,
            CommandType commandType = CommandType.Text,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// 执行SQL语句。
        /// </summary>
        /// <param name="commandText">SQL字符串。</param>
        /// <param name="commandType">SQL类型。</param>
        /// <param name="parameters">参数匿名类型。</param>
        /// <param name="cancellationToken">取消标记。</param>
        /// <returns>返回数据库读取器实例对象。</returns>
        Task<DbDataReader> ExecuteReaderAsync(
            string commandText,
            object parameters = null,
            CommandType commandType = CommandType.Text,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// 执行SQL语句。
        /// </summary>
        /// <param name="commandText">SQL字符串。</param>
        /// <param name="commandType">SQL类型。</param>
        /// <param name="parameters">参数匿名类型。</param>
        /// <param name="cancellationToken">取消标记。</param>
        /// <returns>返回单一结果实例对象。</returns>
        Task<object> ExecuteScalarAsync(string commandText,
            object parameters = null,
            CommandType commandType = CommandType.Text,
            CancellationToken cancellationToken = default);
    }
}