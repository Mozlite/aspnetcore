using System.Collections.Generic;
using Mozlite.Extensions;

namespace Mozlite.Data.Query
{
    /// <summary>
    /// 查询扩展类型。
    /// </summary>
    internal static class QueryExtensions
    {
        /// <summary>
        /// 唯一主键参数名称。
        /// </summary>
        private const string PrimaryKeyParameterName = "__primarykey__";
        /// <summary>
        /// 唯一主键列参数。
        /// </summary>
        /// <param name="helper">辅助接口。</param>
        /// <returns>返回格式化后的参数。</returns>
        public static string PrimaryKeyParameter(this ISqlHelper helper)
        {
            return helper.Parameterized(PrimaryKeyParameterName);
        }

        /// <summary>
        /// 将值转换为唯一主键参数的值。
        /// </summary>
        /// <param name="value">参数值。</param>
        /// <returns>返回字典实例。</returns>
        public static IDictionary<string, object> AsPrimaryKeyParameter(this object value)
        {
            return new Dictionary<string, object>
            {
                [PrimaryKeyParameterName] = value
            };
        }

        /// <summary>
        /// 附加主键值。
        /// </summary>
        /// <param name="dic">当前参数字典实例。</param>
        /// <param name="value">主键值。</param>
        /// <returns>返回字典实例。</returns>
        public static IDictionary<string, object> AddPrimaryKey(this IDictionary<string, object> dic, object value)
        {
            dic.Add(PrimaryKeyParameterName, value);
            return dic;
        }

        /// <summary>
        /// 获取唯一主键条件字符串。
        /// </summary>
        /// <param name="helper">辅助接口。</param>
        /// <param name="entityType">当前实体。</param>
        /// <returns>返回SQL语句。</returns>
        public static string WherePrimaryKey(this ISqlHelper helper, IEntityType entityType)
        {
            var primaryKey = helper.DelimitIdentifier(entityType.SingleKey().Name);
            return $" WHERE {primaryKey} = {helper.PrimaryKeyParameter()}{helper.StatementTerminator}";
        }
    }
}