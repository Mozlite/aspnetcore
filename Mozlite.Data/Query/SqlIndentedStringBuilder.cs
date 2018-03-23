using System;
using System.Linq;
using Mozlite.Extensions;
using System.Collections.Generic;
using System.Reflection;

namespace Mozlite.Data.Query
{
    /// <summary>
    /// SQL脚本。
    /// </summary>
    public class SqlIndentedStringBuilder : IndentedStringBuilder
    {
        private readonly List<string> _parameters;
        /// <summary>
        /// 初始化类<see cref="SqlIndentedStringBuilder"/>。
        /// </summary>
        /// <param name="builder">字符串构建实例。</param>
        public SqlIndentedStringBuilder(IndentedStringBuilder builder)
            : base(builder)
        {
            _parameters = new List<string>();
        }

        /// <summary>
        /// 初始化类<see cref="SqlIndentedStringBuilder"/>。
        /// </summary>
        public SqlIndentedStringBuilder()
        {
            _parameters = new List<string>();
        }

        /// <summary>
        /// 添加一个参数。
        /// </summary>
        /// <param name="parameter">参数名称。</param>
        /// <returns>返回SQL构建实例。</returns>
        public SqlIndentedStringBuilder AddParameter(string parameter)
        {
            _parameters.Add(parameter);
            return this;
        }

        /// <summary>
        /// 添加一个参数。
        /// </summary>
        /// <param name="parameters">参数列表。</param>
        /// <returns>返回SQL构建实例。</returns>
        public SqlIndentedStringBuilder AddParameters(IEnumerable<string> parameters)
        {
            _parameters.AddRange(parameters.Distinct(StringComparer.OrdinalIgnoreCase));
            return this;
        }

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
            else
            {//匿名类型
                Parameters = new Dictionary<string, object>();
                var entityType = instance.GetType().GetEntityType();
                foreach (var parameterName in _parameters)
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
    }
}