﻿using System;

namespace Mozlite.Extensions.Storages.Office
{
    /// <summary>
    /// Excel配置。
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ExcelAttribute : ExcelFontAttribute, IAlignment
    {
        /// <summary>
        /// 初始化类<see cref="ExcelAttribute"/>。
        /// </summary>
        /// <param name="name">名称，导入时做唯一判断。</param>
        public ExcelAttribute(string name = null)
        {
            Name = name;
            Bold = true;
        }

        /// <summary>
        /// 字段名称。
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 导出排序，从小到大。
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// 横向对齐方式。
        /// </summary>
        public HorizontalAlignment Horizontal { get; set; } = HorizontalAlignment.Center;

        /// <summary>
        /// 垂直对齐方式。
        /// </summary>
        public VerticalAlignment Vertical { get; set; } = VerticalAlignment.None;
    }
}