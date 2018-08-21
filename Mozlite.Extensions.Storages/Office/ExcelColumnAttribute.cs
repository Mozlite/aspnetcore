using System;

namespace Mozlite.Extensions.Storages.Office
{
    /// <summary>
    /// Excel导出格式特性。
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ExcelColumnAttribute : Attribute
    {
        /// <summary>
        /// 初始化类<see cref="ExcelColumnAttribute"/>。
        /// </summary>
        /// <param name="columnName">字段名称。</param>
        /// <param name="format">导出格式。</param>
        public ExcelColumnAttribute(string columnName, string format = null)
        {
            ColumnName = columnName;
            Format = format;
        }

        /// <summary>
        /// 字段名称。
        /// </summary>
        public string ColumnName { get; }

        /// <summary>
        /// 导出格式。
        /// </summary>
        public string Format { get; }
    }
}