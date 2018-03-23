using Mozlite.Extensions;

namespace Mozlite.Data.Query
{
    /// <summary>
    /// 查询扩展类型。
    /// </summary>
    internal static class QueryExtensions
    {
        /// <summary>
        /// 获取唯一主键条件字符串。
        /// </summary>
        /// <param name="helper">辅助接口。</param>
        /// <param name="entityType">当前实体。</param>
        /// <returns>返回SQL语句。</returns>
        public static string WherePrimaryKey(this ISqlHelper helper, IEntityType entityType)
        {
            var primaryKey = helper.DelimitIdentifier(entityType.SingleKey().Name);
            return $" WHERE {primaryKey} = {helper.Parameterized(QuerySqlGenerator.PrimaryKeyParameterName)}{helper.StatementTerminator}";
        }
    }
}