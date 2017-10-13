namespace Mozlite.Data.Migrations.Operations
{
    /// <summary>
    /// 添加外键操作。
    /// </summary>
    public class AddForeignKeyOperation : NameTableMigrationOperation
    {
        /// <summary>
        /// 相关列。
        /// </summary>
        public virtual string[] Columns { get;  set; }
        
        /// <summary>
        /// 主键表。
        /// </summary>
        public virtual string PrincipalTable { get;  set; }

        /// <summary>
        /// 主键列。
        /// </summary>
        public virtual string[] PrincipalColumns { get;  set; }

        /// <summary>
        /// 主键更新时候的操作。
        /// </summary>
        public virtual ReferentialAction OnUpdate { get; set; }

        /// <summary>
        /// 主键删除时候的操作。
        /// </summary>
        public virtual ReferentialAction OnDelete { get; set; }
    }
}
