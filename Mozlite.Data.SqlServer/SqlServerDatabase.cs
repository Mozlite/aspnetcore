using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mozlite.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
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
        /// <typeparam name="TModel">模型类型。</typeparam>
        /// <param name="models">模型列表。</param>
        public override Task BulkInsertAsync<TModel>(IEnumerable<TModel> models)
        {
            var type = typeof(TModel).GetEntityType();
            var properties = type.GetProperties().Where(x => x.IsCreatable()).ToList();
            using (var bulkCopy = new SqlBulkCopy(Options.ConnectionString))
            {
                bulkCopy.BatchSize = models.Count();
                bulkCopy.DestinationTableName = type.Table;
                var table = new DataTable();
                foreach (var property in properties)
                {
                    bulkCopy.ColumnMappings.Add(property.Name, property.Name);
                    table.Columns.Add(property.Name, Nullable.GetUnderlyingType(property.ClrType) ?? property.ClrType);
                }
                var values = new object[properties.Count];
                foreach (var model in models)
                {
                    for (var i = 0; i < values.Length; i++)
                    {
                        values[i] = properties[i].Get(model);
                    }
                    table.Rows.Add(values);
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