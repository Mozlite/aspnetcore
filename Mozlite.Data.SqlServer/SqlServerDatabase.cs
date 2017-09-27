using System.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Mozlite.Data.SqlServer
{
    /// <summary>
    /// SQLServer数据库。
    /// </summary>
    public class SqlServerDatabase : Database
    {
        /// <summary>
        /// 初始化 <see cref="SqlServerDatabase"/> 类的新实例。
        /// </summary>
        /// <param name="logger">日志接口。</param>
        /// <param name="options">配置选项。</param>
        /// <param name="sqlHelper">SQL辅助接口。</param>
        public SqlServerDatabase(ILogger<SqlServerDatabase> logger, IOptions<DatabaseOptions> options,
            ISqlHelper sqlHelper) : base(logger, SqlClientFactory.Instance, options, sqlHelper)
        {
        }
    }
}