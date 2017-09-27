namespace Mozlite.Data.Query
{
    /// <summary>
    /// 查询的SQL接口。
    /// </summary>
    public interface IQuerySql
    {
        /// <summary>
        /// FROM语句。
        /// </summary>
        string FromSql { get; }

        /// <summary>
        /// 选择列。
        /// </summary>
        string FieldSql { get; }

        /// <summary>
        /// WHERE语句。
        /// </summary>
        string WhereSql { get; }

        /// <summary>
        /// ORDER BY语句。
        /// </summary>
        string OrderBySql { get; }

        /// <summary>
        /// 获取页码。
        /// </summary>
        int? PageIndex { get; }

        /// <summary>
        /// 获取记录数。
        /// </summary>
        int? Size { get; }

        /// <summary>
        /// 是否多表
        /// </summary>
        bool IsDistinct { get; }

        /// <summary>
        /// 聚合列或表达式。
        /// </summary>
        string Aggregation { get; }
    }
}