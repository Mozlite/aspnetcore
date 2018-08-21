using System;

namespace Mozlite.Extensions.Storages.Office
{
    /// <summary>
    /// Excel配置。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ExcelAttribute : Attribute
    {
        /// <summary>
        /// 初始化类<see cref="ExcelAttribute"/>，名称和类型需要一一对应。
        /// </summary>
        /// <param name="types">字段类型：i忽略，n数值，s字符串，f浮点值，d日期，m为Decimal, b布尔。</param>
        /// <param name="names">字段名称：忽略的字段不需要配置，其他和类型配置一一对应。</param>
        public ExcelAttribute(string types, params string[] names)
        {
            Types = types.Replace(" ", "").ToCharArray();
            Names = names;
            if (types.Length != names.Length)
                throw new Exception("名称和类型需要一一对应。");
        }

        /// <summary>
        /// 名称。
        /// </summary>
        public string[] Names { get; }

        /// <summary>
        /// 类型。
        /// </summary>
        public char[] Types { get; }
    }
}