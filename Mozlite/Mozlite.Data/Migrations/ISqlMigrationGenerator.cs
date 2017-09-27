using System.Collections.Generic;
using Mozlite.Data.Migrations.Operations;

namespace Mozlite.Data.Migrations
{
    /// <summary>
    /// SQL数据迁移生成接口。
    /// </summary>
    public interface IMigrationsSqlGenerator
    {
        /// <summary>
        /// 将操作生成为SQL语句。
        /// </summary>
        /// <param name="operations">操作实例列表。</param>
        /// <returns>返回对应的SQL语句。</returns>
        MigrationCommandListBuilder Generate(IReadOnlyList<MigrationOperation> operations);
    }
}