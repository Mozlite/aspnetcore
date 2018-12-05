using Mozlite.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Mozlite.Data.Query
{
    /// <summary>
    /// SQL脚本。
    /// </summary>
    public class SqlIndentedStringBuilder : IndentedStringBuilder
    {
        private readonly string _sql;
        private readonly List<string> _parameterNames;

        internal SqlIndentedStringBuilder(string sql, List<string> parameterNames)
        {
            _sql = sql;
            _parameterNames = parameterNames;
        }

        /// <summary>
        /// 初始化类<see cref="SqlIndentedStringBuilder"/>。
        /// </summary>
        public SqlIndentedStringBuilder() { }

        /// <summary>
        /// 参数。
        /// </summary>
        public IDictionary<string, object> Parameters { get; private set; }

        /// <summary>
        /// 生成参数对象。
        /// </summary>
        public IDictionary<string, object> CreateEntityParameters(object instance)
        {
            if (instance is IDictionary<string, object> parameters)
                Parameters = parameters;
            else if (_parameterNames != null)
            {//匿名类型
                Parameters = new Dictionary<string, object>();
                var entityType = instance.GetType().GetEntityType();
                var parameterNames = _parameterNames.Distinct(StringComparer.OrdinalIgnoreCase).ToList();
                foreach (var parameterName in parameterNames)
                {
                    Parameters.Add(parameterName, entityType.FindProperty(parameterName).Get(instance));
                }
            }
            return Parameters;
        }

        /// <summary>
        /// 生成参数对象。
        /// </summary>
        public void CreateObjectParameters(object instance)
        {
            if (Parameters == null)
            {
                if (instance is IDictionary<string, object> parameters)
                    Parameters = parameters;
                else
                {//匿名类型
                    Parameters = new Dictionary<string, object>();
                    foreach (var property in instance.GetType().GetRuntimeProperties())
                    {
                        var value = property.GetValue(instance);
                        Parameters.Add(property.Name, value);
                    }
                }
            }
        }

        /// <summary>
        /// 添加主键参数。
        /// </summary>
        /// <param name="key">主键值。</param>
        public void AddPrimaryKey(object key)
        {
            if (Parameters == null)
                Parameters = new Dictionary<string, object>();
            Parameters[QuerySqlGenerator.PrimaryKeyParameterName] = key;
        }

        /// <summary>
        /// 隐式转换为字符串。
        /// </summary>
        /// <param name="builder">SQL构建实例对象。</param>
        public static implicit operator string(SqlIndentedStringBuilder builder) => builder.ToString();

        /// <summary>
        /// 获取当前字符串实例。
        /// </summary>
        /// <returns>返回当前字符串内容实例。</returns>
        public override string ToString()
        {
            return _sql ?? base.ToString();
        }
    }
}