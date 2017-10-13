using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Mozlite.Data.Sqlite
{
    /// <summary>
    /// Sqlite数据库。
    /// </summary>
    public class SqliteDatabase : Database
    {
        /// <summary>
        /// 初始化 <see cref="SqliteDatabase"/> 类的新实例。
        /// </summary>
        /// <param name="logger">日志接口。</param>
        /// <param name="options">配置选项。</param>
        /// <param name="sqlHelper">SQL辅助接口。</param>
        public SqliteDatabase(ILogger<SqliteDatabase> logger, IOptions<DatabaseOptions> options,
            ISqlHelper sqlHelper) : base(logger, SqliteFactory.Instance, options, sqlHelper)
        {
        }
    }
}