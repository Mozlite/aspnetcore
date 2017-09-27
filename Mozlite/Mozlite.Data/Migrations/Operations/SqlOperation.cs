using System;
using System.Linq.Expressions;

namespace Mozlite.Data.Migrations.Operations
{
    /// <summary>
    /// SQL语句操作。
    /// </summary>
    public class SqlOperation : MigrationOperation
    {
        /// <summary>
        /// SQL字符串。
        /// </summary>
        public virtual string Sql { get;  set; }

        /// <summary>
        /// 添加或更新的实体。
        /// </summary>
        public object Instance { get; set; }

        /// <summary>
        /// 模型类型。
        /// </summary>
        public Type EntityType { get; set; }

        /// <summary>
        /// 条件表达式。
        /// </summary>
        public Expression Expression { get; set; }
    }
}
