using System;

namespace Mozlite.Extensions.Storages.Excels
{
    /// <summary>
    /// 数据列特性。
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ExcelColumnAttribute : ExcelFontAttribute, IAlignment
    {
        /// <summary>
        /// 横向对齐方式。
        /// </summary>
        public HorizontalAlignment Horizontal { get; set; } = HorizontalAlignment.General;

        /// <summary>
        /// 垂直对齐方式。
        /// </summary>
        public VerticalAlignment Vertical { get; set; } = VerticalAlignment.None;

        /// <summary>
        /// 数值格式。
        /// </summary>
        public string Format { get; set; }
    }
}