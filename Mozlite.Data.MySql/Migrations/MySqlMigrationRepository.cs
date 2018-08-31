using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Mozlite.Data.Migrations;
using Mozlite.Data.Migrations.Models;

namespace Mozlite.Data.MySql.Migrations
{
    /// <summary>
    /// SQLServer数据库迁移操作脚本实现类。
    /// </summary>
    public class MySqlMigrationRepository : MigrationRepository
    {
        /// <summary>
        /// 判断是否存在的脚本。
        /// </summary>
        protected override string ExistsSql => throw new NotSupportedException();

        /// <summary>
        /// 判断是否已经存在迁移表。
        /// </summary>
        /// <returns>返回判断结果。</returns>
		public override bool EnsureMigrationTableExists()
		{
            return Context.ExecuteNonQuery(CreateSql);
		}

        /// <summary>
        /// 确保已经存在迁移表。
        /// </summary>
        /// <param name="cancellationToken">异步取消标识。</param>
        public override Task EnsureMigrationTableExistsAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return Context.ExecuteNonQueryAsync(CreateSql, cancellationToken: cancellationToken);
        }

		/// <summary>
		/// 创建表格语句。
		/// </summary>
		protected override string CreateSql
        {
            get
            {
                var builder = new StringBuilder();
                builder.Append("CREATE TABLE IF NOT EXISTS ").Append(Table).AppendLine("(");
                builder.AppendLine("    `ID` varchar(256) NOT NULL,");
                builder.AppendLine("    `Version` int(4) NOT NULL DEFAULT '0',");
                builder.AppendLine($"    PRIMARY KEY (`ID`)");
                builder.AppendLine(")ENGINE=InnoDB DEFAULT CHARSET=utf8;");
                return builder.ToString();
            }
        }

        /// <summary>
        /// 初始化类<see cref="MySqlMigrationRepository"/>。
        /// </summary>
        /// <param name="db">数据库操作实例。</param>
        /// <param name="sqlHelper">SQL辅助接口。</param>
        /// <param name="sqlGenerator">SQL迁移脚本生成接口。</param>
        public MySqlMigrationRepository(IDbContext<Migration> db, ISqlHelper sqlHelper, IMigrationsSqlGenerator sqlGenerator) : 
            base(db, sqlHelper, sqlGenerator)
        {
        }
    }
}