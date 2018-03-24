using MySql.Data.MySqlClient;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Mozlite.Data.MySql
{
    /// <summary>
    /// SQLServer数据库。
    /// </summary>
    public class MySqlDatabase : Database
    {
        /// <summary>
        /// 初始化 <see cref="MySqlDatabase"/> 类的新实例。
        /// </summary>
        /// <param name="logger">日志接口。</param>
        /// <param name="options">配置选项。</param>
        /// <param name="sqlHelper">SQL辅助接口。</param>
        public MySqlDatabase(ILogger<MySqlDatabase> logger, IOptions<DatabaseOptions> options, ISqlHelper sqlHelper) 
            : base(logger, MySqlClientFactory.Instance, options, sqlHelper)
        {
        }
    }
}