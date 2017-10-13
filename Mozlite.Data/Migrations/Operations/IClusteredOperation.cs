namespace Mozlite.Data.Migrations.Operations
{
    internal interface IClusteredOperation
    {
        /// <summary>
        /// 是否聚合索引。
        /// </summary>
        bool IsClustered { get; set; }
    }
}