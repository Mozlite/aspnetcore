using System;
using Mozlite.Data.Migrations.Builders;
using Mozlite.Extensions.Data;

namespace Mozlite.Extensions.Documents
{
    /// <summary>
    /// 文档数据库迁移类。
    /// </summary>
    public class DocumentDataMigration : ObjectDataMigration<Document>
    {
        /// <summary>
        /// 添加列。
        /// </summary>
        /// <param name="table">表格构建实例。</param>
        protected override void Create(CreateTableBuilder<Document> table)
        {
            table.Column(x => x.Title)
                 .Column(x => x.Tags)
                 .Column(x => x.Source)
                 .Column(x => x.Html)
                 .Column(x => x.CreatedDate)
                 .Column(x => x.UpdatedDate)
                 .Column(x => x.Good)
                 .Column(x => x.Bad)
                 .Column(x => x.SearchIndexed);
        }
    }
}
