
namespace Mozlite.Data.Migrations.Operations
{
    /// <summary>
    /// 删除架构。
    /// </summary>
    public class DropSchemaOperation : MigrationOperation
    {
        /// <summary>
        /// 名称。
        /// </summary>
        public virtual string Name { get;  set; }
    }
}
