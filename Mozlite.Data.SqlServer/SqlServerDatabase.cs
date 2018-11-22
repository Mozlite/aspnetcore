using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

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

        /// <summary>
        /// 批量插入数据。
        /// </summary>
        /// <param name="table">模型列表。</param>
        public override Task ImportAsync(DataTable table)
        {
            using (var bulkCopy = new SqlBulkCopy(Options.ConnectionString))
            {
                bulkCopy.BatchSize = table.Rows.Count;
                bulkCopy.DestinationTableName = ReplacePrefixed(table.TableName);
                foreach (DataColumn property in table.Columns)
                {
                    bulkCopy.ColumnMappings.Add(property.ColumnName, property.ColumnName);
                }
                return bulkCopy.WriteToServerAsync(table);
            }
        }

        /// <summary>
        /// 获取数据库版本信息。
        /// </summary>
        /// <returns>返回数据库版本信息。</returns>
        public override string GetVersion()
        {
            return ExecuteScalar("SELECT @@VERSION;").ToString();
        }
    }
}